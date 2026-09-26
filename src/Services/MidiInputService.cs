#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using NAudio.Midi;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Events;
using NAudioVisualizer.Exceptions;

namespace NAudioVisualizer.Services;

/// <summary>
/// Captures real-time MIDI input from a connected device and raises structured
/// <see cref="MidiNoteEvent"/> notifications for downstream visualization.
/// </summary>
/// <remarks>
/// Wraps the NAudio <see cref="MidiIn"/> API and bridges incoming MIDI messages into
/// the application event bus so that visualizers can subscribe without coupling to NAudio.
/// </remarks>
public sealed class MidiInputService : IDisposable
{
    private MidiIn? _midiIn;
    private int _activeDeviceIndex = -1;
    private CancellationTokenSource? _cts;
    private bool _isDisposed;

    // Added for tracking held notes
    private readonly object _heldNotesLock = new();
    private readonly Dictionary<(int Channel, int NoteNumber), int> _heldNoteCounts = new();

    /// <summary>
    /// Gets a value indicating whether this service has been disposed.
    /// </summary>
    public bool IsDisposed => _isDisposed;

    /// <summary>
    /// Gets the index of the currently active MIDI device, or -1 if none.
    /// </summary>
    public int ActiveDeviceIndex => _activeDeviceIndex;

    /// <summary>Raised on a thread-pool thread each time a MIDI note event is received from the active device.</summary>
    public event EventHandler<MidiNoteEventArgs>? NoteReceived;

    // Added for held notes
    public event EventHandler<HeldNotesChangedEventArgs>? HeldNotesChanged;

    /// <summary>
    /// Returns a snapshot of all MIDI input devices currently visible to the operating system.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A read-only list of <see cref="MidiDeviceInfo"/> records, one per device.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the service has been disposed.</exception>
    public Task<IReadOnlyList<MidiDeviceInfo>> GetAvailableDevicesAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        cancellationToken.ThrowIfCancellationRequested();

        int count = MidiIn.NumberOfDevices;
        var devices = new List<MidiDeviceInfo>(count);

        for (int i = 0; i < count; i++)
        {
            var caps = MidiIn.DeviceInfo(i);
            devices.Add(new MidiDeviceInfo { Index = i, ProductName = caps.ProductName });
        }

        return Task.FromResult<IReadOnlyList<MidiDeviceInfo>>(devices);
    }

    /// <summary>
    /// Opens the specified MIDI input device and begins forwarding note events.
    /// </summary>
    /// <param name="deviceIndex">Zero-based index of the MIDI input device to open.</param>
    /// <param name="cancellationToken">Token that, when cancelled, automatically calls <see cref="StopAsync"/>.</param>
    /// <exception cref="ObjectDisposedException">Thrown when the service has already been disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a MIDI session is already active.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="deviceIndex"/> exceeds the available range.</exception>
    /// <exception cref="AudioDeviceException">Thrown when the underlying MIDI device cannot be opened.</exception>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task StartAsync(int deviceIndex, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(deviceIndex);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_midiIn is not null)
            throw new InvalidOperationException("A MIDI session is already active. Call StopAsync first.");

        if (deviceIndex >= MidiIn.NumberOfDevices)
            throw new ArgumentOutOfRangeException(nameof(deviceIndex),
                $"Device index {deviceIndex} is outside the available range of 0–{MidiIn.NumberOfDevices - 1}.");

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _cts.Token.Register(static state => _ = ((MidiInputService)state!).StopAsync(), this);

        try
        {
            _activeDeviceIndex = deviceIndex;
            _midiIn = new MidiIn(deviceIndex);
            _midiIn.MessageReceived += OnMidiMessageReceived;
            _midiIn.ErrorReceived += OnMidiErrorReceived;
            _midiIn.Start();
        }
        catch (Exception ex)
        {
            CleanupDevice();
            throw new AudioDeviceException(
                $"Failed to open MIDI device {deviceIndex}: {ex.Message}",
                deviceIndex,
                ex
            );
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Stops the active MIDI session and releases the device handle.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task StopAsync()
    {
        CleanupDevice();
        await Task.CompletedTask;
    }

    private void OnMidiMessageReceived(object? sender, MidiInMessageEventArgs args)
    {
        try
        {
            ProcessMidiMessage(args.MidiEvent);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error processing MIDI message: {ex.Message}");
        }
    }

    private void ProcessMidiMessage(MidiEvent midiEvent)
    {
        try
        {
            if (midiEvent is not NoteEvent noteEvent)
                return;

            bool isNoteOn = noteEvent.CommandCode == MidiCommandCode.NoteOn
                && noteEvent is NoteOnEvent { Velocity: > 0 };

            var evt = new MidiNoteEvent
            {
                Channel = noteEvent.Channel,
                NoteNumber = noteEvent.NoteNumber,
                NoteName = MidiNoteEvent.GetNoteName(noteEvent.NoteNumber),
                Velocity = noteEvent is NoteOnEvent on ? on.Velocity : 0,
                IsNoteOn = isNoteOn,
                Frequency = MidiNoteEvent.GetFrequency(noteEvent.NoteNumber),
                DeviceIndex = _activeDeviceIndex
            };

            NoteReceived?.Invoke(this, new MidiNoteEventArgs { Note = evt });
            EventPublisher.Instance.Publish(evt);

            // Update held notes and raise event if the set of held notes changed
            lock (_heldNotesLock)
            {
                var key = (noteEvent.Channel, noteEvent.NoteNumber);
                if (isNoteOn)
                {
                    if (_heldNoteCounts.TryGetValue(key, out var count))
                    {
                        _heldNoteCounts[key] = count + 1;
                    }
                    else
                    {
                        _heldNoteCounts[key] = 1;
                    }
                }
                else
                {
                    if (_heldNoteCounts.TryGetValue(key, out var count))
                    {
                        if (count == 1)
                        {
                            _heldNoteCounts.Remove(key);
                        }
                        else
                        {
                            _heldNoteCounts[key] = count - 1;
                        }
                    }
                }

                // Generate the list of note names for currently held notes
                var noteNames = _heldNoteCounts.Keys
                    .Select(hn => MidiNoteEvent.GetNoteName(hn.NoteNumber))
                    .OrderBy(n => n) // Sort for consistent display
                    .ToList();

                HeldNotesChanged?.Invoke(this, new HeldNotesChangedEventArgs(noteNames));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error processing MIDI message: {ex.Message}");
        }
    }

    private void OnMidiErrorReceived(object? sender, MidiInMessageEventArgs args)
    {
        System.Diagnostics.Debug.WriteLine(
            $"MIDI error on device {_activeDeviceIndex}: 0x{args.RawMessage:X8}");
    }

    private void CleanupDevice()
    {
        if (_midiIn is null)
            return;

        _midiIn.Stop();
        _midiIn.MessageReceived -= OnMidiMessageReceived;
        _midiIn.ErrorReceived -= OnMidiErrorReceived;
        _midiIn.Dispose();
        _midiIn = null;
        _activeDeviceIndex = -1;
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(GetType().Name);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_isDisposed)
            return;

        CleanupDevice();
        _cts?.Dispose();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Event arguments carrying the <see cref="MidiNoteEvent"/> received from the active MIDI device.
/// </summary>
public sealed class MidiNoteEventArgs : EventArgs
{
    /// <summary>Gets the MIDI note event that was received.</summary>
    /// <value>The MIDI note event containing note details such as channel, note number, velocity, and frequency.</value>
    public required MidiNoteEvent Note { get; init; }

    /// <summary>
    /// Returns a string representation of the MidiNoteEventArgs.
    /// </summary>
    /// <returns>A string in the format "MidiNoteEventArgs: {Note}".</returns>
    public override string ToString()
    {
        return $"MidiNoteEventArgs: {Note}";
    }
}

/// <summary>
/// Event arguments for when the set of held MIDI notes changes.
/// </summary>
public sealed class HeldNotesChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the note names of all currently held notes.
    /// </summary>
    public IReadOnlyList<string> NoteNames { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeldNotesChangedEventArgs"/> class.
    /// </summary>
    /// <param name="noteNames">The note names of the currently held notes.</param>
    public HeldNotesChangedEventArgs(IReadOnlyList<string> noteNames)
    {
        NoteNames = noteNames;
    }
}
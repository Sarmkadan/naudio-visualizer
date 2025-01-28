// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

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

    /// <summary>Raised on a thread-pool thread each time a MIDI note event is received from the active device.</summary>
    public event EventHandler<MidiNoteEventArgs>? NoteReceived;

    /// <summary>
    /// Returns a snapshot of all MIDI input devices currently visible to the operating system.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A read-only list of <see cref="MidiDeviceInfo"/> records, one per device.</returns>
    public Task<IReadOnlyList<MidiDeviceInfo>> GetAvailableDevicesAsync(CancellationToken cancellationToken = default)
    {
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
    public async Task StartAsync(int deviceIndex, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_midiIn is not null)
            throw new InvalidOperationException("A MIDI session is already active. Call StopAsync first.");

        if (deviceIndex < 0 || deviceIndex >= MidiIn.NumberOfDevices)
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
                ex);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Stops the active MIDI session and releases the device handle.
    /// </summary>
    public async Task StopAsync()
    {
        CleanupDevice();
        await Task.CompletedTask;
    }

    private void OnMidiMessageReceived(object? sender, MidiInMessageEventArgs args)
    {
        try
        {
            if (args.MidiEvent is not NoteEvent noteEvent)
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
    public required MidiNoteEvent Note { get; init; }
}

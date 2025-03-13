#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Exceptions;

namespace NAudioVisualizer.Services;

/// <summary>
/// Service for capturing audio from input devices and managing the audio stream.
/// </summary>
public sealed class AudioCaptureService : IDisposable
{
    private const int DefaultBitDepth = 16;
    private const int BytesPerSample = 2; // 16 bits = 2 bytes
    private static readonly int[] CommonSampleRates = { 44100, 48000, 96000, 192000 };

    private WaveInEvent? _waveInput;
    private WaveFileWriter? _waveFileWriter;
    private AudioBuffer? _audioBuffer;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _captureTask;
    private AudioMetadata? _currentMetadata;
    private readonly List<AudioDevice> _availableDevices = [];
    private bool _isDisposed;
    private bool _isRecording;

    public event EventHandler<AudioFrameEventArgs>? FrameCaptured;
    public event EventHandler<AudioDeviceEventArgs>? DeviceStatusChanged;

    /// <summary>
    /// Gets all available audio input devices.
    /// </summary>
    public IReadOnlyList<AudioDevice> GetAvailableDevices()
    {
        UpdateDeviceList();
        return _availableDevices.AsReadOnly();
    }

    /// <summary>
    /// Initializes audio capture on the specified device.
    /// </summary>
    /// <param name="deviceIndex">Zero-based index of the audio input device.</param>
    /// <param name="sampleRate">Desired sample rate in Hz (e.g. 44100, 48000).</param>
    /// <param name="channelCount">Number of audio channels (1 = mono, 2 = stereo).</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="deviceIndex"/> is negative,
    /// <paramref name="sampleRate"/> is not a supported value,
    /// or <paramref name="channelCount"/> is outside the 1-2 range.
    /// </exception>
    /// <exception cref="AudioDeviceException">
    /// Thrown when the target device cannot be initialized with the requested parameters.
    /// </exception>
    public void Initialize(int deviceIndex, int sampleRate, int channelCount)
    {
        ThrowIfDisposed();
        ValidateInitializationParameters(deviceIndex, sampleRate, channelCount);

        try
        {
            InitializeWaveInput(deviceIndex, sampleRate, channelCount);
            InitializeAudioBuffer(sampleRate, channelCount);
            InitializeMetadata(sampleRate, channelCount);
        }
        catch (InvalidOperationException ex)
        {
            Cleanup();
            throw new AudioDeviceException(
                $"Failed to initialize audio device {deviceIndex}: {ex.Message}",
                deviceIndex,
                ex
            );
        }
    }

    private void ValidateInitializationParameters(int deviceIndex, int sampleRate, int channelCount)
    {
        if (deviceIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(deviceIndex), deviceIndex,
                "Device index must be non-negative.");

        if (sampleRate <= 0)
            throw new ArgumentOutOfRangeException(nameof(sampleRate), sampleRate,
                "Sample rate must be a positive value.");

        if (channelCount < 1 || channelCount > 2)
            throw new ArgumentOutOfRangeException(nameof(channelCount), channelCount,
                "Channel count must be 1 (mono) or 2 (stereo).");
    }

    private void InitializeWaveInput(int deviceIndex, int sampleRate, int channelCount)
    {
        _waveInput = new WaveInEvent
        {
            DeviceNumber = deviceIndex,
            WaveFormat = new WaveFormat(sampleRate, DefaultBitDepth, channelCount)
        };

        _waveInput.DataAvailable += OnDataAvailable;
        _waveInput.RecordingStopped += OnRecordingStopped;
    }

    private void InitializeAudioBuffer(int sampleRate, int channelCount)
    {
        // Initialize audio buffer with 2 seconds of audio
        int bufferSize = sampleRate * channelCount * BytesPerSample;
        _audioBuffer = new AudioBuffer(bufferSize, sampleRate, channelCount);
    }

    private void InitializeMetadata(int sampleRate, int channelCount)
    {
        _currentMetadata = new AudioMetadata
        {
            SampleRate = sampleRate,
            ChannelCount = channelCount,
            BitDepth = DefaultBitDepth
        };
    }

    /// <summary>
    /// Starts recording audio from the configured device.
    /// </summary>
    public async Task StartRecordingAsync()
    {
        ThrowIfDisposed();

        if (_waveInput is null)
            throw new InvalidOperationException("Audio device not initialized. Call Initialize() first.");

        if (_isRecording)
            return;

        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            _waveInput.StartRecording();
            _isRecording = true;

            _captureTask = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(50).ConfigureAwait(false);
                }
            }, _cancellationTokenSource.Token);

            if (_currentMetadata is not null)
            {
                _currentMetadata.IsCapturing = true;
            }

            await Task.CompletedTask;
        }
        catch (InvalidOperationException ex)
        {
            throw new AudioStreamException(
                $"Failed to start recording: {ex.Message}",
                AudioStreamErrorCode.InitializationFailed,
                ex
            );
        }
    }

    /// <summary>
    /// Stops recording audio.
    /// </summary>
    public async Task StopRecordingAsync()
    {
        ThrowIfDisposed();

        if (_waveInput is null || !_isRecording)
            return;

        try
        {
            _waveInput.StopRecording();
            _isRecording = false;
            _cancellationTokenSource?.Cancel();

            if (_captureTask is not null)
            {
                await _captureTask;
            }

            if (_currentMetadata is not null)
            {
                _currentMetadata.IsCapturing = false;
            }
        }
        catch (InvalidOperationException ex)
        {
            throw new AudioStreamException(
                $"Failed to stop recording: {ex.Message}",
                AudioStreamErrorCode.Unknown,
                ex
            );
        }
    }

    /// <summary>
    /// Gets the current audio metadata.
    /// </summary>
    public AudioMetadata? GetCurrentMetadata()
    {
        return _currentMetadata;
    }

    /// <summary>
    /// Gets a copy of buffered audio data.
    /// </summary>
    public float[]? GetBufferedAudio()
    {
        return _audioBuffer?.GetAll();
    }

    /// <summary>
    /// Gets the current audio buffer.
    /// </summary>
    public AudioBuffer? GetAudioBuffer()
    {
        return _audioBuffer;
    }

    /// <summary>
    /// Clears the audio buffer.
    /// </summary>
    public void ClearBuffer()
    {
        _audioBuffer?.Clear();
    }

    /// <summary>
    /// Updates the list of available audio devices.
    /// </summary>
    private void UpdateDeviceList()
    {
        _availableDevices.Clear();

        for (int i = 0; i < WaveInEvent.DeviceCount; i++)
        {
            var capabilities = WaveInEvent.GetCapabilities(i);
            var device = new AudioDevice(capabilities.ProductName, i, capabilities.Channels)
            {
                IsDefault = i == 0
            };

            // Add common sample rates
            foreach (int rate in CommonSampleRates)
            {
                device.AddSupportedSampleRate(rate);
            }

            _availableDevices.Add(device);
        }
    }

    /// <summary>
    /// Handles raw audio data from the capture device.
    /// </summary>
    private void OnDataAvailable(object? sender, WaveInEventArgs e)
    {
        if (_audioBuffer is null || _currentMetadata is null || e.BytesRecorded == 0)
            return;

        try
        {
            // Convert byte data to float samples
            var samples = BytesToFloatSamples(e.Buffer, e.BytesRecorded);
            _audioBuffer.Write(samples);

            _currentMetadata.TotalSamplesCaptured += samples.Length;
            _currentMetadata.TotalFramesProcessed++;
            _currentMetadata.UpdateDuration();

            // Calculate RMS level
            float rmsLevel = CalculateRmsLevel(samples);
            _currentMetadata.UpdateLevelMetrics(rmsLevel);

            // Create and raise frame event
            var frame = new AudioFrame(
                samples,
                _currentMetadata.ChannelCount,
                _currentMetadata.SampleRate,
                _currentMetadata.TotalFramesProcessed
            );

            FrameCaptured?.Invoke(this, new AudioFrameEventArgs { Frame = frame });
        }
        catch (InvalidOperationException ex)
        {
            OnRecordingError(ex);
        }
    }

    private const float MaxSampleValue = 32768f;

    /// <summary>
    /// Converts raw byte buffer to float samples (-1.0 to 1.0).
    /// </summary>
    private float[] BytesToFloatSamples(byte[] buffer, int bytesRecorded)
    {
        int sampleCount = bytesRecorded / BytesPerSample;
        var samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            short sample = BitConverter.ToInt16(buffer, i * BytesPerSample);
            samples[i] = sample / MaxSampleValue;
        }

        return samples;
    }

    /// <summary>
    /// Calculates RMS (Root Mean Square) level of audio samples.
    /// </summary>
    private float CalculateRmsLevel(float[] samples)
    {
        if (samples.Length == 0)
            return 0f;

        double sumSquares = 0;
        foreach (var sample in samples)
        {
            sumSquares += sample * sample;
        }

        return (float)Math.Sqrt(sumSquares / samples.Length);
    }

    /// <summary>
    /// Handles recording stopped event.
    /// </summary>
    private void OnRecordingStopped(object? sender, StoppedEventArgs e)
    {
        if (e.Exception is not null)
        {
            OnRecordingError(e.Exception);
        }

        if (_currentMetadata is not null)
        {
            _currentMetadata.IsCapturing = false;
        }
    }

    /// <summary>
    /// Handles recording errors.
    /// </summary>
    private void OnRecordingError(Exception exception)
    {
        DeviceStatusChanged?.Invoke(
            this,
            new AudioDeviceEventArgs
            {
                IsAvailable = false,
                Exception = exception
            }
        );
    }

    /// <summary>
    /// Cleans up resources.
    /// </summary>
    private void Cleanup()
    {
        _waveInput?.Dispose();
        _waveInput = null;
        _waveFileWriter?.Dispose();
        _waveFileWriter = null;
        _audioBuffer = null;
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    /// <summary>
    /// Throws if the service is disposed.
    /// </summary>
    private void ThrowIfDisposed()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(GetType().Name);
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        StopRecordingAsync().Wait(TimeSpan.FromSeconds(5));
        Cleanup();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Event args for audio frame captured event.
/// </summary>
public sealed class AudioFrameEventArgs : EventArgs
{
    public AudioFrame? Frame { get; set; }
}

/// <summary>
/// Event args for audio device status changed event.
/// </summary>
public sealed class AudioDeviceEventArgs : EventArgs
{
    public bool IsAvailable { get; set; }
    public Exception? Exception { get; set; }
}

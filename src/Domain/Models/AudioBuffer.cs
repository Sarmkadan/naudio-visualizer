// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

#nullable enable

using System;
using System.Collections.Generic;

namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Circular buffer for storing audio data with efficient memory usage.
/// </summary>
public sealed class AudioBuffer
{
    /// <summary>
    /// Internal circular buffer storing audio samples.
    /// </summary>
    private readonly float[] _buffer;

    /// <summary>
    /// Current write position in the buffer.
    /// </summary>
    private int _writePos;

    /// <summary>
    /// Current read position in the buffer.
    /// </summary>
    private int _readPos;

    /// <summary>
    /// Total number of samples written to the buffer.
    /// </summary>
    private long _totalSamplesWritten;

    /// <summary>
    /// Lock for thread-safe operations.
    /// </summary>
    private readonly object _lock = new();

    /// <summary>
    /// Number of samples currently in the buffer.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Maximum capacity of the buffer.
    /// </summary>
    public int Capacity { get; private set; }

    /// <summary>
    /// Sample rate of the audio in the buffer.
    /// </summary>
    public int SampleRate { get; set; }

    /// <summary>
    /// Number of channels in the buffer.
    /// </summary>
    public int ChannelCount { get; set; }

    /// <summary>
    /// Initializes a new circular audio buffer.
    /// </summary>
    public AudioBuffer(int capacity, int sampleRate, int channelCount)
    {
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be positive", nameof(capacity));

        _buffer = new float[capacity];
        Capacity = capacity;
        SampleRate = sampleRate;
        ChannelCount = channelCount;
        _writePos = 0;
        _readPos = 0;
        Count = 0;
        _totalSamplesWritten = 0;
    }

    /// <summary>
    /// Writes audio samples to the buffer.
    /// </summary>
    public void Write(float[] samples)
    {
        if (samples is null)
            throw new ArgumentNullException(nameof(samples));

        lock (_lock)
        {
            for (int i = 0; i < samples.Length; i++)
            {
                _buffer[_writePos] = samples[i];
                _writePos = (_writePos + 1) % Capacity;

                if (Count < Capacity)
                {
                    Count++;
                }
                else
                {
                    _readPos = (_readPos + 1) % Capacity;
                }

                _totalSamplesWritten++;
            }
        }
    }

    /// <summary>
    /// Reads samples from the buffer without removing them.
    /// </summary>
    public float[] Peek(int sampleCount)
    {
        lock (_lock)
        {
            if (sampleCount > Count)
                sampleCount = Count;

            var result = new float[sampleCount];
            int pos = _readPos;

            for (int i = 0; i < sampleCount; i++)
            {
                result[i] = _buffer[pos];
                pos = (pos + 1) % Capacity;
            }

            return result;
        }
    }

    /// <summary>
    /// Reads and removes samples from the buffer.
    /// </summary>
    public float[] Read(int sampleCount)
    {
        lock (_lock)
        {
            if (sampleCount > Count)
                sampleCount = Count;

            var result = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                result[i] = _buffer[_readPos];
                _readPos = (_readPos + 1) % Capacity;
                Count--;
            }

            return result;
        }
    }

    /// <summary>
    /// Gets all available samples from the buffer.
    /// </summary>
    public float[] GetAll()
    {
        lock (_lock)
        {
            return Peek(Count);
        }
    }

    /// <summary>
    /// Clears the buffer.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _writePos = 0;
            _readPos = 0;
            Count = 0;
        }
    }

    /// <summary>
    /// Gets the duration of audio in the buffer (seconds).
    /// </summary>
    public double GetDurationSeconds()
    {
        lock (_lock)
        {
            return (double)Count / SampleRate;
        }
    }

    /// <summary>
    /// Checks if the buffer is full.
    /// </summary>
    public bool IsFull()
    {
        lock (_lock)
        {
            return Count >= Capacity;
        }
    }

    /// <summary>
    /// Checks if the buffer is empty.
    /// </summary>
    public bool IsEmpty()
    {
        lock (_lock)
        {
            return Count == 0;
        }
    }

    /// <summary>
    /// Gets the available space in the buffer.
    /// </summary>
    public int AvailableSpace()
    {
        lock (_lock)
        {
            return Capacity - Count;
        }
    }

    /// <summary>
    /// Gets statistics about the buffer usage.
    /// </summary>
    public AudioBufferStats GetStats()
    {
        lock (_lock)
        {
            return new AudioBufferStats
            {
                SamplesWritten = _totalSamplesWritten,
                CurrentCount = Count,
                Capacity = Capacity,
                FillPercentage = (float)Count / Capacity * 100f,
                DurationSeconds = GetDurationSeconds()
            };
        }
    }
}

/// <summary>
/// Statistics about audio buffer usage.
/// </summary>
public class AudioBufferStats
{
    public long SamplesWritten { get; set; }
    public int CurrentCount { get; set; }
    public int Capacity { get; set; }
    public float FillPercentage { get; set; }
    public double DurationSeconds { get; set; }
}

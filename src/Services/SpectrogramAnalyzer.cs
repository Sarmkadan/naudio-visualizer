#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Threading;
using NAudioVisualizer.Constants;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Exceptions;

namespace NAudioVisualizer.Services;

/// <summary>
/// Service for generating spectrogram visualization (time-frequency representation).
/// </summary>
public class SpectrogramAnalyzer
{
    private readonly SpectrumAnalyzer _spectrumAnalyzer;
    private readonly Queue<SpectrumData> _spectrumBuffer;
    private int _maxFramesInBuffer;
    // Guards _spectrumBuffer against concurrent access from the audio-capture
    // callback thread and the render thread, preventing the deadlock that
    // occurs at sample rates above 96 kHz where the callback produces frames
    // faster than the canvas can be flushed.
    private readonly object _bufferLock = new();

    public SpectrogramAnalyzer()
    {
        _spectrumAnalyzer = new SpectrumAnalyzer();
        _spectrumBuffer = new Queue<SpectrumData>();
        _maxFramesInBuffer = 100;
    }

    /// <summary>
    /// Builds a spectrogram from multiple audio frames using a rolling analysis window.
    /// </summary>
    public SpectrogramData BuildSpectrogram(
        AudioFrame[] frames,
        int fftSize = AudioConstants.DEFAULT_FFT_SIZE,
        int hopSize = 512)
    {
        if (frames is null || frames.Length == 0)
            throw new ArgumentException("Frames collection cannot be null or empty", nameof(frames));

        try
        {
            var spectrogramMatrix = new List<float[]>();
            int sampleRate = frames[0].SampleRate;

            // Analyze each frame to get spectrum
            foreach (var frame in frames)
            {
                var spectrum = _spectrumAnalyzer.AnalyzeSpectrum(frame, fftSize);
                spectrogramMatrix.Add(spectrum.GetData());
            }

            // Convert list to 2D array
            var matrix = spectrogramMatrix.ToArray();

            var spectrogram = new SpectrogramData(matrix, sampleRate, fftSize, hopSize)
            {
                SourceFrame = frames.Length > 0 ? frames[frames.Length - 1] : null
            };

            return spectrogram;
        }
        catch (Exception ex)
        {
            throw new VisualizationException(
                $"Failed to build spectrogram: {ex.Message}",
                "Spectrogram",
                ex
            );
        }
    }

    /// <summary>
    /// Adds a spectrum frame to the rolling spectrogram buffer.
    /// When the buffer is full the oldest frame is silently dropped so that
    /// high-rate audio callbacks (e.g. 192 kHz) never block the render thread.
    /// </summary>
    public void AddSpectrumFrame(SpectrumData spectrum)
    {
        if (spectrum is null)
            throw new ArgumentNullException(nameof(spectrum));

        lock (_bufferLock)
        {
            _spectrumBuffer.Enqueue(spectrum);

            // Drop oldest frames when the buffer is full rather than letting it grow unboundedly.
            while (_spectrumBuffer.Count > _maxFramesInBuffer)
            {
                _spectrumBuffer.Dequeue();
            }
        }
    }

    /// <summary>
    /// Gets the current spectrogram from buffered spectrum frames.
    /// </summary>
    public SpectrogramData? GetCurrentSpectrogram()
    {
        SpectrumData[] frames;

        lock (_bufferLock)
        {
            if (_spectrumBuffer.Count == 0)
                return null;

            frames = _spectrumBuffer.ToArray();
        }

        if (frames.Length == 0)
            return null;

        try
        {
            var spectrogramMatrix = new float[frames.Length][];

            for (int i = 0; i < frames.Length; i++)
            {
                spectrogramMatrix[i] = frames[i].GetData();
            }

            var spectrogram = new SpectrogramData(
                spectrogramMatrix,
                frames[0].SampleRate,
                frames[0].FftSize,
                frames[0].FftSize / 2
            );

            return spectrogram;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Clears the spectrum buffer.
    /// </summary>
    public void ClearBuffer()
    {
        lock (_bufferLock)
        {
            _spectrumBuffer.Clear();
        }
    }

    /// <summary>
    /// Sets the maximum number of frames to keep in the buffer.
    /// </summary>
    public void SetBufferSize(int maxFrames)
    {
        if (maxFrames <= 0)
            throw new ArgumentException("Max frames must be positive", nameof(maxFrames));

        lock (_bufferLock)
        {
            _maxFramesInBuffer = maxFrames;

            while (_spectrumBuffer.Count > maxFrames)
            {
                _spectrumBuffer.Dequeue();
            }
        }
    }

    /// <summary>
    /// Gets the number of frames currently in the buffer.
    /// </summary>
    public int GetBufferFrameCount()
    {
        lock (_bufferLock)
        {
            return _spectrumBuffer.Count;
        }
    }

    /// <summary>
    /// Applies logarithmic scaling to spectrogram magnitude values.
    /// </summary>
    public void ApplyLogScaling(SpectrogramData spectrogram)
    {
        if (spectrogram is null)
            throw new ArgumentNullException(nameof(spectrogram));

        spectrogram.ApplyLogScale();
    }

    /// <summary>
    /// Normalizes spectrogram to 0-1 range.
    /// </summary>
    public void NormalizeSpectrogram(SpectrogramData spectrogram)
    {
        if (spectrogram is null)
            throw new ArgumentNullException(nameof(spectrogram));

        spectrogram.Normalize();
    }

    /// <summary>
    /// Extracts a frequency slice across time.
    /// </summary>
    public float[] GetFrequencySlice(SpectrogramData spectrogram, float frequencyHz)
    {
        if (spectrogram is null)
            throw new ArgumentNullException(nameof(spectrogram));

        float frequencyResolution = (float)spectrogram.SampleRate / spectrogram.FftSize;
        int frequencyIndex = (int)(frequencyHz / frequencyResolution);

        if (frequencyIndex < 0 || frequencyIndex >= spectrogram.FrequencyBins)
            throw new ArgumentException("Frequency out of range", nameof(frequencyHz));

        return spectrogram.GetFrequencySlice(frequencyIndex);
    }

    /// <summary>
    /// Extracts a time slice at a specific time point.
    /// </summary>
    public float[] GetTimeSlice(SpectrogramData spectrogram, double timeSeconds)
    {
        if (spectrogram is null)
            throw new ArgumentNullException(nameof(spectrogram));

        int timeIndex = (int)(timeSeconds / spectrogram.TimePerFrame);

        if (timeIndex < 0 || timeIndex >= spectrogram.TimeFrames)
            throw new ArgumentException("Time out of range", nameof(timeSeconds));

        return spectrogram.GetTimeFrame(timeIndex);
    }

    /// <summary>
    /// Calculates spectral flux (change between consecutive frames).
    /// </summary>
    public float[] CalculateSpectralFlux(SpectrogramData spectrogram)
    {
        if (spectrogram is null)
            throw new ArgumentNullException(nameof(spectrogram));

        var flux = new float[spectrogram.TimeFrames];

        for (int t = 0; t < spectrogram.TimeFrames; t++)
        {
            if (t == 0)
            {
                flux[t] = 0f;
                continue;
            }

            var currentFrame = spectrogram.GetTimeFrame(t);
            var previousFrame = spectrogram.GetTimeFrame(t - 1);

            float difference = 0f;
            for (int f = 0; f < Math.Min(currentFrame.Length, previousFrame.Length); f++)
            {
                float delta = currentFrame[f] - previousFrame[f];
                difference += delta * delta;
            }

            flux[t] = (float)Math.Sqrt(difference);
        }

        return flux;
    }

    /// <summary>
    /// Detects attack transients in the spectrogram using spectral flux.
    /// </summary>
    public List<int> DetectTransients(SpectrogramData spectrogram, float threshold = 0.5f)
    {
        if (spectrogram is null)
            throw new ArgumentNullException(nameof(spectrogram));

        var transients = new List<int>();
        var flux = CalculateSpectralFlux(spectrogram);

        float maxFlux = 0f;
        foreach (var f in flux)
        {
            if (f > maxFlux)
                maxFlux = f;
        }

        float fluxThreshold = maxFlux * threshold;

        for (int i = 1; i < flux.Length - 1; i++)
        {
            // Detect peaks in spectral flux
            if (flux[i] > fluxThreshold &&
                flux[i] > flux[i - 1] &&
                flux[i] > flux[i + 1])
            {
                transients.Add(i);
            }
        }

        return transients;
    }
}

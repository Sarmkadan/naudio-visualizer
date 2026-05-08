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
/// Frames are queued in a thread-safe rolling buffer so that high-rate audio capture
/// callbacks (e.g. 192 kHz interfaces) cannot starve or deadlock the render thread.
/// </summary>
/// <example>
/// <code>
/// var analyzer = new SpectrogramAnalyzer();
/// analyzer.SetBufferSize(200);
///
/// // Called from the audio capture callback:
/// var spectrum = spectrumAnalyzer.AnalyzeSpectrum(audioFrame);
/// analyzer.AddSpectrumFrame(spectrum);
///
/// // Called from the render loop:
/// SpectrogramData? specto = analyzer.GetCurrentSpectrogram();
/// if (specto is not null)
///     analyzer.ApplyLogScaling(specto);
/// </code>
/// </example>
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
    /// Builds a spectrogram from an ordered collection of audio frames using a rolling
    /// analysis window. Each frame is individually transformed with FFT and the results
    /// are assembled into a 2-D time × frequency matrix.
    /// </summary>
    /// <param name="frames">
    /// Non-empty ordered array of audio frames. All frames should share the same
    /// sample rate and channel layout.
    /// </param>
    /// <param name="fftSize">
    /// FFT window size in samples (power of two). Defaults to
    /// <see cref="AudioConstants.DEFAULT_FFT_SIZE"/> (2048).
    /// Larger values increase frequency resolution but reduce time resolution.
    /// </param>
    /// <param name="hopSize">
    /// Number of samples between consecutive FFT windows (overlap stride).
    /// Defaults to 512. Smaller values increase time resolution at higher CPU cost.
    /// </param>
    /// <returns>
    /// A <see cref="SpectrogramData"/> whose matrix has dimensions
    /// [<c>frames.Length</c>][<c>fftSize / 2</c>].
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="frames"/> is null or empty.</exception>
    /// <exception cref="VisualizationException">Thrown when FFT processing fails on any frame.</exception>
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
    /// Sets the maximum number of spectrum frames retained in the rolling buffer.
    /// Frames beyond this limit are silently dropped (oldest first) to prevent
    /// memory growth at high audio sample rates.
    /// </summary>
    /// <param name="maxFrames">Maximum frame count. Must be greater than zero.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="maxFrames"/> is not positive.</exception>
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
    /// Applies logarithmic dB scaling to every magnitude value in the spectrogram matrix
    /// using the formula <c>20 × log₁₀(|magnitude| / referenceValue)</c>.
    /// Call this before rendering to map linear FFT magnitudes to perceptual loudness.
    /// </summary>
    /// <param name="spectrogram">The spectrogram data to modify in place.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="spectrogram"/> is null.</exception>
    public void ApplyLogScaling(SpectrogramData spectrogram)
    {
        if (spectrogram is null)
            throw new ArgumentNullException(nameof(spectrogram));

        spectrogram.ApplyLogScale();
    }

    /// <summary>
    /// Normalizes all spectrogram magnitude values to the [0, 1] range.
    /// After normalization the values can be mapped directly to a color palette.
    /// </summary>
    /// <param name="spectrogram">The spectrogram data to normalize in place.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="spectrogram"/> is null.</exception>
    public void NormalizeSpectrogram(SpectrogramData spectrogram)
    {
        if (spectrogram is null)
            throw new ArgumentNullException(nameof(spectrogram));

        spectrogram.Normalize();
    }

    /// <summary>
    /// Returns a time-series of the magnitude at a specific frequency across all frames.
    /// </summary>
    /// <param name="spectrogram">The source spectrogram.</param>
    /// <param name="frequencyHz">
    /// Target frequency in Hz. Must be within [0, SampleRate / 2].
    /// </param>
    /// <returns>
    /// Array of length <see cref="SpectrogramData.TimeFrames"/> containing the magnitude
    /// at the nearest frequency bin for each time frame.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="spectrogram"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="frequencyHz"/> maps to an out-of-range bin.</exception>
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
    /// Returns the full frequency spectrum at a specific point in time.
    /// </summary>
    /// <param name="spectrogram">The source spectrogram.</param>
    /// <param name="timeSeconds">
    /// Offset from the start of the spectrogram in seconds.
    /// Must be in the range [0, TimeFrames × TimePerFrame).
    /// </param>
    /// <returns>
    /// Array of length <see cref="SpectrogramData.FrequencyBins"/> containing the
    /// magnitude at every frequency bin for the nearest time frame.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="spectrogram"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="timeSeconds"/> maps to an out-of-range frame.</exception>
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
    /// Calculates spectral flux—the Euclidean distance between consecutive time frames—
    /// which is commonly used as an onset strength signal for beat/transient detection.
    /// </summary>
    /// <param name="spectrogram">The source spectrogram.</param>
    /// <returns>
    /// Array of length <see cref="SpectrogramData.TimeFrames"/> where index 0 is always 0
    /// and subsequent values represent the per-frame magnitude change.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="spectrogram"/> is null.</exception>
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
    /// Detects attack transients by finding local maxima in the spectral flux curve
    /// that exceed a fraction of the peak flux.
    /// </summary>
    /// <param name="spectrogram">The source spectrogram.</param>
    /// <param name="threshold">
    /// Fraction of the maximum spectral flux value used as the detection threshold.
    /// Must be in (0, 1]. Defaults to 0.5 (50 % of peak flux).
    /// Lower values are more sensitive; higher values require stronger onsets.
    /// </param>
    /// <returns>
    /// List of time-frame indices at which transient onsets were detected.
    /// An empty list is returned when no onsets exceed the threshold.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="spectrogram"/> is null.</exception>
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

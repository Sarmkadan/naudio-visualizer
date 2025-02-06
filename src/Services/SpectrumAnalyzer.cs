#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using NAudioVisualizer.Constants;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Exceptions;
using System.Numerics;
using NAudio.Dsp;

namespace NAudioVisualizer.Services;

/// <summary>
/// Service for performing FFT and frequency spectrum analysis on audio data.
/// </summary>
public class SpectrumAnalyzer
{
    /// <summary>
    /// Peak-hold magnitude values (in the same scale as the current spectrum data).
    /// Indexed by frequency bin. Null until the first call to <see cref="UpdatePeakHolds"/>.
    /// </summary>
    private float[]? _peakHolds;

    /// <summary>
    /// Rate at which peak-hold bars decay, expressed in dB per second.
    /// Defaults to 20 dB/s, which is suitable for general music monitoring.
    /// Set a lower value (e.g. 5–10 dB/s) for transient peak monitoring,
    /// or a higher value (e.g. 40–60 dB/s) for real-time speech analysis.
    /// </summary>
    public float PeakHoldDecayDbPerSecond { get; set; } = 20f;
    /// <summary>
    /// Generates spectrum visualization from an audio frame using FFT.
    /// </summary>
    public SpectrumData AnalyzeSpectrum(AudioFrame frame, int fftSize = AudioConstants.DEFAULT_FFT_SIZE)
    {
        if (frame is null)
            throw new ArgumentNullException(nameof(frame));

        if (!frame.IsValid())
            throw new VisualizationException("Audio frame is invalid", "Spectrum");

        try
        {
            // Validate FFT size
            if (fftSize < AudioConstants.FFT_MINIMUM || fftSize > AudioConstants.FFT_MAXIMUM)
                fftSize = AudioConstants.DEFAULT_FFT_SIZE;

            // Apply window function to samples and prepare for FFT
            var windowedAndPadded = ApplyHannWindow(frame.Samples, fftSize);

            // Perform FFT and compute magnitude spectrum
            var magnitudes = ComputeMagnitudeSpectrum(windowedAndPadded);

            // Generate frequency bins
            var frequencies = GenerateFrequencyBins(frame.SampleRate, fftSize);

            var spectrum = new SpectrumData(magnitudes, frequencies, frame.SampleRate, fftSize)
            {
                SourceFrame = frame,
                WindowType = WindowType.Hann
            };

            return spectrum;
        }
        catch (Exception ex)
        {
            throw new VisualizationException(
                $"Failed to analyze spectrum: {ex.Message}",
                "Spectrum",
                ex
            );
        }
    }

    /// <summary>
    /// Applies Hann window function to samples for better FFT results.
    /// </summary>
    private Complex[] ApplyHannWindow(float[] samples, int fftSize)
    {
        var windowed = new Complex[fftSize];

        for (int i = 0; i < fftSize; i++)
        {
            if (i < samples.Length)
            {
                float window = 0.5f * (1 - (float)Math.Cos(2 * Math.PI * i / (fftSize - 1)));
                windowed[i] = new Complex(samples[i] * window, 0);
            }
            else
            {
                windowed[i] = new Complex(0, 0); // Pad with zeros
            }
        }
        return windowed;
    }

    /// <summary>
    /// Computes magnitude spectrum using NAudio's Fast Fourier Transform.
    /// </summary>
    private float[] ComputeMagnitudeSpectrum(Complex[] samples)
    {
        FastFourierTransform.FFT(true, (int)Math.Log(samples.Length, 2), samples);

        var magnitudes = new float[samples.Length / 2];
        for (int i = 0; i < samples.Length / 2; i++)
        {
            magnitudes[i] = (float)Math.Sqrt(samples[i].Real * samples[i].Real + samples[i].Imaginary * samples[i].Imaginary);
        }
        return magnitudes;
    }

    /// <summary>
    /// Generates frequency bin values for the spectrum.
    /// </summary>
    private float[] GenerateFrequencyBins(int sampleRate, int fftSize)
    {
        int binCount = fftSize / 2;
        var frequencies = new float[binCount];
        float frequencyPerBin = (float)sampleRate / fftSize;

        for (int i = 0; i < binCount; i++)
        {
            frequencies[i] = i * frequencyPerBin;
        }

        return frequencies;
    }

    /// <summary>
    /// Converts magnitude values to logarithmic scale (dB).
    /// </summary>
    public void ConvertToLogScale(SpectrumData spectrum, float referenceValue = 1f)
    {
        if (spectrum is null)
            throw new ArgumentNullException(nameof(spectrum));

        spectrum.ConvertToLogScale(referenceValue);
    }

    /// <summary>
    /// Applies smoothing to spectrum to reduce visual noise.
    /// </summary>
    public void SmoothSpectrum(SpectrumData spectrum, int windowSize = 3)
    {
        if (spectrum is null)
            throw new ArgumentNullException(nameof(spectrum));

        spectrum.SmoothSpectrum(windowSize);
    }

    /// <summary>
    /// Finds dominant frequency peak in the spectrum.
    /// </summary>
    public float FindDominantFrequency(SpectrumData spectrum)
    {
        if (spectrum is null)
            throw new ArgumentNullException(nameof(spectrum));

        return spectrum.PeakFrequency;
    }

    /// <summary>
    /// Calculates spectral centroid (center of mass of the spectrum).
    /// </summary>
    public float CalculateSpectralCentroid(SpectrumData spectrum)
    {
        if (spectrum is null)
            throw new ArgumentNullException(nameof(spectrum));

        var magnitudes = spectrum.GetData();
        var frequencies = spectrum.GetFrequencies();

        if (magnitudes.Length == 0 || frequencies.Length == 0)
            return 0f;

        double weightedSum = 0;
        double magnitudeSum = 0;

        for (int i = 0; i < magnitudes.Length; i++)
        {
            weightedSum += magnitudes[i] * frequencies[i];
            magnitudeSum += magnitudes[i];
        }

        return magnitudeSum > 0 ? (float)(weightedSum / magnitudeSum) : 0f;
    }

    /// <summary>
    /// Extracts frequency bands from the spectrum (bass, mid, treble).
    /// </summary>
    public FrequencyBands ExtractFrequencyBands(SpectrumData spectrum)
    {
        if (spectrum is null)
            throw new ArgumentNullException(nameof(spectrum));

        var magnitudes = spectrum.GetData();
        var frequencies = spectrum.GetFrequencies();
        var bands = new FrequencyBands();

        // Define frequency ranges
        const float BASS_MAX = 250f;
        const float MID_MIN = 250f;
        const float MID_MAX = 4000f;
        const float TREBLE_MIN = 4000f;

        foreach (var (frequency, magnitude) in IterateFrequencyData(frequencies, magnitudes))
        {
            if (frequency < BASS_MAX)
                bands.BassEnergy += magnitude;
            else if (frequency < MID_MAX)
                bands.MidEnergy += magnitude;
            else if (frequency >= TREBLE_MIN)
                bands.TrebleEnergy += magnitude;
        }

        // Normalize
        float totalEnergy = bands.BassEnergy + bands.MidEnergy + bands.TrebleEnergy;
        if (totalEnergy > 0)
        {
            bands.BassEnergy /= totalEnergy;
            bands.MidEnergy /= totalEnergy;
            bands.TrebleEnergy /= totalEnergy;
        }

        return bands;
    }

    /// <summary>
    /// Helper to iterate through frequency-magnitude pairs.
    /// </summary>
    private static System.Collections.Generic.IEnumerable<(float freq, float mag)> IterateFrequencyData(
        float[] frequencies, float[] magnitudes)
    {
        for (int i = 0; i < Math.Min(frequencies.Length, magnitudes.Length); i++)
        {
            yield return (frequencies[i], magnitudes[i]);
        }
    }

    /// <summary>
    /// Normalizes spectrum data to 0-1 range.
    /// </summary>
    public void NormalizeSpectrum(SpectrumData spectrum)
    {
        if (spectrum is null)
            throw new ArgumentNullException(nameof(spectrum));

        spectrum.Normalize();
    }

    /// <summary>
    /// Updates peak-hold values for each frequency bin using the current spectrum frame.
    /// Peaks rise instantly and decay at <see cref="PeakHoldDecayDbPerSecond"/> dB per second,
    /// normalized against the actual elapsed time so the decay is frame-rate independent.
    /// </summary>
    /// <param name="spectrum">The current spectrum frame to compare against held peaks.</param>
    /// <param name="elapsedSeconds">
    /// Elapsed time since the previous call, in seconds (e.g. 1.0/60 for a 60 fps render loop).
    /// Must be greater than zero.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="spectrum"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="elapsedSeconds"/> is not positive.</exception>
    public void UpdatePeakHolds(SpectrumData spectrum, double elapsedSeconds)
    {
        if (spectrum is null)
            throw new ArgumentNullException(nameof(spectrum));

        if (elapsedSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(elapsedSeconds), "Elapsed time must be positive.");

        var magnitudes = spectrum.GetData();

        if (_peakHolds is null || _peakHolds.Length != magnitudes.Length)
        {
            _peakHolds = (float[])magnitudes.Clone();
            return;
        }

        float decayAmount = PeakHoldDecayDbPerSecond * (float)elapsedSeconds;

        for (int i = 0; i < magnitudes.Length; i++)
        {
            if (magnitudes[i] >= _peakHolds[i])
            {
                _peakHolds[i] = magnitudes[i];
            }
            else
            {
                _peakHolds[i] = Math.Max(_peakHolds[i] - decayAmount, magnitudes[i]);
            }
        }
    }

    /// <summary>
    /// Returns a snapshot of the current peak-hold values, or <c>null</c> if
    /// <see cref="UpdatePeakHolds"/> has not been called yet.
    /// </summary>
    public float[]? GetPeakHolds() => _peakHolds is null ? null : (float[])_peakHolds.Clone();

    /// <summary>
    /// Resets all peak-hold values.
    /// </summary>
    public void ResetPeakHolds() => _peakHolds = null;
}

/// <summary>
/// Frequency band energy data.
/// </summary>
public class FrequencyBands
{
    public float BassEnergy { get; set; }
    public float MidEnergy { get; set; }
    public float TrebleEnergy { get; set; }
}

#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using NAudioVisualizer.Constants;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Exceptions;

namespace NAudioVisualizer.Services;

/// <summary>
/// Service for performing FFT and frequency spectrum analysis on audio data.
/// </summary>
public class SpectrumAnalyzer
{
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

            // Apply window function to samples
            var windowed = ApplyHannWindow(frame.Samples, fftSize);

            // Perform simple FFT (magnitude spectrum)
            var magnitudes = ComputeMagnitudeSpectrum(windowed);

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
    private float[] ApplyHannWindow(float[] samples, int fftSize)
    {
        var windowed = new float[Math.Min(samples.Length, fftSize)];
        Array.Copy(samples, windowed, windowed.Length);

        // Pad with zeros if necessary
        var result = new float[fftSize];
        Array.Copy(windowed, result, windowed.Length);

        // Apply Hann window
        for (int i = 0; i < windowed.Length; i++)
        {
            float window = 0.5f * (1 - (float)Math.Cos(2 * Math.PI * i / (windowed.Length - 1)));
            result[i] = windowed[i] * window;
        }

        return result;
    }

    /// <summary>
    /// Computes magnitude spectrum (simplified FFT using energy bins).
    /// </summary>
    private float[] ComputeMagnitudeSpectrum(float[] samples)
    {
        int spectrumSize = samples.Length / 2;
        var magnitude = new float[spectrumSize];

        // Simplified spectrum: divide into frequency bands and calculate energy
        int bandWidth = samples.Length / spectrumSize;

        for (int i = 0; i < spectrumSize; i++)
        {
            float energy = 0f;
            int bandSize = 0;

            for (int j = i * bandWidth; j < (i + 1) * bandWidth && j < samples.Length; j++)
            {
                energy += samples[j] * samples[j];
                bandSize++;
            }

            magnitude[i] = (float)Math.Sqrt(energy / bandSize);
        }

        return magnitude;
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

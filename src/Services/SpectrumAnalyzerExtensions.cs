#nullable enable

// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using NAudioVisualizer.Domain.Models;
using NAudioVisualizer.Services;

namespace NAudioVisualizer.Services;

/// <summary>
/// Extension methods for <see cref="SpectrumAnalyzer"/> that provide additional spectrum analysis utilities.
/// </summary>
public static class SpectrumAnalyzerExtensions
{
	/// <summary>
	/// Calculates the energy ratio between bass and treble frequencies.
	/// Useful for determining if the audio is bass-heavy or treble-heavy.
	/// </summary>
	/// <param name="analyzer">The spectrum analyzer instance.</param>
	/// <param name="spectrum">The spectrum data to analyze.</param>
	/// <returns>A ratio where values greater than 1 indicate bass-heavy audio, values less than 1 indicate treble-heavy audio.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="analyzer"/> or <paramref name="spectrum"/> is null.</exception>
	public static float CalculateBassToTrebleRatio(this SpectrumAnalyzer analyzer, SpectrumData spectrum)
	{
		ArgumentNullException.ThrowIfNull(analyzer);
		ArgumentNullException.ThrowIfNull(spectrum);

		var bands = analyzer.ExtractFrequencyBands(spectrum);

		// Avoid division by zero
		return bands.TrebleEnergy < float.Epsilon
			? bands.BassEnergy > 0 ? float.MaxValue : 0f
			: bands.BassEnergy / bands.TrebleEnergy;
	}

	/// <summary>
	/// Determines if the current spectrum has significant low-frequency content (bass).
	/// </summary>
	/// <param name="analyzer">The spectrum analyzer instance.</param>
	/// <param name="spectrum">The spectrum data to analyze.</param>
	/// <param name="bassThreshold">Minimum bass energy ratio (0-1) to consider as significant bass. Defaults to 0.35.</param>
	/// <returns>True if bass energy exceeds the threshold, false otherwise.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="analyzer"/> or <paramref name="spectrum"/> is null.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="bassThreshold"/> is outside the valid range [0, 1].</exception>
	public static bool HasSignificantBass(this SpectrumAnalyzer analyzer, SpectrumData spectrum, float bassThreshold = 0.35f)
	{
		ArgumentNullException.ThrowIfNull(analyzer);
		ArgumentNullException.ThrowIfNull(spectrum);
		ArgumentOutOfRangeException.ThrowIfLessThan(bassThreshold, 0f);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(bassThreshold, 1f);

		var bands = analyzer.ExtractFrequencyBands(spectrum);
		return bands.BassEnergy >= bassThreshold;
	}

	/// <summary>
	/// Calculates the spectral flux, which measures how much the current spectrum differs from the previous one.
	/// Useful for detecting transients and musical beats.
	/// </summary>
	/// <param name="analyzer">The spectrum analyzer instance.</param>
	/// <param name="currentSpectrum">The current spectrum data.</param>
	/// <param name="previousSpectrum">The previous spectrum data, or null if this is the first frame.</param>
	/// <returns>The spectral flux value (sum of positive differences between current and previous magnitudes).</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="analyzer"/> or <paramref name="currentSpectrum"/> is null.</exception>
	public static float CalculateSpectralFlux(this SpectrumAnalyzer analyzer, SpectrumData currentSpectrum, SpectrumData? previousSpectrum)
	{
		ArgumentNullException.ThrowIfNull(analyzer);
		ArgumentNullException.ThrowIfNull(currentSpectrum);

		var currentMagnitudes = currentSpectrum.GetData();

		if (previousSpectrum is null || previousSpectrum.GetData().Length == 0)
			return 0f;

		var previousMagnitudes = previousSpectrum.GetData();
		float flux = 0f;

		int minLength = Math.Min(currentMagnitudes.Length, previousMagnitudes.Length);

		for (int i = 0; i < minLength; i++)
		{
			float difference = currentMagnitudes[i] - previousMagnitudes[i];
			if (difference > 0)
			{
				flux += difference;
			}
		}

		return flux;
	}

	/// <summary>
	/// Normalizes the spectrum data and applies smoothing in a single operation.
	/// Convenience method for common visualization pipeline.
	/// </summary>
	/// <param name="analyzer">The spectrum analyzer instance.</param>
	/// <param name="spectrum">The spectrum data to process.</param>
	/// <param name="smoothingWindowSize">Size of the smoothing window. Must be ≥ 2 and less than the number of bins. Defaults to 3.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="analyzer"/> or <paramref name="spectrum"/> is null.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="smoothingWindowSize"/> is less than 2.</exception>
	public static void NormalizeAndSmooth(this SpectrumAnalyzer analyzer, SpectrumData spectrum, int smoothingWindowSize = 3)
	{
		ArgumentNullException.ThrowIfNull(analyzer);
		ArgumentNullException.ThrowIfNull(spectrum);
		ArgumentOutOfRangeException.ThrowIfLessThan(smoothingWindowSize, 2);

		analyzer.NormalizeSpectrum(spectrum);
		analyzer.SmoothSpectrum(spectrum, smoothingWindowSize);
	}
}
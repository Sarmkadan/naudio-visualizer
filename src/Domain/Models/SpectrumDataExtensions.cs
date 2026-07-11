namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides extension methods for <see cref="SpectrumData"/>.
/// </summary>
public static class SpectrumDataExtensions
{
    /// <summary>
    /// Determines if the spectrum data has a valid peak frequency and magnitude.
    /// </summary>
    /// <param name="spectrumData">The spectrum data to validate.</param>
    /// <returns><c>true</c> if the spectrum data has a valid peak frequency and magnitude; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="spectrumData"/> is <c>null</c>.</exception>
    public static bool HasValidPeak(this SpectrumData spectrumData)
    {
        ArgumentNullException.ThrowIfNull(spectrumData);

        return spectrumData.PeakFrequency > 0 && spectrumData.PeakMagnitude > 0;
    }

    /// <summary>
    /// Calculates the frequency index of the peak frequency in the spectrum data.
    /// </summary>
    /// <param name="spectrumData">The spectrum data to calculate the frequency index for.</param>
    /// <returns>The frequency index of the peak frequency.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="spectrumData"/> is <c>null</c>.</exception>
    public static int GetPeakFrequencyIndex(this SpectrumData spectrumData)
    {
        ArgumentNullException.ThrowIfNull(spectrumData);

        var frequencies = spectrumData.GetFrequencies();
        var peakFrequency = spectrumData.PeakFrequency;

        for (int i = 0; i < frequencies.Length; i++)
        {
            if (frequencies[i] == peakFrequency)
            {
                return i;
            }
        }

        throw new InvalidOperationException("Peak frequency not found in frequencies array.");
    }

    /// <summary>
    /// Normalizes the spectrum data to a maximum magnitude of 1.
    /// </summary>
    /// <param name="spectrumData">The spectrum data to normalize.</param>
    /// <exception cref="ArgumentNullException"><paramref name="spectrumData"/> is <c>null</c>.</exception>
    public static void NormalizeToOne(this SpectrumData spectrumData)
    {
        ArgumentNullException.ThrowIfNull(spectrumData);

        var maxMagnitude = spectrumData.GetData().Max();

        if (maxMagnitude > 0)
        {
            var data = spectrumData.GetData();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] /= maxMagnitude;
            }
        }
    }
}

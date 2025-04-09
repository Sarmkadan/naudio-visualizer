namespace NAudioVisualizer.Domain.Models;

/// <summary>
/// Provides extension methods for <see cref="VisualizationData"/>.
/// </summary>
public static class VisualizationDataExtensions
{
    /// <summary>
    /// Determines if the <see cref="VisualizationData"/> is within a valid age range.
    /// </summary>
    /// <param name="data">The <see cref="VisualizationData"/> to validate.</param>
    /// <param name="maxAge">The maximum age in seconds.</param>
    /// <returns><c>true</c> if the data is within the valid age range; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
    public static bool IsWithinValidAgeRange(this VisualizationData data, float maxAge)
    {
        ArgumentNullException.ThrowIfNull(data);

        var age = (DateTime.UtcNow - data.GeneratedAt).TotalSeconds;
        return age <= maxAge;
    }

    /// <summary>
    /// Scales the data points of the <see cref="VisualizationData"/> to a specified range.
    /// </summary>
    /// <param name="data">The <see cref="VisualizationData"/> to scale.</param>
    /// <param name="minValue">The minimum value of the scaled range.</param>
    /// <param name="maxValue">The maximum value of the scaled range.</param>
    /// <returns>An array of scaled data points.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
    public static IReadOnlyList<float> ScaleDataPoints(this VisualizationData data, float minValue, float maxValue)
    {
        ArgumentNullException.ThrowIfNull(data);

        var dataPoints = data.GetData();
        var scaledDataPoints = new float[dataPoints.Length];

        var dataMin = data.MinValue;
        var dataMax = data.MaxValue;

        for (int i = 0; i < dataPoints.Length; i++)
        {
            scaledDataPoints[i] = (dataPoints[i] - dataMin) / (dataMax - dataMin) * (maxValue - minValue) + minValue;
        }

        return scaledDataPoints;
    }
}

using System;

namespace NAudioVisualizer.Exceptions
{
    /// <summary>
    /// Provides extension methods for the <see cref="VisualizationException"/> class.
    /// </summary>
    public static class VisualizationExceptionExtensions
    {
        /// <summary>
        /// Returns a formatted message that includes the exception message and the associated visualization type, if available.
        /// </summary>
        /// <param name="exception">The <see cref="VisualizationException"/> to format.</param>
        /// <returns>A formatted string containing the exception message and visualization type.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null.</exception>
        public static string GetFormattedMessage(this VisualizationException exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            return exception.VisualizationType is not null
                ? $"{exception.Message} (Visualization Type: {exception.VisualizationType})"
                : exception.Message;
        }

        /// <summary>
        /// Determines whether the exception is associated with a specific visualization type.
        /// </summary>
        /// <param name="exception">The <see cref="VisualizationException"/> to check.</param>
        /// <param name="visualizationType">The visualization type to compare against.</param>
        /// <returns><c>true</c> if the exception's <see cref="VisualizationException.VisualizationType"/> matches the specified type; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="visualizationType"/> is null or empty.</exception>
        public static bool IsVisualizationType(this VisualizationException exception, string visualizationType)
        {
            ArgumentNullException.ThrowIfNull(exception);
            ArgumentException.ThrowIfNullOrEmpty(visualizationType);

            return string.Equals(exception.VisualizationType, visualizationType, StringComparison.Ordinal);
        }

        /// <summary>
        /// Creates a new <see cref="VisualizationException"/> with the same message and inner exception, but with the specified visualization type.
        /// </summary>
        /// <param name="exception">The original <see cref="VisualizationException"/>.</param>
        /// <param name="visualizationType">The visualization type to associate with the new exception.</param>
        /// <returns>A new <see cref="VisualizationException"/> with the updated visualization type.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="visualizationType"/> is null or empty.</exception>
        public static VisualizationException WithVisualizationType(this VisualizationException exception, string visualizationType)
        {
            ArgumentNullException.ThrowIfNull(exception);
            ArgumentException.ThrowIfNullOrEmpty(visualizationType);

            return new VisualizationException(exception.Message, exception)
            {
                VisualizationType = visualizationType
            };
        }
    }
}

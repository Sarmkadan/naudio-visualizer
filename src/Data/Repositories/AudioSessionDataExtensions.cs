namespace NAudioVisualizer.Data.Repositories;

/// <summary>
/// Extension methods for <see cref="AudioSessionData"/>.
/// </summary>
public static class AudioSessionDataExtensions
{
    /// <summary>
    /// Determines whether the audio session is active.
    /// </summary>
    /// <param name="session">The audio session.</param>
    /// <returns><see langword="true"/> when the session has no end time; otherwise, <see langword="false"/>.</returns>
    public static bool IsActive(this AudioSessionData session)
    {
        return session.EndTime is null;
    }
}

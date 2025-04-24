using System;

namespace NAudioVisualizer.Exceptions
{
	/// <summary>
	/// Extension methods that add useful behaviour to <see cref="AudioDeviceException"/>.
	/// </summary>
	public static class AudioDeviceExceptionExtensions
	{
		/// <summary>
		/// Returns the device index if it is set; otherwise returns the supplied <paramref name="defaultValue"/>.
		/// </summary>
		/// <param name="ex">The exception instance.</param>
		/// <param name="defaultValue">The value to return when <see cref="AudioDeviceException.DeviceIndex"/> is null.</param>
		/// <returns>The device index if set; otherwise the default value.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="ex"/> is <see langword="null"/>.</exception>
		public static int GetDeviceIndexOrDefault(this AudioDeviceException ex, int defaultValue = -1)
		{
			ArgumentNullException.ThrowIfNull(ex);
			return ex.DeviceIndex ?? defaultValue;
		}

		/// <summary>
		/// Creates a new <see cref="AudioDeviceException"/> with the same device index (if any) but a different message.
		/// </summary>
		/// <param name="ex">The exception instance.</param>
		/// <param name="newMessage">The new exception message.</param>
		/// <returns>A new exception with the same device index and the specified message.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="ex"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="newMessage"/> is <see langword="null"/>.</exception>
		public static AudioDeviceException WithMessage(this AudioDeviceException ex, string newMessage)
		{
			ArgumentNullException.ThrowIfNull(ex);
			ArgumentNullException.ThrowIfNull(newMessage);

			return ex.DeviceIndex.HasValue
				? new AudioDeviceException(newMessage, ex.DeviceIndex.Value)
				: new AudioDeviceException(newMessage);
		}

		/// <summary>
		/// Returns a concise string that can be used for logging.
		/// </summary>
		/// <param name="ex">The exception instance.</param>
		/// <returns>A formatted string suitable for logging purposes.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="ex"/> is <see langword="null"/>.</exception>
		public static string ToLogString(this AudioDeviceException ex)
		{
			ArgumentNullException.ThrowIfNull(ex);

			var indexPart = ex.DeviceIndex.HasValue ? ex.DeviceIndex.Value.ToString() : "null";
			return $"AudioDeviceException: {ex.Message} (DeviceIndex: {indexPart})\n";
		}

		/// <summary>
		/// Indicates whether the exception carries a device index.
		/// </summary>
		/// <param name="ex">The exception instance.</param>
		/// <returns><see langword="true"/> if the exception has a device index; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="ex"/> is <see langword="null"/>.</exception>
		public static bool HasDeviceIndex(this AudioDeviceException ex)
		{
			ArgumentNullException.ThrowIfNull(ex);
			return ex.DeviceIndex.HasValue;
		}
	}
}
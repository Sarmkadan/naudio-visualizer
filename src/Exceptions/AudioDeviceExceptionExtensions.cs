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
        public static int GetDeviceIndexOrDefault(this AudioDeviceException ex, int defaultValue = -1)
        {
            return ex.DeviceIndex ?? defaultValue;
        }

        /// <summary>
        /// Creates a new <see cref="AudioDeviceException"/> with the same device index (if any) but a different message.
        /// </summary>
        public static AudioDeviceException WithMessage(this AudioDeviceException ex, string newMessage)
        {
            if (ex.DeviceIndex.HasValue)
            {
                return new AudioDeviceException(newMessage, ex.DeviceIndex.Value);
            }

            return new AudioDeviceException(newMessage);
        }

        /// <summary>
        /// Returns a concise string that can be used for logging.
        /// </summary>
        public static string ToLogString(this AudioDeviceException ex)
        {
            var indexPart = ex.DeviceIndex.HasValue ? ex.DeviceIndex.Value.ToString() : "null";
            return $"AudioDeviceException: {ex.Message} (DeviceIndex: {indexPart})";
        }

        /// <summary>
        /// Indicates whether the exception carries a device index.
        /// </summary>
        public static bool HasDeviceIndex(this AudioDeviceException ex)
        {
            return ex.DeviceIndex.HasValue;
        }
    }
}

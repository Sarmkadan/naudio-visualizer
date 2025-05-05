using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudioVisualizer.Domain.Models
{
    /// <summary>
    /// Provides validation methods for <see cref="AudioDevice"/> instances.
    /// </summary>
    public static class AudioDeviceValidation
    {
        /// <summary>
        /// Validates an <see cref="AudioDevice"/> instance and returns a list of validation errors.
        /// </summary>
        /// <param name="value">The audio device to validate.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of validation error messages. Empty if valid.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        public static IReadOnlyList<string> Validate(this AudioDevice value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var errors = new List<string>();

            if (value.Id == Guid.Empty)
            {
                errors.Add("Id must be a non-empty GUID");
            }

            if (string.IsNullOrWhiteSpace(value.Name))
            {
                errors.Add("Name cannot be null or whitespace");
            }

            if (value.DeviceIndex < 0)
            {
                errors.Add("DeviceIndex cannot be negative");
            }

            if (string.IsNullOrWhiteSpace(value.Manufacturer))
            {
                errors.Add("Manufacturer cannot be null or whitespace");
            }

            if (value.ChannelCount <= 0)
            {
                errors.Add("ChannelCount must be greater than 0");
            }

            if (value.SupportedSampleRates == null)
            {
                errors.Add("SupportedSampleRates cannot be null");
            }
            else if (value.SupportedSampleRates.Count == 0)
            {
                errors.Add("SupportedSampleRates must contain at least one sample rate");
            }
            else
            {
                foreach (var rate in value.SupportedSampleRates)
                {
                    if (rate <= 0)
                    {
                        errors.Add($"SupportedSampleRates contains invalid value {rate}. Sample rates must be positive");
                    }
                }
            }

            if (value.DefaultSampleRate <= 0)
            {
                errors.Add("DefaultSampleRate must be greater than 0");
            }

            if (value.BitDepth <= 0)
            {
                errors.Add("BitDepth must be greater than 0");
            }

            if (value.LastStatusCheck == default)
            {
                errors.Add("LastStatusCheck must be set to a valid DateTime");
            }

            if (value.Capabilities == null)
            {
                errors.Add("Capabilities cannot be null");
            }

            return errors.AsReadOnly();
        }

        /// <summary>
        /// Determines whether the specified <see cref="AudioDevice"/> is valid.
        /// </summary>
        /// <param name="value">The audio device to check.</param>
        /// <returns><see langword="true"/> if the audio device is valid; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        public static bool IsValid(this AudioDevice value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return Validate(value).Count == 0;
        }

        /// <summary>
        /// Validates the specified <see cref="AudioDevice"/> and throws an exception if invalid.
        /// </summary>
        /// <param name="value">The audio device to validate.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The audio device is invalid.</exception>
        public static void EnsureValid(this AudioDevice value)
        {
            ArgumentNullException.ThrowIfNull(value);
            var errors = Validate(value);
            if (errors.Count > 0)
            {
                throw new ArgumentException(
                    $"AudioDevice validation failed:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
            }
        }
    }
}

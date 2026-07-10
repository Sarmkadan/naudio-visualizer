using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudioVisualizer.Domain.Models
{
    public static class AudioDeviceValidation
    {
        public static IReadOnlyList<string> Validate(this AudioDevice value)
        {
            var errors = new List<string>();

            if (value == null)
            {
                errors.Add("AudioDevice cannot be null");
                return errors.AsReadOnly();
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

        public static bool IsValid(this AudioDevice value)
        {
            return Validate(value).Count == 0;
        }

        public static void EnsureValid(this AudioDevice value)
        {
            var errors = Validate(value);
            if (errors.Count > 0)
            {
                throw new ArgumentException(
                    $"AudioDevice validation failed:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
            }
        }
    }
}
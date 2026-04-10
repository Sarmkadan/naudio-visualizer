#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace NAudioVisualizer.CLI;

/// <summary>
/// Parses command-line arguments into structured command objects.
/// This parser supports both positional and flag-based arguments with validation.
/// </summary>
public class CommandLineParser
{
    private readonly Dictionary<string, string> _parsedArgs;
    private readonly List<string> _positionalArgs;
    private const string HelpFlag = "--help";
    private const string VersionFlag = "--version";

    /// <summary>
    /// Initializes a new instance of the command line parser.
    /// </summary>
    public CommandLineParser()
    {
        _parsedArgs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        _positionalArgs = new List<string>();
    }

    /// <summary>
    /// Parses the provided command-line arguments into flags and positional values.
    /// Flags are expected in the format --flag=value or --flag value.
    /// </summary>
    public void Parse(string[] args)
    {
        if (args is null || args.Length == 0)
            return;

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];

            // Handle flag arguments
            if (arg.StartsWith("--"))
            {
                if (arg.Contains('='))
                {
                    string[] parts = arg.Substring(2).Split('=', 2);
                    _parsedArgs[parts[0]] = parts.Length > 1 ? parts[1] : string.Empty;
                }
                else
                {
                    string flagName = arg.Substring(2);
                    // Try to get the next argument as the value
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                    {
                        _parsedArgs[flagName] = args[++i];
                    }
                    else
                    {
                        _parsedArgs[flagName] = "true"; // Boolean flag
                    }
                }
            }
            else if (arg.StartsWith("-") && arg.Length == 2)
            {
                // Handle short flags like -v
                char flagChar = arg[1];
                if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                {
                    _parsedArgs[flagChar.ToString()] = args[++i];
                }
            }
            else
            {
                // Positional argument
                _positionalArgs.Add(arg);
            }
        }
    }

    /// <summary>
    /// Gets a flag value by name. Returns null if the flag is not present.
    /// </summary>
    public string? GetFlagValue(string flagName)
    {
        return _parsedArgs.TryGetValue(flagName, out var value) ? value : null;
    }

    /// <summary>
    /// Gets a boolean flag value, returning true if the flag is present.
    /// </summary>
    public bool GetBoolFlag(string flagName, bool defaultValue = false)
    {
        if (!_parsedArgs.TryGetValue(flagName, out var value))
            return defaultValue;

        return value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
               string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// Gets all positional arguments in the order they were provided.
    /// </summary>
    public IReadOnlyList<string> GetPositionalArgs() => _positionalArgs.AsReadOnly();

    /// <summary>
    /// Gets the first positional argument, or null if none exist.
    /// </summary>
    public string? GetCommand() => _positionalArgs.Count > 0 ? _positionalArgs[0] : null;

    /// <summary>
    /// Checks if a flag is present in the parsed arguments.
    /// </summary>
    public bool HasFlag(string flagName) => _parsedArgs.ContainsKey(flagName);

    /// <summary>
    /// Checks if the help flag was provided.
    /// </summary>
    public bool ShowHelp => HasFlag("help") || HasFlag("h");

    /// <summary>
    /// Checks if the version flag was provided.
    /// </summary>
    public bool ShowVersion => HasFlag("version") || HasFlag("v");

    /// <summary>
    /// Validates that all required flags are present in the parsed arguments.
    /// </summary>
    public bool ValidateRequiredFlags(params string[] requiredFlags)
    {
        return requiredFlags.All(flag => HasFlag(flag));
    }
}

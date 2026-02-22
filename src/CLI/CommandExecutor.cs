#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using NAudioVisualizer.Infrastructure;

namespace NAudioVisualizer.CLI;

/// <summary>
/// Executes parsed commands and manages the command execution pipeline.
/// Commands are registered with their handlers and executed through a dispatcher pattern.
/// </summary>
public class CommandExecutor
{
    private readonly Dictionary<string, Func<CommandLineParser, int>> _commandHandlers;
    private readonly Logger _logger;
    private const int ExitCodeSuccess = 0;
    private const int ExitCodeFailure = 1;

    /// <summary>
    /// Initializes a new instance of the command executor.
    /// </summary>
    public CommandExecutor(Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _commandHandlers = new Dictionary<string, Func<CommandLineParser, int>>(
            StringComparer.OrdinalIgnoreCase
        );
    }

    /// <summary>
    /// Registers a command handler. The handler receives the parsed arguments
    /// and returns an exit code (0 for success, non-zero for failure).
    /// </summary>
    public void RegisterCommand(string commandName, Func<CommandLineParser, int> handler)
    {
        if (string.IsNullOrWhiteSpace(commandName))
            throw new ArgumentException("Command name cannot be null or empty.", nameof(commandName));

        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        _commandHandlers[commandName.ToLower()] = handler;
    }

    /// <summary>
    /// Executes the command specified in the parsed arguments.
    /// Returns an exit code (0 for success, 1 for failure).
    /// </summary>
    public int Execute(CommandLineParser parser)
    {
        if (parser is null)
            throw new ArgumentNullException(nameof(parser));

        try
        {
            string? command = parser.GetCommand();

            // Handle help
            if (parser.ShowHelp)
            {
                DisplayHelpForCommand(command);
                return ExitCodeSuccess;
            }

            // Handle version
            if (parser.ShowVersion)
            {
                DisplayVersion();
                return ExitCodeSuccess;
            }

            // Validate command is provided
            if (string.IsNullOrEmpty(command))
            {
                _logger.Error("No command specified. Use 'visualizer help' for usage information.");
                return ExitCodeFailure;
            }

            // Look up and execute the handler
            if (!_commandHandlers.TryGetValue(command.ToLower(), out var handler))
            {
                _logger.Error($"Unknown command: '{command}'. Use 'visualizer help' for available commands.");
                return ExitCodeFailure;
            }

            _logger.Info($"Executing command: {command}");
            return handler(parser);
        }
        catch (Exception ex)
        {
            _logger.Error($"Command execution failed: {ex.Message}");
            if (ex.InnerException is not null)
                _logger.Error($"Inner exception: {ex.InnerException.Message}");
            return ExitCodeFailure;
        }
    }

    /// <summary>
    /// Gets all registered command names.
    /// </summary>
    public IEnumerable<string> GetRegisteredCommands() => _commandHandlers.Keys;

    /// <summary>
    /// Checks if a command is registered.
    /// </summary>
    public bool IsCommandRegistered(string commandName)
    {
        return _commandHandlers.ContainsKey(commandName.ToLower());
    }

    /// <summary>
    /// Displays help for a specific command or general help.
    /// </summary>
    private void DisplayHelpForCommand(string? commandName)
    {
        if (string.IsNullOrEmpty(commandName))
        {
            Console.WriteLine("\nNAudio Visualizer - Command Line Interface");
            Console.WriteLine("=========================================\n");
            Console.WriteLine("Usage: visualizer <command> [options]\n");
            Console.WriteLine("Available commands:");
            foreach (var cmd in GetRegisteredCommands())
            {
                Console.WriteLine($"  {cmd}");
            }
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  --help, -h      Show this help message");
            Console.WriteLine("  --version, -v   Show version information\n");
        }
        else
        {
            Console.WriteLine($"Help for '{commandName}' command");
            Console.WriteLine("Not implemented yet.");
        }
    }

    /// <summary>
    /// Displays the application version.
    /// </summary>
    private void DisplayVersion()
    {
        Console.WriteLine("NAudio Visualizer v1.0");
        Console.WriteLine("Real-time audio visualization for .NET");
        Console.WriteLine("\nAuthor: Vladyslav Zaiets");
        Console.WriteLine("https://sarmkadan.com");
    }
}

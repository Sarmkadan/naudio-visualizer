#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace NAudioVisualizer.CLI;

/// <summary>
/// Generates formatted help text for commands and options.
/// Provides consistent formatting across all help output.
/// </summary>
public class HelpGenerator
{
    private const int IndentWidth = 2;
    private const int MaxLineWidth = 80;

    /// <summary>
    /// Generates general application help text.
    /// </summary>
    public static string GenerateGeneralHelp()
    {
        var sb = new StringBuilder();
        sb.AppendLine("\n╔════════════════════════════════════════════════════════════════════════════════╗");
        sb.AppendLine("║              NAudio Visualizer - Real-time Audio Visualization              ║");
        sb.AppendLine("╚════════════════════════════════════════════════════════════════════════════════╝\n");

        sb.AppendLine("USAGE:");
        sb.AppendLine("  visualizer <command> [options]\n");

        sb.AppendLine("COMMANDS:");
        sb.AppendLine("  visualize       Start real-time audio visualization");
        sb.AppendLine("  capture         Capture audio to file");
        sb.AppendLine("  analyze         Analyze audio file");
        sb.AppendLine("  export          Export visualization data");
        sb.AppendLine("  list-devices    List available audio devices");
        sb.AppendLine("  info            Display information about audio device");
        sb.AppendLine("  help            Display this help message\n");

        sb.AppendLine("GLOBAL OPTIONS:");
        sb.AppendLine("  --help, -h             Show this help message");
        sb.AppendLine("  --version, -v          Show version information");
        sb.AppendLine("  --verbose              Enable verbose output");
        sb.AppendLine("  --quiet                Suppress non-essential output\n");

        sb.AppendLine("EXAMPLES:");
        sb.AppendLine("  visualizer visualize --device 0");
        sb.AppendLine("  visualizer capture --output audio.wav --duration 30");
        sb.AppendLine("  visualizer analyze --file audio.wav --format json");
        sb.AppendLine("  visualizer list-devices\n");

        return sb.ToString();
    }

    /// <summary>
    /// Generates help text for the visualize command.
    /// </summary>
    public static string GenerateVisualizeHelp()
    {
        var sb = new StringBuilder();
        sb.AppendLine("\nCOMMAND: visualize");
        sb.AppendLine("Description: Start real-time audio visualization\n");

        sb.AppendLine("USAGE:");
        sb.AppendLine("  visualizer visualize [options]\n");

        sb.AppendLine("OPTIONS:");
        sb.AppendLine("  --device <id>          Audio device ID (default: 0)");
        sb.AppendLine("  --sample-rate <hz>    Sample rate in Hz (default: 44100)");
        sb.AppendLine("  --channels <n>        Number of channels (default: 2)");
        sb.AppendLine("  --fft-size <size>     FFT size (default: 2048)");
        sb.AppendLine("  --type <type>         Visualization type (waveform|spectrum|spectrogram)");
        sb.AppendLine("  --fps <n>             Target FPS (default: 60)\n");

        sb.AppendLine("EXAMPLES:");
        sb.AppendLine("  visualizer visualize");
        sb.AppendLine("  visualizer visualize --device 1 --sample-rate 48000");
        sb.AppendLine("  visualizer visualize --type spectrum --fft-size 4096\n");

        return sb.ToString();
    }

    /// <summary>
    /// Generates help text for the capture command.
    /// </summary>
    public static string GenerateCaptureHelp()
    {
        var sb = new StringBuilder();
        sb.AppendLine("\nCOMMAND: capture");
        sb.AppendLine("Description: Capture audio to file\n");

        sb.AppendLine("USAGE:");
        sb.AppendLine("  visualizer capture [options]\n");

        sb.AppendLine("OPTIONS:");
        sb.AppendLine("  --device <id>          Audio device ID (default: 0)");
        sb.AppendLine("  --output <path>        Output file path (required)");
        sb.AppendLine("  --duration <seconds>   Duration in seconds (required)");
        sb.AppendLine("  --sample-rate <hz>    Sample rate in Hz (default: 44100)");
        sb.AppendLine("  --channels <n>        Number of channels (default: 2)\n");

        sb.AppendLine("EXAMPLES:");
        sb.AppendLine("  visualizer capture --output audio.wav --duration 30");
        sb.AppendLine("  visualizer capture --device 1 --output recording.wav --duration 60 --sample-rate 48000\n");

        return sb.ToString();
    }

    /// <summary>
    /// Generates help text for the analyze command.
    /// </summary>
    public static string GenerateAnalyzeHelp()
    {
        var sb = new StringBuilder();
        sb.AppendLine("\nCOMMAND: analyze");
        sb.AppendLine("Description: Analyze audio file\n");

        sb.AppendLine("USAGE:");
        sb.AppendLine("  visualizer analyze <file> [options]\n");

        sb.AppendLine("OPTIONS:");
        sb.AppendLine("  --format <fmt>         Output format (json|csv|xml)");
        sb.AppendLine("  --output <path>        Output file path (optional)");
        sb.AppendLine("  --fft-size <size>     FFT size (default: 2048)");
        sb.AppendLine("  --window <type>       Window function (hann|hamming|blackman)\n");

        sb.AppendLine("EXAMPLES:");
        sb.AppendLine("  visualizer analyze audio.wav");
        sb.AppendLine("  visualizer analyze audio.wav --format json --output analysis.json\n");

        return sb.ToString();
    }

    /// <summary>
    /// Generates help text for the export command.
    /// </summary>
    public static string GenerateExportHelp()
    {
        var sb = new StringBuilder();
        sb.AppendLine("\nCOMMAND: export");
        sb.AppendLine("Description: Export visualization data\n");

        sb.AppendLine("USAGE:");
        sb.AppendLine("  visualizer export <source> <destination> [options]\n");

        sb.AppendLine("OPTIONS:");
        sb.AppendLine("  --format <fmt>         Export format (json|csv|xml)");
        sb.AppendLine("  --compress             Compress output file");
        sb.AppendLine("  --include-metadata     Include metadata in export\n");

        sb.AppendLine("EXAMPLES:");
        sb.AppendLine("  visualizer export visualization.dat export.json");
        sb.AppendLine("  visualizer export visualization.dat export.csv --compress\n");

        return sb.ToString();
    }

    /// <summary>
    /// Formats text to fit within the maximum line width.
    /// </summary>
    private static string FormatText(string text, int indent = 0)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var sb = new StringBuilder();
        string indentStr = new string(' ', indent);
        int currentLineLength = 0;
        var words = text.Split(' ');

        foreach (var word in words)
        {
            if (currentLineLength + word.Length + 1 > MaxLineWidth && currentLineLength > 0)
            {
                sb.AppendLine();
                sb.Append(indentStr);
                currentLineLength = indent;
            }

            sb.Append(word);
            sb.Append(' ');
            currentLineLength += word.Length + 1;
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Creates formatted option documentation.
    /// </summary>
    public static string CreateOptionDoc(string flagName, string description, string? defaultValue = null)
    {
        var sb = new StringBuilder();
        sb.Append($"  --{flagName}");

        if (!string.IsNullOrEmpty(defaultValue))
            sb.Append($" [default: {defaultValue}]");

        sb.AppendLine();
        sb.AppendLine($"      {FormatText(description, 6)}");

        return sb.ToString();
    }
}

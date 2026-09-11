#nullable enable
// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System;
using System.Drawing;
using System.Windows.Forms;
using NAudioVisualizer.Configuration;
using NAudioVisualizer.Constants;
namespace NAudioVisualizer;

/// <summary>
/// Main entry point for the NAudio Visualizer application.
/// </summary>
static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        try
        {
            // Initialize application configuration
            var settings = new ApplicationSettings
            {
                DefaultSampleRate = AudioConstants.DEFAULT_SAMPLE_RATE,
                DefaultFftSize = AudioConstants.DEFAULT_FFT_SIZE,
                TargetFps = VisualizationConstants.DEFAULT_TARGET_FPS,
                MaxFramesPerSession = 5000
            };

            if (!settings.IsValid())
            {
                MessageBox.Show(
                    "Application settings are invalid. Please check configuration.",
                    "Configuration Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // Configure dependency injection
            var serviceContainer = ApplicationConfiguration.ConfigureServices(settings);

            // Setup Windows Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            // Create and run main form
            var mainForm = new MainForm(serviceContainer, settings);
            Application.Run(mainForm);

            // Cleanup
            serviceContainer.Dispose();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Fatal error during startup: {ex.Message}\n\n{ex.StackTrace}",
                "Startup Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
}

/// <summary>
/// Main application window for audio visualization.
/// </summary>
public sealed partial class MainForm : Form
{
    // UI default values extracted as static readonly fields
    private static readonly Size DefaultWindowSize = new Size(1280, 720);
    private static readonly Color WindowBackgroundColor = Color.FromArgb(26, 26, 26);
    private static readonly Color ChromeBackgroundColor = Color.FromArgb(40, 40, 40);

    private readonly ServiceContainer _serviceContainer;
    private readonly ApplicationSettings _settings;

    public MainForm(ServiceContainer serviceContainer, ApplicationSettings settings)
    {
        _serviceContainer = serviceContainer ?? throw new ArgumentNullException(nameof(serviceContainer));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        InitializeComponent();
        LoadApplicationSettings();
    }

    /// <summary>
    /// Initializes the form components.
    /// </summary>
    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.Name = "MainForm";
        this.Text = "NAudio Visualizer - Real-time Audio Visualization";
        this.Size = DefaultWindowSize;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = WindowBackgroundColor;
        this.DoubleBuffered = true;

        // Create menu bar
        var menuStrip = new MenuStrip();
        menuStrip.BackColor = ChromeBackgroundColor;
        menuStrip.ForeColor = Color.White;

        // File menu
        var fileMenu = new ToolStripMenuItem("&File");
        fileMenu.DropDownItems.Add(new ToolStripMenuItem("E&xit", null, (s, e) => this.Close()));
        menuStrip.Items.Add(fileMenu);

        // Audio menu
        var audioMenu = new ToolStripMenuItem("&Audio");
        audioMenu.DropDownItems.Add(new ToolStripMenuItem("&Start Capture", null, OnStartCapture));
        audioMenu.DropDownItems.Add(new ToolStripMenuItem("S&top Capture", null, OnStopCapture));
        audioMenu.DropDownItems.Add(new ToolStripSeparator());
        audioMenu.DropDownItems.Add(new ToolStripMenuItem("&Devices", null, OnShowDevices));
        menuStrip.Items.Add(audioMenu);

        // View menu
        var viewMenu = new ToolStripMenuItem("&View");
        viewMenu.DropDownItems.Add(new ToolStripMenuItem("&Waveform", null, OnShowWaveform));
        viewMenu.DropDownItems.Add(new ToolStripMenuItem("&Spectrum", null, OnShowSpectrum));
        viewMenu.DropDownItems.Add(new ToolStripMenuItem("&Spectrogram", null, OnShowSpectrogram));
        menuStrip.Items.Add(viewMenu);

        // Help menu
        var helpMenu = new ToolStripMenuItem("&Help");
        helpMenu.DropDownItems.Add(new ToolStripMenuItem("&About", null, OnShowAbout));
        menuStrip.Items.Add(helpMenu);

        this.MainMenuStrip = menuStrip;
        this.Controls.Add(menuStrip);

        // Create status bar
        var statusStrip = new StatusStrip();
        statusStrip.BackColor = ChromeBackgroundColor;
        statusStrip.ForeColor = Color.White;

        var statusLabel = new ToolStripStatusLabel("Ready");
        statusLabel.Name = "StatusLabel";
        statusStrip.Items.Add(statusLabel);

        this.Controls.Add(statusStrip);

        // Create main panel for visualizations
        var mainPanel = new Panel();
        mainPanel.Dock = DockStyle.Fill;
        mainPanel.BackColor = WindowBackgroundColor;
        mainPanel.Margin = new Padding(0);
        mainPanel.Name = "MainVisualizationPanel";
        this.Controls.Add(mainPanel);

        this.ResumeLayout(false);
        this.PerformLayout();
    }

    /// <summary>
    /// Loads application settings and initializes UI.
    /// </summary>
    private void LoadApplicationSettings()
    {
        // Apply settings to UI
    }

    /// <summary>
    /// Handles start capture event.
    /// </summary>
    /// <remarks>
    /// Placeholder. Intended to call <see cref="AudioCaptureService.StartRecordingAsync"/>
    /// to begin capturing audio from the selected input device.
    /// </remarks>
    private void OnStartCapture(object? sender, EventArgs e)
    {
    }

    /// <summary>
    /// Handles stop capture event.
    /// </summary>
    /// <remarks>
    /// Placeholder. Intended to call <see cref="AudioCaptureService.StopRecordingAsync"/>
    /// to stop the active audio capture session.
    /// </remarks>
    private void OnStopCapture(object? sender, EventArgs e)
    {
    }

    /// <summary>
    /// Handles show devices event.
    /// </summary>
    /// <remarks>
    /// Placeholder. Intended to enumerate available devices via
    /// <see cref="AudioCaptureService.GetAvailableDevices"/> and present them in a dialog.
    /// </remarks>
    private void OnShowDevices(object? sender, EventArgs e)
    {
    }

    /// <summary>
    /// Handles show waveform visualization.
    /// </summary>
    /// <remarks>
    /// Placeholder. Intended to switch the active view to the waveform renderer,
    /// driven by <see cref="WaveformService"/>.
    /// </remarks>
    private void OnShowWaveform(object? sender, EventArgs e)
    {
    }

    /// <summary>
    /// Handles show spectrum visualization.
    /// </summary>
    /// <remarks>
    /// Placeholder. Intended to switch the active view to the spectrum renderer,
    /// driven by <see cref="SpectrumAnalyzer"/>.
    /// </remarks>
    private void OnShowSpectrum(object? sender, EventArgs e)
    {
    }

    /// <summary>
    /// Handles show spectrogram visualization.
    /// </summary>
    /// <remarks>
    /// Placeholder. Intended to switch the active view to the spectrogram renderer,
    /// driven by <see cref="SpectrogramAnalyzer"/>.
    /// </remarks>
    private void OnShowSpectrogram(object? sender, EventArgs e)
    {
    }

    /// <summary>
    /// Handles show about dialog.
    /// </summary>
    private void OnShowAbout(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "NAudio Visualizer v1.0\n\n" +
            "Real-time audio visualization with NAudio and SkiaSharp.\n\n" +
            "Author: Vladyslav Zaiets\n" +
            "https://sarmkadan.com",
            "About NAudio Visualizer",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _serviceContainer?.Dispose();
        }
        base.Dispose(disposing);
    }
}

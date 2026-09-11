# Program

Entry point and main form for the NAudio Visualizer Windows Forms application.

## Overview

The `Program` class contains the application entry point (`Main` method) and the `MainForm` partial class that defines the main application window. The application uses dependency injection via a `ServiceContainer` to manage services and follows a standard Windows Forms startup/shutdown sequence.

## Main Entry Point

### `static void Main()`

The application entry point marked with `[STAThread]` attribute for Windows Forms compatibility.

**Startup Sequence:**
1. Creates default `ApplicationSettings` with values from constants:
   - `DefaultSampleRate` = `AudioConstants.DEFAULT_SAMPLE_RATE`
   - `DefaultFftSize` = `AudioConstants.DEFAULT_FFT_SIZE`
   - `TargetFps` = `VisualizationConstants.DEFAULT_TARGET_FPS`
   - `MaxFramesPerSession` = 5000
2. Validates settings via `settings.IsValid()`; shows error dialog and exits if invalid
3. Configures dependency injection: `var serviceContainer = ApplicationConfiguration.ConfigureServices(settings)`
4. Initializes Windows Forms:
   - `Application.EnableVisualStyles()`
   - `Application.SetCompatibleTextRenderingDefault(false)`
   - `Application.SetHighDpiMode(HighDpiMode.SystemAware)`
5. Creates and runs main form: `Application.Run(new MainForm(serviceContainer, settings))`
6. Cleans up: `serviceContainer.Dispose()`

**Error Handling:**
- Wraps entire startup in try/catch
- Shows fatal error dialog with exception message and stack trace on any unhandled exception

## MainForm Class

### `public sealed partial class MainForm : Form`

The main application window implementing real-time audio visualization.

**Constructor:**
```csharp
public MainForm(ServiceContainer serviceContainer, ApplicationSettings settings)
```
- Stores service container and settings as readonly fields
- Calls `InitializeComponent()` to create UI
- Calls `LoadApplicationSettings()` to apply settings to UI

**Fields:**
- `_serviceContainer`: Dependency injection container
- `_settings`: Application configuration
- UI constants:
  - `DefaultWindowSize` = 1280x720
  - `WindowBackgroundColor` = Dark gray (26,26,26)
  - `ChromeBackgroundColor` = Slightly lighter gray (40,40,40)

**Lifecycle:**
- Overrides `Dispose(bool)` to dispose service container when disposing
- Calls base `Dispose(disposing)`

## Service Container

Services are configured via `ApplicationConfiguration.ConfigureServices(settings)` (see `ApplicationConfiguration.cs`). The container provides:
- Audio capture services
- Visualization services (waveform, spectrum, spectrogram)
- Worker services for background processing
- Configuration and constants access

The container implements `IDisposable` and is disposed:
- In `Main()` after `Application.Run()` exits
- In `MainForm.Dispose()` when the form closes

## Menu and Command Handlers

The main form creates a menu bar with four menus:

### File Menu
- **E&xit**: Calls `this.Close()` (implemented)

### Audio Menu
- **&Start Capture**: Calls `OnStartCapture` (stub)
- **S&top Capture**: Calls `OnStopCapture` (stub)
- Separator
- **&Devices**: Calls `OnShowDevices` (stub)

### View Menu
- **&Waveform**: Calls `OnShowWaveform` (stub)
- **&Spectrum**: Calls `OnShowSpectrum` (stub)
- **&Spectrogram**: Calls `OnShowSpectrogram` (stub)

### Help Menu
- **&About**: Calls `OnShowAbout` (implemented)

**Stub Methods (placeholders):**
All audio and view menu handlers except Exit and About are currently empty stubs with XML comments indicating intended functionality:
- `OnStartCapture`: Intended to call `AudioCaptureService.StartRecordingAsync()`
- `OnStopCapture`: Intended to call `AudioCaptureService.StopRecordingAsync()`
- `OnShowDevices`: Intended to enumerate devices via `AudioCaptureService.GetAvailableDevices()`
- Visualization handlers: Intended to switch active view to corresponding renderer driven by respective services

**Implemented Methods:**
- `OnShowAbout`: Shows about dialog with version, description, author, and website

## Startup and Shutdown Sequence

### Startup
1. `Main()` method begins
2. Application settings created and validated
3. Service container configured with settings
4. Windows Forms visual styles enabled
5. Main form created with service container and settings
6. `Application.Run()` starts message loop
7. Main form constructor:
   - Stores dependencies
   - `InitializeComponent()` creates menu, status bar, main panel
   - `LoadApplicationSettings()` called (currently empty)
8. Application enters message loop, waits for user interaction

### Shutdown
1. User selects File → Exit or closes window
2. `MainForm.Close()` called
3. `MainForm.Dispose(true)` disposes service container
4. `Application.Run()` exits
5. `Main()` disposes service container again (safe due to null check)
6. Application terminates

## Usage

The application is launched via `dotnet run` or by executing the compiled NAudioVisualizer.exe. No command-line arguments are currently supported.

## Notes

- The `MainForm` class is declared `partial` - additional UI initialization may exist in separate files
- All visualization and audio service stubs are intended to be implemented by accessing services from `_serviceContainer`
- The application uses double buffering to reduce flicker during visualization rendering
- High DPI awareness is set to `SystemAware` for proper scaling on high-resolution displays
- Settings validation currently only checks basic properties; may be extended in future
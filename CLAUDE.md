# CLAUDE.md

Real-time audio visualizer: Windows Forms app (`net10.0-windows`) capturing audio with NAudio and rendering with SkiaSharp; also packed as a NuGet library.

## Build

- SDK pinned in `global.json` (10.0.100, rollForward latestMinor). `EnableWindowsTargeting=true`, so it compiles on Linux but only runs on Windows.
- `dotnet restore`
- `dotnet build -c Release` (or `make build`)
- `dotnet publish -c Release -o ./publish` (or `make publish`)
- Solution file: `naudio-visualizer.slnx` (main project + Tests + Benchmarks).

## Test

- `dotnet test -c Release` (or `make test`) - xUnit + FluentAssertions + Moq in `tests/naudio-visualizer.Tests/`.
- Benchmarks: `dotnet run -c Release --project tests/naudio-visualizer.Benchmarks` (BenchmarkDotNet).
- CI (`.github/workflows/build.yml`) runs restore/build/test on `windows-latest`; CodeQL runs on push/PR.

## Lint / format

- `dotnet format --verify-no-changes` (`make lint`), `dotnet format` (`make format-fix`).
- Style rules live in `.editorconfig`: 4-space indent, LF, Allman braces, file-scoped namespaces, nullable enabled, implicit usings enabled (`Directory.Build.props`).
- `GenerateDocumentationFile=true`: public members need XML doc comments.

## Key directories

- `src/Program.cs` - `Main` entry point and `MainForm` (WinForms shell; view handlers are still stubs).
- `src/Services/` - `AudioCaptureService`, `WaveformService`, `SpectrumAnalyzer`, `SpectrogramAnalyzer`, `MidiInputService`, `AsciiSpectrumRenderer`.
- `src/Domain/Models/` - `AudioFrame`, `AudioBuffer`, `SpectrumData`, `VisualizationSettings`, themes, etc.
- `src/Events/` - `EventBus` (weak-reference pub/sub), `EventPublisher`, event records.
- `src/Configuration/` - `ServiceContainer` (minimal DI), `ApplicationSettings`, `ConfigurationManager`.
- `src/Data/Repositories/` - in-memory, lock-based repositories.
- `src/Caching/`, `src/Infrastructure/` (Logger, AudioDataConverter), `src/Workers/`, `src/Utilities/`, `src/Constants/`, `src/Exceptions/`, `src/Themes/`.
- `tests/naudio-visualizer.Tests/`, `tests/naudio-visualizer.Benchmarks/`.
- `docs/` - per-type reference pages plus `ARCHITECTURE.md`, `DEPLOYMENT.md`, `FAQ.md`, `migration-guide-v2.md`.
- The main csproj excludes `tests/**` and `examples/**` from compilation.

## Conventions

- Root namespace is `NAudioVisualizer` (not the csproj name); sub-namespaces mirror folders (`NAudioVisualizer.Services`, `NAudioVisualizer.Domain.Models`, ...).
- Every `.cs` file starts with `#nullable enable` and the author header comment block.
- One type per file. Companion files per type: `<Type>Extensions.cs` (helper methods), `<Type>JsonExtensions.cs` (serialization), `<Type>Validation.cs` (validation rules).
- Constants are `UPPER_SNAKE_CASE` in `AudioConstants` / `VisualizationConstants`.
- Custom exceptions derive from a base and end with `Exception` (`AudioDeviceException`, `AudioStreamException`, `VisualizationException`).
- Tests: one class per target type named `<Type>Tests`; methods named `Method_Scenario_Expected`; assertions via FluentAssertions `.Should()`.
- Commit messages use conventional prefixes (`docs:`, `chore:`, `feat:`, `fix:`).

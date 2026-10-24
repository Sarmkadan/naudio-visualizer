# Contributing to naudio-visualizer

Thanks for helping out. This guide covers setup, building, testing, code style, and the PR process.

## Prerequisites

- **.NET SDK 10.0.100 or newer in the 10.0 line.** `global.json` pins `10.0.100` with `rollForward: latestMinor`.
- **Windows** to run the app (WinForms, `net10.0-windows`). The project also builds on Linux and macOS because `EnableWindowsTargeting` is set, but the app itself only runs on Windows.
- Git

## Build

```bash
dotnet restore
dotnet build -c Release     # or: make build
```

`make publish` produces a publish folder in `./publish`.

## Test

```bash
dotnet test -c Release      # or: make test
```

Tests live in `tests/naudio-visualizer.Tests/` and use xUnit, FluentAssertions and Moq. Benchmarks are in `tests/naudio-visualizer.Benchmarks/` and run with `dotnet run -c Release --project tests/naudio-visualizer.Benchmarks`. They are not part of CI.

## Code style

Formatting rules live in `.editorconfig`. Check them with `make lint` (`dotnet format --verify-no-changes`) and fix with `make format-fix`.

- 4-space indentation, LF line endings, Allman braces (opening brace on its own line).
- File-scoped namespaces. Root namespace is `NAudioVisualizer`; sub-namespaces mirror folders.
- Every `.cs` file starts with `#nullable enable`. Nullable reference types are enabled project-wide.
- Public members need XML doc comments (`GenerateDocumentationFile` is on).
- One type per file. Companion files are named `<Type>Extensions.cs`, `<Type>JsonExtensions.cs`, or `<Type>Validation.cs`.
- Constants are `UPPER_SNAKE_CASE` and live in `AudioConstants` or `VisualizationConstants`.
- Custom exceptions derive from a base and end in `Exception`.
- Tests: one class per target type, named `<Type>Tests`. Methods are named `Method_Scenario_Expected` and use FluentAssertions `.Should()`.

## Branches and commits

Branch from `main` using a prefix and a short description:

- `feature/<short-description>`
- `fix/<short-description>`
- `docs/<short-description>`
- `chore/<short-description>`

Use conventional commit prefixes: `feat:`, `fix:`, `docs:`, `chore:`, `refactor:`, `test:`.

## Pull requests

1. Keep PRs small and focused on one change. Split unrelated work into separate PRs.
2. Add or update tests for any behaviour you change.
3. Run `make lint`, `dotnet build -c Release` and `dotnet test -c Release` locally first.
4. Open the PR against `main` and fill in `.github/PULL_REQUEST_TEMPLATE.md`. Describe what changed and why, and how you tested it. Link related issues with `Closes #<n>`.
5. CI (GitHub Actions, `windows-latest`) runs build and test on every PR to `main`. All checks must pass before merge.

## Reporting issues

Use [GitHub Issues](https://github.com/sarmkadan/naudio-visualizer/issues). Include steps to reproduce, expected and actual behaviour, the output of `dotnet --version`, your Windows version, and any relevant logs.

## License

By contributing, you agree that your contributions are licensed under the [MIT License](LICENSE).

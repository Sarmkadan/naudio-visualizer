# Contributing to naudio-visualizer

Thank you for considering contributing to naudio-visualizer!

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Windows OS (the application targets `net10.0-windows` with WinForms)
- Git

## Building Locally

```bash
# Clone the repository
git clone https://github.com/your-username/naudio-visualizer.git
cd naudio-visualizer

# Restore dependencies
dotnet restore

# Build in Release mode
dotnet build --configuration Release

# Or use the Makefile
make build
```

## Running Tests

```bash
# Run all tests
dotnet test --configuration Release --verbosity normal

# Run with detailed output and save results
dotnet test --configuration Release --verbosity normal --logger "trx;LogFileName=test-results.trx"

# Run a specific test project
dotnet test tests/naudio-visualizer.Tests/ --configuration Release
```

## Code Style

The project uses an `.editorconfig` to enforce consistent formatting. Key conventions:

- **Indentation**: 4 spaces for C# files
- **Namespaces**: File-scoped namespace declarations
- **Nullability**: Nullable reference types enabled (`#nullable enable`)
- **Naming**:
  - Types and public members: `PascalCase`
  - Private fields: `_camelCase` with underscore prefix
  - Interfaces: `IPascalCase`
- **Documentation**: XML doc comments required for all public APIs
- Braces on new lines (`Allman` style)
- Warnings are treated as errors — keep the build clean

## Workflow

### 1. Fork and branch

```bash
git checkout -b feature/your-feature-name
# or
git checkout -b fix/issue-description
```

### 2. Make your changes

- Write clean, focused commits with descriptive messages
- Use conventional commit prefixes: `feat:`, `fix:`, `docs:`, `refactor:`, `test:`
- Add or update tests for any changed behaviour
- Ensure all existing tests still pass

### 3. Submit a Pull Request

1. Push your branch to your fork
2. Open a PR against the `main` branch
3. Fill in the PR template completely
4. Link any related issues with `Closes #<issue-number>`

PRs require passing CI checks before merging.

## Reporting Issues

Use [GitHub Issues](https://github.com/sarmkadan/naudio-visualizer/issues). Include:

- Steps to reproduce
- Expected vs. actual behaviour
- OS version and .NET SDK version (`dotnet --version`)
- Relevant log output or error messages

## License

By contributing, you agree that your contributions will be licensed under the [MIT License](LICENSE).

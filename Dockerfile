# NAudio Visualizer - Cross-platform Audio Visualization Application
# Multi-stage build for Linux containers

# Build stage using .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder

WORKDIR /build

# Copy project files and restore dependencies
COPY *.csproj ./
COPY naudio-visualizer.slnx ./
# Exclude test projects from restore to avoid missing file errors
RUN dotnet restore --locked-mode naudio-visualizer.csproj


# Copy remaining source code
COPY . .

# Build the application
RUN dotnet publish naudio-visualizer.csproj -c Release \
  --self-contained false \
  --runtime linux-x64 \
  -o /app/publish \
  /p:GenerateDocumentationFile=false

# Runtime stage using .NET runtime image
FROM mcr.microsoft.com/dotnet/runtime:10.0

# Install required system dependencies for audio processing
RUN apt-get update && apt-get install -y --no-install-recommends \
    libasound2t64 \
    && rm -rf /var/lib/apt/lists/*

# Create application directories
RUN mkdir -p /app/logs && \
    mkdir -p /app/data && \
    mkdir -p /app/config

WORKDIR /app

# Copy published application from builder
COPY --from=builder /app/publish .

# Set environment variables
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV DOTNET_PRINT_TELEMETRY_MESSAGE=false
ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH="${DOTNET_ROOT}:${PATH}"

# Health check - verify application files exist
HEALTHCHECK --interval=30s --timeout=3s --start-period=30s --retries=3 \
  CMD test -f /app/naudio-visualizer.dll

# Set entry point to run the application
ENTRYPOINT ["dotnet", "naudio-visualizer.dll"]
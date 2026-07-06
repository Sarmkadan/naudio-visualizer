# NAudio Visualizer - Windows Forms GUI Application
# Multi-stage build for Windows containers

# Build stage using Windows SDK image
FROM mcr.microsoft.com/dotnet/sdk:10.0-windowsservercore-ltsc2022 AS builder

WORKDIR /build

# Copy project files and restore dependencies
COPY *.csproj ./
COPY naudio-visualizer.slnx ./
RUN dotnet restore --locked-mode

# Copy remaining source code
COPY . .

# Build the application
RUN dotnet publish naudio-visualizer.csproj -c Release \
    --self-contained false \
    --runtime win-x64 \
    -o /app/publish \
    /p:GenerateDocumentationFile=false

# Runtime stage using Windows Server Core with desktop support
FROM mcr.microsoft.com/dotnet/aspnet:10.0-windowsservercore-ltsc2022

# Create application directory
RUN mkdir C:\app\logs && \
    mkdir C:\app\data && \
    mkdir C:\app\config

WORKDIR /app

# Copy published application from builder
COPY --from=builder /app/publish .

# Expose port 8080
EXPOSE 8080

# Set environment variables
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV DOTNET_PRINT_TELEMETRY_MESSAGE=false
ENV DOTNET_ROOT="C:\Program Files\dotnet"
ENV PATH="${DOTNET_ROOT};${PATH}"

# Health check - verify application files exist
HEALTHCHECK --interval=30s --timeout=3s --start-period=30s --retries=3 \
    CMD powershell -Command "Test-Path 'C:\\app\\naudio-visualizer.dll'"

# Note: Windows Forms GUI applications in containers require interactive mode and display passthrough
# For actual GUI display, use: docker run -it --rm naudio-visualizer:1.0.0
# For headless testing/validation, the application can be tested via command line
# Set entry point to run the application
ENTRYPOINT ["dotnet", "naudio-visualizer.dll"]

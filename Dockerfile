# NAudio Visualizer - Multi-stage Docker build
# Optimized for minimal size and proper layer caching

# Build stage using Alpine-based .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS builder

WORKDIR /build

# Copy project files and restore dependencies
COPY *.csproj ./
COPY naudio-visualizer.slnx ./
RUN dotnet restore --locked-mode

# Copy remaining source code
COPY . .

# Build the application
RUN dotnet publish -c Release \
    --self-contained false \
    --runtime linux-x64 \
    -o /app/publish

# Runtime stage using Alpine-based ASP.NET image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine

# Install required system dependencies for GUI applications
RUN apk add --no-cache \
    ca-certificates \
    ttf-dejavu \
    libgdiplus \
    libx11 \
    libxext \
    libxi \
    libxtst \
    alsa-lib \
    ttf-freefont

# Create non-root user for security
RUN adduser -D -u 1001 appuser

# Create necessary directories
RUN mkdir -p /app/logs /app/data /app/config
RUN chown appuser:appuser /app/logs /app/data /app/config

WORKDIR /app

# Copy published application from builder
COPY --from=builder --chown=appuser:appuser /app/publish .

# Set environment variables
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV DOTNET_PRINT_TELEMETRY_MESSAGE=false
ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH="${DOTNET_ROOT}:${PATH}"

# Health check configuration
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD ["dotnet", "/app/NAudioVisualizer.dll", "--health"]

# Switch to non-root user
USER appuser

# Set entry point
ENTRYPOINT ["dotnet", "/app/NAudioVisualizer.dll"]

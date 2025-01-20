# NAudio Visualizer - Multi-stage Docker build
# Author: Vladyslav Zaiets | https://sarmkadan.com
# CTO & Software Architect

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 as builder

WORKDIR /build
COPY . .

# Restore and publish
RUN dotnet restore
RUN dotnet publish -c Release \
    --self-contained \
    --runtime linux-x64 \
    --output /app

# Runtime stage
FROM ubuntu:24.04

LABEL maintainer="Vladyslav Zaiets <rutova2@gmail.com>"
LABEL org.opencontainers.image.title="NAudio Visualizer"
LABEL org.opencontainers.image.description="Real-time audio visualization with NAudio and SkiaSharp"
LABEL org.opencontainers.image.version="1.2.0"
LABEL org.opencontainers.image.source="https://github.com/Sarmkadan/naudio-visualizer"

# Install runtime dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    curl \
    libx11-6 \
    libxrandr2 \
    libxinerama1 \
    libxcursor1 \
    libxext6 \
    libxfixes3 \
    libxi6 \
    libxdamage1 \
    libxcomposite1 \
    libxrender1 \
    libasound2 \
    libudev1 \
    libssl3 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy compiled application from builder
COPY --from=builder /app .

# Create necessary directories
RUN mkdir -p /app/logs /app/data

# Set environment variables
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV DISPLAY=:0
ENV PATH="/app:${PATH}"

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/api/health || exit 1

# Default command
ENTRYPOINT ["./naudio-visualizer"]
CMD ["--help"]

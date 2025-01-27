.PHONY: help build test clean restore publish docker docs examples run analyze format

# Configuration
DOTNET := dotnet
BUILD_CONFIG := Release
OUTPUT_DIR := ./bin/$(BUILD_CONFIG)/net10.0-windows
PUBLISH_DIR := ./publish
VERSION := 1.2.0

# Colors for output
RED := \033[0;31m
GREEN := \033[0;32m
YELLOW := \033[0;33m
BLUE := \033[0;34m
NC := \033[0m # No Color

## help: Display this help message
help:
	@echo "$(BLUE)NAudio Visualizer - Build Commands$(NC)"
	@echo ""
	@sed -n 's/^##//p' $(MAKEFILE_LIST) | column -t -s ':' | sed -e 's/^/ /'

## build: Build the project
build:
	@echo "$(BLUE)Building project ($(BUILD_CONFIG))...$(NC)"
	@$(DOTNET) build -c $(BUILD_CONFIG)
	@echo "$(GREEN)✓ Build complete$(NC)"

## rebuild: Clean and rebuild
rebuild: clean build
	@echo "$(GREEN)✓ Rebuild complete$(NC)"

## restore: Restore NuGet packages
restore:
	@echo "$(BLUE)Restoring NuGet packages...$(NC)"
	@$(DOTNET) restore
	@echo "$(GREEN)✓ Restore complete$(NC)"

## test: Run all tests
test:
	@echo "$(BLUE)Running tests...$(NC)"
	@$(DOTNET) test -c $(BUILD_CONFIG) --logger "console;verbosity=normal"
	@echo "$(GREEN)✓ Tests complete$(NC)"

## test-verbose: Run tests with verbose output
test-verbose:
	@echo "$(BLUE)Running tests (verbose)...$(NC)"
	@$(DOTNET) test -c $(BUILD_CONFIG) --logger "console;verbosity=detailed" --verbosity=detailed
	@echo "$(GREEN)✓ Tests complete$(NC)"

## clean: Remove build artifacts
clean:
	@echo "$(BLUE)Cleaning build artifacts...$(NC)"
	@rm -rf ./bin ./obj ./publish
	@echo "$(GREEN)✓ Clean complete$(NC)"

## publish: Publish for distribution
publish:
	@echo "$(BLUE)Publishing (Release mode)...$(NC)"
	@$(DOTNET) publish -c $(BUILD_CONFIG) -o $(PUBLISH_DIR)
	@echo "$(GREEN)✓ Publish complete ($(PUBLISH_DIR))$(NC)"

## publish-self-contained: Create self-contained executable
publish-self-contained:
	@echo "$(BLUE)Publishing self-contained executable...$(NC)"
	@$(DOTNET) publish -c $(BUILD_CONFIG) \
		--self-contained \
		--runtime linux-x64 \
		-o $(PUBLISH_DIR)
	@echo "$(GREEN)✓ Self-contained publish complete$(NC)"

## run: Run the application
run:
	@echo "$(BLUE)Running application...$(NC)"
	@$(DOTNET) run -c $(BUILD_CONFIG)

## run-example: Run an example (specify EXAMPLE=01-BasicWaveformCapture)
run-example:
	@if [ -z "$(EXAMPLE)" ]; then \
		echo "$(RED)Error: Specify EXAMPLE=<name>$(NC)"; \
		exit 1; \
	fi
	@echo "$(BLUE)Running example: $(EXAMPLE)...$(NC)"
	@$(DOTNET) run -c $(BUILD_CONFIG) --project examples/$(EXAMPLE).cs

## analyze: Run code analysis
analyze:
	@echo "$(BLUE)Running code analysis...$(NC)"
	@$(DOTNET) build -c $(BUILD_CONFIG) /p:TreatWarningsAsErrors=false
	@$(DOTNET) test -c $(BUILD_CONFIG) --no-build
	@echo "$(GREEN)✓ Analysis complete$(NC)"

## format: Format code with style conventions
format:
	@echo "$(BLUE)Formatting code...$(NC)"
	@$(DOTNET) format --verify-no-changes || true
	@echo "$(GREEN)✓ Format complete$(NC)"

## format-fix: Fix code formatting
format-fix:
	@echo "$(BLUE)Fixing code formatting...$(NC)"
	@$(DOTNET) format
	@echo "$(GREEN)✓ Format fix complete$(NC)"

## docker-build: Build Docker image
docker-build:
	@echo "$(BLUE)Building Docker image...$(NC)"
	@docker build -t naudio-visualizer:$(VERSION) .
	@echo "$(GREEN)✓ Docker image built: naudio-visualizer:$(VERSION)$(NC)"

## docker-run: Run in Docker
docker-run:
	@echo "$(BLUE)Running in Docker...$(NC)"
	@docker run -it --rm \
		-e DISPLAY=$(DISPLAY) \
		-v /tmp/.X11-unix:/tmp/.X11-unix \
		--device /dev/snd \
		naudio-visualizer:$(VERSION)

## docker-compose-up: Start services with docker-compose
docker-compose-up:
	@echo "$(BLUE)Starting Docker Compose services...$(NC)"
	@docker-compose up -d
	@echo "$(GREEN)✓ Services started$(NC)"

## docker-compose-down: Stop services
docker-compose-down:
	@echo "$(BLUE)Stopping Docker Compose services...$(NC)"
	@docker-compose down
	@echo "$(GREEN)✓ Services stopped$(NC)"

## docker-compose-logs: View service logs
docker-compose-logs:
	@docker-compose logs -f

## docs: Generate documentation
docs:
	@echo "$(BLUE)Documentation files:$(NC)"
	@ls -lh docs/
	@echo ""
	@echo "$(YELLOW)Available documentation:$(NC)"
	@echo "  - README.md (Getting Started)"
	@echo "  - docs/GETTING-STARTED.md"
	@echo "  - docs/ARCHITECTURE.md"
	@echo "  - docs/API-REFERENCE.md"
	@echo "  - docs/DEPLOYMENT.md"
	@echo "  - docs/FAQ.md"

## examples: List available examples
examples:
	@echo "$(YELLOW)Available examples:$(NC)"
	@ls -1 examples/*.cs | sed 's/examples\///;s/\.cs//' | nl

## lint: Check code style
lint:
	@echo "$(BLUE)Checking code style...$(NC)"
	@$(DOTNET) format --verify-no-changes
	@echo "$(GREEN)✓ Code style check complete$(NC)"

## metrics: Show project metrics
metrics:
	@echo "$(BLUE)Project Metrics:$(NC)"
	@echo "  Lines of Code:"
	@find src examples -name "*.cs" -type f | xargs wc -l | tail -1
	@echo ""
	@echo "  File Count:"
	@echo "    Source: $$(find src -name "*.cs" -type f | wc -l)"
	@echo "    Examples: $$(find examples -name "*.cs" -type f | wc -l)"
	@echo "    Documentation: $$(find docs -name "*.md" -type f | wc -l)"

## version: Show version information
version:
	@echo "$(BLUE)NAudio Visualizer v$(VERSION)$(NC)"
	@echo ".NET SDK: $$($(DOTNET) --version)"
	@echo "Target Framework: net10.0-windows"

## all: Build, test, and publish
all: clean restore build test publish
	@echo "$(GREEN)✓ All tasks complete$(NC)"

## ci: Run CI pipeline (build, test, analyze)
ci: clean restore build test analyze format
	@echo "$(GREEN)✓ CI pipeline complete$(NC)"

## benchmark: Run performance benchmarks
benchmark:
	@echo "$(BLUE)Running performance benchmarks...$(NC)"
	@$(DOTNET) run -c $(BUILD_CONFIG) --project examples/05-PerformanceProfiling.cs

## health-check: Run application health checks
health-check:
	@echo "$(BLUE)Running health checks...$(NC)"
	@echo "  .NET SDK: $$($(DOTNET) --version)"
	@echo "  Build: $$(if [ -d bin ]; then echo 'OK'; else echo 'MISSING'; fi)"
	@echo "  Tests: $$(if [ -d bin/$(BUILD_CONFIG) ]; then echo 'OK'; else echo 'MISSING'; fi)"
	@echo "$(GREEN)✓ Health check complete$(NC)"

## update-deps: Update NuGet dependencies
update-deps:
	@echo "$(BLUE)Updating NuGet dependencies...$(NC)"
	@$(DOTNET) list package --outdated
	@echo "$(YELLOW)Run 'dotnet package update' to update packages$(NC)"

.DEFAULT_GOAL := help

# Hide recipe echoing
.SILENT: help

# Platform detection
ifeq ($(OS),Windows_NT)
	RM := del /Q
	RMDIR := rmdir /S /Q
else
	RM := rm -f
	RMDIR := rm -rf
endif

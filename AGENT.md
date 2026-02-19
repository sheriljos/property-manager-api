# AGENT.md

## Build & Run

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run the API
dotnet run --project src/PropertyManager.Api
```

## Test

```bash
# Run all tests
dotnet test
```

Tests use **xUnit** with **FluentAssertions**. Follow the Arrange-Act-Assert pattern.

## Lint

No dedicated linter is configured. Use `dotnet build` to catch compilation errors and warnings.

## Project Structure

This project uses **Hexagonal Architecture** (Ports & Adapters) with three layers:

- **src/PropertyManager.Api/** — ASP.NET Core Web API entry point (controllers, DTOs, DI setup)
- **src/PropertyManager.Domain/** — Business logic, domain entities, ports (interfaces), and use cases
- **src/PropertyManager.Adapter/** — External integrations (Funda API client, resilience policies, configuration)
- **test/PropertyManager.Domain.Tests.Unit/** — Unit tests for domain logic

## Coding Conventions

- **.NET 8.0** / C#
- PascalCase for classes, methods, and properties; camelCase for local variables and parameters
- Constructor injection with interfaces for all dependencies
- Async/await throughout; return `Task` from async methods
- Ports (interfaces) live in `PropertyManager.Domain.Ports`
- Adapters implement ports and are registered in `Bootstrapper.cs`
- DTOs use `FromDomain()` static methods for domain-to-DTO conversion

# EasyMessages Documentation

Welcome to the complete documentation for **EasyMessages** - a lightweight .NET library for consistent, fluent message management.

---

## Packages

| Package | Version | Description |
|---------|---------|-------------|
| [RecurPixel.EasyMessages](https://www.nuget.org/packages/RecurPixel.EasyMessages) | [![NuGet](https://img.shields.io/nuget/v/RecurPixel.EasyMessages.svg)](https://www.nuget.org/packages/RecurPixel.EasyMessages) | Core library for all .NET applications |
| [RecurPixel.EasyMessages.AspNetCore](https://www.nuget.org/packages/RecurPixel.EasyMessages.AspNetCore) | [![NuGet](https://img.shields.io/nuget/v/RecurPixel.EasyMessages.AspNetCore.svg)](https://www.nuget.org/packages/RecurPixel.EasyMessages.AspNetCore) | ASP.NET Core integration |

---

## Quick Links

### New to EasyMessages?
- **[Getting Started in 5 Minutes](Getting-Started/Your-First-Message)** - Quick tutorial
- **[Installation Guide](Getting-Started/Installation)** - Setup instructions
- **[Package Comparison](Home#package-comparison)** - Which package do you need?

### Building Applications?
- **[Console vs Web Applications](Getting-Started/Console-vs-Web-Applications)** - Choose the right approach
- **[ASP.NET Core Integration](ASP-NET-Core/Overview)** - Complete guide
- **[API Response Patterns](ASP-NET-Core/API-Response-Patterns)** - REST API patterns

### Configuration & Setup?
- **[Setup and Configuration](ASP-NET-Core/Setup-and-Configuration)** - Complete setup guide
- **[Configuration Presets](ASP-NET-Core/Configuration-Presets)** - Environment-specific configs
- **[Logging Integration](ASP-NET-Core/Logging-Integration)** - ILogger integration

---

## Documentation Sections

### [Getting Started](Getting-Started/Installation)
Step-by-step tutorials to get you up and running quickly.
- Installation
- Your First Message (5 minutes)
- Console vs Web Applications
- Next Steps

### [Core Concepts](Core-Concepts/Messages-and-Message-Types)
Understand the fundamental concepts behind EasyMessages.
- Messages and Message Types
- Message Registry and Stores
- Facades (Msg.Auth, Msg.Crud, etc.)
- Formatters and Outputs
- Interceptors
- Architecture Overview

### [ASP.NET Core](ASP-NET-Core/Overview)
Complete guide for ASP.NET Core integration.
- [Overview](ASP-NET-Core/Overview) - Package features and quick start
- [Setup and Configuration](ASP-NET-Core/Setup-and-Configuration) - Installation and configuration
- [Configuration Presets](ASP-NET-Core/Configuration-Presets) - Environment-specific presets
- [API Response Patterns](ASP-NET-Core/API-Response-Patterns) - REST API patterns
- [Logging Integration](ASP-NET-Core/Logging-Integration) - ILogger integration

---

## Package Comparison

### Which Package Do I Need?

| Scenario | Package | Why? |
|----------|---------|------|
| **Console Application** | `RecurPixel.EasyMessages` | Core library has everything you need |
| **Background Jobs** | `RecurPixel.EasyMessages` | No web dependencies required |
| **Class Library** | `RecurPixel.EasyMessages` | Keep dependencies minimal |
| **ASP.NET Core Web API** | `RecurPixel.EasyMessages.AspNetCore` | Includes Core + web-specific features |
| **Minimal APIs** | `RecurPixel.EasyMessages.AspNetCore` | Full support for minimal API patterns |

### Feature Matrix

| Feature | Core | AspNetCore |
|---------|------|------------|
| Message Creation | [✓] | [✓] |
| Multiple Output Formats | [✓] | [✓] |
| Custom Messages | [✓] | [✓] |
| Custom Formatters | [✓] | [✓] |
| Custom Interceptors | [✓] | [✓] |
| IOptions Configuration | [ ] | [✓] |
| `.ToApiResponse()` | [ ] | [✓] |
| `.Log(ILogger)` | [ ] | [✓] |
| DI Integration | [ ] | [✓] |
| Configuration Presets | [ ] | [✓] |

---

## Documentation Conventions

Throughout this wiki, we use these conventions:

- **Code blocks** show working examples you can copy-paste
- ****Note:** Tips** highlight best practices and shortcuts
- ****Warning:** Warnings** point out common pitfalls
- **Notes** provide additional context
- **Links** are provided for related topics

---

## 🤝 Contributing

Found an issue in the documentation? Have a suggestion?

- [Report Documentation Issues](https://github.com/RecurPixel/RecurPixel.EasyMessages/issues)
- [Suggest Improvements](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)
- [Contributing Guide](https://github.com/RecurPixel/RecurPixel.EasyMessages/blob/main/CONTRIBUTING.md)

---

## License

EasyMessages is licensed under the [MIT License](https://github.com/RecurPixel/RecurPixel.EasyMessages/blob/main/LICENSE).

---

## Documentation Status

| Section | Status | Pages | Notes |
|---------|--------|-------|-------|
| Getting Started | [✓] Complete | 4 pages | Ready to use |
| Core Concepts | [✓] Complete | 6 pages | Comprehensive coverage |
| ASP.NET Core | [✓] Complete | 5 pages | Full integration guide |
| How-To Guides | 📋 Planned | - | Coming soon |
| Examples | 📋 Planned | - | Coming soon |
| API Reference | 📋 Planned | - | Coming soon |
| Advanced Topics | 📋 Planned | - | Coming soon |
| Migration Guides | 📋 Planned | - | Coming soon |

---

**Last Updated:** 2026-01-10
**Current Version:** 0.1.0-beta.1
**Status:** Beta Release
**Documentation:** 15 pages created, 30,000+ lines

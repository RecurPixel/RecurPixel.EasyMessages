---
layout: default
title: Home
nav_order: 1
description: "EasyMessages - Centralized message management for .NET applications"
permalink: /
---

# EasyMessages Documentation
{: .fs-9 }

**Centralized message management for .NET applications**
{: .fs-6 .fw-300 }

[Get Started](latest/Getting-Started/Installation){: .btn .btn-primary .fs-5 .mb-4 .mb-md-0 .mr-2 }
[View on GitHub](https://github.com/RecurPixel/RecurPixel.EasyMessages){: .btn .fs-5 .mb-4 .mb-md-0 }

---

[![NuGet Core](https://img.shields.io/nuget/v/RecurPixel.EasyMessages.svg?label=Core)](https://www.nuget.org/packages/RecurPixel.EasyMessages)
[![NuGet AspNetCore](https://img.shields.io/nuget/v/RecurPixel.EasyMessages.AspNetCore.svg?label=AspNetCore)](https://www.nuget.org/packages/RecurPixel.EasyMessages.AspNetCore)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/RecurPixel/RecurPixel.EasyMessages/blob/main/LICENSE)

---

## Quick Links

### For New Users
- [**Installation**](latest/Getting-Started/Installation) - Get started in 5 minutes
- [**Your First Message**](latest/Getting-Started/Your-First-Message) - Quick tutorial
- [**Package Comparison**](latest/#package-comparison) - Which package do you need?

### For Developers
- [**Core Concepts**](latest/Core-Concepts/Messages-and-Message-Types) - Understand the architecture
- [**ASP.NET Core Integration**](latest/ASP-NET-Core/Overview) - Web API patterns
- [**Configuration Guide**](latest/ASP-NET-Core/Setup-and-Configuration) - Setup and configuration

### Reference
- [**API Reference**](latest/API-Reference/Message-Codes-Reference) - Complete API documentation

### For Contributors
- [**Contributing Guide**](https://github.com/RecurPixel/RecurPixel.EasyMessages/blob/main/CONTRIBUTING.md) - How to contribute

---

## Packages

| Package | Version | Description |
|---------|---------|-------------|
| [**RecurPixel.EasyMessages**](https://www.nuget.org/packages/RecurPixel.EasyMessages) | [![NuGet](https://img.shields.io/nuget/v/RecurPixel.EasyMessages.svg)](https://www.nuget.org/packages/RecurPixel.EasyMessages) | Core library for console apps, background jobs, class libraries |
| [**RecurPixel.EasyMessages.AspNetCore**](https://www.nuget.org/packages/RecurPixel.EasyMessages.AspNetCore) | [![NuGet](https://img.shields.io/nuget/v/RecurPixel.EasyMessages.AspNetCore.svg)](https://www.nuget.org/packages/RecurPixel.EasyMessages.AspNetCore) | ASP.NET Core extensions for Web APIs |

### Installation

```bash
# For console apps, background jobs, class libraries
dotnet add package RecurPixel.EasyMessages --version 0.1.0-beta.*

# For ASP.NET Core web applications
dotnet add package RecurPixel.EasyMessages.AspNetCore --version 0.1.0-beta.*
```

---

## Quick Example

### Console Application
```csharp
using RecurPixel.EasyMessages;

// Works immediately - no setup needed!
Msg.Auth.LoginFailed().ToConsole(useColors: true);

var json = Msg.Crud.Created("User").ToJson();
Console.WriteLine(json);
```

### ASP.NET Core Web API
```csharp
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

// Program.cs
builder.Services.AddEasyMessages();

// Controller
[HttpPost]
public IActionResult Create(CreateUserDto dto)
{
    var user = _userService.Create(dto);
    return Msg.Crud.Created("User")
        .WithData(user)
        .ToApiResponse();
}
```

---

## Features

- [✓] **100+ Pre-built Messages** - Authentication, CRUD, Validation, System errors
- [✓] **Type-Safe** - IntelliSense-driven API, compile-time safety
- [✓] **Multiple Formats** - JSON, XML, Console, Plain Text
- [✓] **Zero Configuration** - Works immediately out of the box
- [✓] **Fluent API** - Chainable methods for clean, readable code
- [✓] **ASP.NET Core Integration** - `.ToApiResponse()`, logging, DI
- [✓] **IOptions Pattern** - Full configuration support with presets
- [✓] **Extensible** - Custom messages, formatters, interceptors
- [✓] **High Performance** - Optimized for .NET 8, 9, 10

---

## Documentation Structure

### [Getting Started](latest/Getting-Started/Installation.md)
Step-by-step tutorials to get you up and running quickly.
- Installation
- Your First Message
- Console vs Web Applications
- Next Steps

### [Core Concepts](latest/Core-Concepts/Messages-and-Message-Types.md)
Understand the fundamental concepts behind EasyMessages.
- Messages and Message Types
- Message Registry and Stores
- Facades (Msg.Auth, Msg.Crud, etc.)
- Formatters and Outputs
- Interceptors
- Architecture Overview

### [ASP.NET Core](latest/ASP-NET-Core/Overview.md)
Complete guide for ASP.NET Core integration.
- Overview
- Setup and Configuration
- Configuration Presets
- API Response Patterns
- Logging Integration

---

## Links

- [**GitHub Repository**](https://github.com/RecurPixel/RecurPixel.EasyMessages)
- [**Issue Tracker**](https://github.com/RecurPixel/RecurPixel.EasyMessages/issues)
- [**Discussions**](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)
- [**NuGet (Core)**](https://www.nuget.org/packages/RecurPixel.EasyMessages)
- [**NuGet (AspNetCore)**](https://www.nuget.org/packages/RecurPixel.EasyMessages.AspNetCore)
- [**Changelog**](https://github.com/RecurPixel/RecurPixel.EasyMessages/blob/main/CHANGELOG.md)

---

## License

EasyMessages is licensed under the [MIT License](https://github.com/RecurPixel/RecurPixel.EasyMessages/blob/main/LICENSE).

---

**Built with ❤️ by [RecurPixel](https://github.com/RecurPixel)**

*Last Updated: 2026-01-10 | Current Version: 0.1.0-beta.1*

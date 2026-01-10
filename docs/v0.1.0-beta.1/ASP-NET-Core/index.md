---
layout: default
title: ASP.NET Core
parent: Latest Documentation
nav_order: 5
has_children: true
---

# ASP.NET Core Integration
{: .no_toc }

Complete guide for integrating EasyMessages with ASP.NET Core web applications.
{: .fs-6 .fw-300 }

---

## In This Section

- [**Overview**](Overview) - ASP.NET Core package features and benefits
- [**Setup and Configuration**](Setup-and-Configuration) - Installation and configuration guide
- [**Configuration Presets**](Configuration-Presets) - Environment-specific configuration presets
- [**API Response Patterns**](API-Response-Patterns) - Standard REST API response patterns
- [**Logging Integration**](Logging-Integration) - ILogger integration for automatic logging

---

## Quick Start

### 1. Install the Package

```bash
dotnet add package RecurPixel.EasyMessages.AspNetCore --version 0.1.0-beta.*
```

### 2. Register Services

```csharp
// Program.cs
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

builder.Services.AddEasyMessages();
```

### 3. Use in Controllers

```csharp
[HttpPost]
public IActionResult Create(CreateUserDto dto)
{
    var user = _userService.Create(dto);
    return Msg.Crud.Created("User")
        .WithData(user)
        .ToApiResponse();
}
```

Get started with the [Overview](Overview) guide!

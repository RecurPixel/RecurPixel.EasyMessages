# EasyMessages

[![NuGet Core](https://img.shields.io/nuget/v/RecurPixel.EasyMessages.svg?label=Core)](https://www.nuget.org/packages/RecurPixel.EasyMessages)
[![NuGet AspNetCore](https://img.shields.io/nuget/v/RecurPixel.EasyMessages.AspNetCore.svg?label=AspNetCore)](https://www.nuget.org/packages/RecurPixel.EasyMessages.AspNetCore)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-purple)](https://dotnet.microsoft.com/)

> **Tired of writing the same error messages over and over?**
> EasyMessages gives you 100+ pre-built, standardized messages with a fluent API that just works.

---

## 🎯 The Problem

```csharp
// ❌ Inconsistent, scattered, hard to maintain
return BadRequest(new { message = "Invalid credentials" });
return Unauthorized(new { error = "Not authorized" });
return NotFound("User not found");
```

## ✨ The Solution

```csharp
// ✅ Standardized, fluent, discoverable, consistent
Msg.Auth.LoginFailed().ToJson();
Msg.Auth.Unauthorized().ToApiResponse();
Msg.Crud.NotFound("User").ToApiResponse();
```

---

## 🚀 Quick Start

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

// Program.cs
builder.Services.AddEasyMessages();

// Controller
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpPost]
    public IActionResult Create(CreateUserDto dto)
    {
        var user = _userService.Create(dto);
        return Msg.Crud.Created("User")
            .WithData(user)
            .ToApiResponse();
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _userService.GetById(id);
        if (user == null)
            return Msg.Crud.NotFound("User").ToApiResponse();

        return Msg.Crud.Retrieved("User")
            .WithData(user)
            .ToApiResponse();
    }
}
```

---

## ⚡ Features at a Glance

- ✅ **100+ Pre-built Messages** - Authentication, CRUD, Validation, System errors
- ✅ **Type-Safe** - IntelliSense-driven API, compile-time safety
- ✅ **Multiple Formats** - JSON, XML, Console, Plain Text
- ✅ **Zero Configuration** - Works immediately out of the box
- ✅ **Fluent API** - Chainable methods for clean, readable code
- ✅ **ASP.NET Core Integration** - `.ToApiResponse()`, logging, DI
- ✅ **IOptions Pattern** - Full configuration support with presets
- ✅ **Extensible** - Custom messages, formatters, interceptors
- ✅ **High Performance** - Optimized for .NET 5-10, minimal overhead

---

## 📦 Packages

| Package | Version | Description | Use For |
|---------|---------|-------------|---------|
| [**RecurPixel.EasyMessages**](https://www.nuget.org/packages/RecurPixel.EasyMessages) | [![NuGet](https://img.shields.io/nuget/v/RecurPixel.EasyMessages.svg)](https://www.nuget.org/packages/RecurPixel.EasyMessages) | Core library | Console apps, background jobs, class libraries |
| [**RecurPixel.EasyMessages.AspNetCore**](https://www.nuget.org/packages/RecurPixel.EasyMessages.AspNetCore) | [![NuGet](https://img.shields.io/nuget/v/RecurPixel.EasyMessages.AspNetCore.svg)](https://www.nuget.org/packages/RecurPixel.EasyMessages.AspNetCore) | ASP.NET Core extensions | Web APIs, minimal APIs |

### Installation

```bash
# For console apps, background jobs, class libraries
dotnet add package RecurPixel.EasyMessages --version 0.1.0-alpha.*

# For ASP.NET Core web applications
dotnet add package RecurPixel.EasyMessages.AspNetCore --version 0.1.0-alpha.*
```

---

## 📚 Documentation

📖 **[Complete Documentation Wiki](https://github.com/RecurPixel/RecurPixel.EasyMessages/wiki)**

### Quick Links
- 🚀 [Getting Started in 5 Minutes](https://github.com/RecurPixel/RecurPixel.EasyMessages/wiki/Getting-Started/Your-First-Message)
- 📖 [Core Concepts](https://github.com/RecurPixel/RecurPixel.EasyMessages/wiki/Core-Concepts/Messages-and-Message-Types)
- 🌐 [ASP.NET Core Integration](https://github.com/RecurPixel/RecurPixel.EasyMessages/wiki/ASP.NET-Core/Overview)
- 📝 [Configuration Guide](https://github.com/RecurPixel/RecurPixel.EasyMessages/wiki/ASP.NET-Core/Configuration-Guide)
- 💡 [Examples](https://github.com/RecurPixel/RecurPixel.EasyMessages/wiki/Examples/Console-Application)
- 📚 [API Reference](https://github.com/RecurPixel/RecurPixel.EasyMessages/wiki/API-Reference/Message-Codes-Reference)

---

## 💡 Why EasyMessages?

### Before EasyMessages

```csharp
// Manual, inconsistent, scattered across codebase
public IActionResult Login(LoginDto dto)
{
    if (!_authService.ValidateCredentials(dto))
    {
        _logger.LogWarning("Login failed for user {Email}", dto.Email);
        return Unauthorized(new
        {
            success = false,
            message = "Invalid credentials",
            timestamp = DateTime.UtcNow
        });
    }

    var token = _authService.GenerateToken(dto);
    _logger.LogInformation("User {Email} logged in", dto.Email);
    return Ok(new
    {
        success = true,
        message = "Login successful",
        data = new { token },
        timestamp = DateTime.UtcNow
    });
}
```

### With EasyMessages

```csharp
// Clean, consistent, discoverable
public IActionResult Login(LoginDto dto)
{
    if (!_authService.ValidateCredentials(dto))
        return Msg.Auth.LoginFailed().Log(_logger).ToApiResponse();

    var token = _authService.GenerateToken(dto);
    return Msg.Auth.LoginSuccess()
        .WithData(new { token })
        .Log(_logger)
        .ToApiResponse();
}
```

---

## ⚡ Performance

EasyMessages is optimized for .NET 5-10 with **minimal overhead**:

### Benchmark Highlights (BenchmarkDotNet)

| Operation | .NET 8.0 | .NET 10.0 | Memory |
|-----------|----------|-----------|--------|
| **Baseline: Convert to API response** | **111.0 ns** | **106.0 ns** (4.5% faster) | 256 B |
| Simple message with parameters | 1,630 ns | 1,592 ns (2.4% faster) | 560 B |
| Add single metadata | 176.9 ns | 169.4 ns (4.2% faster) | 320 B |
| Chained operations | 2,170 ns | 2,120 ns (2.3% faster) | 1.5 KB |

**Key Findings:**
- ✅ Ultra-fast conversions: ~106ns for API response (9.4M ops/sec on .NET 10)
- ✅ .NET 10 is 2-4.5% faster across all operations
- ✅ Low memory overhead: 256B-1.5KB per operation
- ✅ Predictable performance with low standard deviation

### Real-World Performance

```
✅ Registry lookups: 10,000 operations in ~10-20ms (100-200ns each)
✅ JSON formatting: 1,000 operations in ~3ms (3μs each)
✅ API conversion: 1,000,000 operations in ~119ms (119ns each)
```

### Optimizations
- Span&lt;T&gt; and Memory&lt;T&gt; for zero-allocation string operations
- ArrayPool&lt;T&gt; for temporary buffers
- ValueStringBuilder for efficient string concatenation
- Lazy initialization patterns
- Object pooling for frequently used instances

📊 **[Detailed Benchmarks & Performance Analysis](https://github.com/RecurPixel/RecurPixel.EasyMessages/wiki/Advanced-Topics/Performance-Considerations-and-Benchmarks)**

---

## 🎯 Design Philosophy

### 1. Zero Configuration
Works immediately without setup. Configuration is optional, not required.

### 2. Fail Fast, Fail Clear
Exceptions are descriptive with helpful context for quick debugging.

### 3. Immutable by Design
Thread-safe operations with predictable, reliable behavior.

### 4. Extensible Architecture
Easy to add custom messages, formatters, interceptors, and stores.

---

## 📖 Message Categories

| Category | Prefix | Count | Examples |
|----------|--------|-------|----------|
| **Authentication** | `AUTH_*` | 10 | Login failed, Unauthorized, Token expired |
| **CRUD Operations** | `CRUD_*` | 15 | Created, Updated, Deleted, Not found |
| **Validation** | `VAL_*` | 12 | Required field, Invalid format, Out of range |
| **System** | `SYS_*` | 8 | Error, Processing, Maintenance |
| **Database** | `DB_*` | 5 | Connection failed, Query timeout |
| **Files** | `FILE_*` | 10 | Uploaded, Invalid type, Size exceeded |
| **Network** | `NET_*` | 8 | Timeout, Service unavailable |
| **Custom** | `*` | ∞ | Your own messages |

**Total: 100+ built-in messages**

---

## 🔧 Configuration Example

```csharp
// ASP.NET Core - appsettings.json
{
  "EasyMessages": {
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Warning"
    },
    "Storage": {
      "CustomMessagesPath": "messages/custom.json"
    },
    "Localization": {
      "DefaultLocale": "en-US",
      "EnableLocalization": true
    }
  }
}

// Program.cs - Use configuration presets
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    // Or configure manually
    options.Logging.AutoLog = true;
    options.Storage.CustomMessagesPath = "custom.json";
});

// Or use presets for common scenarios
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Production // or Development, Testing, Staging, Api
);
```

📝 **[Complete Configuration Guide](https://github.com/RecurPixel/RecurPixel.EasyMessages/wiki/ASP.NET-Core/Configuration-Guide)**

---

## 🚦 Roadmap

- [x] **v0.1.0-alpha** - Core functionality (Current)
- [ ] **v0.2.0-beta** - ASP.NET Core package complete
- [ ] **v1.0.0** - Production-ready, 200+ messages
- [ ] **v1.x** - Additional integrations (Serilog, FluentValidation, etc.)

---

## ⚠️ Current Status: Alpha Release

This is an **alpha preview** (v0.1.0-alpha.x). APIs may change. Not recommended for production use.

**What's Working:**
- ✅ Core message system
- ✅ 100+ built-in messages
- ✅ JSON, XML, Console, PlainText formatters
- ✅ Parameter substitution
- ✅ Custom messages, formatters, interceptors
- ✅ ASP.NET Core integration (basic)

**What's Coming in Beta:**
- 🔜 Complete ASP.NET Core package
- 🔜 IOptions configuration (✅ Done!)
- 🔜 Enhanced logging integration
- 🔜 150+ messages
- 🔜 Comprehensive documentation (✅ In progress!)

📢 **[Report Issues](https://github.com/RecurPixel/RecurPixel.EasyMessages/issues)** | **[Give Feedback](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)**

---

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

- 🐛 [Report Bugs](https://github.com/RecurPixel/RecurPixel.EasyMessages/issues)
- 💡 [Request Features](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)
- 📖 [Improve Documentation](https://github.com/RecurPixel/RecurPixel.EasyMessages/wiki)

---

## 📄 License

EasyMessages is licensed under the [MIT License](LICENSE).

---

## 🙏 Acknowledgments

Built with ❤️ by [RecurPixel](https://github.com/RecurPixel)

Special thanks to:
- Early alpha testers
- The .NET community
- Everyone who provides feedback

---

## ⭐ Show Your Support

If you find EasyMessages useful:
- ⭐ Star the repository
- 🐛 Report bugs
- 💡 Suggest features
- 📢 Share with others

```bash
# Get started now!
dotnet add package RecurPixel.EasyMessages --version 0.1.0-alpha.*
```

---

**Your feedback will shape the future of EasyMessages!** 🚀

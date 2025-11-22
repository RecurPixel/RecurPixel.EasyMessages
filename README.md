# EasyMessages

> **Tired of writing the same error messages over and over?**  
> EasyMessages gives you 200+ pre-built, standardized messages with a fluent API that just works.

[![NuGet](https://img.shields.io/nuget/v/RecurPixel.EasyMessages.svg)](https://www.nuget.org/packages/RecurPixel.EasyMessages/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6%2C%207%2C%208-purple)](https://dotnet.microsoft.com/)

---

## ✨ What is EasyMessages?

EasyMessages is a lightweight .NET library that provides **standardized, reusable messages** for common scenarios like authentication, CRUD operations, validation, and system errors. Stop reinventing the wheel—use battle-tested messages that work everywhere.

### The Problem
```csharp
// ❌ Before: Inconsistent, scattered messages
return BadRequest(new { message = "Invalid credentials" });
return Unauthorized(new { error = "Not authorized" });
return NotFound("User not found");
```

### The Solution
```csharp
// ✅ After: Standardized, fluent, discoverable
return Msg.Auth.LoginFailed().ToApiResponse();
return Msg.Auth.Unauthorized().ToApiResponse();
return Msg.Crud.NotFound("User").ToApiResponse();
```

---

## 🚀 Quick Start

### Installation

```bash
# Core library
dotnet add package RecurPixel.EasyMessages

# ASP.NET Core integration
dotnet add package RecurPixel.EasyMessages.AspNetCore
```

### Basic Usage

```csharp
using EasyMessages;

// Use built-in messages
var message = Msg.Auth.LoginFailed();

// Add context and output
return Msg.Crud.Created("User")
    .WithData(newUser)
    .ToApiResponse();

// Validation with parameters
return Msg.Validation.RequiredField("Email")
    .ToApiResponse();

// Console output with colors
Msg.System.Error()
    .WithData(exception)
    .ToConsole();
```

---

## 🎯 Why EasyMessages?

### For You
- **Zero Configuration** – Works out of the box
- **IntelliSense-Friendly** – Discover messages as you type
- **Type-Safe** – No magic strings
- **Fluent API** – Chainable, readable code
- **200+ Built-in Messages** – Cover 90% of scenarios

### For Your Team
- **Consistency** – Everyone uses the same messages
- **Localization-Ready** – Built for i18n from day one
- **Customizable** – Override any message
- **Testable** – Mock-friendly design
- **Production-Ready** – Thread-safe, performant

### For Your Users
- **Clear Error Messages** – Users understand what went wrong
- **Standardized Responses** – Predictable API contracts
- **Better UX** – Consistent language across your app

---

## 📚 Message Categories

EasyMessages organizes messages into logical categories:

| Category | Prefix | Examples |
|----------|--------|----------|
| **Authentication** | `AUTH_*` | Login failed, Unauthorized, Session expired |
| **CRUD Operations** | `CRUD_*` | Created, Updated, Deleted, Not found |
| **Validation** | `VAL_*` | Required field, Invalid format, Out of range |
| **System** | `SYS_*` | Error, Processing, Maintenance mode |
| **Database** | `DB_*` | Connection failed, Query timeout |
| **Files** | `FILE_*` | Uploaded, Invalid type, Size exceeded |
| **Network** | `NET_*` | Request timeout, Service unavailable |
| **Custom** | `*` | Your own business-specific messages |

---

## 💡 Core Features

### 1. Fluent API Design

```csharp
// Chain methods naturally
Msg.Auth.LoginFailed()
    .WithData(new { attempts = 3 })
    .WithCorrelationId(HttpContext.TraceIdentifier)
    .Log(_logger)
    .ToApiResponse();
```

### 2. Parameter Substitution

```csharp
// Messages support placeholders
Msg.Validation.RequiredField("Email")
// Result: "Email is required."

Msg.File.InvalidType("PDF", "DOCX", "TXT")
// Result: "Only PDF, DOCX, TXT files are allowed."
```

### 3. Multiple Output Formats

```csharp
// API Response (ASP.NET Core)
return message.ToApiResponse();

// Minimal API
return message.ToMinimalApiResult();

// Console (with colors)
message.ToConsole();

// Raw JSON
var json = message.ToJson();
```

### 4. Custom Messages

```json
// custom-messages.json
{
  "PAYMENT_001": {
    "type": "Success",
    "title": "Payment Processed",
    "description": "Your payment of {amount} was successful.",
    "httpStatusCode": 200
  }
}
```

```csharp
// Use your custom message
Msg.Custom("PAYMENT_001")
    .WithParams(new { amount = "$50.00" })
    .ToApiResponse();
```

### 5. Smart HTTP Status Codes

```csharp
// Automatically maps to correct status codes
Msg.Auth.Unauthorized()     // 403 Forbidden
Msg.Crud.NotFound("User")   // 404 Not Found
Msg.Validation.Failed()     // 422 Unprocessable Entity
Msg.System.Error()          // 500 Internal Server Error

// Override when needed
Msg.Auth.LoginFailed()
    .WithStatusCode(429) // Too Many Requests
    .ToApiResponse();
```

---

## 🔧 ASP.NET Core Integration

### Setup (Optional)

```csharp
// Program.cs
builder.Services.AddEasyMessages(options =>
{
    // Load custom messages
    options.CustomMessagesPath = "messages/custom.json";
    
    // Configure behavior
    options.AutoLog = true;
    options.MinimumLogLevel = LogLevel.Warning;
    options.IncludeTimestamp = true;
});
```

### Controller Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
            return Msg.Validation.Failed().ToApiResponse();

        var user = await _userService.CreateAsync(dto);
        
        return Msg.Crud.Created("User")
            .WithData(user)
            .ToApiResponse(); // Returns 201 Created
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        
        if (user == null)
            return Msg.Crud.NotFound("User").ToApiResponse(); // 404

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        
        if (!result.Success)
            return Msg.Auth.LoginFailed().ToApiResponse(); // 401

        return Msg.Auth.LoginSuccess()
            .WithData(new { token = result.Token })
            .ToApiResponse();
    }
}
```

### Minimal API Example

```csharp
app.MapPost("/api/users", async (CreateUserDto dto, IUserService service) =>
{
    var user = await service.CreateAsync(dto);
    
    return Msg.Crud.Created("User")
        .WithData(user)
        .ToMinimalApiResult(); // Returns TypedResults
});
```

---

## 🎨 Console Applications

```csharp
using EasyMessages;

class Program
{
    static async Task Main(string[] args)
    {
        Msg.System.Processing()
            .WithParams(new { task = "File processing" })
            .ToConsole(); // ℹ Processing: Your request is being processed.

        try
        {
            await ProcessFilesAsync();
            
            Msg.File.Uploaded()
                .WithData(new { count = 42 })
                .ToConsole(); // ✓ File Uploaded: File uploaded successfully.
        }
        catch (Exception ex)
        {
            Msg.System.Error()
                .WithData(ex)
                .ToConsole(); // ✗ System Error: An unexpected error occurred.
        }
    }
}
```

---

## 📖 API Response Format

EasyMessages produces consistent, predictable API responses:

```json
{
  "success": true,
  "code": "CRUD_001",
  "type": "success",
  "title": "Created Successfully",
  "description": "User has been created.",
  "data": {
    "id": 123,
    "email": "user@example.com"
  },
  "timestamp": "2025-11-17T10:30:00Z",
  "correlationId": "abc-123-def",
  "metadata": {}
}
```

---

## 🔌 Extensibility

### Custom Formatters

#### With Interceptor Support (Recommended)
Extend `MessageFormatterBase` to automatically invoke registered interceptors:
```csharp
public class CsvFormatter : MessageFormatterBase
{
    protected override string FormatCore(Message message)
    {
        return $"{message.Code},{message.Title}";
    }
    
    protected override object FormatAsObjectCore(Message message)
    {
        return new[] { message.Code, message.Title };
    }
}
```

#### Without Interceptor Support (Advanced)
Implement `IMessageFormatter` directly for full control:
```csharp
public class SimpleFormatter : IMessageFormatter
{
    public string Format(Message message)
    {
        return message.Title;
    }
    
    public object FormatAsObject(Message message)
    {
        return Format(message);
    }
}
```

**Note:** Formatters that don't extend `MessageFormatterBase` won't invoke interceptors 
(logging, correlation ID enrichment, etc.). This is useful for performance-critical scenarios.
```

### Custom Output Targets

```csharp
public class EmailOutput : IMessageOutput
{
    public async Task SendAsync(Message message)
    {
        await _emailService.SendAsync(message);
    }
}

// Use it
await message.ToAsync<EmailOutput>();
```

---

## 🛠️ Configuration Options

```csharp
builder.Services.AddEasyMessages(options =>
{
    // Custom messages
    options.CustomMessagesPath = "messages/custom.json";
    
    // Or load from database
    options.UseStore<DatabaseMessageStore>();
    
    // Localization (future)
    options.DefaultLocale = "en-US";
    options.SupportedLocales = new[] { "en-US", "es-ES", "fr-FR" };
    
    // Formatting
    options.IncludeStackTrace = builder.Environment.IsDevelopment();
    options.IncludeTimestamp = true;
    options.IncludeCorrelationId = true;
    
    // Auto-logging
    options.AutoLog = true;
    options.MinimumLogLevel = LogLevel.Warning;
    
    // Custom formatters
    options.RegisterFormatter<SlackFormatter>();
    
    // Interceptors (middleware)
    options.AddInterceptor<UserContextInterceptor>();
});
```

---

## 📦 Packages

| Package | Description | Status |
|---------|-------------|--------|
| `RecurPixel.EasyMessages` | Core library | ✅ Available |
| `RecurPixel.EasyMessages.AspNetCore` | ASP.NET Core integration | ✅ Available |

---

## 🎯 Design Philosophy

### 1. **Zero Configuration**
Works immediately without setup. Configuration is optional.

### 2. **Fail Fast, Fail Clear**
Exceptions are descriptive with helpful context.

### 3. **Work WITH Everything**
Integrates with Serilog, FluentValidation, ProblemDetails, etc.

### 4. **Replace Nothing**
Complements your existing code—no vendor lock-in.

### 5. **Immutable by Design**
Thread-safe, predictable behavior.

---

## 🚦 Roadmap

### Current: v0.1.0 (Beta)
- ✅ Core message system
- ✅ 200+ built-in messages
- ✅ ASP.NET Core integration
- ✅ Parameter substitution
- ✅ Custom messages

### Next: v0.2.0
- 🔜 Localization (i18n)
- 🔜 Advanced templating
- 🔜 Async logging
- 🔜 Performance optimizations

### Future: v1.0.0
- 🔮 Telemetry integration
- 🔮 Message versioning
- 🔮 Community message packs

---

## 🤝 Contributing

We welcome contributions! See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

**Ways to contribute:**
- 🐛 Report bugs
- 💡 Suggest features
- 📝 Improve documentation
- 🌍 Add translations
- 🎨 Submit message templates

---

## 📄 License

MIT License - see [LICENSE](LICENSE) for details.

---

## 🙏 Acknowledgments

Built with ❤️ by [RecurPixel](https://github.com/RecurPixel)

Inspired by real-world pain points in building consistent APIs.

---

## 📞 Support

- **Documentation:** [Wiki](https://github.com/RecurPixel/EasyMessages/wiki)
- **Issues:** [GitHub Issues](https://github.com/RecurPixel/EasyMessages/issues)
- **Discussions:** [GitHub Discussions](https://github.com/RecurPixel/EasyMessages/discussions)
- **Email:** support@recurpixel.io

---

## ⭐ Show Your Support

If EasyMessages makes your life easier, give it a star! ⭐

```bash
# Quick start
dotnet new webapi -n MyApi
cd MyApi
dotnet add package RecurPixel.EasyMessages.AspNetCore
# Start coding!
```

**Happy Messaging! 🚀**
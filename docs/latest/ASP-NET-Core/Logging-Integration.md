# Logging Integration

Comprehensive guide to integrating EasyMessages with ASP.NET Core logging infrastructure (ILogger, structured logging, and log aggregation).

---

## Table of Contents

1. [Logging Basics](#logging-basics)
2. [Manual Logging](#manual-logging)
3. [Automatic Logging](#automatic-logging)
4. [Structured Logging](#structured-logging)
5. [Log Levels](#log-levels)
6. [Third-Party Integrations](#third-party-integrations)
7. [Best Practices](#best-practices)

---

## Logging Basics

EasyMessages integrates seamlessly with ASP.NET Core's `ILogger` interface, providing:

- **Manual logging** via `.Log()` extension method
- **Automatic logging** via `LoggingInterceptor`
- **Structured logging** with message properties
- **Log level mapping** based on message type

---

## Manual Logging

### Basic Usage

**Controller:**
```csharp
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Create(CreateUserDto dto)
    {
        var user = _userService.Create(dto);

        return Msg.Crud.Created("User")
            .WithData(user)
            .Log(_logger)  // ← Manual logging
            .ToApiResponse();
    }
}
```

**Console Output:**
```
info: YourApp.Controllers.UsersController[0]
      [CRUD_001] User Created: User has been created successfully.
```

---

### Chaining with Fluent API

The `.Log()` method returns the message, allowing method chaining:

```csharp
return Msg.Crud.Created("User")
    .WithData(user)
    .WithMetadata("source", "WebAPI")
    .Log(_logger)        // ← Logs and returns message
    .ToApiResponse();    // ← Continues chain
```

---

### Logging with Metadata

Metadata is automatically included in structured logs:

```csharp
return Msg.Crud.NotFound("User")
    .WithMetadata("userId", id)
    .WithMetadata("requestedBy", User.Identity.Name)
    .Log(_logger)
    .ToApiResponse();
```

**Console Output:**
```
warn: YourApp.Controllers.UsersController[0]
      [CRUD_004] User Not Found: The requested User could not be found.
      => userId: 999
      => requestedBy: john.doe@example.com
```

---

### Conditional Logging

Log only when a condition is met:

```csharp
[HttpGet("{id}")]
public IActionResult GetById(int id)
{
    var user = _userService.FindById(id);

    if (user == null)
    {
        // Always log not found
        return Msg.Crud.NotFound("User")
            .WithMetadata("userId", id)
            .Log(_logger)
            .ToApiResponse();
    }

    // Only log if verbose logging enabled
    var message = Msg.Crud.Retrieved("User").WithData(user);

    if (_configuration.GetValue<bool>("VerboseLogging"))
    {
        message.Log(_logger);
    }

    return message.ToApiResponse();
}
```

---

## Automatic Logging

### Enable Auto-Logging

**appsettings.json:**
```json
{
  "EasyMessages": {
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Information"
    }
  }
}
```

**Program.cs:**
```csharp
builder.Services.AddEasyMessages(builder.Configuration);
```

**What happens:**
- Every message is automatically logged via `LoggingInterceptor`
- No need to call `.Log()` manually
- Respects `MinimumLogLevel` setting

---

### Auto-Logging Example

**Controller (no manual logging):**
```csharp
[HttpPost]
public IActionResult Create(CreateUserDto dto)
{
    var user = _userService.Create(dto);

    return Msg.Crud.Created("User")
        .WithData(user)
        .ToApiResponse(); // ← Automatically logged!
}
```

**Console Output:**
```
info: EasyMessages[0]
      [CRUD_001] User Created: User has been created successfully.
      => CorrelationId: 0HMVQK8F3J8QK:00000001
```

---

### Minimum Log Level

Control which messages are logged automatically:

**appsettings.Development.json:**
```json
{
  "EasyMessages": {
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Debug"  // Log everything
    }
  }
}
```

**appsettings.Production.json:**
```json
{
  "EasyMessages": {
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Warning"  // Only warnings, errors, critical
    }
  }
}
```

---

## Structured Logging

### Message Properties in Logs

EasyMessages automatically includes message properties in structured logs:

```csharp
return Msg.Crud.Created("User")
    .WithData(new { Id = 123, Name = "John Doe" })
    .WithMetadata("source", "WebAPI")
    .WithCorrelationId("abc-123")
    .Log(_logger)
    .ToApiResponse();
```

**Structured Log Output (Serilog):**
```json
{
  "Timestamp": "2026-01-09T14:30:00.000Z",
  "Level": "Information",
  "MessageTemplate": "[{Code}] {Title}: {Description}",
  "Properties": {
    "Code": "CRUD_001",
    "Title": "User Created",
    "Description": "User has been created successfully.",
    "CorrelationId": "abc-123",
    "Metadata": {
      "source": "WebAPI"
    }
  }
}
```

---

### Custom Log Scope

Add custom log scope for additional context:

```csharp
using (_logger.BeginScope(new Dictionary<string, object>
{
    ["UserId"] = currentUserId,
    ["Action"] = "CreateUser"
}))
{
    return Msg.Crud.Created("User")
        .WithData(user)
        .Log(_logger)
        .ToApiResponse();
}
```

**Output:**
```
info: YourApp.Controllers.UsersController[0]
      => UserId: 456
      => Action: CreateUser
      [CRUD_001] User Created: User has been created successfully.
```

---

## Log Levels

### Message Type to Log Level Mapping

EasyMessages automatically maps message types to appropriate log levels:

| MessageType | LogLevel | When to Use |
|-------------|----------|-------------|
| **Success** | Information | Successful operations |
| **Info** | Information | Informational messages |
| **Warning** | Warning | Warnings, non-critical issues |
| **Error** | Error | Operation failures |
| **Critical** | Critical | System failures |

---

### Log Level Examples

**Success → Information:**
```csharp
Msg.Crud.Created("User").Log(_logger);
// Logged at: Information
```

**Warning → Warning:**
```csharp
Msg.Crud.NoChangesDetected().Log(_logger);
// Logged at: Warning
```

**Error → Error:**
```csharp
Msg.Auth.LoginFailed().Log(_logger);
// Logged at: Error
```

**Critical → Critical:**
```csharp
Msg.Database.ConnectionFailed().Log(_logger);
// Logged at: Critical
```

---

### Filtering by Log Level

**appsettings.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information",
      "EasyMessages": "Information",  // ← Control EasyMessages logging
      "YourApp.Controllers": "Debug"
    }
  }
}
```

**Production Settings:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "EasyMessages": "Warning"  // Only warnings and errors
    }
  }
}
```

---

## Third-Party Integrations

### Serilog Integration

**Install Serilog:**
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
```

**Program.cs:**
```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/easymessages-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    options.Logging.AutoLog = true;
    options.Logging.MinimumLogLevel = LogLevel.Information;
});

var app = builder.Build();

app.Run();
```

**appsettings.json:**
```json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/easymessages-.txt",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"]
  }
}
```

**Console Output:**
```
2026-01-09 14:30:00.123 +00:00 [INF] [CRUD_001] User Created: User has been created successfully.
```

---

### Application Insights Integration

**Install Application Insights:**
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

**Program.cs:**
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    options.Logging.AutoLog = true;
    options.Logging.MinimumLogLevel = LogLevel.Information;
});

var app = builder.Build();
app.Run();
```

**appsettings.json:**
```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=your-key-here"
  },
  "EasyMessages": {
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Information"
    }
  }
}
```

**Result:** All EasyMessages logs automatically sent to Application Insights with structured properties.

---

### Seq Integration

**Install Serilog Seq Sink:**
```bash
dotnet add package Serilog.Sinks.Seq
```

**Program.cs:**
```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    options.Logging.AutoLog = true;
});
```

**Result:** Structured logs sent to Seq for powerful querying and visualization.

---

### Elasticsearch Integration

**Install Serilog Elasticsearch Sink:**
```bash
dotnet add package Serilog.Sinks.Elasticsearch
```

**Program.cs:**
```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "easymessages-{0:yyyy.MM}"
    })
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddEasyMessages(builder.Configuration);
```

---

## Best Practices

### [✓] DO:

1. **Log errors and warnings**
   ```csharp
   // Good - Always log errors
   return Msg.Auth.LoginFailed()
       .WithMetadata("email", dto.Email)
       .Log(_logger)  // [✓] Log failures
       .ToApiResponse();
   ```

2. **Use auto-logging in production**
   ```json
   {
     "EasyMessages": {
       "Logging": {
         "AutoLog": true,
         "MinimumLogLevel": "Warning"  // [✓] Capture errors automatically
       }
     }
   }
   ```

3. **Include correlation IDs**
   ```csharp
   // Good - Correlation ID added automatically via interceptor
   builder.Services.AddEasyMessages(builder.Configuration, options =>
   {
       options.Interceptors.AutoAddCorrelationId = true;  // [✓] Enable tracing
   });
   ```

4. **Add contextual metadata**
   ```csharp
   // Good - Metadata helps debugging
   return Msg.Crud.NotFound("User")
       .WithMetadata("userId", id)
       .WithMetadata("requestedBy", User.Identity.Name)
       .Log(_logger)
       .ToApiResponse();
   ```

5. **Use structured logging**
   ```csharp
   // Good - Structured properties for querying
   _logger.LogInformation(
       "User {UserId} created by {CreatedBy}",
       user.Id,
       User.Identity.Name
   );
   ```

---

### [ ] DON'T:

1. **Don't log sensitive data**
   ```csharp
   // Bad - Logging password!
   return Msg.Auth.LoginFailed()
       .WithMetadata("password", dto.Password)  // [ ] Security risk!
       .Log(_logger)
       .ToApiResponse();

   // Good - Only log non-sensitive info
   return Msg.Auth.LoginFailed()
       .WithMetadata("email", dto.Email)  // [✓] Safe
       .Log(_logger)
       .ToApiResponse();
   ```

2. **Don't log too verbosely in production**
   ```json
   // Bad - Too verbose for production
   {
     "EasyMessages": {
       "Logging": {
         "AutoLog": true,
         "MinimumLogLevel": "Debug"  // [ ] Performance impact!
       }
     }
   }

   // Good - Minimal logging in production
   {
     "EasyMessages": {
       "Logging": {
         "AutoLog": true,
         "MinimumLogLevel": "Warning"  // [✓] Errors only
       }
     }
   }
   ```

3. **Don't ignore exceptions**
   ```csharp
   // Bad - Silent failure
   try
   {
       _userService.Delete(id);
   }
   catch { }  // [ ] No logging!

   // Good - Log exceptions
   try
   {
       _userService.Delete(id);
   }
   catch (Exception ex)
   {
       return Msg.System.Error()
           .WithMetadata("error", ex.Message)
           .Log(_logger)  // [✓] Logged
           .ToApiResponse();
   }
   ```

4. **Don't log PII without consent**
   ```csharp
   // Bad - Logging PII
   return Msg.Crud.Created("User")
       .WithMetadata("email", user.Email)      // [ ] PII
       .WithMetadata("phone", user.Phone)      // [ ] PII
       .WithMetadata("address", user.Address)  // [ ] PII
       .Log(_logger)
       .ToApiResponse();

   // Good - Use user ID only
   return Msg.Crud.Created("User")
       .WithMetadata("userId", user.Id)  // [✓] No PII
       .Log(_logger)
       .ToApiResponse();
   ```

5. **Don't mix logging strategies**
   ```csharp
   // Bad - Mixing manual and auto-logging
   builder.Services.AddEasyMessages(builder.Configuration, options =>
   {
       options.Logging.AutoLog = true;  // Auto-logging ON
   });

   // Then manually logging too
   return Msg.Crud.Created("User")
       .Log(_logger)  // [ ] Logged twice!
       .ToApiResponse();

   // Good - Choose one strategy
   // Either auto-log OR manual log, not both
   ```

---

## Logging Patterns

### Pattern 1: Manual Logging (Development)

**Use when:** You want explicit control over what gets logged

**Configuration:**
```json
{
  "EasyMessages": {
    "Logging": {
      "AutoLog": false
    }
  }
}
```

**Usage:**
```csharp
return Msg.Crud.Created("User")
    .WithData(user)
    .Log(_logger)  // ← Explicit logging
    .ToApiResponse();
```

---

### Pattern 2: Auto-Logging (Production)

**Use when:** You want all messages logged automatically

**Configuration:**
```json
{
  "EasyMessages": {
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Warning"
    }
  }
}
```

**Usage:**
```csharp
return Msg.Crud.Created("User")
    .WithData(user)
    .ToApiResponse();  // ← Automatically logged
```

---

### Pattern 3: Hybrid (Staging)

**Use when:** Auto-log errors, manually log important events

**Configuration:**
```json
{
  "EasyMessages": {
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Error"  // Only auto-log errors
    }
  }
}
```

**Usage:**
```csharp
// Manually log important success events
return Msg.Crud.Created("User")
    .WithData(user)
    .Log(_logger)  // ← Explicit for important events
    .ToApiResponse();

// Errors logged automatically via interceptor
return Msg.Crud.NotFound("User")
    .ToApiResponse();  // ← Auto-logged (error level)
```

---

## Next Steps

- **[Overview](Overview.md)** - ASP.NET Core integration overview
- **[Setup and Configuration](Setup-and-Configuration.md)** - Complete setup guide
- **[API Response Patterns](API-Response-Patterns.md)** - REST API patterns
- **[How-To Guides](../How-To-Guides/)** - Practical recipes

---

**Questions?** [Ask in Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

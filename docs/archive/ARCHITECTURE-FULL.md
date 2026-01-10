# EasyMessages - Complete Architecture & Documentation

## Table of Contents
1. [Architecture Overview](#architecture-overview)
2. [Project Structure](#project-structure)
3. [Core Concepts](#core-concepts)
4. [Installation](#installation)
5. [Quick Start](#quick-start)
6. [Configuration](#configuration)
7. [Advanced Features](#advanced-features)
8. [API Reference](#api-reference)
9. [Examples](#examples)
10. [Best Practices](#best-practices)

---

## Architecture Overview

### Design Philosophy
- **Zero Configuration Required** - Works immediately out of the box
- **Configure When Needed** - Extensive customization options available
- **Separation of Concerns** - Clear boundaries between Core and AspNetCore
- **Extensible by Design** - Plugin architecture for formatters, stores, and interceptors
- **Thread-Safe** - All registries use proper locking mechanisms

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    User Application                         │
└─────────────────┬───────────────────────────────────────────┘
                  │
    ┌─────────────┴─────────────┐
    │                           │
    ▼                           ▼
┌─────────────────┐    ┌──────────────────────┐
│  Core Library   │    │  AspNetCore Library  │
│  (Standalone)   │    │  (Web Extensions)    │
└─────────────────┘    └──────────────────────┘
    │                           │
    ├─ Messages                 ├─ API Responses
    ├─ Formatters               ├─ Logging Integration
    ├─ Stores                   ├─ Interceptors
    ├─ Registries               ├─ DI Configuration
    └─ Extensions               └─ HTTP Context Access
```

---

## Project Structure

```
RecurPixel.EasyMessages/                          (Core Library)
├── Core/
│   ├── Message.cs                               // Immutable message record
│   ├── MessageType.cs                           // Success, Info, Warning, Error, Critical
│   ├── MessageTemplate.cs                       // Internal JSON mapping
│   └── MessageRegistry.cs                       // Message loading & lookup
├── Facades/
│   └── Msg.cs                                   // Main API entry point
│       ├── Auth                                 // Authentication messages
│       ├── Crud                                 // CRUD operation messages
│       ├── Validation                           // Validation messages
│       ├── System                               // System messages
│       ├── Database                             // Database messages
│       ├── File                                 // File operation messages
│       └── Custom(code)                         // User-defined messages
├── Formatters/
│   ├── IMessageFormatter.cs                     // Formatter interface
│   ├── MessageFormatterBase.cs                  // Base with interceptor support
│   ├── FormatterRegistry.cs                     // Formatter management
│   ├── FormatterOptions.cs                      // Output control options
│   ├── JsonFormatter.cs                         // JSON output
│   ├── XmlFormatter.cs                          // XML output
│   ├── PlainTextFormatter.cs                    // Plain text output
│   └── ConsoleFormatter.cs                      // Colored console output
├── Configuration/
│   └── FormatterConfiguration.cs                // Global formatter defaults & presets
├── Stores/
│   ├── IMessageStore.cs                         // Store interface
│   ├── EmbeddedMessageStore.cs                  // Loads defaults.json
│   ├── FileMessageStore.cs                      // Loads custom JSON files
│   ├── DictionaryMessageStore.cs                // In-memory store
│   ├── DatabaseMessageStore.cs                  // Abstract base for DB stores
│   └── CompositeMessageStore.cs                 // Combines multiple stores
├── Interceptors/
│   ├── IMessageInterceptor.cs                   // Interceptor interface
│   └── InterceptorRegistry.cs                   // Interceptor management
├── Extensions/
│   ├── MessageExtensions.Core.cs                // .WithData(), .WithParams()
│   └── MessageExtensions.Output.cs              // .ToJson(), .ToXml(), .ToConsole()
├── Helpers/
│   └── ParameterSubstitution.cs                 // Template parameter replacement
├── Exceptions/
│   ├── MessageNotFoundException.cs              // Code not found
│   └── FormatterNotFoundException.cs            // Formatter not found
└── Messages/
    └── defaults.json                            // 200+ built-in messages

RecurPixel.EasyMessages.AspNetCore/              (ASP.NET Core Extensions)
├── Extensions/
│   ├── MessageResultExtensions.cs               // .ToApiResponse(), .ToMinimalApiResult()
│   └── MessageLoggingExtensions.cs              // .Log(ILogger)
├── Interceptors/
│   ├── LoggingInterceptor.cs                    // Logs to ILogger
│   ├── CorrelationIdInterceptor.cs              // Adds HttpContext.TraceIdentifier
│   └── MetadataEnrichmentInterceptor.cs         // Adds request metadata
├── Configuration/
│   ├── MessageConfiguration.cs                  // Main configuration options
│   ├── InterceptorOptions.cs                    // Interceptor behavior config
│   ├── MetadataEnrichmentFields.cs              // Metadata field selection
│   ├── InterceptorInitializer.cs                // Lazy interceptor setup
│   └── ServiceCollectionExtensions.cs           // .AddEasyMessages()
└── Models/
    └── ApiResponse.cs                           // Standard API response DTO
```

---

## Core Concepts

### 1. Message (Immutable Record)

```csharp
public sealed record Message
{
    public string Code { get; init; }                    // Message code (e.g., "AUTH_001")
    public MessageType Type { get; init; }               // Success, Info, Warning, Error, Critical
    public string Title { get; init; }                   // Short title
    public string Description { get; init; }             // Detailed description
    public int HttpStatusCode { get; init; }             // HTTP status code
    public DateTime Timestamp { get; init; }             // UTC timestamp
    public string? CorrelationId { get; init; }          // Correlation/trace ID
    public object? Data { get; init; }                   // Additional payload
    public Dictionary<string, object>? Metadata { get; init; }  // Context metadata
}
```

**Key Features:**
- Immutable by design (thread-safe)
- All `.With*()` methods return new instances
- Auto-generated timestamp on creation

### 2. Message Registry (Static Singleton)

```csharp
public static class MessageRegistry
{
    // Loads messages from stores (embedded defaults + custom overrides)
    public static Message Get(string code);
    
    // Configure custom message sources
    public static void LoadCustomMessages(string jsonPath);
    public static void UseStore(IMessageStore store);
    public static void UseStores(params IMessageStore[] stores);
    
    // Introspection
    public static IEnumerable<string> GetAllCodes();
}
```

**Loading Priority:**
1. Custom stores (last wins)
2. Embedded defaults (always loaded first)

### 3. Formatter Registry (Static Singleton)

```csharp
public static class FormatterRegistry
{
    // Register formatters
    public static void Register(string name, Func<IMessageFormatter> factory);
    public static void Register<TFormatter>(string? name = null) where TFormatter : IMessageFormatter, new();
    public static void RegisterSingleton(string name, IMessageFormatter instance);
    
    // Get formatters
    public static IMessageFormatter Get(string name);
    public static TFormatter Get<TFormatter>() where TFormatter : IMessageFormatter, new();
    
    // Introspection
    public static bool IsRegistered(string name);
    public static IEnumerable<string> GetRegisteredNames();
}
```

**Built-in Formatters:**
- `json` - JSON output
- `xml` - XML output
- `text` - Plain text output
- `console` - Colored console output

### 4. Interceptor Registry (Static Singleton)

```csharp
public static class InterceptorRegistry
{
    // Register interceptors
    public static void Register(IMessageInterceptor interceptor);
    public static void Clear();
    
    // Internal - called by MessageFormatterBase
    internal static Message InvokeBeforeFormat(Message message);
    internal static Message InvokeAfterFormat(Message message);
}
```

**Execution Order:**
1. `OnBeforeFormat()` - All interceptors in registration order
2. Formatter logic
3. `OnAfterFormat()` - All interceptors in registration order

### 5. Formatter Configuration (Static Singleton)

```csharp
public static class FormatterConfiguration
{
    // Global defaults
    public static FormatterOptions DefaultOptions { get; }
    public static void SetDefaultOptions(FormatterOptions options);
    public static void Configure(Action<FormatterOptions> configure);
    public static void Reset();
    
    // Presets
    public static FormatterOptions Minimal { get; }
    public static FormatterOptions Verbose { get; }
    public static FormatterOptions ProductionSafe { get; }
    public static FormatterOptions Debug { get; }
    public static FormatterOptions ApiClient { get; }
    public static FormatterOptions Logging { get; }
}
```

---

## Installation

### For Console, Desktop, or Class Libraries
```bash
dotnet add package RecurPixel.EasyMessages
```

### For ASP.NET Core Web APIs
```bash
dotnet add package RecurPixel.EasyMessages
dotnet add package RecurPixel.EasyMessages.AspNetCore
```

---

## Quick Start

### Console Application

```csharp
using RecurPixel.EasyMessages;

class Program
{
    static void Main(string[] args)
    {
        // Works immediately - no configuration needed
        Msg.Auth.LoginFailed().ToConsole(useColors: true);
        
        // Output to JSON
        var json = Msg.Crud.Created("User")
            .WithData(new { Id = 123, Name = "John" })
            .ToJson();
        
        Console.WriteLine(json);
    }
}
```

**Output:**
```
✗ Authentication Failed
  Invalid username or password.
  [2024-01-15 14:30:00] [AUTH_001]

{
  "success": true,
  "code": "CRUD_001",
  "type": "success",
  "title": "Created Successfully",
  "description": "User has been created.",
  "timestamp": "2024-01-15T14:30:00Z",
  "data": {
    "id": 123,
    "name": "John"
  }
}
```

### ASP.NET Core Application

```csharp
// Program.cs
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure EasyMessages
builder.Services.AddEasyMessages(options =>
{
    // Enable automatic logging
    options.AutoLog = true;
    options.MinimumLogLevel = LogLevel.Warning;
    
    // Enable correlation ID
    options.InterceptorOptions.AutoAddCorrelationId = true;
    
    // Configure formatter output
    options.FormatterOptions.IncludeTimestamp = true;
    options.FormatterOptions.IncludeMetadata = false;
});

var app = builder.Build();
app.Run();

// Controller
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }
    
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Validate credentials
        var user = ValidateCredentials(request.Username, request.Password);
        
        if (user == null)
        {
            // Returns 401 with error message
            // Automatically logged with correlation ID
            return Msg.Auth.LoginFailed()
                .WithMetadata("username", request.Username)
                .ToApiResponse();
        }
        
        // Returns 200 with success message
        return Msg.Auth.LoginSuccessful()
            .WithData(new { Token = GenerateToken(user) })
            .ToApiResponse();
    }
}
```

**API Response (Error):**
```json
{
  "success": false,
  "code": "AUTH_001",
  "type": "error",
  "title": "Authentication Failed",
  "description": "Invalid username or password.",
  "timestamp": "2024-01-15T14:30:00Z",
  "correlationId": "0HMVD8F3K2T9A",
  "metadata": {
    "username": "john.doe"
  }
}
```

**Console Log:**
```
[2024-01-15 14:30:00] [Error] EasyMessages: [AUTH_001] Authentication Failed: Invalid username or password.
  => CorrelationId: 0HMVD8F3K2T9A
```

---

## Configuration

### Core Library Configuration

#### Global Formatter Configuration

```csharp
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Configuration;

// Console or desktop app
FormatterConfiguration.Configure(options =>
{
    options.IncludeTimestamp = true;
    options.IncludeCorrelationId = true;
    options.IncludeMetadata = false;
    options.IncludeData = true;
});

// All messages now use these settings
var json = Msg.Auth.LoginFailed().ToJson();
```

#### Use Presets

```csharp
// Minimal output (only essential fields)
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Minimal);

// Verbose output (include everything)
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Verbose);

// Production-safe (no sensitive data)
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.ProductionSafe);

// Debug (everything for troubleshooting)
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Debug);
```

#### Custom Message Stores

```csharp
// Load from file
MessageRegistry.LoadCustomMessages("messages/custom.json");

// Or use store pattern
MessageRegistry.UseStore(new FileMessageStore("messages/custom.json"));

// Composite stores (priority: last wins)
MessageRegistry.UseStores(
    new EmbeddedMessageStore(),     // Built-in defaults
    new FileMessageStore("custom.json"),
    new MyDatabaseStore(connectionString)
);
```

#### Custom Formatters

```csharp
// Create custom formatter
public class CsvFormatter : MessageFormatterBase
{
    protected override string FormatCore(Message message)
    {
        return $"{message.Code},{message.Type},{message.Title},{message.Description}";
    }
    
    protected override object FormatAsObjectCore(Message message)
    {
        return new[] { message.Code, message.Type.ToString(), message.Title, message.Description };
    }
}

// Register it
FormatterRegistry.Register("csv", () => new CsvFormatter());

// Use it
var csv = message.ToFormat("csv");
```

### ASP.NET Core Configuration

#### Basic Configuration

```csharp
builder.Services.AddEasyMessages(options =>
{
    // Store configuration
    options.CustomMessagesPath = "messages/custom.json";
    
    // Logging
    options.AutoLog = true;
    options.MinimumLogLevel = LogLevel.Warning;
    
    // Interceptors
    options.InterceptorOptions.AutoAddCorrelationId = true;
    options.InterceptorOptions.AutoEnrichMetadata = false;
    
    // Formatter output
    options.FormatterOptions.IncludeTimestamp = true;
    options.FormatterOptions.IncludeCorrelationId = true;
    options.FormatterOptions.IncludeMetadata = false;
});
```

#### Environment-Based Configuration

```csharp
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEasyMessages(options =>
    {
        // Rich debugging info
        options.AutoLog = true;
        options.InterceptorOptions.AutoEnrichMetadata = true;
        options.InterceptorOptions.MetadataFields.IncludeRequestPath = true;
        options.InterceptorOptions.MetadataFields.IncludeRequestMethod = true;
        options.InterceptorOptions.MetadataFields.IncludeUserAgent = true;
        options.FormatterOptions = FormatterConfiguration.Debug;
    });
}
else if (builder.Environment.IsStaging())
{
    builder.Services.AddEasyMessages(options =>
    {
        options.AutoLog = true;
        options.InterceptorOptions.AutoAddCorrelationId = true;
        options.FormatterOptions = FormatterConfiguration.Verbose;
    });
}
else // Production
{
    builder.Services.AddEasyMessages(options =>
    {
        options.AutoLog = true;
        options.MinimumLogLevel = LogLevel.Error;
        options.InterceptorOptions.AutoAddCorrelationId = true;
        options.FormatterOptions = FormatterConfiguration.ProductionSafe;
    });
}
```

#### Advanced Interceptor Configuration

```csharp
builder.Services.AddEasyMessages(options =>
{
    // Correlation ID
    options.InterceptorOptions.AutoAddCorrelationId = true;
    
    // Metadata enrichment
    options.InterceptorOptions.AutoEnrichMetadata = true;
    options.InterceptorOptions.MetadataFields.IncludeRequestPath = true;
    options.InterceptorOptions.MetadataFields.IncludeRequestMethod = true;
    options.InterceptorOptions.MetadataFields.IncludeUserAgent = false;
    options.InterceptorOptions.MetadataFields.IncludeIpAddress = true;
    options.InterceptorOptions.MetadataFields.IncludeUserId = true;
    options.InterceptorOptions.MetadataFields.IncludeUserName = true;
    
    // Custom interceptors
    options.Interceptors = new List<IMessageInterceptor>
    {
        new MyCustomInterceptor()
    };
});
```

#### Custom Stores in ASP.NET

```csharp
builder.Services.AddEasyMessages(options =>
{
    // Multiple stores
    options.CustomStores = new List<IMessageStore>
    {
        new FileMessageStore("messages/custom.json"),
        new MyDatabaseStore(builder.Configuration.GetConnectionString("Default"))
    };
});
```

#### Custom Formatters in ASP.NET

```csharp
builder.Services.AddEasyMessages(options =>
{
    options.CustomFormatters = new Dictionary<string, Func<IMessageFormatter>>
    {
        ["csv"] = () => new CsvFormatter(),
        ["markdown"] = () => new MarkdownFormatter(includeMetadata: true),
        ["slack"] = () => new SlackFormatter(httpContextAccessor)
    };
});
```

---

## Advanced Features

### 1. Parameter Substitution

**Template (defaults.json):**
```json
{
  "CRUD_001": {
    "type": "Success",
    "title": "Created Successfully",
    "description": "{resource} has been created."
  },
  "VAL_002": {
    "type": "Error",
    "title": "Required Field",
    "description": "{field} is required."
  }
}
```

**Usage:**
```csharp
// Simple substitution
Msg.Crud.Created("User");
// Result: "User has been created."

// Multiple parameters
Msg.Custom("WELCOME_001")
    .WithParams(new { name = "John", role = "Admin" });
// Template: "Welcome {name}, your role is {role}."
// Result: "Welcome John, your role is Admin."

// Case-insensitive
Msg.Validation.RequiredField("Email");
// or
Msg.Custom("VAL_002").WithParams(new { Field = "Email" });
// or
Msg.Custom("VAL_002").WithParams(new { field = "Email" });
// All produce: "Email is required."
```

### 2. Message Chaining

```csharp
var response = Msg.Auth.LoginFailed()
    .WithData(new { UserId = 123 })
    .WithMetadata("attempt", 3)
    .WithMetadata("ipAddress", "192.168.1.1")
    .WithCorrelationId(Guid.NewGuid().ToString())
    .WithStatusCode(403)  // Override default 401
    .ToApiResponse();
```

### 3. Custom Message Stores

#### File-Based Store
```csharp
// custom.json
{
  "PAYMENT_001": {
    "type": "Success",
    "title": "Payment Processed",
    "description": "Payment of {amount} processed successfully.",
    "httpStatusCode": 200
  }
}

// Usage
MessageRegistry.LoadCustomMessages("custom.json");

Msg.Custom("PAYMENT_001")
    .WithParams(new { amount = "$99.99" })
    .ToApiResponse();
```

#### Database Store (User Implementation)
```csharp
public class SqlServerMessageStore : DatabaseMessageStore
{
    private readonly string _connectionString;
    
    public SqlServerMessageStore(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public override async Task<Dictionary<string, MessageTemplate>> LoadAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        var messages = await connection.QueryAsync<MessageTemplate>(
            "SELECT Code, Type, Title, Description, HttpStatusCode FROM Messages");
        
        return messages.ToDictionary(m => m.Code, m => m);
    }
}

// Register
MessageRegistry.UseStore(new SqlServerMessageStore(connectionString));
```

### 4. Custom Interceptors

#### Simple Interceptor (Core)
```csharp
public class TimestampInterceptor : IMessageInterceptor
{
    public Message OnBeforeFormat(Message message)
    {
        Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Formatting message {message.Code}");
        return message;
    }
    
    public Message OnAfterFormat(Message message)
    {
        Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Formatted message {message.Code}");
        return message;
    }
}

// Register
InterceptorRegistry.Register(new TimestampInterceptor());
```

#### AspNetCore Interceptor with DI
```csharp
public class TenantInterceptor : IMessageInterceptor
{
    private readonly Func<IHttpContextAccessor> _httpContextAccessorFactory;
    private IHttpContextAccessor? _httpContextAccessor;
    
    public TenantInterceptor(Func<IHttpContextAccessor> httpContextAccessorFactory)
    {
        _httpContextAccessorFactory = httpContextAccessorFactory;
    }
    
    private IHttpContextAccessor HttpContextAccessor => 
        _httpContextAccessor ??= _httpContextAccessorFactory();
    
    public Message OnBeforeFormat(Message message)
    {
        var tenantId = HttpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Id"].ToString();
        
        if (!string.IsNullOrEmpty(tenantId))
        {
            var metadata = new Dictionary<string, object>(message.Metadata ?? new())
            {
                ["TenantId"] = tenantId
            };
            return message with { Metadata = metadata };
        }
        
        return message;
    }
    
    public Message OnAfterFormat(Message message) => message;
}

// Register
builder.Services.AddEasyMessages(options =>
{
    options.Interceptors = new List<IMessageInterceptor>
    {
        new TenantInterceptor(() => serviceProvider.GetRequiredService<IHttpContextAccessor>())
    };
});
```

### 5. Custom Formatters

#### Markdown Formatter
```csharp
public class MarkdownFormatter : MessageFormatterBase
{
    private readonly bool _includeMetadata;
    
    public MarkdownFormatter(bool includeMetadata = false)
    {
        _includeMetadata = includeMetadata;
    }
    
    protected override string FormatCore(Message message)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# {message.Title}");
        sb.AppendLine($"**Code:** `{message.Code}`");
        sb.AppendLine($"**Type:** {message.Type}");
        sb.AppendLine($"**Status:** {message.HttpStatusCode}");
        sb.AppendLine();
        sb.AppendLine(message.Description);
        
        if (_includeMetadata && message.Metadata?.Any() == true)
        {
            sb.AppendLine();
            sb.AppendLine("## Metadata");
            foreach (var (key, value) in message.Metadata)
            {
                sb.AppendLine($"- **{key}:** {value}");
            }
        }
        
        return sb.ToString();
    }
    
    protected override object FormatAsObjectCore(Message message)
    {
        return FormatCore(message);
    }
}

// Register
FormatterRegistry.Register("markdown", () => new MarkdownFormatter(includeMetadata: true));

// Use
var markdown = message.ToFormat("markdown");
```

#### Slack Formatter
```csharp
public class SlackFormatter : MessageFormatterBase
{
    protected override object FormatAsObjectCore(Message message)
    {
        var icon = message.Type switch
        {
            MessageType.Success => ":white_check_mark:",
            MessageType.Info => ":information_source:",
            MessageType.Warning => ":warning:",
            MessageType.Error => ":x:",
            MessageType.Critical => ":fire:",
            _ => ":bell:"
        };
        
        return new
        {
            text = $"{icon} *{message.Title}*\n_{message.Description}_",
            attachments = new[]
            {
                new
                {
                    color = GetColor(message.Type),
                    fields = new[]
                    {
                        new { title = "Code", value = message.Code, @short = true },
                        new { title = "Type", value = message.Type.ToString(), @short = true }
                    }
                }
            }
        };
    }
    
    protected override string FormatCore(Message message)
    {
        return JsonSerializer.Serialize(FormatAsObjectCore(message));
    }
    
    private static string GetColor(MessageType type) => type switch
    {
        MessageType.Success => "good",
        MessageType.Info => "#439FE0",
        MessageType.Warning => "warning",
        MessageType.Error => "danger",
        MessageType.Critical => "#FF0000",
        _ => "#808080"
    };
}
```

### 6. Message Override Strategies

#### Partial Override (Merge with Defaults)
```json
// defaults.json (built-in)
{
  "AUTH_001": {
    "type": "Error",
    "title": "Authentication Failed",
    "description": "Invalid username or password.",
    "httpStatusCode": 401
  }
}

// custom.json (user override)
{
  "AUTH_001": {
    "title": "Login Failed",
    "description": "The credentials you provided are incorrect. Please try again."
    // type and httpStatusCode inherited from defaults
  }
}
```

#### Complete Override
```json
// custom.json
{
  "AUTH_001": {
    "type": "Error",
    "title": "Login Failed",
    "description": "The credentials you provided are incorrect. Please try again.",
    "httpStatusCode": 403
  }
}
```

#### Add New Messages
```json
// custom.json
{
  "MYAPP_001": {
    "type": "Success",
    "title": "Welcome",
    "description": "Welcome to {appName}!"
  },
  "MYAPP_002": {
    "type": "Error",
    "title": "Subscription Required",
    "description": "This feature requires an active subscription."
  }
}

// Usage
Msg.Custom("MYAPP_001")
    .WithParams(new { appName = "MyApp" })
    .ToApiResponse();
```

---

## API Reference

### Message Facade (Msg)

```csharp
// Authentication
Msg.Auth.LoginFailed()
Msg.Auth.Unauthorized()
Msg.Auth.LoginSuccessful()

// CRUD Operations
Msg.Crud.Created(string resource)
Msg.Crud.Updated(string resource)
Msg.Crud.Deleted(string resource)
Msg.Crud.NotFound(string resource)

// Validation
Msg.Validation.Failed()
Msg.Validation.RequiredField(string field)
Msg.Validation.InvalidFormat(string field)

// System
Msg.System.Error()
Msg.System.Processing()

// Database
Msg.Database.ConnectionFailed()

// File Operations
Msg.File.Uploaded()
Msg.File.InvalidType(params string[] types)

// Custom Messages
Msg.Custom(string code)
```

### Message Extensions (Core)

```csharp
// Data enrichment
message.WithData(object data)
message.WithMetadata(string key, object value)
message.WithParams(object parameters)
message.WithCorrelationId(string correlationId)
message.WithStatusCode(int statusCode)

// Output formats
message.ToJson(FormatterOptions? options, JsonSerializerOptions? jsonOptions)
message.ToJsonObject(FormatterOptions? options, JsonSerializerOptions? jsonOptions)
message.ToXml(FormatterOptions? options)
message.ToXmlDocument(FormatterOptions? options)
message.ToPlainText(FormatterOptions? options)
message.ToConsole(bool useColors = true)
message.ToFormat<TFormatter>()
message.ToFormat(string formatterName)
message.ToFormatObject<TFormatter>()
```

### Message Extensions (AspNetCore)

```csharp
// API responses
message.ToApiResponse()           // Returns IActionResult
message.ToMinimalApiResult()      // Returns IResult

// Logging
message.Log(ILogger logger)
```

### Built-in Message Codes

#### Authentication (AUTH_*)
- `AUTH_001` - Login Failed (401)
- `AUTH_002` - Unauthorized (403)
- `AUTH_003` - Login Success (200)

#### CRUD Operations (CRUD_*)
- `CRUD_001` - Created Successfully (200)
- `CRUD_002` - Updated Successfully (200)
- `CRUD_003` - Deleted Successfully (200)
- `CRUD_004` - Not Found (404)

#### Validation (VAL_*)
- `VAL_001` - Validation Failed (422)
- `VAL_002` - Required Field (422)
- `VAL_003` - Invalid Format (422)

#### System (SYS_*)
- `SYS_001` - System Error (500)
- `SYS_002` - Processing (200)

#### Database (DB_*)
- `DB_001` - Connection Failed (503)

#### File Operations (FILE_*)
- `FILE_001` - File Uploaded (200)
- `FILE_002` - Invalid File Type (415)

*Note: 200+ messages available in defaults.json*

---

## Examples

### Example 1: REST API with Validation

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;
    
    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        // Validation
        if (string.IsNullOrEmpty(request.Email))
        {
            return Msg.Validation.RequiredField("Email")
                .ToApiResponse();
        }
        
        if (!IsValidEmail(request.Email))
        {
            return Msg.Validation.InvalidFormat("Email")
                .ToApiResponse();
        }
        
        // Check for duplicates
        if (await _userService.EmailExistsAsync(request.Email))
        {
            return Msg.Validation.Failed()
                .WithMetadata("reason", "duplicate_email")
                .WithMetadata("field", "email")
                .ToApiResponse();
        }
        
        // Create user
        try
        {
            var user = await _userService.CreateAsync(request);
            
            return Msg.Crud.Created("User")
                .WithData(new 
                { 
                    user.Id, 
                    user.Email, 
                    user.Name,
                    CreatedAt = user.CreatedAt
                })
                .ToApiResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user");
            
            return Msg.System.Error()
                .WithMetadata("error", ex.Message)
                .ToApiResponse();
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        
        if (user == null)
        {
            return Msg.Crud.NotFound("User")
                .WithMetadata("userId", id)
                .ToApiResponse();
        }
        
        return Ok(user);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userService.GetByIdAsync(id);
        
        if (user == null)
        {
            return Msg.Crud.NotFound("User")
                .WithMetadata("userId", id)
                .ToApiResponse();
        }
        
        await _userService.UpdateAsync(id, request);
        
        return Msg.Crud.Updated("User")
            .WithData(new { id })
            .ToApiResponse();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        
        if (user == null)
        {
            return Msg.Crud.NotFound("User")
                .WithMetadata("userId", id)
                .ToApiResponse();
        }
        
        await _userService.DeleteAsync(id);
        
        return Msg.Crud.Deleted("User")
            .WithData(new { id })
            .ToApiResponse();
    }
    
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
```

### Example 2: Global Exception Handler

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasyMessages(options =>
{
    options.AutoLog = true;
    options.InterceptorOptions.AutoAddCorrelationId = true;
});

var app = builder.Build();

// Global exception handler middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unhandled exception");
        
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        
        var errorMessage = Msg.System.Error()
            .WithMetadata("path", context.Request.Path.Value ?? "")
            .WithMetadata("method", context.Request.Method)
            .WithCorrelationId(context.TraceIdentifier);
        
        if (app.Environment.IsDevelopment())
        {
            errorMessage = errorMessage
                .WithMetadata("exceptionType", ex.GetType().Name)
                .WithMetadata("exceptionMessage", ex.Message)
                .WithMetadata("stackTrace", ex.StackTrace ?? "");
        }
        
        var json = errorMessage.ToJson();
        await context.Response.WriteAsync(json);
    }
});

app.MapControllers();
app.Run();
```

### Example 3: FluentValidation Integration

```csharp
using FluentValidation;

public class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(Msg.Validation.RequiredField("Email").Description)
            .EmailAddress()
            .WithMessage(Msg.Validation.InvalidFormat("Email").Description);
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(Msg.Validation.RequiredField("Name").Description)
            .MinimumLength(2)
            .WithMessage("Name must be at least 2 characters long");
        
        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18)
            .WithMessage("User must be at least 18 years old");
    }
}

// In controller
[HttpPost]
public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
{
    var validator = new CreateUserValidator();
    var validationResult = await validator.ValidateAsync(request);
    
    if (!validationResult.IsValid)
    {
        return Msg.Validation.Failed()
            .WithData(validationResult.Errors.Select(e => new
            {
                Field = e.PropertyName,
                Error = e.ErrorMessage
            }))
            .ToApiResponse();
    }
    
    // Continue with user creation...
}
```

### Example 4: Background Job with Console Output

```csharp
using RecurPixel.EasyMessages;

public class DataImportJob
{
    private readonly ILogger<DataImportJob> _logger;
    
    public DataImportJob(ILogger<DataImportJob> logger)
    {
        _logger = logger;
    }
    
    public async Task ImportUsersAsync(string filePath)
    {
        Msg.System.Processing()
            .WithData(new { File = filePath })
            .ToConsole(useColors: true);
        
        try
        {
            var users = await ReadUsersFromFileAsync(filePath);
            
            foreach (var user in users)
            {
                try
                {
                    await ImportUserAsync(user);
                    
                    Msg.Crud.Created("User")
                        .WithData(new { user.Email })
                        .ToConsole(useColors: true);
                }
                catch (Exception ex)
                {
                    Msg.System.Error()
                        .WithData(new { user.Email, Error = ex.Message })
                        .ToConsole(useColors: true);
                }
            }
            
            Msg.Custom("IMPORT_COMPLETE")
                .WithParams(new { count = users.Count })
                .ToConsole(useColors: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Import failed");
            
            Msg.System.Error()
                .WithMetadata("file", filePath)
                .WithMetadata("error", ex.Message)
                .ToConsole(useColors: true);
        }
    }
    
    private async Task<List<UserImportDto>> ReadUsersFromFileAsync(string filePath)
    {
        // Implementation
        throw new NotImplementedException();
    }
    
    private async Task ImportUserAsync(UserImportDto user)
    {
        // Implementation
        throw new NotImplementedException();
    }
}
```

### Example 5: Minimal API with Custom Messages

```csharp
// Program.cs
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Load custom messages
builder.Services.AddEasyMessages(options =>
{
    options.CustomMessagesPath = "messages/custom.json";
    options.AutoLog = true;
    options.InterceptorOptions.AutoAddCorrelationId = true;
});

var app = builder.Build();

// Payment endpoint with custom messages
app.MapPost("/api/payments", async (PaymentRequest request, IPaymentService paymentService) =>
{
    if (request.Amount <= 0)
    {
        return Msg.Custom("PAYMENT_INVALID_AMOUNT")
            .WithParams(new { amount = request.Amount })
            .ToMinimalApiResult();
    }
    
    try
    {
        var result = await paymentService.ProcessAsync(request);
        
        return Msg.Custom("PAYMENT_SUCCESS")
            .WithParams(new { transactionId = result.TransactionId, amount = request.Amount })
            .WithData(new
            {
                result.TransactionId,
                result.Status,
                ProcessedAt = DateTime.UtcNow
            })
            .ToMinimalApiResult();
    }
    catch (InsufficientFundsException)
    {
        return Msg.Custom("PAYMENT_INSUFFICIENT_FUNDS")
            .WithParams(new { required = request.Amount, available = request.AccountBalance })
            .ToMinimalApiResult();
    }
    catch (Exception ex)
    {
        return Msg.Custom("PAYMENT_FAILED")
            .WithMetadata("error", ex.Message)
            .ToMinimalApiResult();
    }
});

app.Run();
```

**custom.json:**
```json
{
  "PAYMENT_SUCCESS": {
    "type": "Success",
    "title": "Payment Processed",
    "description": "Payment of {amount} processed successfully. Transaction ID: {transactionId}",
    "httpStatusCode": 200
  },
  "PAYMENT_FAILED": {
    "type": "Error",
    "title": "Payment Failed",
    "description": "Unable to process payment. Please try again later.",
    "httpStatusCode": 400
  },
  "PAYMENT_INSUFFICIENT_FUNDS": {
    "type": "Error",
    "title": "Insufficient Funds",
    "description": "Payment requires {required}, but only {available} is available.",
    "httpStatusCode": 402
  },
  "PAYMENT_INVALID_AMOUNT": {
    "type": "Error",
    "title": "Invalid Amount",
    "description": "Payment amount {amount} is invalid. Amount must be greater than zero.",
    "httpStatusCode": 400
  }
}
```

### Example 6: Multi-Tenant Application

```csharp
// Custom interceptor for tenant context
public class TenantInterceptor : IMessageInterceptor
{
    private readonly Func<IHttpContextAccessor> _httpContextAccessorFactory;
    private IHttpContextAccessor? _httpContextAccessor;
    
    public TenantInterceptor(Func<IHttpContextAccessor> httpContextAccessorFactory)
    {
        _httpContextAccessorFactory = httpContextAccessorFactory;
    }
    
    private IHttpContextAccessor HttpContextAccessor => 
        _httpContextAccessor ??= _httpContextAccessorFactory();
    
    public Message OnBeforeFormat(Message message)
    {
        var context = HttpContextAccessor.HttpContext;
        if (context == null) return message;
        
        var tenantId = context.Request.Headers["X-Tenant-Id"].ToString();
        
        if (!string.IsNullOrEmpty(tenantId))
        {
            var metadata = new Dictionary<string, object>(message.Metadata ?? new())
            {
                ["TenantId"] = tenantId
            };
            return message with { Metadata = metadata };
        }
        
        return message;
    }
    
    public Message OnAfterFormat(Message message) => message;
}

// Program.cs
builder.Services.AddHttpContextAccessor();

builder.Services.AddEasyMessages(options =>
{
    options.AutoLog = true;
    options.InterceptorOptions.AutoAddCorrelationId = true;
    
    // Add tenant interceptor
    options.Interceptors = new List<IMessageInterceptor>
    {
        new TenantInterceptor(() => 
            builder.Services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>())
    };
});
```

### Example 7: Export to Different Formats

```csharp
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus([FromQuery] string format = "json")
    {
        var message = Msg.System.Processing()
            .WithData(new
            {
                Server = Environment.MachineName,
                Uptime = GetUptime(),
                ActiveConnections = GetActiveConnections()
            })
            .WithMetadata("generatedAt", DateTime.UtcNow);
        
        return format.ToLower() switch
        {
            "json" => Ok(message.ToJsonObject()),
            "xml" => Content(message.ToXml(), "application/xml"),
            "text" => Content(message.ToPlainText(), "text/plain"),
            "csv" => Content(message.ToFormat("csv"), "text/csv"),
            "markdown" => Content(message.ToFormat("markdown"), "text/markdown"),
            _ => BadRequest(new { error = "Invalid format. Supported: json, xml, text, csv, markdown" })
        };
    }
    
    private TimeSpan GetUptime()
    {
        return TimeSpan.FromMilliseconds(Environment.TickCount64);
    }
    
    private int GetActiveConnections()
    {
        // Implementation
        return 42;
    }
}
```

### Example 8: Testing with EasyMessages

```csharp
using Xunit;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Core;

public class MessageTests
{
    [Fact]
    public void LoginFailed_ShouldReturnCorrectMessage()
    {
        // Arrange & Act
        var message = Msg.Auth.LoginFailed();
        
        // Assert
        Assert.Equal("AUTH_001", message.Code);
        Assert.Equal(MessageType.Error, message.Type);
        Assert.Equal("Authentication Failed", message.Title);
        Assert.Equal(401, message.HttpStatusCode);
    }
    
    [Fact]
    public void WithData_ShouldAddDataToMessage()
    {
        // Arrange
        var userData = new { Id = 123, Name = "John" };
        
        // Act
        var message = Msg.Auth.LoginSuccessful()
            .WithData(userData);
        
        // Assert
        Assert.NotNull(message.Data);
        Assert.Equal(userData, message.Data);
    }
    
    [Fact]
    public void WithParams_ShouldSubstituteParameters()
    {
        // Arrange & Act
        var message = Msg.Crud.Created("User");
        
        // Assert
        Assert.Contains("User has been created", message.Description);
    }
    
    [Fact]
    public void ToJson_ShouldProduceValidJson()
    {
        // Arrange
        var message = Msg.Auth.LoginFailed();
        
        // Act
        var json = message.ToJson();
        
        // Assert
        Assert.Contains("\"code\":\"AUTH_001\"", json);
        Assert.Contains("\"success\":false", json);
    }
    
    [Fact]
    public void CustomMessage_ShouldWork()
    {
        // Arrange
        MessageRegistry.LoadCustomMessages("test-messages.json");
        
        // Act
        var message = Msg.Custom("TEST_001");
        
        // Assert
        Assert.Equal("TEST_001", message.Code);
        
        // Cleanup
        MessageRegistry.Reset();
    }
}

public class InterceptorTests
{
    [Fact]
    public void Interceptor_ShouldModifyMessage()
    {
        // Arrange
        InterceptorRegistry.Register(new TestInterceptor());
        
        // Act
        var message = Msg.Auth.LoginFailed()
            .ToJson();
        
        // Assert
        Assert.Contains("intercepted", message.ToLower());
        
        // Cleanup
        InterceptorRegistry.Clear();
    }
    
    private class TestInterceptor : IMessageInterceptor
    {
        public Message OnBeforeFormat(Message message)
        {
            return message.WithMetadata("intercepted", true);
        }
        
        public Message OnAfterFormat(Message message) => message;
    }
}
```

---

## Best Practices

### 1. Message Code Naming Convention

```csharp
// [✓] GOOD - Clear category prefix
AUTH_001, AUTH_002, AUTH_003       // Authentication
CRUD_001, CRUD_002, CRUD_003       // CRUD operations
VAL_001, VAL_002, VAL_003          // Validation
PAY_001, PAY_002, PAY_003          // Payments

// [✓] GOOD - User custom codes
MYAPP_001, MYAPP_002               // App-specific
COMPANY_001, COMPANY_002           // Company-specific

// [ ] BAD - No clear category
ERROR_001, MSG_001, CODE_001
```

### 2. Use Built-in Messages When Possible

```csharp
// [✓] GOOD - Use built-in messages
return Msg.Auth.LoginFailed().ToApiResponse();
return Msg.Crud.NotFound("User").ToApiResponse();
return Msg.Validation.RequiredField("Email").ToApiResponse();

// [ ] BAD - Creating custom messages for common scenarios
return Msg.Custom("MY_LOGIN_FAILED").ToApiResponse();
return Msg.Custom("MY_NOT_FOUND").ToApiResponse();
```

### 3. Enrich Messages with Context

```csharp
// [✓] GOOD - Rich context for debugging
return Msg.Auth.LoginFailed()
    .WithMetadata("username", request.Username)
    .WithMetadata("ipAddress", HttpContext.Connection.RemoteIpAddress?.ToString())
    .WithMetadata("attemptCount", GetLoginAttempts(request.Username))
    .ToApiResponse();

// [ ] BAD - No context
return Msg.Auth.LoginFailed().ToApiResponse();
```

### 4. Use Appropriate Log Levels

```csharp
// Configure logging appropriately
builder.Services.AddEasyMessages(options =>
{
    if (builder.Environment.IsProduction())
    {
        // Only log errors and critical issues in production
        options.MinimumLogLevel = LogLevel.Error;
    }
    else
    {
        // Log warnings and above in development
        options.MinimumLogLevel = LogLevel.Warning;
    }
});
```

### 5. Separate Data Addition from Output Control

```csharp
// [✓] GOOD - Clear separation
builder.Services.AddEasyMessages(options =>
{
    // What to ADD (interceptors)
    options.InterceptorOptions.AutoAddCorrelationId = true;
    options.InterceptorOptions.AutoEnrichMetadata = true;
    
    // What to SHOW (formatters)
    options.FormatterOptions.IncludeCorrelationId = true;
    options.FormatterOptions.IncludeMetadata = false;  // Add but don't show
});

// Now:
// - Logs will have metadata for debugging
// - API responses won't expose internal metadata to clients
```

### 6. Use Presets for Common Scenarios

```csharp
// [✓] GOOD - Use presets
if (builder.Environment.IsProduction())
{
    options.FormatterOptions = FormatterConfiguration.ProductionSafe;
}
else if (builder.Environment.IsDevelopment())
{
    options.FormatterOptions = FormatterConfiguration.Debug;
}

// [ ] BAD - Manual configuration of every field
options.FormatterOptions.IncludeTimestamp = true;
options.FormatterOptions.IncludeCorrelationId = true;
options.FormatterOptions.IncludeMetadata = false;
// ... 10 more lines
```

### 7. Keep Custom Messages in Version Control

```csharp
// [✓] GOOD - Store custom messages in repository
/messages
  /custom.json              // Base messages
  /custom.dev.json          // Development overrides
  /custom.staging.json      // Staging overrides
  /custom.production.json   // Production overrides

// Load based on environment
var messagesPath = builder.Environment.EnvironmentName switch
{
    "Development" => "messages/custom.dev.json",
    "Staging" => "messages/custom.staging.json",
    "Production" => "messages/custom.production.json",
    _ => "messages/custom.json"
};

builder.Services.AddEasyMessages(options =>
{
    options.CustomMessagesPath = messagesPath;
});
```

### 8. Thread Safety Considerations

```csharp
// [✓] GOOD - Configure once at startup
// Program.cs
MessageRegistry.LoadCustomMessages("custom.json");
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.ProductionSafe);
InterceptorRegistry.Register(new MyInterceptor());

// [ ] BAD - Don't reconfigure during requests
// Controller
public IActionResult Get()
{
    MessageRegistry.LoadCustomMessages("other.json");  // [ ] NOT thread-safe!
    return Msg.Auth.LoginSuccessful().ToApiResponse();
}
```

### 9. Immutability Pattern

```csharp
// [✓] GOOD - Chain immutable operations
var baseMessage = Msg.Auth.LoginFailed();
var enrichedMessage = baseMessage
    .WithData(userData)
    .WithMetadata("attempt", 3);

// baseMessage is unchanged
Assert.Null(baseMessage.Data);
Assert.NotNull(enrichedMessage.Data);

// [ ] BAD - Assuming mutation
var message = Msg.Auth.LoginFailed();
message.WithData(userData);  // This doesn't modify message!
return message.ToApiResponse();  // Data is null!
```

### 10. Error Handling

```csharp
// [✓] GOOD - Handle exceptions gracefully
try
{
    MessageRegistry.LoadCustomMessages("custom.json");
}
catch (FileNotFoundException)
{
    // Log warning and continue with defaults
    logger.LogWarning("Custom messages file not found, using defaults");
}
catch (JsonException ex)
{
    // Log error - this is a configuration issue
    logger.LogError(ex, "Invalid JSON in custom messages file");
    throw;  // Fail fast in this case
}

// [✓] GOOD - Handle missing message codes
try
{
    var message = Msg.Custom("UNKNOWN_CODE");
}
catch (MessageNotFoundException ex)
{
    logger.LogError(ex, "Message code not found");
    // Fallback to generic error
    return Msg.System.Error().ToApiResponse();
}
```

---

## Performance Considerations

### 1. Message Registry Caching
- Messages are loaded once at startup
- `MessageRegistry.Get()` is O(1) dictionary lookup
- Thread-safe via lazy initialization

### 2. Formatter Registry Caching
- Formatters are cached after first creation
- Singleton registration for shared instances
- Factory registration for per-use instances

### 3. Interceptor Execution
- Interceptors run sequentially
- Keep interceptor logic lightweight
- Async operations in interceptors should be avoided (use background jobs instead)

### 4. Memory Footprint
- Core library: ~100 KB
- AspNetCore extensions: ~50 KB
- Embedded messages: ~20 KB
- Total: ~170 KB minimal overhead

### 5. Benchmarks (Typical Operations)

```
MessageRegistry.Get():           < 0.001ms
Message.WithData():              < 0.001ms
Message.ToJson():                < 0.1ms
Message.ToApiResponse():         < 0.1ms
Interceptor execution (3):       < 0.05ms
Full pipeline (Get → Format):    < 0.2ms
```

---

## Troubleshooting

### Common Issues

#### 1. Message Code Not Found
```
MessageNotFoundException: Message code 'XYZ_001' not found
```

**Solution:**
```csharp
// Check available codes
var codes = MessageRegistry.GetAllCodes();
Console.WriteLine(string.Join(", ", codes));

// Or load custom messages
MessageRegistry.LoadCustomMessages("custom.json");
```

#### 2. Formatter Not Found
```
FormatterNotFoundException: Formatter 'csv' not found
```

**Solution:**
```csharp
// Check registered formatters
var formatters = FormatterRegistry.GetRegisteredNames();
Console.WriteLine(string.Join(", ", formatters));

// Register the formatter
FormatterRegistry.Register("csv", () => new CsvFormatter());
```

#### 3. Interceptors Not Running
```
// Logs or correlation IDs not appearing
```

**Solution:**
```csharp
// Ensure AddEasyMessages is called
builder.Services.AddEasyMessages(options =>
{
    options.AutoLog = true;
    options.InterceptorOptions.AutoAddCorrelationId = true;
});

// Ensure formatters extend MessageFormatterBase
public class MyFormatter : MessageFormatterBase  // [✓] Interceptors work
{
    // ...
}

// Not this:
public class MyFormatter : IMessageFormatter  // [ ] Interceptors don't run
{
    // ...
}
```

#### 4. Custom Messages Not Loading
```
// Custom messages from custom.json not applied
```

**Solution:**
```csharp
// Ensure file path is correct
builder.Services.AddEasyMessages(options =>
{
    // Use absolute or relative path from app root
    options.CustomMessagesPath = Path.Combine(
        builder.Environment.ContentRootPath, 
        "messages", 
        "custom.json");
});

// Ensure JSON is valid
// Check file exists and has correct format
```

#### 5. Configuration Not Applied
```
// Global FormatterConfiguration settings not working in ASP.NET
```

**Solution:**
```csharp
// In ASP.NET, use MessageConfiguration, not FormatterConfiguration directly
builder.Services.AddEasyMessages(options =>
{
    // [✓] Correct
    options.FormatterOptions.IncludeMetadata = false;
    
    // [ ] This won't work in ASP.NET (gets overridden)
    FormatterConfiguration.Configure(opts => opts.IncludeMetadata = false);
});
```

---

## Migration Guide

### From Manual Error Handling

**Before:**
```csharp
public IActionResult Login(LoginRequest request)
{
    var user = _authService.Validate(request.Username, request.Password);
    
    if (user == null)
    {
        return Unauthorized(new
        {
            error = "Authentication failed",
            message = "Invalid username or password",
            code = "AUTH_FAILED",
            timestamp = DateTime.UtcNow
        });
    }
    
    return Ok(new
    {
        success = true,
        message = "Login successful",
        data = new { token = GenerateToken(user) }
    });
}
```

**After:**
```csharp
public IActionResult Login(LoginRequest request)
{
    var user = _authService.Validate(request.Username, request.Password);
    
    if (user == null)
    {
        return Msg.Auth.LoginFailed()
            .ToApiResponse();
    }
    
    return Msg.Auth.LoginSuccessful()
        .WithData(new { token = GenerateToken(user) })
        .ToApiResponse();
}
```

### From ProblemDetails

**Before:**
```csharp
return Problem(
    statusCode: 404,
    title: "Not Found",
    detail: "User not found",
    instance: HttpContext.Request.Path
);
```

**After:**
```csharp
return Msg.Crud.NotFound("User")
    .ToApiResponse();
```

---

## Changelog

### Version 1.0.0 (Initial Release)

**Core Features:**
- [✓] Immutable Message record type
- [✓] 200+ built-in messages (AUTH, CRUD, VAL, SYS, DB, FILE)
- [✓] Static Message Registry with store support
- [✓] Fluent API (Msg.Category.Action())
- [✓] Parameter substitution in templates
- [✓] Multiple output formatters (JSON, XML, Plain Text, Console)
- [✓] Formatter Registry with plugin architecture
- [✓] Interceptor Registry with before/after hooks
- [✓] FormatterConfiguration with presets
- [✓] Thread-safe registries
- [✓] Zero external dependencies (Core uses BCL only)

**AspNetCore Features:**
- [✓] .ToApiResponse() for MVC controllers
- [✓] .ToMinimalApiResult() for Minimal APIs
- [✓] .Log(ILogger) integration
- [✓] LoggingInterceptor with auto-logging
- [✓] CorrelationIdInterceptor with HttpContext integration
- [✓] MetadataEnrichmentInterceptor with configurable fields
- [✓] ServiceCollection extensions (.AddEasyMessages())
- [✓] Comprehensive configuration options
- [✓] Environment-based configuration support

**Store System:**
- [✓] IMessageStore interface
- [✓] EmbeddedMessageStore (defaults.json)
- [✓] FileMessageStore (custom JSON files)
- [✓] DictionaryMessageStore (in-memory)
- [✓] DatabaseMessageStore (abstract base)
- [✓] CompositeMessageStore (priority chain)

**Configuration:**
- [✓] Global FormatterConfiguration (Core)
- [✓] MessageConfiguration (AspNetCore)
- [✓] InterceptorOptions with fine-grained control
- [✓] FormatterOptions with output control
- [✓] Presets (Minimal, Verbose, ProductionSafe, Debug, ApiClient, Logging)

---

## Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

### Areas for Contribution
- Additional built-in messages
- New formatters (Markdown, CSV, etc.)
- New interceptors
- Documentation improvements
- Performance optimizations
- Bug fixes

---

## License

MIT License - see [LICENSE](LICENSE) for details.

---

## Support

- **Documentation:** [https://github.com/RecurPixel/EasyMessages/wiki](https://github.com/RecurPixel/EasyMessages/wiki)
- **Issues:** [https://github.com/RecurPixel/EasyMessages/issues](https://github.com/RecurPixel/EasyMessages/issues)
- **Discussions:** [https://github.com/RecurPixel/EasyMessages/discussions](https://github.com/RecurPixel/EasyMessages/discussions)
- **NuGet:** [https://www.nuget.org/packages/RecurPixel.EasyMessages](https://www.nuget.org/packages/RecurPixel.EasyMessages)

---

## Acknowledgments

Built with ❤️ by RecurPixel

Special thanks to:
- The .NET community for feedback and suggestions
- All contributors who helped shape this library
- Early adopters who provided valuable insights

---

**Happy Coding! 🚀**
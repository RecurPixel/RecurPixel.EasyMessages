# EasyMessages - Development Roadmap

## Overview

**Total Estimated Time:** 4-6 weeks (part-time)  
**Target:** Production-ready NuGet package  
**Approach:** Build → Test → Document → Iterate

---

## Phase 1: Foundation (Week 1)
**Goal:** Core infrastructure that everything else builds on

### Sprint 1.1: Project Setup (4-6 hours)
- [ ] Create GitHub repository with README
- [ ] Set up solution structure
  ```
  RecurPixel.EasyMessages/
  ├── src/
  │   ├── RecurPixel.EasyMessages/              (Main library)
  │   ├── RecurPixel.EasyMessages.AspNetCore/   (ASP.NET integration)
  │   └── RecurPixel.EasyMessages.Console/      (Console helpers)
  ├── tests/
  │   ├── RecurPixel.EasyMessages.Tests/
  │   └── RecurPixel.EasyMessages.Integration.Tests/
  └── samples/
      ├── WebApiSample/
      └── ConsoleSample/
  ```
- [ ] Configure .NET 8 project files
- [ ] Set up NuGet package metadata
- [ ] Initialize Git with `.gitignore`
- [ ] Set up CI/CD pipeline (GitHub Actions)

**Files to create:**
- `RecurPixel.EasyMessages.sln`
- `Directory.Build.props` (shared project properties)
- `.github/workflows/build.yml`
- `LICENSE` MIT

---

### Sprint 1.2: Core Domain Models (6-8 hours)

#### Create Core Classes

**Message.cs** - The heart of the library
```csharp
namespace EasyMessages.Core;

// public sealed record Message
public sealed record Message
{
    // Core properties
    public string Code { get; init; }
    public MessageType Type { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    
    // Optional context
    public object? Data { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }
    
    // Tracking
    public DateTime Timestamp { get; init; }
    public string? CorrelationId { get; init; }
    
    // HTTP
    public int HttpStatusCode { get; init; }
    
    // Internal constructor - force creation through MessageRegistry
    internal Message()
    {
        Timestamp = DateTime.UtcNow;

        Metadata = new Dictionary<string, object>();
    }
}
```

**MessageType.cs**
```csharp
namespace EasyMessages.Core;

public enum MessageType
{
    Success = 0,
    Info = 1,
    Warning = 2,
    Error = 3,
    Critical = 4
}
```

**MessageTemplate.cs** - What's stored in JSON
```csharp
namespace EasyMessages.Core;

internal sealed class MessageTemplate
{
    public MessageType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; }
    
    public Message ToMessage(string code)
    {
        return new Message
        {
            Code = code,
            Type = Type,
            Title = Title,
            Description = Description,
            HttpStatusCode = HttpStatusCode ?? GetDefaultStatusCode(Type)
        };
    }
    
    private static int GetDefaultStatusCode(MessageType type)
    {
        return type switch
        {
            MessageType.Success => 200,
            MessageType.Info => 200,
            MessageType.Warning => 200,
            MessageType.Error => 400,
            MessageType.Critical => 500,
            _ => 200
        };
    }
}
```

**Testing:**
```csharp
[Fact]
public void Message_ShouldHaveTimestamp()
{
    var message = new Message();
    Assert.True((DateTime.UtcNow - message.Timestamp).TotalSeconds < 1);
}

[Fact]
public void MessageTemplate_ShouldMapToMessage()
{
    var template = new MessageTemplate
    {
        Type = MessageType.Error,
        Title = "Test",
        Description = "Test Description"
    };
    
    var message = template.ToMessage("TEST_001");
    
    Assert.Equal("TEST_001", message.Code);
    Assert.Equal(MessageType.Error, message.Type);
    Assert.Equal(400, message.HttpStatusCode);
}
```
```csharp
// Precedence (highest to lowest):
1. .WithStatusCode(xxx)           // Explicit override wins
2. defaults.json httpStatusCode   // Template value
3. MessageType default mapping    // Fallback only * we will not use Mapping if it is absend first 2 steps we will keep it empty
```

---

### Sprint 1.3: Message Registry (8-10 hours)

**MessageRegistry.cs** - Message lookup engine
```csharp
namespace EasyMessages.Core;

public static class MessageRegistry
{
    private static readonly Lazy<Dictionary<string, MessageTemplate>> _defaults =
        new(() => LoadEmbeddedMessages());
    
    private static Dictionary<string, MessageTemplate>? _custom;
    private static readonly object _lock = new();
    
    public static Message Get(string code)
    {
        // Check custom first
        if (_custom?.TryGetValue(code, out var customTemplate) == true)
            return customTemplate.ToMessage(code);
        
        // Fall back to defaults
        if (_defaults.Value.TryGetValue(code, out var defaultTemplate))
            return defaultTemplate.ToMessage(code);
        
        // Not found
        throw new MessageNotFoundException(
            $"Message code '{code}' not found in registry. " +
            $"Available codes: {string.Join(", ", GetAllCodes().Take(10))}...");
    }
    
    public static void LoadCustomMessages(string jsonPath)
    {
        lock (_lock)
        {
            var json = File.ReadAllText(jsonPath);
            _custom = JsonSerializer.Deserialize<Dictionary<string, MessageTemplate>>(json);
        }
    }
    
    public static void LoadCustomMessages(Dictionary<string, MessageTemplate> messages)
    {
        lock (_lock)
        {
            _custom = messages;
        }
    }
    
    public static IEnumerable<string> GetAllCodes()
    {
        var codes = new HashSet<string>(_defaults.Value.Keys);
        if (_custom != null)
            codes.UnionWith(_custom.Keys);
        return codes.OrderBy(c => c);
    }
    
    private static Dictionary<string, MessageTemplate> LoadEmbeddedMessages()
    {
        var assembly = typeof(MessageRegistry).Assembly;
        var resourceName = "EasyMessages.Core.Messages.defaults.json";
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new InvalidOperationException($"Embedded resource '{resourceName}' not found");
        
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        
        return JsonSerializer.Deserialize<Dictionary<string, MessageTemplate>>(json)
            ?? throw new InvalidOperationException("Failed to deserialize default messages");
    }
}

public class MessageNotFoundException : Exception
{
    public MessageNotFoundException(string message) : base(message) { }
}
```

---

### Error Handling Strategy

**Specification:**
```csharp
// 1. defaults.json corrupted
try {
    var message = MessageRegistry.Get("AUTH_001");
} catch (InvalidOperationException ex) {
    // Message: "Failed to deserialize default messages"
    // Cause: Malformed JSON in embedded resource
}

// 2. custom.json invalid
try {
    MessageRegistry.LoadCustomMessages("bad.json");
} catch (JsonException ex) {
    // Message: "Custom messages file contains invalid JSON"
    // Cause: Malformed custom JSON file
}

// 3. Embedded resource missing (should never happen)
try {
    var message = MessageRegistry.Get("AUTH_001");
} catch (InvalidOperationException ex) {
    // Message: "Embedded resource 'defaults.json' not found"
    // Critical error - library is broken
}

// 4. Code not found
try {
    var message = MessageRegistry.Get("INVALID_999");
} catch (MessageNotFoundException ex) {
    // Message: "Message code 'INVALID_999' not found in registry. Available codes: AUTH_001, AUTH_002..."
    // User error - requested non-existent code
}
```

**Actions:**
- All exceptions inherit from `EasyMessagesException` base class
- Include helpful context (available codes, file paths, etc.)
- Log warnings for custom message load failures
- Fail fast for critical errors (defaults.json issues)
- 
---


**defaults.json** (Embedded Resource) - Start with 20 essential messages
```json
{
  "AUTH_001": {
    "type": "Error",
    "title": "Authentication Failed",
    "description": "Invalid username or password.",
    "httpStatusCode": 401
  },
  "AUTH_002": {
    "type": "Error",
    "title": "Unauthorized",
    "description": "You don't have permission to access this resource.",
    "httpStatusCode": 403
  },
  "AUTH_003": {
    "type": "Success",
    "title": "Login Successful",
    "description": "Welcome back!"
  },
  "CRUD_001": {
    "type": "Success",
    "title": "Created Successfully",
    "description": "{resource} has been created."
  },
  "CRUD_002": {
    "type": "Success",
    "title": "Updated Successfully",
    "description": "{resource} has been updated."
  },
  "CRUD_003": {
    "type": "Success",
    "title": "Deleted Successfully",
    "description": "{resource} has been deleted."
  },
  "CRUD_004": {
    "type": "Error",
    "title": "Not Found",
    "description": "{resource} was not found.",
    "httpStatusCode": 404
  },
  "VAL_001": {
    "type": "Error",
    "title": "Validation Failed",
    "description": "Please check your input and try again.",
    "httpStatusCode": 422
  },
  "VAL_002": {
    "type": "Error",
    "title": "Required Field",
    "description": "{field} is required.",
    "httpStatusCode": 422
  },
  "VAL_003": {
    "type": "Error",
    "title": "Invalid Format",
    "description": "{field} has an invalid format.",
    "httpStatusCode": 422
  },
  "SYS_001": {
    "type": "Critical",
    "title": "System Error",
    "description": "An unexpected error occurred. Please try again later.",
    "httpStatusCode": 500
  },
  "SYS_002": {
    "type": "Info",
    "title": "Processing",
    "description": "Your request is being processed."
  },
  "DB_001": {
    "type": "Critical",
    "title": "Database Error",
    "description": "Unable to connect to the database.",
    "httpStatusCode": 503
  },
  "FILE_001": {
    "type": "Success",
    "title": "File Uploaded",
    "description": "File uploaded successfully."
  },
  "FILE_002": {
    "type": "Error",
    "title": "Invalid File Type",
    "description": "Only {types} files are allowed.",
    "httpStatusCode": 415
  }
}
```

**Configure as Embedded Resource** in `.csproj`:
```xml
<ItemGroup>
  <EmbeddedResource Include="Messages\defaults.json" />
</ItemGroup>
```

**Testing:**
```csharp
[Fact]
public void Registry_ShouldLoadDefaultMessages()
{
    var message = MessageRegistry.Get("AUTH_001");
    Assert.Equal("AUTH_001", message.Code);
    Assert.Equal("Authentication Failed", message.Title);
}

[Fact]
public void MessageCodes_Constants_ShouldMatchDefaultsJson()
{
    // Ensure MessageCodes.cs stays in sync with defaults.json
    var constantCodes = typeof(MessageCodes)
        .GetNestedTypes()
        .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static))
        .Where(f => f.FieldType == typeof(string))
        .Select(f => f.GetValue(null) as string)
        .Where(s => s != null)
        .ToHashSet();
    
    var jsonCodes = MessageRegistry.GetAllCodes().ToHashSet();
    
    // All constants must exist in JSON
    var missing = constantCodes.Except(jsonCodes).ToList();
    Assert.Empty(missing); // Fail if MessageCodes has codes not in defaults.json
}

[Fact]
public void Registry_CustomShouldOverrideDefault()
{
    var custom = new Dictionary<string, MessageTemplate>
    {
        ["AUTH_001"] = new() { Title = "Custom Title", Description = "Custom" }
    };
    MessageRegistry.LoadCustomMessages(custom);
    
    var message = MessageRegistry.Get("AUTH_001");
    Assert.Equal("Custom Title", message.Title);
}

[Fact]
public void Registry_ShouldThrowForInvalidCode()
{
    Assert.Throws<MessageNotFoundException>(() => 
        MessageRegistry.Get("INVALID_999"));
}
```

```csharp
// Priority order:
1. Check custom messages (file-based or DI-registered)
2. Fall back to embedded defaults.json
3. Throw MessageNotFoundException if code not found

// Loading strategy:
- Embedded: Load once at startup (static Lazy<T>)
- Custom: Load on AddEasyMessages() call or first access
- Hot reload: NOT supported in v1.0 (document this limitation)
```

---

## Phase 2: Fluent API (Week 2)
**Goal:** Intuitive, chainable message builder API

### Sprint 2.1: Extension Methods (6-8 hours)

**MessageExtensions.cs**
```csharp
namespace EasyMessages.Core.Extensions;

public static class MessageExtensions
{
    public static Message WithData(this Message message, object data)
    {
        return message with { Data = data };
    }
    
    public static Message WithCorrelationId(this Message message, string correlationId)
    {
        return message with { CorrelationId = correlationId };
    }
    
    public static Message WithMetadata(this Message message, string key, object value)
    {
        var metadata = new Dictionary<string, object>(message.Metadata ?? new());
        metadata[key] = value;
        return message with { Metadata = metadata };
    }
    
    public static Message WithStatusCode(this Message message, int statusCode)
    {
        return message with { HttpStatusCode = statusCode };
    }
    
/*
    **Parameter Substitution Specification:**
// Rules:
// 1. Placeholders: {fieldName} - case-insensitive matching
// 2. Missing params: Leave placeholder unchanged (e.g., "{field} is required")
// 3. Type conversion: Always .ToString() on parameter values
// 4. No formatting in v1.0: {amount:C} NOT supported
// 5. Null values: Convert to empty string

// Examples:
var msg1 = MessageRegistry.Get("VAL_002")
    .WithParams(new { Field = "Email" });      // "Email is required"
    
var msg2 = MessageRegistry.Get("VAL_002")
    .WithParams(new { field = "Email" });      // Same result (case-insensitive)
    
var msg3 = MessageRegistry.Get("VAL_002")
    .WithParams(new { WrongParam = "X" });     // "{field} is required" (unchanged)

*/
    
    public static Message WithParams(this Message message, object parameters)
    {
        var properties = parameters.GetType().GetProperties();
        var title = message.Title;
        var description = message.Description;
        
        foreach (var prop in properties)
        {
            var value = prop.GetValue(parameters)?.ToString() ?? "";
            var placeholder = $"{{{prop.Name}}}";
            
            title = title.Replace(placeholder, value);
            description = description.Replace(placeholder, value);
        }
        
        return message with 
        { 
            Title = title, 
            Description = description 
        };
    }
}
```

```csharp
// Specification:
- Placeholders: {fieldName} (case-insensitive match)
- Missing params: Leave placeholder as-is (e.g., "Field {field} is required")
- Type conversion: Always ToString() on parameter values
- No formatting support in v1.0 (e.g., {amount:C} not supported)

// Example:
var message = MessageRegistry.Get("VAL_002")
    .WithParams(new { Field = "Email" });  // or "field" - both work
    
// Result: "Field Email is required"
```


**Testing:**
```csharp
[Fact]
public void WithData_ShouldAddData()
{
    var message = MessageRegistry.Get("AUTH_003")
        .WithData(new { UserId = 123 });
    
    Assert.NotNull(message.Data);
}

[Fact]
public void WithParams_ShouldReplaceTemplateValues()
{
    var message = MessageRegistry.Get("CRUD_001")
        .WithParams(new { resource = "User" });
    
    Assert.Equal("User has been created.", message.Description);
}
```

---

### Sprint 2.2: Facade Classes (10-12 hours)

**AuthMessages.cs**
```csharp
namespace EasyMessages.Facades;

public static class AuthMessages
{
    public static Message LoginFailed() => MessageRegistry.Get("AUTH_001");
    public static Message Unauthorized() => MessageRegistry.Get("AUTH_002");
    public static Message LoginSuccess() => MessageRegistry.Get("AUTH_003");
    public static Message Forbidden() => Unauthorized(); // Alias
}
```

**CrudMessages.cs**
```csharp
namespace EasyMessages.Facades;

public static class CrudMessages
{
    public static Message Created(string resource) =>
        MessageRegistry.Get("CRUD_001").WithParams(new { resource });
    
    public static Message Updated(string resource) =>
        MessageRegistry.Get("CRUD_002").WithParams(new { resource });
    
    public static Message Deleted(string resource) =>
        MessageRegistry.Get("CRUD_003").WithParams(new { resource });
    
    public static Message NotFound(string resource) =>
        MessageRegistry.Get("CRUD_004").WithParams(new { resource });
}
```

**ValidationMessages.cs**
```csharp
namespace EasyMessages.Facades;

public static class ValidationMessages
{
    public static Message Failed() => MessageRegistry.Get("VAL_001");
    
    public static Message RequiredField(string field) =>
        MessageRegistry.Get("VAL_002").WithParams(new { field });
    
    public static Message InvalidFormat(string field) =>
        MessageRegistry.Get("VAL_003").WithParams(new { field });
}
```

**SystemMessages.cs**
```csharp
namespace EasyMessages.Facades;

public static class SystemMessages
{
    public static Message Error() => MessageRegistry.Get("SYS_001");
    public static Message Processing() => MessageRegistry.Get("SYS_002");
}
```

**DatabaseMessages.cs**
```csharp
namespace EasyMessages.Facades;

public static class DatabaseMessages
{
    public static Message ConnectionFailed() => MessageRegistry.Get("DB_001");
}
```

**FileMessages.cs**
```csharp
namespace EasyMessages.Facades;

public static class FileMessages
{
    public static Message Uploaded() => MessageRegistry.Get("FILE_001");
    
    public static Message InvalidType(params string[] types) =>
        MessageRegistry.Get("FILE_002")
            .WithParams(new { types = string.Join(", ", types) });
}
```

**CustomMessages.cs**
```csharp
namespace EasyMessages.Facades;

public static class CustomMessages
{
    public static Message Get(string code) => MessageRegistry.Get(code);
}
```

**Main Entry Point: Msg.cs**
```csharp
namespace EasyMessages;

public static class Msg
{
    public static AuthMessages Auth => new();
    public static CrudMessages Crud => new();
    public static ValidationMessages Validation => new();
    public static SystemMessages System => new();
    public static DatabaseMessages Database => new();
    public static FileMessages File => new();
    public static CustomMessages Custom => new();
}
```

**Wait! Better approach - Static nested classes:**
```csharp
namespace EasyMessages;

public static class Msg
{
    public static class Auth
    {
        public static Message LoginFailed() => MessageRegistry.Get("AUTH_001");
        public static Message Unauthorized() => MessageRegistry.Get("AUTH_002");
        public static Message LoginSuccess() => MessageRegistry.Get("AUTH_003");
    }
    
    public static class Crud
    {
        public static Message Created(string resource) =>
            MessageRegistry.Get("CRUD_001").WithParams(new { resource });
        
        public static Message Updated(string resource) =>
            MessageRegistry.Get("CRUD_002").WithParams(new { resource });
        
        public static Message Deleted(string resource) =>
            MessageRegistry.Get("CRUD_003").WithParams(new { resource });
        
        public static Message NotFound(string resource) =>
            MessageRegistry.Get("CRUD_004").WithParams(new { resource });
    }
    
    public static class Validation
    {
        public static Message Failed() => MessageRegistry.Get("VAL_001");
        
        public static Message RequiredField(string field) =>
            MessageRegistry.Get("VAL_002").WithParams(new { field });
        
        public static Message InvalidFormat(string field) =>
            MessageRegistry.Get("VAL_003").WithParams(new { field });
    }
    
    public static class System
    {
        public static Message Error() => MessageRegistry.Get("SYS_001");
        public static Message Processing() => MessageRegistry.Get("SYS_002");
    }
    
    public static class Database
    {
        public static Message ConnectionFailed() => MessageRegistry.Get("DB_001");
    }
    
    public static class File
    {
        public static Message Uploaded() => MessageRegistry.Get("FILE_001");
        
        public static Message InvalidType(params string[] types) =>
            MessageRegistry.Get("FILE_002")
                .WithParams(new { types = string.Join(", ", types) });
    }
    
    // Custom messages
    public static Message Custom(string code) => MessageRegistry.Get(code);
}
```

**Testing:**
```csharp
[Fact]
public void Msg_Auth_LoginFailed_ShouldWork()
{
    var message = Msg.Auth.LoginFailed();
    Assert.Equal("AUTH_001", message.Code);
}

[Fact]
public void Msg_Crud_Created_ShouldReplaceResource()
{
    var message = Msg.Crud.Created("User");
    Assert.Contains("User", message.Description);
}

[Fact]
public void Msg_Custom_ShouldAccessAnyCode()
{
    var message = Msg.Custom("AUTH_001");
    Assert.Equal("AUTH_001", message.Code);
}
```

---

## Phase 3: Output Formatters (Week 3)
**Goal:** Convert messages to various output formats

### Sprint 3.1: Core Formatter Interface (4 hours)

**IMessageFormatter.cs**
```csharp
namespace EasyMessages.Formatters;

public interface IMessageFormatter
{
    string Format(Message message);
    object FormatAsObject(Message message);
}
```

---

### Sprint 3.2: JSON Formatter (4-6 hours)

**JsonFormatter.cs**
```csharp
namespace EasyMessages.Formatters;

public class JsonFormatter : IMessageFormatter
{
    private readonly JsonSerializerOptions _options;
    
    public JsonFormatter(JsonSerializerOptions? options = null)
    {
        _options = options ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
    
    public string Format(Message message)
    {
        return JsonSerializer.Serialize(FormatAsObject(message), _options);
    }
    
    public object FormatAsObject(Message message)
    {
        return new
        {
            success = message.Type is MessageType.Success or MessageType.Info,
            code = message.Code,
            type = message.Type.ToString().ToLowerInvariant(),
            title = message.Title,
            description = message.Description,
            data = message.Data,
            timestamp = message.Timestamp,
            correlationId = message.CorrelationId,
            metadata = message.Metadata
        };
    }
}
```

**MessageExtensions.cs (add):**
```csharp
public static string ToJson(this Message message, JsonSerializerOptions? options = null)
{
    return new JsonFormatter(options).Format(message);
}

public static object ToJsonObject(this Message message)
{
    return new JsonFormatter().FormatAsObject(message);
}
```

**Testing:**
```csharp
[Fact]
public void ToJson_ShouldSerializeCorrectly()
{
    var message = Msg.Auth.LoginSuccess();
    var json = message.ToJson();
    
    Assert.Contains("\"success\":true", json);
    Assert.Contains("\"code\":\"AUTH_003\"", json);
}
```

---

### Sprint 3.3: Console Formatter (6-8 hours)

**ConsoleFormatter.cs**
```csharp
namespace EasyMessages.Formatters;

public class ConsoleFormatter : IMessageFormatter
{
    private readonly bool _useColors;
    private readonly bool _showTimestamp;
    
    public ConsoleFormatter(bool useColors = true, bool showTimestamp = true)
    {
        _useColors = useColors;
        _showTimestamp = showTimestamp;
    }
    
    public string Format(Message message)
    {
        var icon = GetIcon(message.Type);
        var timestamp = _showTimestamp ? $"[{message.Timestamp:HH:mm:ss}] " : "";
        
        return $"{icon} {message.Title}\n  {message.Description}\n  {timestamp}[{message.Code}]";
    }
    
    public object FormatAsObject(Message message)
    {
        return new
        {
            Icon = GetIcon(message.Type),
            Color = GetColor(message.Type),
            Message = Format(message)
        };
    }
    
    private static string GetIcon(MessageType type) => type switch
    {
        MessageType.Success => "✓",
        MessageType.Info => "ℹ",
        MessageType.Warning => "⚠",
        MessageType.Error => "✗",
        MessageType.Critical => "☠",
        _ => "•"
    };
    
    private static ConsoleColor GetColor(MessageType type) => type switch
    {
        MessageType.Success => ConsoleColor.Green,
        MessageType.Info => ConsoleColor.Cyan,
        MessageType.Warning => ConsoleColor.Yellow,
        MessageType.Error => ConsoleColor.Red,
        MessageType.Critical => ConsoleColor.DarkRed,
        _ => ConsoleColor.White
    };
    
    public void WriteToConsole(Message message)
    {
        if (!_useColors)
        {
            Console.WriteLine(Format(message));
            return;
        }
        
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = GetColor(message.Type);
        Console.WriteLine(Format(message));
        Console.ForegroundColor = originalColor;
    }
}
```

**MessageExtensions.cs (add):**
```csharp
public static void ToConsole(this Message message, bool useColors = true)
{
    new ConsoleFormatter(useColors).WriteToConsole(message);
}
```

---

### Sprint 3.4: ASP.NET Core Integration (8-10 hours)

**New project: EasyMessages.AspNetCore**

**ApiResponse.cs**
```csharp
namespace EasyMessages.AspNetCore;

public class ApiResponse
{
    public bool Success { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public object? Data { get; set; }
    public DateTime Timestamp { get; set; }
    public string? CorrelationId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
```

**MessageResultExtensions.cs**
```csharp
namespace EasyMessages.AspNetCore;

public static class MessageResultExtensions
{
    public static IActionResult ToApiResponse(this Message message)
    {
        var response = new ApiResponse
        {
            Success = message.Type is MessageType.Success or MessageType.Info,
            Code = message.Code,
            Type = message.Type.ToString().ToLowerInvariant(),
            Title = message.Title,
            Description = message.Description,
            Data = message.Data,
            Timestamp = message.Timestamp,
            CorrelationId = message.CorrelationId,
            Metadata = message.Metadata
        };
        
        return new ObjectResult(response)
        {
            StatusCode = message.HttpStatusCode
        };
    }
    
    public static IResult ToMinimalApiResult(this Message message)
    {
        var response = new ApiResponse
        {
            Success = message.Type is MessageType.Success or MessageType.Info,
            Code = message.Code,
            Type = message.Type.ToString().ToLowerInvariant(),
            Title = message.Title,
            Description = message.Description,
            Data = message.Data,
            Timestamp = message.Timestamp,
            CorrelationId = message.CorrelationId,
            Metadata = message.Metadata
        };
        
        return Results.Json(response, statusCode: message.HttpStatusCode);
    }
}
```

**Testing (Integration Test):**
```csharp
[Fact]
public async Task ToApiResponse_ShouldReturn401ForUnauthorized()
{
    var message = Msg.Auth.Unauthorized();
    var result = message.ToApiResponse() as ObjectResult;
    
    Assert.NotNull(result);
    Assert.Equal(403, result.StatusCode);
    
    var response = result.Value as ApiResponse;
    Assert.False(response.Success);
}
```

---

## Phase 4: Advanced Features (Week 4)
**Goal:** Logging, configuration, extensibility

### Sprint 4.1: Logging Integration (6-8 hours)

**Strategy: Hybrid Static + DI Approach**
```csharp
// Static facade for convenience (uses service locator)
Msg.Auth.LoginFailed()
    .Log()  // Uses globally configured logger
    .ToApiResponse();

// DI for testability (explicit logger)
Msg.Auth.LoginFailed()
    .Log(_logger)  // Uses injected logger
    .ToApiResponse();
```

**Implementation:**

**MessageLogging.cs** (Internal service locator)
```csharp
namespace EasyMessages.Core;

internal static class MessageLogging
{
    internal static ILogger? GlobalLogger { get; set; }
    
    // Called by DI setup
    internal static void ConfigureGlobalLogger(ILogger logger)
    {
        GlobalLogger = logger;
    }
}
```

**ServiceCollectionExtensions.cs** (Update to register logger)
```csharp
public static IServiceCollection AddEasyMessages(
    this IServiceCollection services,
    Action<MessageConfiguration>? configure = null)
{
    var config = new MessageConfiguration();
    configure?.Invoke(config);
    
    // Register logger for static access
    services.AddSingleton<IMessageLogger>(sp =>
    {
        var logger = sp.GetRequiredService<ILogger<MessageRegistry>>();
        MessageLogging.ConfigureGlobalLogger(logger);
        return new MessageLogger(logger);
    });
    
    // ... rest of setup
    
    return services;
}
```

**LoggingExtensions.cs**
```csharp
namespace EasyMessages.Core.Extensions;

public static class LoggingExtensions
{
    public static Message Log(this Message message, ILogger? logger = null)
    {
        logger ??= GetDefaultLogger();
        
        var logLevel = MapToLogLevel(message.Type);
        
        logger.Log(logLevel,
            "[{Code}] {Title}: {Description}",
            message.Code,
            message.Title,
            message.Description);
        
        return message;
    }
    
    private static LogLevel MapToLogLevel(MessageType type) => type switch
    {
        MessageType.Success => LogLevel.Information,
        MessageType.Info => LogLevel.Information,
        MessageType.Warning => LogLevel.Warning,
        MessageType.Error => LogLevel.Error,
        MessageType.Critical => LogLevel.Critical,
        _ => LogLevel.Information
    };
    
    private static ILogger GetDefaultLogger()
    {
        // Return null logger if no DI setup
        return NullLogger.Instance;
    }
}
```

---

### Sprint 4.2: Configuration System (8-10 hours)

**MessageConfiguration.cs**
```csharp
namespace EasyMessages.Configuration;

public class MessageConfiguration
{
    public string? CustomMessagesPath { get; set; }
    public string DefaultLocale { get; set; } = "en-US";
    public bool IncludeStackTrace { get; set; } = false;
    public bool IncludeTimestamp { get; set; } = true;
    public bool IncludeCorrelationId { get; set; } = true;
    public bool AutoLog { get; set; } = false;
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Warning;
}
```

**ServiceCollectionExtensions.cs**
```csharp
namespace EasyMessages.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEasyMessages(
        this IServiceCollection services,
        Action<MessageConfiguration>? configure = null)
    {
        var config = new MessageConfiguration();
        configure?.Invoke(config);
        
        // Load custom messages if path provided
        if (!string.IsNullOrEmpty(config.CustomMessagesPath))
        {
            MessageRegistry.LoadCustomMessages(config.CustomMessagesPath);
        }
        
        services.AddSingleton(config);
        
        return services;
    }
}
```

---

### Sprint 4.3: Expand Message Library (10-12 hours)

**Add 100+ more messages to defaults.json**

Categories to add:
- Network/API errors (NET_001 - NET_020)
- Payment processing (PAY_001 - PAY_020)
- Email operations (EMAIL_001 - EMAIL_010)
- Cache operations (CACHE_001 - CACHE_010)
- Queue operations (QUEUE_001 - QUEUE_010)
- Search operations (SEARCH_001 - SEARCH_010)
- Export operations (EXPORT_001 - EXPORT_010)
- Import operations (IMPORT_001 - IMPORT_010)

**Update Msg.cs with new facades**

---

## Phase 5: Documentation & Samples (Week 5)
**Goal:** Make the library easy to adopt

### Sprint 5.1: Sample Projects (12-16 hours)

**WebApiSample** - Complete REST API example
- Login endpoint
- CRUD operations
- Error handling
- Custom messages

**ConsoleSample** - CLI tool example
- Colored output
- Progress messages
- Error handling

**BlazorSample** - UI notifications
- Toast messages
- Validation feedback

---

### Sprint 5.2: Documentation (8-10 hours)

- [ ] Comprehensive README.md
- [ ] API documentation (XML comments)
- [ ] Wiki pages
  - Getting Started
  - Configuration
  - Custom Messages
  - Extending the Library
  - Best Practices
- [ ] Migration guide (future)
- [ ] Changelog

---

## Phase 6: Testing & Polish (Week 6)
**Goal:** Production-ready quality

### Sprint 6.1: Comprehensive Testing (12-16 hours)

- [ ] Unit tests (80%+ coverage)
- [ ] Integration tests
- [ ] Performance benchmarks
- [ ] Load testing

**Test categories:**
- Core functionality
- Edge cases
- Custom message override
- Concurrent access
- Memory leaks
- Performance (< 1ms message retrieval)
- chema validation
```csharp
  [Fact]
  public void DefaultsJson_ShouldMatchSchema()
  {
      // Validate structure of defaults.json
      var schema = JSchema.Parse(File.ReadAllText("message-schema.json"));
      var json = JObject.Parse(LoadEmbeddedDefaults());
      
      Assert.True(json.IsValid(schema, out IList errors));
  }
  
  [Fact]
  public void DefaultsJson_AllCodesShouldHaveRequiredFields()
  {
      var messages = LoadDefaultMessages();
      
      foreach (var (code, template) in messages)
      {
          Assert.NotNull(template.Type);
          Assert.NotEmpty(template.Title);
          Assert.NotEmpty(template.Description);
          // httpStatusCode is optional (has default)
      }
  }
```

---

### Sprint 6.2: NuGet Package (4-6 hours)

- [ ] Package metadata
- [ ] Icon and readme
- [ ] Version 0.1.0-beta
- [ ] Publish to NuGet.org

---

### Sprint 6.3: Polish & Launch (4-6 hours)

- [ ] Code cleanup
- [ ] Performance optimization
- [ ] Final documentation review
- [ ] Create demo video (5 minutes)
- [ ] Blog post
- [ ] Social media announcement

---

## Success Criteria

### Technical
- ✅ Zero configuration works
- ✅ < 100 KB package size
- ✅ < 1ms message retrieval
- ✅ 200+ built-in messages
- ✅ 80%+ test coverage
- ✅ .NET 6, 7, 8+ support
- ✅ Thread-safe operations
- ✅ No external dependencies (Core package)

### User Experience
- ✅ Works in < 5 minutes (first use)
- ✅ IntelliSense shows all options
- ✅ Clear error messages
- ✅ Comprehensive examples

### Portfolio Impact
- ✅ Demonstrates architecture skills
- ✅ Shows API design expertise
- ✅ Real-world problem solving
- ✅ Production-ready code quality

---

## Week-by-Week Breakdown

### Week 1: Foundation ✅
**Deliverable:** Core library with basic message retrieval

**What works:**
```csharp
var message = MessageRegistry.Get("AUTH_001");
Console.WriteLine(message.Title); // "Authentication Failed"
```

---

### Week 2: Fluent API ✅
**Deliverable:** Intuitive developer experience

**What works:**
```csharp
Msg.Auth.LoginFailed()
    .WithData(user)
    .ToConsole();
```

---

### Week 3: Output Formats ✅
**Deliverable:** Multi-format output support

**What works:**
```csharp
// API
return Msg.Crud.Created("User").ToApiResponse();

// Console
Msg.System.Error().ToConsole();

// JSON
var json = Msg.Auth.LoginSuccess().ToJson();
```

---

### Week 4: Advanced Features ✅
**Deliverable:** Logging, configuration, 200+ messages

**What works:**
```csharp
// Logging
Msg.Database.ConnectionFailed()
    .Log(_logger)
    .ToApiResponse();

// Configuration
builder.Services.AddEasyMessages(options =>
{
    options.CustomMessagesPath = "messages/custom.json";
});
```

---

### Week 5: Documentation ✅
**Deliverable:** Easy onboarding for new users

**What's ready:**
- Sample projects
- Wiki documentation
- Video tutorial
- Getting started guide

---

### Week 6: Launch ✅
**Deliverable:** Published NuGet package

**What's live:**
- NuGet.org package
- GitHub repository
- Documentation site
- Demo video

---

## Daily Development Guide

### Day 1-2: Project Setup
**Time:** 8-10 hours

1. Create GitHub repo
2. Set up solution structure
3. Configure projects
4. Set up CI/CD
5. Write initial README

**Validation:**
- Solution builds
- GitHub Actions runs
- Can create NuGet package

---

### Day 3-4: Core Models
**Time:** 10-12 hours

1. Create Message class
2. Create MessageType enum
3. Create MessageTemplate class
4. Write unit tests
5. Create defaults.json (20 messages)

**Validation:**
- All tests pass
- Message class is immutable
- Timestamp auto-generates

---

### Day 5-7: Message Registry
**Time:** 12-16 hours

1. Implement MessageRegistry
2. Load embedded resources
3. Support custom message loading
4. Thread-safe implementation
5. Comprehensive error handling
6. Unit tests

**Validation:**
- Can load defaults.json
- Can override with custom.json
- Thread-safe under load
- Clear error messages

---

### Day 8-10: Fluent API
**Time:** 14-18 hours

1. Create extension methods
2. Build Msg facade
3. Create category facades (Auth, Crud, etc.)
4. Implement parameter substitution
5. Unit tests
6. Integration tests

**Validation:**
- `Msg.Auth.LoginFailed()` works
- Parameter substitution works
- IntelliSense shows all options
- Fluent chaining works

---

### Day 11-13: Formatters
**Time:** 12-16 hours

1. Create IMessageFormatter interface
2. Implement JsonFormatter
3. Implement ConsoleFormatter
4. Create extension methods
5. Unit tests
6. Visual validation (console colors)

**Validation:**
- JSON output is valid
- Console shows colors
- Formatters are extensible

---

### Day 14-16: ASP.NET Core Integration
**Time:** 12-16 hours

1. Create EasyMessages.AspNetCore project
2. Implement ToApiResponse()
3. Implement ToMinimalApiResult()
4. Create ServiceCollectionExtensions
5. Integration tests
6. Sample Web API project

**Validation:**
- Works in MVC controllers
- Works in Minimal APIs
- Correct HTTP status codes
- DI integration works

---

### Day 17-19: Logging & Configuration
**Time:** 12-16 hours

1. Implement logging extensions
2. Create MessageConfiguration
3. Build configuration system
4. Auto-logging feature
5. Unit tests

**Validation:**
- Logs to ILogger correctly
- Configuration is respected
- Auto-logging works

---

### Day 20-23: Expand Message Library
**Time:** 16-20 hours

1. Plan message categories
2. Write 100+ new messages
3. Create new facades
4. Update Msg.cs
5. Test all messages
6. Document message codes

**Categories to add:**
```
NET_001 - NET_020    (Network/API)
PAY_001 - PAY_020    (Payments)
EMAIL_001 - EMAIL_010 (Email)
CACHE_001 - CACHE_010 (Caching)
QUEUE_001 - QUEUE_010 (Queues)
SEARCH_001 - SEARCH_010 (Search)
EXPORT_001 - EXPORT_010 (Export)
IMPORT_001 - IMPORT_010 (Import)
NOTIF_001 - NOTIF_010 (Notifications)
RATE_001 - RATE_010  (Rate limiting)
```

**Validation:**
- All 200+ messages work
- Categories are logical
- Messages are well-written

---

### Day 24-27: Sample Projects
**Time:** 16-20 hours

**WebApiSample:**
- Login endpoint with JWT
- CRUD operations (Users)
- Global error handling
- Custom messages
- Swagger documentation

**ConsoleSample:**
- File processor with progress
- Colored output
- Error scenarios
- Command-line arguments

**BlazorSample:**
- Toast notifications
- Form validation
- API integration
- Real-time updates

**Validation:**
- All samples run
- Clear demonstration of features
- Easy to understand

---

### Day 28-30: Documentation
**Time:** 12-16 hours

**README.md:**
- Quick start (< 5 min)
- Installation
- Basic usage
- Advanced features
- Links to samples

**Wiki Pages:**
1. Getting Started
2. Message Categories
3. Custom Messages
4. Configuration Options
5. Extending the Library
6. Best Practices
7. Troubleshooting
8. API Reference

**Code Documentation:**
- XML comments on all public APIs
- Code examples in comments
- Clear parameter descriptions

**Validation:**
- Can follow README and get started
- Wiki covers all features
- IntelliSense shows helpful info

---

### Day 31-33: Testing
**Time:** 16-20 hours

**Unit Tests:**
- Core functionality
- Edge cases
- Error handling
- Thread safety
- Parameter substitution
- Custom message override

**Integration Tests:**
- ASP.NET Core integration
- Logging integration
- Configuration loading
- Sample projects

**Performance Tests:**
- Message retrieval speed
- Memory usage
- Concurrent access
- Large message files

**Target Metrics:**
- 80%+ code coverage
- < 1ms message retrieval
- No memory leaks
- Thread-safe under load

**Validation:**
- All tests pass
- Coverage target met
- Performance benchmarks met

---

### Day 34-35: NuGet Package
**Time:** 8-10 hours

**Package Preparation:**
1. Icon design
2. Package metadata
3. README for NuGet
4. License file
5. Release notes
6. Version 0.1.0-beta

**Package Contents:**
- EasyMessages.Core
- EasyMessages.AspNetCore
- EasyMessages.Console (optional)

**Validation:**
- Package builds correctly
- Dependencies are correct
- Icon displays properly
- README is clear

---

### Day 36-38: Polish & Launch
**Time:** 12-16 hours

**Code Polish:**
- Code cleanup
- Remove TODOs
- Optimize performance
- Final refactoring
- Code review

**Documentation Polish:**
- Proofread all docs
- Fix typos
- Add missing examples
- Update screenshots

**Launch Materials:**
- Demo video (5 min)
- Blog post
- Twitter/LinkedIn post
- Dev.to article
- Reddit r/csharp post

**Validation:**
- Package published to NuGet
- GitHub repo is public
- Documentation is live
- Video is published

---

## Post-Launch Roadmap (Future)

### Version 0.2.0 (2-3 weeks later)
- Localization support (i18n)
- Message templates with complex formatting
- Async logging
- Performance optimizations

### Version 0.3.0 (1-2 months later)
- Telemetry integration
- Additional formatters (XML, CSV)
- Message versioning
- Breaking change detection

### Version 1.0.0 (3-4 months later)
- Production-ready
- Battle-tested
- Community feedback incorporated
- Full documentation
- Video tutorial series

---

## Risk Mitigation

### Technical Risks

**Risk:** Embedded resource not loading
**Mitigation:** 
- Test on multiple platforms
- Fallback to file system
- Clear error messages

**Risk:** Performance bottlenecks
**Mitigation:**
- Benchmark early
- Cache aggressively
- Profile memory usage

**Risk:** Breaking changes needed
**Mitigation:**
- Use semantic versioning
- Deprecate, don't remove
- Provide migration guides

### Adoption Risks

**Risk:** Low visibility
**Mitigation:**
- Post on multiple platforms
- Create useful content
- Solve real problems

**Risk:** Competition from established libraries
**Mitigation:**
- Emphasize simplicity
- Show clear advantages
- Provide easy migration

**Risk:** Maintenance burden
**Mitigation:**
- Clean architecture
- Good documentation
- Community contributions

---

## Key Development Principles

### 1. Test-Driven Development
Write tests first for critical paths:
```csharp
[Fact]
public void Should_ReturnMessage_When_CodeExists()
{
    // Arrange
    var code = "AUTH_001";
    
    // Act
    var message = MessageRegistry.Get(code);
    
    // Assert
    Assert.Equal(code, message.Code);
    Assert.NotNull(message.Title);
}
```

### 2. Keep It Simple
Resist feature creep:
- ❌ No: Complex templating engine
- ✅ Yes: Simple {placeholder} replacement

### 3. Performance First
- Cache everything possible
- Lazy load when appropriate
- Profile regularly

### 4. Developer Experience
- IntelliSense-friendly APIs
- Clear error messages
- Helpful documentation
- Working examples

### 5. Backward Compatibility
- Never remove message codes
- Deprecate, don't delete
- Version carefully

---

## Tools & Technologies

### Development
- Visual Studio 2022 or VS Code
- .NET 8 SDK
- Git
- GitHub Actions

### Testing
- xUnit
- FluentAssertions
- Moq (for integration tests)
- BenchmarkDotNet (performance)

### Documentation
- DocFX (API docs)
- Markdown (Wiki)
- OBS Studio (video recording)
- Canva (graphics)

### Deployment
- NuGet.org
- GitHub Releases
- GitHub Pages (docs)

---

## Metrics to Track

### Development Metrics
- Lines of code
- Test coverage
- Build time
- Package size

### Performance Metrics
- Message retrieval time
- Memory usage
- Concurrent request handling
- Startup time

### Adoption Metrics
- NuGet downloads
- GitHub stars
- Issues opened
- Community contributions

---

## Success Milestones

### Week 1: ✅ Foundation Complete
- [ ] Solution builds
- [ ] Core models implemented
- [ ] 20+ messages loaded
- [ ] Basic tests pass

### Week 2: ✅ API Complete
- [ ] Fluent API works
- [ ] All facades implemented
- [ ] Parameter substitution works
- [ ] 80+ tests pass

### Week 3: ✅ Outputs Complete
- [ ] JSON formatter works
- [ ] Console formatter works
- [ ] ASP.NET Core integration done
- [ ] Sample API works

### Week 4: ✅ Features Complete
- [ ] Logging integration works
- [ ] Configuration system done
- [ ] 200+ messages available
- [ ] All formatters done

### Week 5: ✅ Documentation Complete
- [ ] README is clear
- [ ] Wiki is comprehensive
- [ ] Samples are polished
- [ ] Video is recorded

### Week 6: ✅ Launch Complete
- [ ] Package on NuGet
- [ ] GitHub repo public
- [ ] Blog post published
- [ ] Community notified

---

## Quick Reference: Core APIs

### Getting Messages
```csharp
// Built-in categories
Msg.Auth.LoginFailed()
Msg.Crud.Created("User")
Msg.Validation.RequiredField("Email")
Msg.System.Error()
Msg.File.Uploaded()

// Custom messages
Msg.Custom("MYAPP_001")
```

### Enriching Messages
```csharp
message
    .WithData(user)
    .WithCorrelationId(correlationId)
    .WithMetadata("key", value)
    .WithParams(new { field = "Email" })
    .WithStatusCode(403)
```

### Output Formats
```csharp
// API
message.ToApiResponse()
message.ToMinimalApiResult()

// Console
message.ToConsole()

// JSON
message.ToJson()
message.ToJsonObject()

// Logging
message.Log(logger)
```

### Configuration
```csharp
builder.Services.AddEasyMessages(options =>
{
    options.CustomMessagesPath = "messages/custom.json";
    options.DefaultLocale = "en-US";
    options.AutoLog = true;
});
```

---
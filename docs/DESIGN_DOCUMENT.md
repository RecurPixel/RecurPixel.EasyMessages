# C#/.Net Unified Messaging Package - Design Document (Initial Draft)

## Package Name Ideas
- `EasyMessages`
- `FluentMessages`
- `StandardMessages`
- `UnifiedMessaging`
- `MessageKit`

**Final Consideration:** `EasyMessages` (simple, memorable, SEO-friendly)

---

## 1. Core Architecture

### Message Structure
```csharp
public class Message
{
    public string Code { get; set; }           // e.g., "USER_001"
    public MessageType Type { get; set; }      // Success, Info, Warning, Error, Critical
    public string Title { get; set; }          // Short summary
    public string Description { get; set; }    // Detailed message
    public object Data { get; set; }           // Optional payload
    public DateTime Timestamp { get; set; }    // Auto-generated
    public string CorrelationId { get; set; }  // For tracing
    public Dictionary<string, object> Metadata { get; set; } // Extensible
}

public enum MessageType
{
    Success,
    Info,
    Warning,
    Error,
    Critical
}
```

### Output Formats
- **API Response** (JSON with proper HTTP status)
- **Console** (Colored text with icons)
- **Log** (Structured logging format)
- **Exception** (Throwable with context)
- **UI Notification** (Toast/alert format)

---

## 2. File Structure

```
EasyMessages/
├── Messages/
│   ├── defaults.json          # Built-in messages (embedded resource)
│   └── custom.json            # User overrides (optional)
├── Core/
│   ├── Message.cs
│   ├── MessageType.cs
│   └── MessageCodes.cs        # Constants (e.g., AUTH_001)
├── Facades/
│   ├── Msg.cs                 # Main entry point
│   ├── ApiMsg.cs              # API-specific helpers
│   ├── ConsoleMsg.cs          # Console-specific helpers
│   └── LogMsg.cs              # Logging helpers
├── Formatters/
│   ├── IMessageFormatter.cs
│   ├── JsonFormatter.cs
│   ├── ConsoleFormatter.cs
│   └── LogFormatter.cs
├── Configuration/
│   └── MessageConfig.cs
└── Extensions/
    ├── ServiceCollectionExtensions.cs // shift to ASP.NET part of DI
    └── MessageExtensions.cs
```

---

## 3. Default Message Categories (defaults.json)

### Authentication & Authorization
```json
{
  "AUTH_001": {
    "type": "Error",
    "title": "Authentication Failed",
    "description": "Invalid username or password."
  },
  "AUTH_002": {
    "type": "Error",
    "title": "Unauthorized Access",
    "description": "You don't have permission to access this resource."
  },
  "AUTH_003": {
    "type": "Success",
    "title": "Login Successful",
    "description": "Welcome back!"
  }
}
```

### CRUD Operations
```json
{
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
    "description": "{resource} was not found."
  }
}
```

### Validation
```json
{
  "VAL_001": {
    "type": "Error",
    "title": "Validation Failed",
    "description": "Please check your input and try again."
  },
  "VAL_002": {
    "type": "Error",
    "title": "Required Field Missing",
    "description": "{field} is required."
  },
  "VAL_003": {
    "type": "Error",
    "title": "Invalid Format",
    "description": "{field} has an invalid format."
  }
}
```

### System
```json
{
  "SYS_001": {
    "type": "Critical",
    "title": "System Error",
    "description": "An unexpected error occurred. Please try again later."
  },
  "SYS_002": {
    "type": "Info",
    "title": "Processing",
    "description": "Your request is being processed."
  },
  "SYS_003": {
    "type": "Warning",
    "title": "Service Degraded",
    "description": "Some features may be temporarily unavailable."
  }
}
```

### Database
```json
{
  "DB_001": {
    "type": "Critical",
    "title": "Database Connection Failed",
    "description": "Unable to connect to the database."
  },
  "DB_002": {
    "type": "Error",
    "title": "Duplicate Entry",
    "description": "{resource} already exists."
  }
}
```

### File Operations
```json
{
  "FILE_001": {
    "type": "Success",
    "title": "File Uploaded",
    "description": "File uploaded successfully."
  },
  "FILE_002": {
    "type": "Error",
    "title": "Invalid File Type",
    "description": "Only {types} files are allowed."
  },
  "FILE_003": {
    "type": "Error",
    "title": "File Too Large",
    "description": "Maximum file size is {size}."
  }
}
```

---

## 4. API Design (Usage Examples)

### Basic Usage - Plug & Play
```csharp
// No setup required - just use it!
using EasyMessages;

// In your controller
public IActionResult Login(LoginDto dto)
{
    var user = _authService.Authenticate(dto);
    
    if (user == null)
        return Msg.Auth.LoginFailed().ToApiResponse(); // 401 Unauthorized
    
    return Msg.Auth.LoginSuccess()
        .WithData(user)
        .ToApiResponse(); // 200 OK with user data
}
```

### Fluent Chaining
```csharp
// Create user
return Msg.Crud.Created("User")
    .WithData(newUser)
    .Log()                    // Automatically log
    .ToApiResponse();         // Return as API response
```

### Console Messages
```csharp
// Colored console output with icons
Msg.Console.Success("Application started successfully");
// Output: ✓ Application started successfully (in green)

Msg.Console.Error("Failed to connect to database");
// Output: ✗ Failed to connect to database (in red)

Msg.Console.Warning("Configuration file not found, using defaults");
// Output: ⚠ Configuration file not found, using defaults (in yellow)
```

### Exception Handling
```csharp
try
{
    // Some operation
}
catch (Exception ex)
{
    // Convert to friendly message
    var message = Msg.System.UnexpectedError()
        .WithException(ex)
        .Log();
    
    return message.ToApiResponse(); // 500 with safe error message
}
```

### Custom Messages
```csharp
// Use custom code from your custom.json
return Msg.Custom("MYAPP_001")
    .WithParams(new { username = "John" })
    .ToApiResponse();

// Or create inline
return Msg.Create(MessageType.Success)
    .WithTitle("Custom Success")
    .WithDescription("This is a custom message")
    .ToApiResponse();
```

---

## 5. Configuration & Customization

### Startup Configuration (Optional)
```csharp
// Program.cs or Startup.cs
builder.Services.AddEasyMessages(options =>
{
    options.CustomMessagesPath = "messages/custom.json";
    options.DefaultLocale = "en-US";
    options.IncludeStackTrace = builder.Environment.IsDevelopment();
    options.AutoLog = true; // Auto-log all error messages
});
```

### Custom Messages File (custom.json)
```json
{
  "MYAPP_001": {
    "type": "Success",
    "title": "Payment Processed",
    "description": "Your payment of {amount} has been processed."
  },
  "MYAPP_002": {
    "type": "Error",
    "title": "Insufficient Funds",
    "description": "Account balance is too low."
  }
}
```

### Localization Support
```
messages/
├── custom.en-US.json
├── custom.es-ES.json
├── custom.fr-FR.json
└── custom.de-DE.json
```

```csharp
// Automatic locale detection from request
return Msg.Auth.LoginSuccess()
    .ToApiResponse(); // Uses Accept-Language header

// Or explicit
return Msg.Auth.LoginSuccess()
    .WithLocale("es-ES")
    .ToApiResponse();
```

---

## 6. Pre-built Facades

### Msg.Auth
- `LoginSuccess()`, `LoginFailed()`
- `Unauthorized()`, `Forbidden()`
- `TokenExpired()`, `InvalidToken()`

### Msg.Crud
- `Created(string resource)`, `Updated(string resource)`
- `Deleted(string resource)`, `NotFound(string resource)`
- `ListRetrieved(string resource)`

### Msg.Validation
- `ValidationFailed()`, `RequiredField(string field)`
- `InvalidFormat(string field)`, `InvalidRange(string field)`

### Msg.System
- `UnexpectedError()`, `ServiceUnavailable()`
- `Processing()`, `Completed()`

### Msg.Database
- `ConnectionFailed()`, `DuplicateEntry(string resource)`
- `QueryFailed()`

### Msg.File
- `UploadSuccess()`, `InvalidFileType(params string[] types)`
- `FileTooLarge(string maxSize)`

### Msg.Console
- `Success(string message)`, `Error(string message)`
- `Warning(string message)`, `Info(string message)`

---

## 7. Output Formats

### API Response (ASP.NET Core)
```json
{
  "success": true,
  "code": "AUTH_003",
  "title": "Login Successful",
  "description": "Welcome back!",
  "data": {
    "userId": 123,
    "username": "john_doe"
  },
  "timestamp": "2025-10-21T10:30:00Z",
  "correlationId": "abc-123-def"
}
```

### Console Output
```
✓ Login Successful
  Welcome back!
  [AUTH_003] 10:30:00
```

### Log Output (Serilog/NLog compatible)
```json
{
  "Level": "Information",
  "MessageCode": "AUTH_003",
  "Title": "Login Successful",
  "Description": "Welcome back!",
  "Timestamp": "2025-10-21T10:30:00Z",
  "CorrelationId": "abc-123-def"
}
```

---

## 8. HTTP Status Code Mapping

### Automatic Mapping
```csharp
MessageType.Success   → 200 OK
MessageType.Info      → 200 OK
MessageType.Warning   → 200 OK (with warning flag)
MessageType.Error     → 400 Bad Request
MessageType.Critical  → 500 Internal Server Error
```

### Special Cases
```csharp
Msg.Auth.Unauthorized()  → 401 Unauthorized
Msg.Auth.Forbidden()     → 403 Forbidden
Msg.Crud.NotFound()      → 404 Not Found
Msg.Validation.*         → 422 Unprocessable Entity
```

---

## 9. Extension Methods

```csharp
public static class MessageExtensions
{
    public static Message WithData(this Message msg, object data);
    public static Message WithParams(this Message msg, object parameters);
    public static Message WithException(this Message msg, Exception ex);
    public static Message WithCorrelationId(this Message msg, string id);
    public static Message Log(this Message msg);
    public static IActionResult ToApiResponse(this Message msg);
    public static string ToJson(this Message msg);
    public static void ToConsole(this Message msg);
}
```

---

## 10. Advanced Features

### Conditional Logging
```csharp
return Msg.System.UnexpectedError()
    .LogIf(condition: _env.IsProduction())
    .ToApiResponse();
```

### Message Aggregation
```csharp
var messages = new MessageCollection()
    .Add(Msg.Validation.RequiredField("Email"))
    .Add(Msg.Validation.InvalidFormat("Phone"));

return messages.ToApiResponse(); // Returns all validation errors
```

### Async Logging
```csharp
await Msg.Database.QueryFailed()
    .LogAsync()
    .ToApiResponseAsync();
```

---

## 11. Testing Support

```csharp
// Unit test helper
[Fact]
public void Should_Return_LoginFailed_Message()
{
    var result = Msg.Auth.LoginFailed();
    
    Assert.Equal("AUTH_001", result.Code);
    Assert.Equal(MessageType.Error, result.Type);
}
```

---

## 12. Package Distribution

### NuGet Package Structure
```
EasyMessages/
├── EasyMessages.Core          (Main package)
├── EasyMessages.AspNetCore    (ASP.NET Core integration)
├── EasyMessages.Console       (Console helpers)
└── EasyMessages.Serilog       (Serilog integration)
```

### Installation
```bash
dotnet add package EasyMessages
```

---

## 13. Documentation Strategy

- **README.md** - Quick start guide
- **Wiki** - Detailed documentation
- **Code samples** - GitHub repo with examples
- **Video tutorial** - 5-minute walkthrough
- **Interactive demo** - Blazor WASM playground

---

## 14. Success Metrics

- Zero configuration required for basic usage
- < 100 KB package size
- < 1ms message retrieval time
- 200+ pre-built messages out of the box
- Support for .NET 6, 7, 8+

---


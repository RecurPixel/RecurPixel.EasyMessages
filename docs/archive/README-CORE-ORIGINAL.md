# RecurPixel.EasyMessages (Core)

> The foundation library for standardized messaging in .NET applications

[![NuGet](https://img.shields.io/nuget/v/RecurPixel.EasyMessages.svg)](https://www.nuget.org/packages/RecurPixel.EasyMessages/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

---

## 📦 Installation

```bash
dotnet add package RecurPixel.EasyMessages
```

---

## 🎯 What's Inside

The core library provides:

- **Message System** – Immutable, thread-safe message objects
- **Message Registry** – 200+ pre-built, categorized messages
- **Fluent API** – Chainable methods for message composition
- **Formatters** – JSON, console, and extensible formatting
- **Custom Messages** – Override defaults or add your own

---

## 🚀 Quick Examples

### Basic Usage

```csharp
using EasyMessages;

// Get a message
var message = Msg.Auth.LoginFailed();

Console.WriteLine(message.Code);        // "AUTH_001"
Console.WriteLine(message.Title);       // "Authentication Failed"
Console.WriteLine(message.Description); // "Invalid username or password."
Console.WriteLine(message.HttpStatusCode); // 401
```

### Fluent Chaining

```csharp
var enrichedMessage = Msg.Crud.Created("User")
    .WithData(new { id = 123, email = "user@example.com" })
    .WithCorrelationId(Guid.NewGuid().ToString())
    .WithMetadata("source", "API");

// Message is immutable - each With* returns a new instance
```

### Parameter Substitution

```csharp
// Messages support {placeholder} syntax
var message = Msg.Validation.RequiredField("Email");
// Result: "Email is required."

var fileMessage = Msg.File.InvalidType("PDF", "DOCX");
// Result: "Only PDF, DOCX files are allowed."
```

### Output Formats

```csharp
// JSON string
string json = message.ToJson();

// JSON object
object obj = message.ToJsonObject();

// Console with colors
message.ToConsole();
```

---

## 📂 Message Categories

### Authentication (`Msg.Auth.*`)

```csharp
Msg.Auth.LoginFailed()          // AUTH_001 - Invalid credentials
Msg.Auth.Unauthorized()         // AUTH_002 - No permission
Msg.Auth.LoginSuccess()         // AUTH_003 - Welcome back
```

### CRUD Operations (`Msg.Crud.*`)

```csharp
Msg.Crud.Created("User")        // CRUD_001 - Created successfully
Msg.Crud.Updated("User")        // CRUD_002 - Updated successfully
Msg.Crud.Deleted("User")        // CRUD_003 - Deleted successfully
Msg.Crud.NotFound("User")       // CRUD_004 - Not found (404)
```

### Validation (`Msg.Validation.*`)

```csharp
Msg.Validation.Failed()                  // VAL_001 - General validation failure
Msg.Validation.RequiredField("Email")    // VAL_002 - Field is required
Msg.Validation.InvalidFormat("Phone")    // VAL_003 - Invalid format
```

### System Messages (`Msg.System.*`)

```csharp
Msg.System.Error()              // SYS_001 - Unexpected error (500)
Msg.System.Processing()         // SYS_002 - Request processing
```

### Database (`Msg.Database.*`)

```csharp
Msg.Database.ConnectionFailed() // DB_001 - Cannot connect (503)
```

### File Operations (`Msg.File.*`)

```csharp
Msg.File.Uploaded()                    // FILE_001 - Upload successful
Msg.File.InvalidType("PDF", "DOCX")    // FILE_002 - Invalid file type
```

### Custom Messages

```csharp
Msg.Custom("MYAPP_001")         // Access any custom message by code
```

---

## 🔧 Core API

### Message Class

```csharp
public sealed record Message
{
    public string Code { get; init; }              // Message code (e.g., "AUTH_001")
    public MessageType Type { get; init; }         // Success, Info, Warning, Error, Critical
    public string Title { get; init; }             // Short title
    public string Description { get; init; }       // Detailed description
    public int HttpStatusCode { get; init; }       // HTTP status (default based on type)
    
    public object? Data { get; init; }             // Custom data payload
    public string? CorrelationId { get; init; }    // Tracing ID
    public DateTime Timestamp { get; init; }       // Creation time (UTC)
    public Dictionary<string, object>? Metadata { get; init; } // Additional context
}
```

### Extension Methods

#### Enrichment

```csharp
// Add custom data
message.WithData(userData)

// Add correlation ID for tracing
message.WithCorrelationId(traceId)

// Add metadata
message.WithMetadata("userId", 123)
message.WithMetadata("source", "mobile-app")

// Override HTTP status code
message.WithStatusCode(429) // Too Many Requests

// Substitute parameters
message.WithParams(new { field = "Email", min = 8 })
```

#### Output

```csharp
// JSON output
string json = message.ToJson();
object obj = message.ToJsonObject();

// Console output (with colors)
message.ToConsole(useColors: true);
```

---

## 🎨 Console Output

EasyMessages provides rich console output with colors and icons:

```csharp
Msg.System.Processing().ToConsole();
// Output: ℹ Processing
//         Your request is being processed.
//         [16:30:45] [SYS_002]

Msg.Validation.Failed().ToConsole();
// Output: ✗ Validation Failed
//         Please check your input and try again.
//         [16:30:46] [VAL_001]
```

**Icons by Type:**
- âœ" Success (green)
- ℹ Info (cyan)
- âš  Warning (yellow)
- âœ— Error (red)
- â˜  Critical (dark red)

---

## 🛠️ Custom Messages

### Create Custom Message File

```json
// custom-messages.json
{
  "PAYMENT_001": {
    "type": "Success",
    "title": "Payment Processed",
    "description": "Your payment of {amount} was successful.",
    "httpStatusCode": 200
  },
  "PAYMENT_002": {
    "type": "Error",
    "title": "Payment Failed",
    "description": "Payment could not be processed: {reason}",
    "httpStatusCode": 402
  },
  "MYAPP_001": {
    "type": "Info",
    "title": "Custom Business Logic",
    "description": "This is my custom message."
  }
}
```

### Load Custom Messages

```csharp
// Load from file
MessageRegistry.LoadCustomMessages("path/to/custom-messages.json");

// Or load from dictionary
var customMessages = new Dictionary<string, MessageTemplate>
{
    ["CUSTOM_001"] = new MessageTemplate
    {
        Type = MessageType.Success,
        Title = "Custom Title",
        Description = "Custom description"
    }
};
MessageRegistry.LoadCustomMessages(customMessages);
```

### Use Custom Messages

```csharp
// Access via Custom method
var message = Msg.Custom("PAYMENT_001")
    .WithParams(new { amount = "$50.00" });

Console.WriteLine(message.Description);
// Output: "Your payment of $50.00 was successful."
```

### Override Built-in Messages

```json
// custom-messages.json
{
  "AUTH_001": {
    "title": "Oops! Login Failed",
    "description": "Hmm, those credentials didn't work. Try again?"
  }
}
```

```csharp
// Now AUTH_001 uses your custom text
Msg.Auth.LoginFailed().Description
// Output: "Hmm, those credentials didn't work. Try again?"
```

---

## 📊 Message Types

```csharp
public enum MessageType
{
    Success = 0,    // Positive outcome (green) → 200 OK
    Info = 1,       // Informational (cyan) → 200 OK
    Warning = 2,    // Caution (yellow) → 200 OK
    Error = 3,      // Client error (red) → 400 Bad Request
    Critical = 4    // Server error (dark red) → 500 Internal Server Error
}
```

**Default HTTP Status Codes:**
- `Success` → 200 OK
- `Info` → 200 OK
- `Warning` → 200 OK
- `Error` → 400 Bad Request
- `Critical` → 500 Internal Server Error

Override with `.WithStatusCode(xxx)` when needed.

---

## 🔌 Formatters

### Built-in Formatters

#### JsonFormatter

```csharp
var formatter = new JsonFormatter();
string json = formatter.Format(message);

// Output:
// {
//   "success": true,
//   "code": "CRUD_001",
//   "type": "success",
//   "title": "Created Successfully",
//   "description": "User has been created.",
//   "data": { "id": 123 },
//   "timestamp": "2025-11-17T10:30:00Z",
//   "correlationId": null,
//   "metadata": {}
// }
```

#### ConsoleFormatter

```csharp
var formatter = new ConsoleFormatter(useColors: true, showTimestamp: true);
formatter.WriteToConsole(message);

// Output: ✓ Created Successfully
//         User has been created.
//         [10:30:45] [CRUD_001]
```

### Custom Formatter

```csharp
public class XmlFormatter : IMessageFormatter
{
    public string Format(Message message)
    {
        return $"<message code='{message.Code}'>" +
               $"<title>{message.Title}</title>" +
               $"<description>{message.Description}</description>" +
               $"</message>";
    }
    
    public object FormatAsObject(Message message)
    {
        // Return structured object for serialization
        return new { /* ... */ };
    }
}

// Use it
var xml = new XmlFormatter().Format(message);
```

---

## ⚙️ Configuration

### Thread Safety

- **✅ Thread-Safe:** `MessageRegistry.Get(code)` – Unlimited concurrent reads
- **✅ Thread-Safe:** All `Message` instances (immutable records)
- **✅ Thread-Safe:** Extension methods (return new instances)
- **⚠️ NOT Thread-Safe:** `MessageRegistry.LoadCustomMessages()` – Call ONCE at startup

Note about tests and real-world behavior:

- **Testing decision:** In production the recommended usage is to call `MessageRegistry.LoadCustomMessages()` once during application startup. To mirror this real-world constraint, the test assembly runs tests sequentially by default (see `tests/RecurPixel.EasyMessages.Tests/AssemblyInfo.cs`) so tests that configure the `MessageRegistry` don't race each other. If you prefer to enable parallel test execution, the registry implementation has been updated to use immutable snapshots for custom messages; you may remove the assembly-level `DisableTestParallelization` attribute and run the tests in parallel, but keep in mind tests that intentionally reload messages can still interact in ways that may not reflect typical runtime usage.

### Performance

- **Message Retrieval:** < 1ms (cached lookups)
- **Memory:** Messages are cached, minimal allocations
- **Immutability:** Zero-cost cloning via records

### Best Practices

```csharp
// ✅ DO: Load custom messages once at startup
MessageRegistry.LoadCustomMessages("custom-messages.json");

// ✅ DO: Use immutable pattern
var newMessage = message.WithData(data); // Creates new instance

// ✅ DO: Chain fluently
var result = Msg.Auth.LoginFailed()
    .WithCorrelationId(traceId)
    .ToJson();

// ❌ DON'T: Reload messages at runtime
// MessageRegistry.LoadCustomMessages(...); // NOT thread-safe

// ❌ DON'T: Try to mutate messages
// message.Data = newData; // Won't compile - records are immutable
```

---

## 🧪 Testing

### Unit Testing

```csharp
[Fact]
public void LoginFailed_ShouldHaveCorrectProperties()
{
    // Arrange & Act
    var message = Msg.Auth.LoginFailed();
    
    // Assert
    Assert.Equal("AUTH_001", message.Code);
    Assert.Equal(MessageType.Error, message.Type);
    Assert.Equal(401, message.HttpStatusCode);
    Assert.Equal("Authentication Failed", message.Title);
}

[Fact]
public void WithParams_ShouldReplaceTemplateValues()
{
    // Arrange
    var message = Msg.Validation.RequiredField("Email");
    
    // Assert
    Assert.Contains("Email is required", message.Description);
}
```

### Mock Messages

```csharp
// Create test messages easily
var testMessage = Msg.Custom("TEST_001")
    .WithData(new { test = true })
    .WithStatusCode(200);
```

---

## 📋 API Reference

### Static Entry Points

```csharp
Msg.Auth.*           // Authentication messages
Msg.Crud.*           // CRUD operation messages
Msg.Validation.*     // Validation messages
Msg.System.*         // System messages
Msg.Database.*       // Database messages
Msg.File.*           // File operation messages
Msg.Custom(code)     // Custom message access
```

### Extension Methods

```csharp
.WithData(object)                      // Add custom data
.WithCorrelationId(string)             // Add correlation ID
.WithMetadata(string, object)          // Add metadata key-value
.WithStatusCode(int)                   // Override HTTP status
.WithParams(object)                    // Substitute parameters
.ToJson(JsonSerializerOptions?)        // Format as JSON string
.ToJsonObject()                        // Format as object
.ToConsole(bool useColors)             // Write to console
```

---

## 🌍 Localization (Coming Soon)

```csharp
// Future: Localization support
Msg.Auth.LoginFailed()
    .WithLocale("es-ES")
    .ToApiResponse();
// Result: "Autenticación Fallida"
```

---

## 📦 Package Details

**Package:** `RecurPixel.EasyMessages`  
**Version:** 0.1.0-beta  
**Target Frameworks:** .NET 6.0, .NET 7.0, .NET 8.0  
**Dependencies:** None (zero dependencies!)  
**Size:** < 100 KB

---

## 🔗 Related Packages

- **RecurPixel.EasyMessages.AspNetCore** – ASP.NET Core integration

---

## 📖 Documentation

- **Main Docs:** [GitHub Wiki](https://github.com/RecurPixel/EasyMessages/wiki)
- **API Reference:** [API Docs](https://recurpixel.github.io/EasyMessages/)
- **Samples:** [GitHub Samples](https://github.com/RecurPixel/RecurPixel.EasyMessages/tree/main/samples)

---

## 🤝 Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

---

## 📄 License

MIT License - see [LICENSE](LICENSE)

---

**Built with ❤️ by RecurPixel**
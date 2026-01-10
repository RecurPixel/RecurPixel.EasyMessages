# Your First Message (5 Minutes)

Get started with EasyMessages in just 5 minutes. This tutorial covers the basics for both console and web applications.

---

## Console Application - Hello World

### Step 1: Create a Console Application

```bash
dotnet new console -n MyFirstEasyMessages
cd MyFirstEasyMessages
dotnet add package RecurPixel.EasyMessages --version 0.1.0-beta.1
```

### Step 2: Write Your First Message

Replace the contents of `Program.cs`:

```csharp
using RecurPixel.EasyMessages;

Console.WriteLine("=== EasyMessages - Your First Message ===\n");

// 1. Simple message to console
Msg.Auth.LoginFailed().ToConsole(useColors: true);

Console.WriteLine(); // Blank line

// 2. Success message
Msg.Crud.Created("User").ToConsole(useColors: true);

Console.WriteLine(); // Blank line

// 3. Message with data to JSON
var json = Msg.Crud.Created("Order")
    .WithData(new { OrderId = 12345, Amount = 99.99 })
    .ToJson();

Console.WriteLine("JSON Output:");
Console.WriteLine(json);
```

### Step 3: Run It

```bash
dotnet run
```

### Expected Output:

```
=== EasyMessages - Your First Message ===

✗ Authentication Failed
  Invalid username or password.
  [2026-01-09 14:30:00] [AUTH_001]

✓ User Created
  User has been created successfully.
  [2026-01-09 14:30:01] [CRUD_001]

JSON Output:
{"code":"CRUD_001","type":"success","title":"Created Successfully","description":"Order has been created successfully.","timestamp":"2026-01-09T14:30:01Z","data":{"orderId":12345,"amount":99.99}}
```

**Congratulations!** You've created your first EasyMessages!

---

## ASP.NET Core Web API - Hello World

### Step 1: Create an ASP.NET Core API

```bash
dotnet new webapi -n MyFirstEasyMessagesApi
cd MyFirstEasyMessagesApi
dotnet add package RecurPixel.EasyMessages.AspNetCore --version 0.1.0-beta.1
```

### Step 2: Register EasyMessages

Update `Program.cs`:

```csharp
using RecurPixel.EasyMessages.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEasyMessages(builder.Configuration); // Add this line

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### Step 3: Create Your First API Endpoint

Create `Controllers/HelloController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

namespace MyFirstEasyMessagesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    private readonly ILogger<HelloController> _logger;

    public HelloController(ILogger<HelloController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Msg.Crud.Retrieved("Welcome")
            .WithData(new
            {
                Message = "Hello from EasyMessages!",
                Timestamp = DateTime.UtcNow
            })
            .Log(_logger)
            .ToApiResponse();
    }

    [HttpGet("user/{id}")]
    public IActionResult GetUser(int id)
    {
        // Simulate user not found
        if (id <= 0)
        {
            return Msg.Crud.NotFound("User")
                .WithMetadata("userId", id)
                .ToApiResponse();
        }

        // Return success
        return Msg.Crud.Retrieved("User")
            .WithData(new
            {
                Id = id,
                Name = $"User_{id}",
                Email = $"user{id}@example.com"
            })
            .Log(_logger)
            .ToApiResponse();
    }

    [HttpPost("user")]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        return Msg.Crud.Created("User")
            .WithData(new
            {
                Id = Random.Shared.Next(1000, 9999),
                Name = request.Name,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow
            })
            .Log(_logger)
            .ToApiResponse();
    }
}

public record CreateUserRequest(string Name, string Email);
```

### Step 4: Run It

```bash
dotnet run
```

### Step 5: Test the Endpoints

**Test 1: GET /api/hello**
```bash
curl https://localhost:5001/api/hello
```

**Response:**
```json
{
  "success": true,
  "code": "CRUD_002",
  "type": "success",
  "title": "Retrieved Successfully",
  "description": "Welcome has been retrieved successfully.",
  "timestamp": "2026-01-09T14:30:00Z",
  "data": {
    "message": "Hello from EasyMessages!",
    "timestamp": "2026-01-09T14:30:00Z"
  }
}
```

**Test 2: GET /api/hello/user/0 (Not Found)**
```bash
curl https://localhost:5001/api/hello/user/0
```

**Response (404):**
```json
{
  "success": false,
  "code": "CRUD_004",
  "type": "error",
  "title": "Not Found",
  "description": "User was not found.",
  "timestamp": "2026-01-09T14:30:01Z",
  "metadata": {
    "userId": 0
  }
}
```

**Test 3: GET /api/hello/user/123 (Success)**
```bash
curl https://localhost:5001/api/hello/user/123
```

**Response (200):**
```json
{
  "success": true,
  "code": "CRUD_002",
  "type": "success",
  "title": "Retrieved Successfully",
  "description": "User has been retrieved successfully.",
  "timestamp": "2026-01-09T14:30:02Z",
  "data": {
    "id": 123,
    "name": "User_123",
    "email": "user123@example.com"
  }
}
```

**Test 4: POST /api/hello/user (Create)**
```bash
curl -X POST https://localhost:5001/api/hello/user \
  -H "Content-Type: application/json" \
  -d '{"name":"John Doe","email":"john@example.com"}'
```

**Response (200):**
```json
{
  "success": true,
  "code": "CRUD_001",
  "type": "success",
  "title": "Created Successfully",
  "description": "User has been created successfully.",
  "timestamp": "2026-01-09T14:30:03Z",
  "data": {
    "id": 5432,
    "name": "John Doe",
    "email": "john@example.com",
    "createdAt": "2026-01-09T14:30:03Z"
  }
}
```

**Congratulations!** Your first EasyMessages API is working!

---

## What You Just Learned

### Console Applications
[✓] How to create messages with `Msg.*` facades
[✓] How to output messages to console with colors
[✓] How to convert messages to JSON
[✓] How to add custom data to messages

### ASP.NET Core APIs
[✓] How to register EasyMessages with DI
[✓] How to use `.ToApiResponse()` for API responses
[✓] How to log messages with `.Log(ILogger)`
[✓] How to add metadata to messages
[✓] Automatic HTTP status code mapping

---

## Understanding the Basics

### Message Facades

EasyMessages provides pre-built message facades for common scenarios:

```csharp
Msg.Auth.*         // Authentication messages
Msg.Crud.*         // CRUD operation messages
Msg.Validation.*   // Validation messages
Msg.System.*       // System messages
Msg.Database.*     // Database messages
Msg.File.*         // File operation messages
Msg.Network.*      // Network messages
Msg.Custom(code)   // Custom messages
```

### Message Properties

Every message has these properties:

```csharp
var message = Msg.Auth.LoginFailed();

Console.WriteLine(message.Code);        // "AUTH_001"
Console.WriteLine(message.Type);        // MessageType.Error
Console.WriteLine(message.Title);       // "Authentication Failed"
Console.WriteLine(message.Description); // "Invalid username or password."
Console.WriteLine(message.HttpStatusCode); // 401
Console.WriteLine(message.Timestamp);   // DateTime.UtcNow
```

### Fluent API Methods

Chain methods to customize messages:

```csharp
var message = Msg.Crud.Created("Product")
    .WithData(new { Id = 1, Name = "Widget" })      // Add data payload
    .WithMetadata("key", "value")                    // Add metadata
    .WithCorrelationId("abc-123")                    // Add correlation ID
    .WithStatusCode(201);                            // Override HTTP status

// Console output
message.ToConsole(useColors: true);

// JSON output
var json = message.ToJson();

// XML output
var xml = message.ToXml();

// Plain text output
var text = message.ToPlainText();

// ASP.NET Core - API response
return message.ToApiResponse();

// ASP.NET Core - with logging
return message.Log(_logger).ToApiResponse();
```

---

## Common Patterns

### Pattern 1: Error Handling in Console Apps

```csharp
try
{
    // Your code here
    var result = ProcessData();

    Msg.System.Processing()
        .WithData(new { Result = result })
        .ToConsole();
}
catch (Exception ex)
{
    Msg.System.Error()
        .WithMetadata("error", ex.Message)
        .WithMetadata("stackTrace", ex.StackTrace)
        .ToConsole();
}
```

### Pattern 2: Error Handling in APIs

```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpPost]
public IActionResult Create(CreateDto dto)
{
    try
    {
        var entity = _service.Create(dto);

        return Msg.Crud.Created("Entity")
            .WithData(entity)
            .Log(_logger)
            .ToApiResponse();
    }
    catch (ValidationException ex)
    {
        return Msg.Validation.Failed()
            .WithMetadata("errors", ex.Errors)
            .Log(_logger)
            .ToApiResponse();
    }
    catch (Exception ex)
    {
        return Msg.System.Error()
            .WithMetadata("error", ex.Message)
            .Log(_logger)
            .ToApiResponse();
    }
}
```

### Pattern 3: Validation Results

```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpPost]
public IActionResult Create(CreateDto dto)
{
    if (!ModelState.IsValid)
    {
        return Msg.Validation.Failed()
            .WithData(ModelState)
            .ToApiResponse();
    }

    // Process...
}
```

### Pattern 4: Not Found Checks

```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpGet("{id}")]
public IActionResult GetById(int id)
{
    var entity = _service.GetById(id);

    if (entity == null)
    {
        return Msg.Crud.NotFound("Entity")
            .WithMetadata("id", id)
            .ToApiResponse();
    }

    return Msg.Crud.Retrieved("Entity")
        .WithData(entity)
        .ToApiResponse();
}
```

---

## Quick Reference

### Most Common Messages

```csharp
// Authentication
Msg.Auth.LoginFailed()
Msg.Auth.LoginSuccessful()
Msg.Auth.Unauthorized()
Msg.Auth.SessionExpired()

// CRUD Operations
Msg.Crud.Created("Entity")
Msg.Crud.Retrieved("Entity")
Msg.Crud.Updated("Entity")
Msg.Crud.Deleted("Entity")
Msg.Crud.NotFound("Entity")

// Validation
Msg.Validation.Failed()
Msg.Validation.RequiredField("FieldName")
Msg.Validation.InvalidFormat("FieldName")

// System
Msg.System.Error()
Msg.System.Processing()
Msg.System.OperationCompleted()

// Files
Msg.File.UploadSuccessful("document.pdf")
Msg.File.InvalidType("pdf", "docx")
Msg.File.TooLarge("10MB")
```

### Most Common Methods

```csharp
.WithData(object)              // Add data payload
.WithMetadata("key", value)    // Add metadata
.WithCorrelationId(string)     // Add correlation ID
.WithStatusCode(int)           // Override HTTP status
.ToConsole(bool useColors)     // Output to console
.ToJson()                      // Convert to JSON string
.ToXml()                       // Convert to XML string
.ToPlainText()                 // Convert to plain text
.ToApiResponse()               // Convert to IActionResult (ASP.NET Core)
.Log(ILogger)                  // Log the message (ASP.NET Core)
```

---

## Next Steps

Now that you've created your first messages, dive deeper:

### Learn the Concepts
1. **[Console vs Web Applications](Console-vs-Web-Applications.md)** - Understand the differences
2. **[Messages and Message Types](../Core-Concepts/Messages-and-Message-Types.md)** - Learn about message structure
3. **[Message Registry and Stores](../Core-Concepts/Message-Registry-and-Stores.md)** - Understand how messages are stored

### Explore Features
4. **[Formatters and Outputs](../Core-Concepts/Formatters-and-Outputs.md)** - Multiple output formats
5. **[ASP.NET Core Configuration](../ASP.NET-Core/Configuration-Guide.md)** - IOptions pattern
6. **[Custom Messages](../How-To-Guides/Create-Custom-Messages.md)** - Define your own messages

### See Examples
7. **[Console Application Example](../Examples/Console-Application.md)** - Complete console app
8. **[REST API Example](../Examples/REST-API-with-Validation.md)** - Complete API with validation
9. **[Background Job Example](../Examples/Background-Job-Processing.md)** - Data processing

---

## Troubleshooting

### Colors not showing in console?

Some terminals don't support ANSI colors. Try:
```csharp
// Disable colors
Msg.Auth.LoginFailed().ToConsole(useColors: false);
```

### Messages not appearing in logs?

Check your logging configuration in `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### JSON output has too many fields?

Configure the formatter:
```csharp
using RecurPixel.EasyMessages.Configuration;

FormatterConfiguration.Configure(options =>
{
    options.IncludeTimestamp = false;
    options.IncludeMetadata = false;
    options.IncludeNullFields = false;
});
```

---

**Questions?** Check out the [Core Concepts](../Core-Concepts/Messages-and-Message-Types.md) or [ask in Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)!

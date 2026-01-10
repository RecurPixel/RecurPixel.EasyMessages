# ASP.NET Core Integration Overview

The **RecurPixel.EasyMessages.AspNetCore** package extends the core library with ASP.NET Core-specific features, making it effortless to use standardized messages in web APIs.

---

## Why ASP.NET Core Package?

The core **RecurPixel.EasyMessages** package works perfectly for console apps, background workers, and general .NET applications. The **AspNetCore** package adds:

### Web-Specific Features

| Feature | Description | Benefit |
|---------|-------------|---------|
| **`.ToApiResponse()`** | Convert messages to IActionResult | Standard REST API responses |
| **IOptions Pattern** | Configuration via appsettings.json | Environment-specific settings |
| **Built-in Interceptors** | Auto-add correlation ID, request context | Automatic enrichment |
| **Automatic Logging** | Log all messages via ILogger | Integrated observability |
| **Dependency Injection** | Register in DI container | Follow ASP.NET Core patterns |
| **Configuration Presets** | Development, Production, Testing, API | Quick environment setup |
| **ApiResponse Model** | Standardized JSON response format | Consistent API contracts |

---

## Package Comparison

### RecurPixel.EasyMessages (Core)

**For:** Console apps, background workers, desktop apps, class libraries

```csharp
// Console output
Msg.Auth.LoginFailed().ToConsole(useColors: true);

// JSON string
var json = Msg.Crud.Created("User").ToJson();

// XML string
var xml = Msg.System.Processing().ToXml();
```

**Features:**
- [✓] Message creation (Msg.Auth, Msg.Crud, etc.)
- [✓] Formatters (JSON, XML, Console, PlainText)
- [✓] Message stores (File, Embedded, Database, etc.)
- [✓] Interceptors (custom cross-cutting concerns)
- [✓] Extension methods (WithData, WithMetadata, etc.)

---

### RecurPixel.EasyMessages.AspNetCore

**For:** ASP.NET Core Web APIs, Minimal APIs

```csharp
// Controller action
[HttpPost]
public IActionResult Create(CreateUserDto dto)
{
    return Msg.Crud.Created("User")
        .WithData(user)
        .Log(_logger)
        .ToApiResponse(); // ← Returns IActionResult with HTTP 200
}

// Minimal API
app.MapPost("/users", (CreateUserDto dto) =>
    Msg.Crud.Created("User")
        .WithData(user)
        .ToMinimalApiResult() // ← Returns IResult
);
```

**Additional Features:**
- [✓] Everything from core package
- [✓] `.ToApiResponse()` - IActionResult for controllers
- [✓] `.ToMinimalApiResult()` - IResult for Minimal APIs
- [✓] `.Log()` - Integration with ILogger
- [✓] `AddEasyMessages()` - DI registration
- [✓] IOptions pattern - appsettings.json configuration
- [✓] Built-in interceptors (CorrelationId, Metadata, Logging)
- [✓] Configuration presets (Development, Production, etc.)
- [✓] ApiResponse model - standardized JSON format

---

## Quick Start

### Installation

```bash
dotnet add package RecurPixel.EasyMessages.AspNetCore --version 0.1.0-alpha.*
```

---

### Basic Setup

**Program.cs:**
```csharp
using RecurPixel.EasyMessages.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Register EasyMessages with default settings
builder.Services.AddEasyMessages(builder.Configuration);

// OR use a preset
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Development
);

var app = builder.Build();

app.MapControllers();
app.Run();
```

---

### Controller Usage

**UsersController.cs:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _userRepository.FindById(id);

        if (user == null)
        {
            return Msg.Crud.NotFound("User")
                .WithMetadata("userId", id)
                .Log(_logger)
                .ToApiResponse(); // Returns 404 Not Found
        }

        return Msg.Crud.Retrieved("User")
            .WithData(user)
            .ToApiResponse(); // Returns 200 OK
    }

    [HttpPost]
    public IActionResult Create(CreateUserDto dto)
    {
        var user = _userRepository.Create(dto);

        return Msg.Crud.Created("User")
            .WithData(user)
            .Log(_logger)
            .ToCreated($"/api/users/{user.Id}"); // Returns 201 Created with Location header
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateUserDto dto)
    {
        var user = _userRepository.Update(id, dto);

        return Msg.Crud.Updated("User")
            .WithData(user)
            .Log(_logger)
            .ToApiResponse(); // Returns 200 OK
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _userRepository.Delete(id);

        return Msg.Crud.Deleted("User")
            .Log(_logger)
            .ToNoContent(); // Returns 204 No Content
    }
}
```

---

### Minimal API Usage

**Program.cs:**
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasyMessages(builder.Configuration);

var app = builder.Build();

var users = app.MapGroup("/api/users");

users.MapGet("/{id}", (int id, ILogger<Program> logger) =>
{
    var user = userRepository.FindById(id);

    if (user == null)
    {
        return Msg.Crud.NotFound("User")
            .WithMetadata("userId", id)
            .Log(logger)
            .ToMinimalApiResult(); // Returns 404 Not Found
    }

    return Msg.Crud.Retrieved("User")
        .WithData(user)
        .ToMinimalApiResult(); // Returns 200 OK
});

users.MapPost("/", (CreateUserDto dto, ILogger<Program> logger) =>
{
    var user = userRepository.Create(dto);

    return Msg.Crud.Created("User")
        .WithData(user)
        .Log(logger)
        .ToMinimalApiResult(); // Returns 201 Created
});

app.Run();
```

---

## API Response Format

All messages converted to API responses use a standardized JSON format:

### Success Response

**Code:**
```csharp
return Msg.Crud.Created("User")
    .WithData(new { Id = 123, Name = "John Doe" })
    .ToApiResponse();
```

**HTTP Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "success": true,
  "code": "CRUD_001",
  "type": "success",
  "title": "User Created",
  "description": "User has been created successfully.",
  "data": {
    "id": 123,
    "name": "John Doe"
  },
  "timestamp": "2026-01-09T14:30:00.000Z",
  "correlationId": "0HMVQK8F3J8QK:00000001",
  "metadata": null
}
```

---

### Error Response

**Code:**
```csharp
return Msg.Crud.NotFound("User")
    .WithMetadata("userId", 999)
    .ToApiResponse();
```

**HTTP Response:**
```http
HTTP/1.1 404 Not Found
Content-Type: application/json

{
  "success": false,
  "code": "CRUD_004",
  "type": "error",
  "title": "User Not Found",
  "description": "The requested User could not be found.",
  "data": null,
  "timestamp": "2026-01-09T14:30:00.000Z",
  "correlationId": "0HMVQK8F3J8QK:00000002",
  "metadata": {
    "userId": 999
  }
}
```

---

### Validation Error Response

**Code:**
```csharp
return Msg.Validation.Failed()
    .WithData(new
    {
        errors = new Dictionary<string, string[]>
        {
            ["Email"] = new[] { "Invalid email format" },
            ["Password"] = new[] { "Password too weak" }
        }
    })
    .ToApiResponse();
```

**HTTP Response:**
```http
HTTP/1.1 422 Unprocessable Entity
Content-Type: application/json

{
  "success": false,
  "code": "VAL_001",
  "type": "error",
  "title": "Validation Failed",
  "description": "One or more validation errors occurred.",
  "data": {
    "errors": {
      "Email": ["Invalid email format"],
      "Password": ["Password too weak"]
    }
  },
  "timestamp": "2026-01-09T14:30:00.000Z",
  "correlationId": "0HMVQK8F3J8QK:00000003",
  "metadata": null
}
```

---

## Built-in Interceptors

The AspNetCore package includes three powerful interceptors that automatically enrich your messages:

### 1. CorrelationIdInterceptor (Auto-enabled)

**What it does:** Automatically adds correlation IDs from `HttpContext.TraceIdentifier`

**Configuration:**
```csharp
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    options.Interceptors.AutoAddCorrelationId = true; // ← Enabled by default
});
```

**Result:**
```json
{
  "correlationId": "0HMVQK8F3J8QK:00000001"
}
```

---

### 2. MetadataEnrichmentInterceptor (Opt-in)

**What it does:** Automatically adds request context to metadata

**Configuration:**
```csharp
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    options.Interceptors.AutoEnrichMetadata = true; // ← Enable it
    options.Interceptors.MetadataFields.IncludeRequestPath = true;
    options.Interceptors.MetadataFields.IncludeRequestMethod = true;
    options.Interceptors.MetadataFields.IncludeUserAgent = true;
    options.Interceptors.MetadataFields.IncludeIpAddress = true;
    options.Interceptors.MetadataFields.IncludeUserId = true;
    options.Interceptors.MetadataFields.IncludeUserName = true;
});
```

**Result:**
```json
{
  "metadata": {
    "RequestPath": "/api/users",
    "RequestMethod": "POST",
    "UserAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
    "IpAddress": "192.168.1.100",
    "UserId": "usr_123456",
    "UserName": "john.doe@example.com"
  }
}
```

---

### 3. LoggingInterceptor (Via AutoLog)

**What it does:** Automatically logs all messages via ILogger

**Configuration:**
```csharp
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    options.Logging.AutoLog = true;
    options.Logging.MinimumLogLevel = LogLevel.Warning; // Only log warnings and above
});
```

**Console Output:**
```
info: EasyMessages[0]
      [CRUD_001] User Created: User has been created successfully.
      => CorrelationId: 0HMVQK8F3J8QK:00000001
```

---

## Configuration via appsettings.json

Configure EasyMessages entirely through appsettings.json:

**appsettings.Development.json:**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": true,
      "IncludeMetadata": true,
      "IncludeCorrelationId": true
    },
    "Interceptors": {
      "AutoAddCorrelationId": true,
      "AutoEnrichMetadata": true,
      "MetadataFields": {
        "IncludeRequestPath": true,
        "IncludeRequestMethod": true,
        "IncludeUserAgent": false,
        "IncludeIpAddress": false
      }
    },
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Information"
    },
    "Storage": {
      "CustomMessagesPath": "messages/custom.json"
    }
  }
}
```

**appsettings.Production.json:**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": false,
      "IncludeMetadata": false
    },
    "Interceptors": {
      "AutoAddCorrelationId": true,
      "AutoEnrichMetadata": false
    },
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Warning"
    }
  }
}
```

**Program.cs:**
```csharp
// Automatically loads from appsettings.{Environment}.json
builder.Services.AddEasyMessages(builder.Configuration);
```

---

## Configuration Presets

Quick environment-specific setup with built-in presets:

### Development Preset

**Verbose output, detailed logging, metadata enrichment**

```csharp
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Development
);
```

**Settings:**
- [✓] All fields included (timestamp, metadata, correlation ID, etc.)
- [✓] Auto-logging enabled (Information level)
- [✓] Metadata enrichment enabled
- [✓] Correlation ID auto-added

---

### Production Preset

**Minimal output, performance-optimized**

```csharp
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Production
);
```

**Settings:**
- **Warning:** Minimal fields (no timestamp, no metadata)
- [✓] Auto-logging enabled (Warning level and above)
- [ ] Metadata enrichment disabled
- [✓] Correlation ID auto-added

---

### Testing Preset

**Consistent output for test assertions**

```csharp
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Testing
);
```

**Settings:**
- [✓] Essential fields only
- [ ] Auto-logging disabled
- [ ] Metadata enrichment disabled
- [ ] Correlation ID disabled (for deterministic tests)

---

### API Preset

**Client-friendly responses**

```csharp
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Api
);
```

**Settings:**
- [✓] Client-relevant fields (code, type, title, description, data)
- [✓] Correlation ID for request tracing
- [ ] Internal metadata excluded
- [ ] Timestamps excluded

---

## Logging Integration

### Manual Logging

```csharp
public IActionResult Create(CreateUserDto dto)
{
    var user = _userRepository.Create(dto);

    return Msg.Crud.Created("User")
        .WithData(user)
        .Log(_logger) // ← Manually log
        .ToApiResponse();
}
```

---

### Automatic Logging

```csharp
// Enable auto-logging via configuration
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    options.Logging.AutoLog = true;
    options.Logging.MinimumLogLevel = LogLevel.Information;
});

// Now all messages are logged automatically
public IActionResult Create(CreateUserDto dto)
{
    return Msg.Crud.Created("User")
        .WithData(user)
        .ToApiResponse(); // ← Automatically logged!
}
```

---

### Log Levels by Message Type

| Message Type | Log Level |
|-------------|-----------|
| Success | Information |
| Info | Information |
| Warning | Warning |
| Error | Error |
| Critical | Critical |

---

## Dependency Injection

### Service Registration

```csharp
builder.Services.AddEasyMessages(builder.Configuration);
```

**What gets registered:**
- `IOptions<EasyMessagesOptions>` - Configuration options
- `IHttpContextAccessor` - For correlation ID and metadata
- `EasyMessagesConfigurator` - Applies configuration on startup
- `EasyMessagesStartupService` - Hosted service for initialization

---

### Using IOptions

```csharp
public class UsersController : ControllerBase
{
    private readonly EasyMessagesOptions _options;

    public UsersController(IOptions<EasyMessagesOptions> options)
    {
        _options = options.Value;
    }

    public IActionResult Get()
    {
        // Access configuration
        var includeMetadata = _options.Formatter.IncludeMetadata;
        // ...
    }
}
```

---

## HTTP Status Code Mapping

Messages automatically map to appropriate HTTP status codes:

| Message | HTTP Status | IActionResult |
|---------|-------------|---------------|
| `Msg.Crud.Created()` | 200 OK | `OkObjectResult` |
| `Msg.Crud.Retrieved()` | 200 OK | `OkObjectResult` |
| `Msg.Crud.Updated()` | 200 OK | `OkObjectResult` |
| `Msg.Crud.Deleted()` | 200 OK | `OkObjectResult` |
| `Msg.Crud.NotFound()` | 404 Not Found | `NotFoundObjectResult` |
| `Msg.Auth.LoginFailed()` | 401 Unauthorized | `UnauthorizedObjectResult` |
| `Msg.Auth.Unauthorized()` | 403 Forbidden | `ObjectResult` (403) |
| `Msg.Validation.Failed()` | 422 Unprocessable Entity | `UnprocessableEntityObjectResult` |
| `Msg.System.Error()` | 500 Internal Server Error | `ObjectResult` (500) |
| `Msg.Database.ConnectionFailed()` | 503 Service Unavailable | `ObjectResult` (503) |

**Override if needed:**
```csharp
return Msg.Crud.Created("User")
    .WithStatusCode(201) // ← Override to 201 Created
    .ToApiResponse();
```

---

## Common Patterns

### REST API CRUD

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
        => Msg.Crud.Retrieved("Products")
            .WithData(_repo.GetAll())
            .ToApiResponse();

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var product = _repo.FindById(id);
        return product != null
            ? Msg.Crud.Retrieved("Product").WithData(product).ToApiResponse()
            : Msg.Crud.NotFound("Product").WithMetadata("id", id).ToApiResponse();
    }

    [HttpPost]
    public IActionResult Create(CreateProductDto dto)
        => Msg.Crud.Created("Product")
            .WithData(_repo.Create(dto))
            .ToCreated($"/api/products/{product.Id}");

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateProductDto dto)
        => Msg.Crud.Updated("Product")
            .WithData(_repo.Update(id, dto))
            .ToApiResponse();

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _repo.Delete(id);
        return Msg.Crud.Deleted("Product").ToNoContent();
    }
}
```

---

### Validation Results

```csharp
[HttpPost]
public IActionResult Create(CreateUserDto dto)
{
    var validationResult = _validator.Validate(dto);

    if (!validationResult.IsValid)
    {
        return Msg.Validation.Failed()
            .WithData(new
            {
                errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            })
            .ToApiResponse(); // Returns 422 Unprocessable Entity
    }

    // ... create user
}
```

---

### Error Handling

```csharp
[HttpGet("{id}")]
public IActionResult GetById(int id)
{
    try
    {
        var user = _repo.FindById(id);
        return user != null
            ? Msg.Crud.Retrieved("User").WithData(user).ToApiResponse()
            : Msg.Crud.NotFound("User").WithMetadata("id", id).ToApiResponse();
    }
    catch (DbException ex)
    {
        return Msg.Database.TransactionFailed()
            .WithMetadata("error", ex.Message)
            .Log(_logger)
            .ToApiResponse(); // Returns 500 Internal Server Error
    }
}
```

---

## Best Practices

### [✓] DO:

1. **Use `.ToApiResponse()` for controllers** - Returns IActionResult
2. **Use `.ToMinimalApiResult()` for Minimal APIs** - Returns IResult
3. **Use `.Log(_logger)` before `.ToApiResponse()`** - Chain logging
4. **Use configuration presets** - Quick environment setup
5. **Enable auto-correlation ID** - Distributed tracing
6. **Use `.WithData()` for payloads** - Return data to clients
7. **Use `.WithMetadata()` for debugging** - Additional context

```csharp
// Good
return Msg.Crud.Created("User")
    .WithData(user)
    .WithMetadata("source", "WebAPI")
    .Log(_logger)
    .ToApiResponse();
```

---

### [ ] DON'T:

1. **Don't use `.ToJson()` in controllers** - Use `.ToApiResponse()` instead
2. **Don't hardcode HTTP status codes** - Trust the automatic mapping
3. **Don't skip logging errors** - Always log failures
4. **Don't include sensitive data in metadata** - Security risk
5. **Don't use console formatters in APIs** - Wrong output format

```csharp
// Bad
return Ok(Msg.Crud.Created("User").ToJson()); // [ ] Wrong!

// Good
return Msg.Crud.Created("User").ToApiResponse(); // [✓] Correct
```

---

## Next Steps

Explore the ASP.NET Core integration in depth:

- **[Setup and Configuration](Setup-and-Configuration.md)** - Complete setup guide
- **[Configuration Guide](Configuration-Guide.md)** - Comprehensive configuration
- **[Configuration Presets](Configuration-Presets.md)** - Environment presets
- **[API Response Patterns](API-Response-Patterns.md)** - Response patterns
- **[Logging Integration](Logging-Integration.md)** - Logging strategies

---

**Questions?** [Ask in Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

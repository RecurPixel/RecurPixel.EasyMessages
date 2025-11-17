# RecurPixel.EasyMessages.AspNetCore

> Seamless EasyMessages integration for ASP.NET Core applications

[![NuGet](https://img.shields.io/nuget/v/RecurPixel.EasyMessages.AspNetCore.svg)](https://www.nuget.org/packages/RecurPixel.EasyMessages.AspNetCore/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

---

## 📦 Installation

```bash
# Installs both Core and AspNetCore packages
dotnet add package RecurPixel.EasyMessages.AspNetCore
```

---

## 🎯 What's Included

- **`ToApiResponse()`** – Convert messages to `IActionResult` (MVC/Web API)
- **`ToMinimalApiResult()`** – Convert messages to `IResult` (Minimal APIs)
- **`AddEasyMessages()`** – Dependency injection setup
- **Logging Integration** – Automatic ILogger support
- **Configuration System** – Customize behavior

---

## 🚀 Quick Start

### 1. Setup (Optional)

```csharp
// Program.cs
using EasyMessages;
using EasyMessages.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Optional: Configure EasyMessages
builder.Services.AddEasyMessages(options =>
{
    options.CustomMessagesPath = "messages/custom.json";
    options.AutoLog = true;
});

var app = builder.Build();
app.Run();
```

### 2. Use in Controllers

```csharp
using EasyMessages;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrEmpty(dto.Username))
            return Msg.Validation.RequiredField("Username").ToApiResponse();

        var isValid = ValidateCredentials(dto);
        
        if (!isValid)
            return Msg.Auth.LoginFailed().ToApiResponse();

        return Msg.Auth.LoginSuccess()
            .WithData(new { token = GenerateToken() })
            .ToApiResponse();
    }
}
```

### 3. Use in Minimal APIs

```csharp
using EasyMessages;

app.MapPost("/api/users", async (CreateUserDto dto, IUserService service) =>
{
    var user = await service.CreateAsync(dto);
    
    return Msg.Crud.Created("User")
        .WithData(user)
        .ToMinimalApiResult();
});

app.MapGet("/api/users/{id}", async (int id, IUserService service) =>
{
    var user = await service.GetByIdAsync(id);
    
    return user == null
        ? Msg.Crud.NotFound("User").ToMinimalApiResult()
        : Results.Ok(user);
});
```

---

## 📊 API Response Format

All responses follow a consistent structure:

```json
{
  "success": false,
  "code": "AUTH_001",
  "type": "error",
  "title": "Authentication Failed",
  "description": "Invalid username or password.",
  "data": null,
  "timestamp": "2025-11-17T10:30:00Z",
  "correlationId": "abc-123-def-456",
  "metadata": {}
}
```

**HTTP Status Code:** Automatically set based on message type or explicitly via `.WithStatusCode()`

---

## 🎨 Real-World Examples

### Complete CRUD Controller

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
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        // Validation
        if (!ModelState.IsValid)
            return Msg.Validation.Failed()
                .WithData(ModelState)
                .ToApiResponse();

        // Business logic
        try
        {
            var user = await _userService.CreateAsync(dto);
            
            return Msg.Crud.Created("User")
                .WithData(user)
                .WithCorrelationId(HttpContext.TraceIdentifier)
                .Log(_logger)
                .ToApiResponse(); // Returns 201 Created
        }
        catch (DuplicateEmailException ex)
        {
            return Msg.Validation.InvalidFormat("Email")
                .WithData(new { reason = "Email already exists" })
                .ToApiResponse();
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        
        if (user == null)
            return Msg.Crud.NotFound("User")
                .WithMetadata("requestedId", id)
                .ToApiResponse(); // Returns 404

        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return Msg.Crud.NotFound("User").ToApiResponse();

        await _userService.UpdateAsync(id, dto);
        
        return Msg.Crud.Updated("User")
            .WithData(new { id })
            .ToApiResponse(); // Returns 200 OK
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var exists = await _userService.ExistsAsync(id);
        if (!exists)
            return Msg.Crud.NotFound("User").ToApiResponse();

        await _userService.DeleteAsync(id);
        
        return Msg.Crud.Deleted("User")
            .WithMetadata("deletedId", id)
            .ToApiResponse(); // Returns 200 OK
    }
}
```

### Authentication & Authorization

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.AuthenticateAsync(dto);
        
        if (!result.Success)
            return Msg.Auth.LoginFailed()
                .WithMetadata("attempts", result.FailedAttempts)
                .ToApiResponse(); // 401 Unauthorized

        return Msg.Auth.LoginSuccess()
            .WithData(new 
            { 
                token = result.Token,
                expiresIn = 3600
            })
            .ToApiResponse();
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        // Logout logic...
        
        return Msg.Auth.LoginSuccess() // Reuse for logout
            .WithParams(new { message = "Logged out successfully" })
            .ToApiResponse();
    }

    [HttpGet("protected")]
    [Authorize]
    public IActionResult Protected()
    {
        // This will automatically return 401 if not authenticated
        // via ASP.NET Core's [Authorize] attribute
        
        return Ok(new { message = "You have access!" });
    }
}
```

### File Upload

```csharp
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly string[] _allowedTypes = { ".pdf", ".docx", ".txt" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        // Check if file exists
        if (file == null || file.Length == 0)
            return Msg.Validation.RequiredField("File").ToApiResponse();

        // Check file size
        if (file.Length > MaxFileSize)
            return Msg.Custom("FILE_003") // Assume you have this custom message
                .WithParams(new { maxSize = "10MB" })
                .ToApiResponse();

        // Check file type
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedTypes.Contains(extension))
            return Msg.File.InvalidType(_allowedTypes).ToApiResponse();

        // Process upload
        var filePath = await SaveFileAsync(file);
        
        return Msg.File.Uploaded()
            .WithData(new { path = filePath, size = file.Length })
            .ToApiResponse();
    }
}
```

### Global Error Handling

```csharp
// Middleware for unhandled exceptions
public class GlobalExceptionHandler : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");

            var message = ex switch
            {
                UnauthorizedAccessException => Msg.Auth.Unauthorized(),
                ArgumentException => Msg.Validation.Failed(),
                _ => Msg.System.Error()
                    .WithData(new { error = ex.Message })
                    .WithCorrelationId(context.TraceIdentifier)
            };

            var result = message.ToApiResponse();
            await result.ExecuteResultAsync(new ActionContext
            {
                HttpContext = context
            });
        }
    }
}

// Register in Program.cs
app.UseMiddleware<GlobalExceptionHandler>();
```

---

## ⚙️ Configuration

### Basic Configuration

```csharp
builder.Services.AddEasyMessages(options =>
{
    // Load custom messages from file
    options.CustomMessagesPath = "messages/custom.json";
});
```

### Advanced Configuration

```csharp
builder.Services.AddEasyMessages(options =>
{
    // Custom messages
    options.CustomMessagesPath = "messages/custom.json";
    
    // Logging
    options.AutoLog = true; // Automatically log errors/warnings
    options.MinimumLogLevel = LogLevel.Warning; // Only log Warning+
    
    // Response formatting
    options.IncludeTimestamp = true;
    options.IncludeCorrelationId = true;
    options.IncludeStackTrace = builder.Environment.IsDevelopment();
    
    // Localization (future)
    options.DefaultLocale = "en-US";
});
```

### Custom Message Loader

```csharp
// Load messages from database instead of file
public class DatabaseMessageLoader : IMessageLoader
{
    private readonly ApplicationDbContext _context;

    public async Task<Dictionary<string, MessageTemplate>> LoadAsync()
    {
        return await _context.Messages
            .ToDictionaryAsync(m => m.Code, m => new MessageTemplate
            {
                Type = m.Type,
                Title = m.Title,
                Description = m.Description,
                HttpStatusCode = m.HttpStatusCode
            });
    }
}

// Register it
builder.Services.AddEasyMessages(options =>
{
    options.UseLoader<DatabaseMessageLoader>();
});
```

---

## 🔌 Logging Integration

EasyMessages integrates seamlessly with `Microsoft.Extensions.Logging`:

### Manual Logging

```csharp
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;

    [HttpPost]
    public IActionResult Create([FromBody] CreateOrderDto dto)
    {
        var result = ProcessOrder(dto);
        
        if (!result.Success)
            return Msg.Validation.Failed()
                .Log(_logger) // Logs to ILogger
                .ToApiResponse();
        
        return Ok(result);
    }
}
```

### Auto-Logging

```csharp
// Enable auto-logging in configuration
builder.Services.AddEasyMessages(options =>
{
    options.AutoLog = true;
    options.MinimumLogLevel = LogLevel.Warning; // Only auto-log warnings and errors
});

// Now all Warning/Error/Critical messages are automatically logged
return Msg.System.Error().ToApiResponse(); // Auto-logged ✅
```

---

## 🧪 Testing

### Controller Testing

```csharp
public class UsersControllerTests
{
    [Fact]
    public async Task Create_WithInvalidData_ReturnsValidationError()
    {
        // Arrange
        var controller = new UsersController(Mock.Of<IUserService>(), Mock.Of<ILogger<UsersController>>());
        var dto = new CreateUserDto(); // Invalid

        // Act
        var result = await controller.Create(dto);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(422, objectResult.StatusCode);
        
        var response = Assert.IsType<ApiResponse>(objectResult.Value);
        Assert.False(response.Success);
        Assert.Equal("VAL_001", response.Code);
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        // Arrange
        var userService = Mock.Of<IUserService>(s => 
            s.GetByIdAsync(It.IsAny<int>()) == Task.FromResult<User>(null));
        
        var controller = new UsersController(userService, Mock.Of<ILogger<UsersController>>());

        // Act
        var result = await controller.GetById(999);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(404, objectResult.StatusCode);
        
        var response = Assert.IsType<ApiResponse>(objectResult.Value);
        Assert.Equal("CRUD_004", response.Code);
    }
}
```

### Integration Testing

```csharp
public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401()
    {
        // Arrange
        var loginDto = new { username = "test", password = "wrong" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("AUTH_001", result.Code);
    }
}
```

---

## 🎯 Best Practices

### 1. Use Correlation IDs

```csharp
[HttpPost]
public IActionResult Create([FromBody] CreateDto dto)
{
    return Msg.Crud.Created("Resource")
        .WithCorrelationId(HttpContext.TraceIdentifier)
        .ToApiResponse();
}
```

### 2. Enrich with Context

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetById(int id)
{
    var item = await _service.GetByIdAsync(id);
    
    if (item == null)
        return Msg.Crud.NotFound("Item")
            .WithMetadata("requestedId", id)
            .WithMetadata("userId", User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            .ToApiResponse();
    
    return Ok(item);
}
```

### 3. Log Important Actions

```csharp
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    await _service.DeleteAsync(id);
    
    return Msg.Crud.Deleted("User")
        .WithData(new { deletedId = id })
        .Log(_logger) // Log the deletion
        .ToApiResponse();
}
```

### 4. Use Custom Messages for Business Logic

```json
// custom-messages.json
{
  "ORDER_001": {
    "type": "Error",
    "title": "Order Cannot Be Cancelled",
    "description": "Orders can only be cancelled within 24 hours of placement.",
    "httpStatusCode": 400
  }
}
```

```csharp
[HttpPost("{id}/cancel")]
public async Task<IActionResult> CancelOrder(int id)
{
    var order = await _service.GetByIdAsync(id);
    
    if (!order.CanBeCancelled())
        return Msg.Custom("ORDER_001")
            .WithData(new { orderId = id, placedAt = order.CreatedAt })
            .ToApiResponse();
    
    await _service.CancelAsync(id);
    return Ok();
}
```

---

## 📋 API Reference

### Extension Methods

```csharp
// Convert to MVC/Web API result
IActionResult ToApiResponse()

// Convert to Minimal API result
IResult ToMinimalApiResult()

// Log message
Message Log(ILogger logger)
```

### Response Model

```csharp
public class ApiResponse
{
    public bool Success { get; set; }
    public string Code { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public object? Data { get; set; }
    public DateTime Timestamp { get; set; }
    public string? CorrelationId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
```

---

## 🔗 Related Packages

- **RecurPixel.EasyMessages** – Core library (required)

---

## 📖 Documentation

- **Main Docs:** [GitHub Wiki](https://github.com/RecurPixel/EasyMessages/wiki)
- **Samples:** [ASP.NET Core Examples](https://github.com/RecurPixel/EasyMessages/tree/main/samples/WebApiSample)

---

## 🤝 Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md)

---

## 📄 License

MIT License - see [LICENSE](LICENSE)

---

**Built with ❤️ by RecurPixel**
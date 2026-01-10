# Console vs Web Applications

Understand the differences between using EasyMessages in console applications versus ASP.NET Core web applications, and learn which package and features to use for each scenario.

---

## Quick Comparison

| Feature | Console Apps | ASP.NET Core Apps |
|---------|--------------|-------------------|
| **Package** | `RecurPixel.EasyMessages` | `RecurPixel.EasyMessages.AspNetCore` |
| **Message Creation** | [✓] `Msg.*` | [✓] `Msg.*` |
| **Console Output** | [✓] `.ToConsole()` | **Warning:** Available but not typical |
| **JSON/XML Output** | [✓] `.ToJson()`, `.ToXml()` | [✓] `.ToJson()`, `.ToXml()` |
| **API Response** | [ ] Not available | [✓] `.ToApiResponse()` |
| **Logging Integration** | **Warning:** Manual only | [✓] `.Log(ILogger)` |
| **DI Registration** | [ ] Not needed | [✓] `.AddEasyMessages()` |
| **IOptions Configuration** | [ ] Not available | [✓] Full support |
| **Configuration Presets** | [ ] Not available | [✓] Dev/Prod/Test/Staging/Api |
| **HTTP Context** | [ ] Not applicable | [✓] Automatic enrichment |
| **Correlation IDs** | **Warning:** Manual only | [✓] Automatic |

---

## Console Applications

### When to Use Console Package

Use `RecurPixel.EasyMessages` (Core package) for:

- [✓] Command-line tools and utilities
- [✓] Background workers and Windows Services
- [✓] Data processing and ETL jobs
- [✓] Scheduled tasks and cron jobs
- [✓] Desktop applications (WPF, WinForms)
- [✓] Class libraries (non-web)
- [✓] Azure Functions with output bindings
- [✓] AWS Lambda functions
- [✓] Test projects
- [✓] Migration scripts

### Installation

```bash
dotnet add package RecurPixel.EasyMessages --version 0.1.0-beta.1
```

### Typical Usage Pattern

```csharp
using RecurPixel.EasyMessages;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Processing data...");

        try
        {
            // Process data
            var result = ProcessData();

            // Output to console
            Msg.System.Success()
                .WithData(new { RecordsProcessed = result.Count })
                .ToConsole(useColors: true);

            // Or save to file as JSON
            var json = Msg.System.Success()
                .WithData(result)
                .ToJson();

            File.WriteAllText("result.json", json);
        }
        catch (Exception ex)
        {
            // Error to console
            Msg.System.Error()
                .WithMetadata("error", ex.Message)
                .ToConsole(useColors: true);

            Environment.Exit(1);
        }
    }
}
```

### Features Available

#### 1. Console Output with Colors

```csharp
// Success messages (green)
Msg.System.Success().ToConsole(useColors: true);

// Error messages (red)
Msg.System.Error().ToConsole(useColors: true);

// Warning messages (yellow)
Msg.Validation.Failed().ToConsole(useColors: true);

// Info messages (blue)
Msg.System.Processing().ToConsole(useColors: true);

// Disable colors
Msg.System.Success().ToConsole(useColors: false);
```

#### 2. File Output

```csharp
// Save as JSON
var json = Msg.Crud.Created("Record")
    .WithData(myData)
    .ToJson();
File.WriteAllText("output.json", json);

// Save as XML
var xml = Msg.Crud.Created("Record")
    .WithData(myData)
    .ToXml();
File.WriteAllText("output.xml", xml);

// Save as plain text
var text = Msg.System.Success()
    .WithData(myData)
    .ToPlainText();
File.WriteAllText("output.txt", text);
```

#### 3. Custom Messages

```csharp
// Load custom messages from file
MessageRegistry.LoadCustomMessages("messages/custom.json");

// Use custom messages
Msg.Custom("MY_CUSTOM_001")
    .WithParams(new { field = "value" })
    .ToConsole();
```

#### 4. Manual Logging

```csharp
// You can still use standard logging
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

var logger = loggerFactory.CreateLogger<Program>();

// Manual logging with EasyMessages
var message = Msg.System.Error();
logger.LogError("[{Code}] {Title}: {Description}",
    message.Code, message.Title, message.Description);
```

#### 5. Formatter Configuration

```csharp
using RecurPixel.EasyMessages.Configuration;

// Configure global formatting
FormatterConfiguration.Configure(options =>
{
    options.IncludeTimestamp = true;
    options.IncludeCorrelationId = true;
    options.IncludeMetadata = true;
    options.IncludeData = true;
    options.IncludeNullFields = false;
});

// Or use presets
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Minimal);
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Verbose);
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Debug);
```

### What's NOT Available

[ ] **`.ToApiResponse()`** - This is ASP.NET Core specific
[ ] **`.Log(ILogger)` extension** - Use manual logging instead
[ ] **DI registration** - Not needed for console apps
[ ] **IOptions configuration** - Use FormatterConfiguration instead
[ ] **Configuration presets** - Use FormatterConfiguration presets
[ ] **HTTP context enrichment** - Not applicable

### Complete Console Example

```csharp
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Configuration;

Console.WriteLine("=== Data Processing Tool ===\n");

// Configure formatting
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Verbose);

try
{
    // Start processing
    Msg.System.Processing()
        .WithParams(new { task = "Data Import" })
        .ToConsole(useColors: true);

    // Load data
    var records = LoadDataFromCsv("data.csv");

    Msg.System.Success()
        .WithData(new { RecordsLoaded = records.Count })
        .ToConsole(useColors: true);

    // Process each record
    int successCount = 0;
    int errorCount = 0;

    foreach (var record in records)
    {
        try
        {
            ProcessRecord(record);
            successCount++;
        }
        catch (Exception ex)
        {
            Msg.System.Error()
                .WithData(new { Record = record.Id })
                .WithMetadata("error", ex.Message)
                .ToConsole(useColors: true);

            errorCount++;
        }
    }

    // Summary
    Console.WriteLine($"\nProcessing complete:");
    Console.WriteLine($"  Success: {successCount}");
    Console.WriteLine($"  Errors: {errorCount}");

    // Save results
    var summary = new
    {
        TotalRecords = records.Count,
        Successful = successCount,
        Failed = errorCount,
        ProcessedAt = DateTime.UtcNow
    };

    var json = Msg.System.Success()
        .WithData(summary)
        .ToJson();

    File.WriteAllText("processing-summary.json", json);

    Msg.File.Uploaded()
        .WithParams(new { filename = "processing-summary.json" })
        .ToConsole(useColors: true);
}
catch (Exception ex)
{
    Msg.System.Error()
        .WithMetadata("error", ex.Message)
        .WithMetadata("stackTrace", ex.StackTrace)
        .ToConsole(useColors: true);

    Environment.Exit(1);
}
```

---

## ASP.NET Core Applications

### When to Use AspNetCore Package

Use `RecurPixel.EasyMessages.AspNetCore` for:

- [✓] ASP.NET Core Web APIs
- [✓] REST APIs and GraphQL APIs
- [✓] MVC Applications
- [✓] Minimal APIs
- [✓] Blazor Server applications
- [✓] gRPC services (with HTTP endpoints)
- [✓] SignalR hubs (with HTTP endpoints)

### Installation

```bash
dotnet add package RecurPixel.EasyMessages.AspNetCore --version 0.1.0-beta.1
```

**Note:** **Note:** This automatically includes the Core package.

### Typical Usage Pattern

```csharp
// Program.cs
using RecurPixel.EasyMessages.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Register EasyMessages with DI
builder.Services.AddEasyMessages(builder.Configuration);

// Or use configuration presets
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Production
);

builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();
```

```csharp
// Controller
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductService _service;

    public ProductsController(
        ILogger<ProductsController> logger,
        IProductService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var product = _service.GetById(id);

        if (product == null)
        {
            return Msg.Crud.NotFound("Product")
                .WithMetadata("productId", id)
                .Log(_logger)
                .ToApiResponse(); // Returns 404
        }

        return Msg.Crud.Retrieved("Product")
            .WithData(product)
            .Log(_logger)
            .ToApiResponse(); // Returns 200
    }

    [HttpPost]
    public IActionResult Create(CreateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return Msg.Validation.Failed()
                .WithData(ModelState)
                .ToApiResponse(); // Returns 422
        }

        var product = _service.Create(dto);

        return Msg.Crud.Created("Product")
            .WithData(product)
            .Log(_logger)
            .ToApiResponse(); // Returns 200
    }
}
```

### Features Available

#### 1. API Response Conversion

```csharp
// Automatic HTTP status code mapping
return Msg.Auth.Unauthorized().ToApiResponse();     // 403 Forbidden
return Msg.Crud.NotFound("User").ToApiResponse();   // 404 Not Found
return Msg.Validation.Failed().ToApiResponse();     // 422 Unprocessable Entity
return Msg.System.Error().ToApiResponse();          // 500 Internal Server Error
return Msg.Crud.Created("User").ToApiResponse();    // 200 OK (success)

// Custom status code
return Msg.Auth.LoginFailed()
    .WithStatusCode(429) // Too Many Requests
    .ToApiResponse();
```

#### 2. Logging Integration

```csharp
// Automatic logging based on message type
return Msg.Crud.Created("User")
    .WithData(user)
    .Log(_logger) // Logs at Information level
    .ToApiResponse();

return Msg.System.Error()
    .WithMetadata("error", ex.Message)
    .Log(_logger) // Logs at Error level
    .ToApiResponse();

return Msg.Validation.Failed()
    .WithData(errors)
    .Log(_logger) // Logs at Warning level
    .ToApiResponse();
```

#### 3. IOptions Configuration

```json
// appsettings.json
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
    },
    "Formatter": {
      "IncludeTimestamp": true,
      "IncludeCorrelationId": true
    },
    "Interceptor": {
      "AutoAddCorrelationId": true,
      "EnrichWithHttpContext": true
    }
  }
}
```

```csharp
// Program.cs
builder.Services.AddEasyMessages(builder.Configuration);
```

#### 4. Configuration Presets

```csharp
// Development: Verbose logging, all metadata
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Development
);

// Production: Minimal logging, essential data only
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Production
);

// Testing: No logging, fast execution
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Testing
);

// Staging: Production-like with validation
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Staging
);

// Api: Optimized for API responses
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Api
);
```

#### 5. Minimal API Support

```csharp
var app = builder.Build();

app.MapGet("/products/{id}", (int id, IProductService service, ILogger<Program> logger) =>
{
    var product = service.GetById(id);

    if (product == null)
    {
        return Results.NotFound(
            Msg.Crud.NotFound("Product")
                .WithMetadata("productId", id)
                .Log(logger)
                .ToApiResponse()
        );
    }

    return Results.Ok(
        Msg.Crud.Retrieved("Product")
            .WithData(product)
            .Log(logger)
            .ToApiResponse()
    );
});

app.MapPost("/products", (CreateProductDto dto, IProductService service, ILogger<Program> logger) =>
{
    var product = service.Create(dto);

    return Results.Created($"/products/{product.Id}",
        Msg.Crud.Created("Product")
            .WithData(product)
            .Log(logger)
            .ToApiResponse()
    );
});
```

#### 6. Built-in Interceptors

When enabled in configuration, these interceptors run automatically:

```csharp
// Correlation ID Interceptor
// Adds unique correlation IDs to track requests across services

// Metadata Enrichment Interceptor
// Adds HTTP context data (user, IP address, request path)

// Logging Interceptor
// Automatically logs all messages based on configuration
```

### What's NOT Available (from Console Package)

**Warning:** **Console output** - Available but not typical for APIs
**Warning:** **Manual file saving** - APIs return responses, not save files

### Complete API Example

```csharp
// Program.cs
using RecurPixel.EasyMessages.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasyMessages(
    builder.Configuration,
    builder.Environment.IsDevelopment()
        ? EasyMessagesPresets.Development
        : EasyMessagesPresets.Production
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

```csharp
// Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUserService _userService;

    public UsersController(
        ILogger<UsersController> logger,
        IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Msg.Crud.Retrieved("Users")
            .WithData(users)
            .Log(_logger)
            .ToApiResponse();
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _userService.GetById(id);
        if (user == null)
        {
            return Msg.Crud.NotFound("User")
                .WithMetadata("userId", id)
                .Log(_logger)
                .ToApiResponse();
        }

        return Msg.Crud.Retrieved("User")
            .WithData(user)
            .ToApiResponse();
    }

    [HttpPost]
    public IActionResult Create(CreateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return Msg.Validation.Failed()
                .WithData(ModelState)
                .ToApiResponse();
        }

        try
        {
            var user = _userService.Create(dto);
            return Msg.Crud.Created("User")
                .WithData(user)
                .Log(_logger)
                .ToApiResponse();
        }
        catch (DuplicateEmailException ex)
        {
            return Msg.Validation.Failed()
                .WithMetadata("field", "email")
                .WithMetadata("error", ex.Message)
                .Log(_logger)
                .ToApiResponse();
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateUserDto dto)
    {
        if (!_userService.Exists(id))
        {
            return Msg.Crud.NotFound("User")
                .WithMetadata("userId", id)
                .ToApiResponse();
        }

        var user = _userService.Update(id, dto);
        return Msg.Crud.Updated("User")
            .WithData(user)
            .Log(_logger)
            .ToApiResponse();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (!_userService.Exists(id))
        {
            return Msg.Crud.NotFound("User")
                .WithMetadata("userId", id)
                .ToApiResponse();
        }

        _userService.Delete(id);
        return Msg.Crud.Deleted("User")
            .WithMetadata("userId", id)
            .Log(_logger)
            .ToApiResponse();
    }
}
```

---

## Side-by-Side Comparison

### Console App: Data Processing

```csharp
// Console - RecurPixel.EasyMessages
using RecurPixel.EasyMessages;

static void ProcessFile(string path)
{
    try
    {
        var data = File.ReadAllLines(path);

        Msg.System.Processing()
            .WithData(new { File = path, Lines = data.Length })
            .ToConsole(useColors: true);

        // Process...

        Msg.System.Success()
            .WithData(new { Processed = data.Length })
            .ToConsole(useColors: true);
    }
    catch (Exception ex)
    {
        Msg.System.Error()
            .WithMetadata("error", ex.Message)
            .ToConsole(useColors: true);

        Environment.Exit(1);
    }
}
```

### Web API: File Upload

```csharp
// ASP.NET Core - RecurPixel.EasyMessages.AspNetCore
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly ILogger<FilesController> _logger;

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return Msg.Validation.RequiredField("file")
                    .ToApiResponse();
            }

            var path = await SaveFileAsync(file);

            return Msg.File.Uploaded()
                .WithData(new
                {
                    FileName = file.FileName,
                    Size = file.Length,
                    Path = path
                })
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
}
```

---

## Migration Between Packages

### From Console to Web

If you're converting a console app to a web API:

1. **Change package:**
```bash
dotnet remove package RecurPixel.EasyMessages
dotnet add package RecurPixel.EasyMessages.AspNetCore --version 0.1.0-beta.1
```

2. **Register with DI:**
```csharp
builder.Services.AddEasyMessages(builder.Configuration);
```

3. **Replace console output with API responses:**
```csharp
// Before (console)
Msg.System.Success().ToConsole();

// After (web API)
return Msg.System.Success().ToApiResponse();
```

4. **Add logging:**
```csharp
// Before (console)
Msg.System.Error().ToConsole();

// After (web API)
return Msg.System.Error().Log(_logger).ToApiResponse();
```

### From Web to Console (Less Common)

If you need to use EasyMessages in both console and web:

1. **Keep Core package for console code:**
```bash
dotnet add package RecurPixel.EasyMessages --version 0.1.0-beta.1
```

2. **Use AspNetCore package for web code:**
```bash
dotnet add package RecurPixel.EasyMessages.AspNetCore --version 0.1.0-beta.1
```

---

## Best Practices

### Console Applications

[✓] **DO:**
- Use colored console output for better readability
- Save important results to JSON/XML files
- Include timestamps and correlation IDs for debugging
- Handle exceptions and exit with appropriate codes
- Use FormatterConfiguration for consistent output

[ ] **DON'T:**
- Try to use `.ToApiResponse()` (not available)
- Expect automatic logging (use manual logging)
- Mix console and file output without clear separation

### Web Applications

[✓] **DO:**
- Always use `.ToApiResponse()` for API endpoints
- Chain `.Log(_logger)` for important operations
- Use configuration presets per environment
- Let HTTP status codes be set automatically
- Enable correlation IDs in production
- Use validation messages for ModelState errors

[ ] **DON'T:**
- Use `.ToConsole()` in API endpoints (use logging instead)
- Forget to register with DI
- Skip configuration in appsettings.json
- Disable logging in production
- Return raw message objects (use `.ToApiResponse()`)

---

## Next Steps

- **[Core Concepts](../Core-Concepts/Messages-and-Message-Types.md)** - Deep dive into messages
- **[Configuration Guide](../ASP.NET-Core/Configuration-Guide.md)** - Complete IOptions setup
- **[Examples](../Examples/Console-Application.md)** - Real-world examples

---

**Questions?** [Ask in Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

# EasyMessages

⚠️ **ALPHA RELEASE - v0.1.0-alpha.x**  
This is an early preview. APIs may change. Not recommended for production use.  
[Report Issues](https://github.com/RecurPixel/RecurPixel.EasyMessages/issues) | [Give Feedback](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

---

> **Tired of writing the same error messages over and over?**  
> EasyMessages gives you 100+ pre-built, standardized messages with a fluent API that just works.

[![NuGet](https://img.shields.io/badge/alpha-0.1.0--alpha.1-orange)](https://www.nuget.org/packages/RecurPixel.EasyMessages)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-purple)](https://dotnet.microsoft.com/)

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
Msg.Auth.LoginFailed().ToJson();
Msg.Auth.Unauthorized().ToConsole();
Msg.Crud.NotFound("User").ToJson();
```

---
### Simple Alpha Demo

[![Alpha Demo](https://img.youtube.com/vi/83ABUbKuIJc/0.jpg)](https://youtu.be/83ABUbKuIJc)

---

## 🚀 Quick Start (Alpha)

### Requirements
- .NET 8, 9, or 10
- Broader version support (6, 7, Standard 2.1) coming in Beta

### Installation

```bash
dotnet add package RecurPixel.EasyMessages --version 0.1.0-alpha.*
```

**Note:** If you're on .NET 5, 6, or 7, please wait for Beta release (2 weeks) or upgrade to .NET 8+.

### Your First Message (5 seconds)

```csharp
using RecurPixel.EasyMessages;

// Console app - works immediately!
Msg.Auth.LoginFailed().ToConsole(useColors: true);

// Output to JSON
var json = Msg.Crud.Created("User").ToJson();
Console.WriteLine(json);
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
  "timestamp": "2024-01-15T14:30:00Z"
}
```

---

## 🎯 Why EasyMessages?

### Zero Configuration
```csharp
// Works immediately - no setup needed
Msg.System.Error().ToConsole();
```

### IntelliSense-Friendly
```csharp
// Discover messages as you type
Msg.Auth.         // LoginFailed(), Unauthorized(), LoginSuccess()
Msg.Crud.         // Created(), Updated(), Deleted(), NotFound()
Msg.Validation.   // Failed(), RequiredField(), InvalidFormat()
```

### Type-Safe
```csharp
// No magic strings - compile-time safety
Msg.Auth.LoginFailed();  // ✅ Compiler validated
```

---

## 📚 Message Categories (Alpha)

| Category | Prefix | Count | Examples |
|----------|--------|-------|----------|
| **Authentication** | `AUTH_*` | 10 | Login failed, Unauthorized, Token expired |
| **CRUD Operations** | `CRUD_*` | 15 | Created, Updated, Deleted, Not found |
| **Validation** | `VAL_*` | 12 | Required field, Invalid format, Out of range |
| **System** | `SYS_*` | 8 | Error, Processing, Maintenance |
| **Database** | `DB_*` | 5 | Connection failed, Query timeout |
| **Files** | `FILE_*` | 10 | Uploaded, Invalid type, Size exceeded |
| **Network** | `NET_*` | 8 | Timeout, Service unavailable |
| **Custom** | `*` | ∞ | Your own messages |

**Total: 100+ built-in messages in alpha**

---

## 💡 Core Features

### 1. Simple Console Output

```csharp
using RecurPixel.EasyMessages;

class Program
{
    static void Main()
    {
        // Colored console output
        Msg.Auth.LoginFailed().ToConsole(useColors: true);
        
        // Success messages
        Msg.Crud.Created("User").ToConsole();
        
        // Error messages
        Msg.System.Error().ToConsole();
    }
}
```

### 2. JSON Output

```csharp
// Basic JSON
var json = Msg.Auth.LoginFailed().ToJson();

// With data
var json = Msg.Crud.Created("User")
    .WithData(new { Id = 123, Name = "John" })
    .ToJson();

// Custom JSON options
var json = Msg.Auth.LoginFailed().ToJson(
    jsonOptions: new JsonSerializerOptions 
    { 
        WriteIndented = true 
    });
```

### 3. Multiple Output Formats

```csharp
// JSON
var json = message.ToJson();

// XML
var xml = message.ToXml();

// Plain text
var text = message.ToPlainText();

// Console (with colors)
message.ToConsole();
```

### 4. Parameter Substitution

```csharp
// Simple substitution
Msg.Crud.Created("User");
// Result: "User has been created."

Msg.Validation.RequiredField("Email");
// Result: "Email is required."

Msg.File.InvalidType("PDF", "DOCX");
// Result: "Only PDF, DOCX files are allowed."

// Multiple parameters
Msg.Custom("WELCOME_001")
    .WithParams(new { name = "John", role = "Admin" });
// Template: "Welcome {name}, your role is {role}."
// Result: "Welcome John, your role is Admin."
```

### 5. Rich Context with Metadata

```csharp
// Add contextual data
Msg.Auth.LoginFailed()
    .WithData(new { UserId = 123 })
    .WithMetadata("attempt", 3)
    .WithMetadata("ipAddress", "192.168.1.1")
    .WithCorrelationId(Guid.NewGuid().ToString())
    .ToJson();
```

### 6. Smart HTTP Status Codes

```csharp
// Automatically mapped status codes
Msg.Auth.Unauthorized();     // 403
Msg.Crud.NotFound("User");   // 404
Msg.Validation.Failed();     // 422
Msg.System.Error();          // 500

// Override when needed
Msg.Auth.LoginFailed()
    .WithStatusCode(429)  // Too Many Requests
    .ToJson();
```

---

## 📖 Examples

### Example 1: Console Application

```csharp
using RecurPixel.EasyMessages;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting file processing...");
        
        Msg.System.Processing()
            .WithParams(new { task = "File processing" })
            .ToConsole();

        try
        {
            await ProcessFilesAsync();
            
            Msg.File.Uploaded()
                .WithData(new { count = 42, size = "2.5 MB" })
                .ToConsole();
        }
        catch (Exception ex)
        {
            Msg.System.Error()
                .WithMetadata("error", ex.Message)
                .WithMetadata("stackTrace", ex.StackTrace)
                .ToConsole();
        }
    }
    
    static async Task ProcessFilesAsync()
    {
        // Your logic here
        await Task.Delay(1000);
    }
}
```

**Output:**
```
ℹ Processing
  Your request is being processed.
  [2024-01-15 10:30:00] [SYS_002]

✓ File Uploaded
  File uploaded successfully.
  [2024-01-15 10:30:01] [FILE_001]
```

### Example 2: Data Import Job

```csharp
using RecurPixel.EasyMessages;

public class DataImportService
{
    public async Task ImportUsersAsync(string filePath)
    {
        Msg.System.Processing()
            .WithData(new { File = filePath })
            .ToConsole(useColors: true);
        
        var users = await ReadUsersFromFileAsync(filePath);
        int successCount = 0;
        int errorCount = 0;
        
        foreach (var user in users)
        {
            try
            {
                await ImportUserAsync(user);
                successCount++;
                
                Msg.Crud.Created("User")
                    .WithData(new { user.Email })
                    .ToConsole(useColors: true);
            }
            catch (Exception ex)
            {
                errorCount++;
                
                Msg.System.Error()
                    .WithData(new { user.Email, Error = ex.Message })
                    .ToConsole(useColors: true);
            }
        }
        
        // Summary
        Console.WriteLine($"\nImport complete: {successCount} succeeded, {errorCount} failed");
    }
}
```

### Example 3: Custom Messages

```json
// messages/custom.json
{
  "PAYMENT_001": {
    "type": "Success",
    "title": "Payment Processed",
    "description": "Payment of {amount} was successful. Transaction ID: {transactionId}",
    "httpStatusCode": 200
  },
  "PAYMENT_002": {
    "type": "Error",
    "title": "Payment Failed",
    "description": "Unable to process payment of {amount}. Reason: {reason}",
    "httpStatusCode": 400
  }
}
```

```csharp
using RecurPixel.EasyMessages;

// Load custom messages
MessageRegistry.LoadCustomMessages("messages/custom.json");

// Use your custom messages
Msg.Custom("PAYMENT_001")
    .WithParams(new { amount = "$50.00", transactionId = "TXN123" })
    .ToConsole();
    
Msg.Custom("PAYMENT_002")
    .WithParams(new { amount = "$50.00", reason = "Insufficient funds" })
    .ToConsole();
```

### Example 4: Format Configuration

```csharp
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Configuration;

// Configure global formatting options
FormatterConfiguration.Configure(options =>
{
    options.IncludeTimestamp = true;
    options.IncludeCorrelationId = true;
    options.IncludeMetadata = true;
    options.IncludeData = true;
});

// All messages now use these settings
var json = Msg.Auth.LoginFailed().ToJson();

// Or use presets
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Minimal);
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Verbose);
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Debug);
```

### Example 5: Custom Formatter

```csharp
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Formatters;

// Create custom CSV formatter
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
var csv = Msg.Auth.LoginFailed().ToFormat("csv");
Console.WriteLine(csv);
// Output: AUTH_001,Error,Authentication Failed,Invalid username or password.
```

### Example 6: Custom Interceptor

```csharp
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Interceptors;

// Create custom interceptor
public class TimestampInterceptor : IMessageInterceptor
{
    public Message OnBeforeFormat(Message message)
    {
        Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Formatting message {message.Code}");
        return message;
    }
    
    public Message OnAfterFormat(Message message)
    {
        Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] Formatted successfully");
        return message;
    }
}

// Register it
InterceptorRegistry.Register(new TimestampInterceptor());

// All messages now trigger the interceptor
Msg.Auth.LoginFailed().ToConsole();
```

### Example 7: Database Store

```csharp
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Stores;

// Create custom database store
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

// Use it
MessageRegistry.UseStore(new SqlServerMessageStore(connectionString));

// Or combine multiple stores (priority: last wins)
MessageRegistry.UseStores(
    new EmbeddedMessageStore(),     // Built-in defaults
    new FileMessageStore("custom.json"),
    new SqlServerMessageStore(connectionString)
);
```

### Example 8: File Processing with Validation

```csharp
using RecurPixel.EasyMessages;

public class FileProcessor
{
    private readonly string[] _allowedTypes = { "pdf", "docx", "txt" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB
    
    public void ProcessFile(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        var extension = fileInfo.Extension.TrimStart('.').ToLower();
        
        // Validate file type
        if (!_allowedTypes.Contains(extension))
        {
            Msg.File.InvalidType(_allowedTypes)
                .WithMetadata("actualType", extension)
                .ToConsole();
            return;
        }
        
        // Validate file size
        if (fileInfo.Length > MaxFileSize)
        {
            Msg.Custom("FILE_003")  // Assuming you have this message
                .WithParams(new 
                { 
                    maxSize = "10 MB", 
                    actualSize = $"{fileInfo.Length / 1024 / 1024} MB" 
                })
                .ToConsole();
            return;
        }
        
        // Process file
        try
        {
            // Your processing logic
            Msg.File.Uploaded()
                .WithData(new 
                { 
                    FileName = fileInfo.Name,
                    Size = $"{fileInfo.Length / 1024} KB"
                })
                .ToConsole();
        }
        catch (Exception ex)
        {
            Msg.System.Error()
                .WithMetadata("file", filePath)
                .WithMetadata("error", ex.Message)
                .ToConsole();
        }
    }
}
```

---

## 🔧 Configuration (Alpha)

### Formatter Configuration

```csharp
using RecurPixel.EasyMessages.Configuration;

// Global configuration
FormatterConfiguration.Configure(options =>
{
    options.IncludeTimestamp = true;
    options.IncludeCorrelationId = true;
    options.IncludeHttpStatusCode = true;
    options.IncludeMetadata = true;
    options.IncludeData = true;
    options.IncludeNullFields = false;
});

// Or use presets
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Minimal);
// Minimal: Only code, type, title, description

FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Verbose);
// Verbose: Include everything

FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Debug);
// Debug: Everything including null fields
```

### Custom Message Stores

```csharp
// Load from file
MessageRegistry.LoadCustomMessages("messages/custom.json");

// Or use store pattern
MessageRegistry.UseStore(new FileMessageStore("messages/custom.json"));

// Composite stores (priority: last wins)
MessageRegistry.UseStores(
    new EmbeddedMessageStore(),
    new FileMessageStore("custom.json"),
    new MyDatabaseStore(connectionString)
);
```

### Custom Formatters

```csharp
// Register custom formatter
FormatterRegistry.Register("csv", () => new CsvFormatter());
FormatterRegistry.Register("markdown", () => new MarkdownFormatter());

// Check registered formatters
var formatters = FormatterRegistry.GetRegisteredNames();
Console.WriteLine(string.Join(", ", formatters));
// Output: json, xml, text, console, csv, markdown
```

### Custom Interceptors

```csharp
// Register interceptors
InterceptorRegistry.Register(new TimestampInterceptor());
InterceptorRegistry.Register(new LoggingInterceptor());

// Clear all interceptors
InterceptorRegistry.Clear();
```

---

## 📦 ASP.NET Core Integration (Coming in Beta!)

The following features are coming in the next release:

```csharp
// 🔜 Coming in Beta v0.2.0-beta.1

// Easy API responses
return Msg.Auth.LoginFailed().ToApiResponse();

// Automatic logging
return Msg.Crud.Created("User")
    .Log(_logger)
    .ToApiResponse();

// DI Configuration
builder.Services.AddEasyMessages(options =>
{
    options.AutoLog = true;
    options.InterceptorOptions.AutoAddCorrelationId = true;
});

// Controller integration
[ApiController]
public class UsersController : ControllerBase
{
    [HttpPost]
    public IActionResult Create(CreateUserDto dto)
    {
        var user = _userService.Create(dto);
        return Msg.Crud.Created("User")
            .WithData(user)
            .ToApiResponse();  // 🔜 Beta feature
    }
}
```

**Want ASP.NET Core support?** [Star the repo](https://github.com/RecurPixel/RecurPixel.EasyMessages) and watch for beta release announcement!

---

## 📖 API Response Format

EasyMessages produces consistent, predictable responses:

```json
{
  "success": true,
  "code": "CRUD_001",
  "type": "success",
  "title": "Created Successfully",
  "description": "User has been created.",
  "timestamp": "2024-01-15T14:30:00Z",
  "data": {
    "id": 123,
    "name": "John Doe"
  }
}
```

---

## ⚠️ Known Limitations (Alpha)

This is an early preview. Here's what's not included yet:

- ⚠️ **Limited .NET Version Support**
  - Supports: .NET 8, 9, 10 only
  - Coming in Beta: .NET 6, 7
  - Coming in Stable: .NET Standard 2.1 (covers .NET 5+)

- ❌ **ASP.NET Core package** - Coming in Beta (v0.2.0-beta.1)
  - No `.ToApiResponse()` method
  - No `.Log(ILogger)` integration
  - No DI configuration (`AddEasyMessages()`)
  
- ❌ **Limited messages** - 100+ messages (200+ in beta)

- ❌ **Advanced interceptors** - Basic interceptors only
  - No correlation ID interceptor (Beta)
  - No metadata enrichment (Beta)
  - Manual registration only

- ❌ **Documentation** - Work in progress
  - Wiki coming after beta
  - More examples coming

- ❌ **Testing** - No(exhaustive) unit tests yet
  - **This is why we need your help!** Please test and report issues

---

## 🤝 How You Can Help (Alpha Testing)

### We Need Your Feedback!

1. **Try It Out**
   ```bash
   dotnet add package RecurPixel.EasyMessages --version 0.1.0-alpha.*
   ```

2. **Test These Scenarios**
   - Console applications
   - Background jobs
   - Data processing scripts
   - Custom message files
   - Custom formatters
   - Different output formats

3. **Report Issues**
   - [Open an issue](https://github.com/RecurPixel/RecurPixel.EasyMessages/issues)
   - Include code examples
   - Share your use case

4. **Suggest Features**
   - [Start a discussion](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)
   - What messages do you need?
   - What features would help?

---

## 🚦 Roadmap

### Current: v0.1.0-alpha.x ✅
- Core message system
- 100+ built-in messages
- JSON, XML, Console, PlainText formatters
- Parameter substitution
- Custom messages
- Formatter configuration
- Basic interceptors

### Next: v0.2.0-beta.1 (2 weeks)
- ASP.NET Core package
- `.ToApiResponse()` extension
- `.Log(ILogger)` integration
- DI configuration
- Correlation ID interceptor
- Metadata enrichment
- Expand to 150+ messages

### Future: v1.0.0 (6 weeks)
- Production-ready
- 200+ messages
- Full documentation
- Performance optimization
- Semantic versioning commitment

---

## 📊 Packages

| Package | Version | Status |
|---------|---------|--------|
| `RecurPixel.EasyMessages` | 0.1.0-alpha.x | ⚠️ **Alpha** - Available Now |
| `RecurPixel.EasyMessages.AspNetCore` | - | 🔜 **Coming in Beta** |

---

## 🎯 Design Philosophy

### 1. Zero Configuration
Works immediately without setup. Configuration is optional.

### 2. Fail Fast, Fail Clear
Exceptions are descriptive with helpful context.

### 3. Immutable by Design
Thread-safe, predictable behavior.

### 4. Extensible
Easy to add custom messages, formatters, and interceptors.

---

## 📄 License

MIT License - see [LICENSE](LICENSE) for details.

---

## 🙏 Acknowledgments

Built with ❤️ by [RecurPixel](https://github.com/RecurPixel)

Special thanks to:
- Early alpha testers
- The .NET community
- Everyone who provides feedback

---

## 📞 Support

- **Issues:** [GitHub Issues](https://github.com/RecurPixel/RecurPixel.EasyMessages/issues)
- **Discussions:** [GitHub Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)
- **Documentation:** [Wiki](https://github.com/RecurPixel/RecurPixel.EasyMessages/wiki) (Coming Soon)

---

## ⭐ Show Your Support

If you find EasyMessages useful, please:
- ⭐ Star the repository
- 🐛 Report bugs
- 💡 Suggest features
- 📢 Share with others

```bash
# Get started now!
dotnet add package RecurPixel.EasyMessages --version 0.1.0-alpha.*
```

---

**Remember:** This is an alpha release. APIs may change. Use for testing only.

**Your feedback will shape the future of EasyMessages!** 🚀

# RecurPixel.EasyMessages

[![NuGet](https://img.shields.io/nuget/v/RecurPixel.EasyMessages.svg)](https://www.nuget.org/packages/RecurPixel.EasyMessages)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RecurPixel.EasyMessages.svg)](https://www.nuget.org/packages/RecurPixel.EasyMessages)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/RecurPixel/RecurPixel.EasyMessages/blob/main/LICENSE)

**Core library for consistent, fluent message management in .NET applications.**

---

## What is it?

EasyMessages provides **100+ pre-built, standardized messages** for common scenarios like authentication, CRUD operations, validation, and system errors. Perfect for console applications, background jobs, class libraries, and any .NET application needing consistent messaging.

---

## Installation

```bash
dotnet add package RecurPixel.EasyMessages --version 0.1.0-beta.*
```

**Requirements:**
- .NET 8.0, 9.0, 10.0

---

## Quick Start (Console App)

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

        // With custom data
        Msg.File.Uploaded()
            .WithData(new { FileName = "report.pdf", Size = "2.5 MB" })
            .ToConsole();

        // Output to JSON
        var json = Msg.Crud.NotFound("User").ToJson();
        Console.WriteLine(json);
    }
}
```

**Output:**
```
✗ Authentication Failed
  Invalid username or password.
  [2024-01-15 14:30:00] [AUTH_001]

✓ User Created
  User has been created successfully.
  [2024-01-15 14:30:01] [CRUD_001]
```

---

## Core Features

### 1. Fluent Message Creation

```csharp
// Discover messages with IntelliSense
Msg.Auth.LoginFailed();
Msg.Crud.NotFound("Product");
Msg.Validation.RequiredField("Email");
Msg.System.Error();
```

### 2. Multiple Output Formats

```csharp
var message = Msg.Auth.LoginFailed();

// JSON
var json = message.ToJson();

// XML
var xml = message.ToXml();

// Plain text
var text = message.ToPlainText();

// Console with colors
message.ToConsole(useColors: true);
```

### 3. Built-in Message Categories

- **Auth** (10 messages) - Authentication, authorization
- **CRUD** (15 messages) - Create, read, update, delete operations
- **Validation** (12 messages) - Input validation, format checks
- **System** (8 messages) - System errors, processing states
- **Database** (5 messages) - Database operations
- **File** (10 messages) - File upload, download, validation
- **Network** (8 messages) - Network errors, timeouts
- **Custom** (∞) - Your own messages

### 4. Parameter Substitution

```csharp
// Simple substitution
Msg.Crud.Created("User");
// Result: "User has been created."

Msg.Validation.RequiredField("Email");
// Result: "Email is required."

// Multiple parameters
Msg.File.InvalidType("PDF", "DOCX");
// Result: "Only PDF, DOCX files are allowed."
```

### 5. Rich Context with Metadata

```csharp
Msg.Auth.LoginFailed()
    .WithData(new { UserId = 123 })
    .WithMetadata("attempt", 3)
    .WithMetadata("ipAddress", "192.168.1.1")
    .WithCorrelationId(Guid.NewGuid().ToString())
    .ToJson();
```

### 6. Custom Messages

```json
// messages/custom.json
{
  "PAYMENT_001": {
    "type": "Success",
    "title": "Payment Processed",
    "description": "Payment of {amount} was successful.",
    "httpStatusCode": 200
  }
}
```

```csharp
// Load and use custom messages
MessageRegistry.LoadCustomMessages("messages/custom.json");

Msg.Custom("PAYMENT_001")
    .WithParams(new { amount = "$50.00" })
    .ToConsole();
```

### 7. Flexible Configuration

```csharp
using RecurPixel.EasyMessages.Configuration;

// Configure global formatting options
FormatterConfiguration.Configure(options =>
{
    options.IncludeTimestamp = true;
    options.IncludeCorrelationId = true;
    options.IncludeMetadata = true;
});

// Or use presets
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Minimal);
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Verbose);
```

### 8. Extension Points

```csharp
// Custom formatters
public class CsvFormatter : MessageFormatterBase
{
    protected override string FormatCore(Message message)
    {
        return $"{message.Code},{message.Type},{message.Title}";
    }
}

FormatterRegistry.Register("csv", () => new CsvFormatter());

// Custom interceptors
public class LoggingInterceptor : IMessageInterceptor
{
    public Message OnBeforeFormat(Message message)
    {
        Console.WriteLine($"Formatting: {message.Code}");
        return message;
    }
}

InterceptorRegistry.Register(new LoggingInterceptor());
```

---

## When to Use

✅ **Perfect for:**
- Console applications
- Background jobs and workers
- Class libraries
- Data processing scripts
- File processing pipelines
- Import/export tools
- Command-line tools
- Any .NET application needing consistent messaging

❌ **Not needed for:**
- ASP.NET Core web applications (use `RecurPixel.EasyMessages.AspNetCore` instead)

---

## Complete Example: File Processing

```csharp
using RecurPixel.EasyMessages;

public class FileProcessor
{
    private readonly string[] _allowedTypes = { "pdf", "docx", "txt" };

    public void ProcessFile(string filePath)
    {
        Msg.System.Processing()
            .WithParams(new { task = "File processing" })
            .ToConsole();

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

## Documentation

📚 **[Complete Documentation](https://recurpixel.github.io/RecurPixel.EasyMessages/)**

### Quick Links
- 🚀 [Getting Started](https://recurpixel.github.io/RecurPixel.EasyMessages/latest/Getting-Started/Your-First-Message)
- 📖 [Core Concepts](https://recurpixel.github.io/RecurPixel.EasyMessages/latest/Core-Concepts/Messages-and-Message-Types)
- 💡 [Examples](https://recurpixel.github.io/RecurPixel.EasyMessages/latest/Examples/Console-Application)
- 📝 [How-To Guides](https://recurpixel.github.io/RecurPixel.EasyMessages/latest/How-To-Guides/Create-Custom-Messages)
- 📚 [API Reference](https://recurpixel.github.io/RecurPixel.EasyMessages/latest/API-Reference/Message-Codes-Reference)

---

## Compatibility

- **.NET 8.0, 9.0, 10.0** (Current support)
- **.NET 6.0, 7.0** (Coming in future releases)
- **.NET Standard 2.1** (Coming in Stable - covers .NET 5+)

---

## Related Packages

### RecurPixel.EasyMessages.AspNetCore
ASP.NET Core integration with DI, IOptions configuration, `.ToApiResponse()`, and logging integration.

```bash
dotnet add package RecurPixel.EasyMessages.AspNetCore --version 0.1.0-beta.*
```

📖 [ASP.NET Core Documentation](https://recurpixel.github.io/RecurPixel.EasyMessages/latest/ASP-NET-Core/Overview)

---

## Performance

EasyMessages is optimized for .NET 5-10:

- ✅ ~106ns per API response conversion (9.4M ops/sec on .NET 10)
- ✅ ~169ns for metadata operations
- ✅ 256B-1.5KB memory per operation
- ✅ Span&lt;T&gt;, ArrayPool&lt;T&gt;, ValueStringBuilder optimizations

📊 [Detailed Benchmarks](https://recurpixel.github.io/RecurPixel.EasyMessages/latest/Advanced-Topics/Performance-Considerations-and-Benchmarks)

---

## Support

- 📖 [Documentation](https://recurpixel.github.io/RecurPixel.EasyMessages/)
- 🐛 [Report Issues](https://github.com/RecurPixel/RecurPixel.EasyMessages/issues)
- 💡 [Request Features](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)
- 📧 [Contact](https://github.com/RecurPixel)

---

## License

MIT License - see [LICENSE](https://github.com/RecurPixel/RecurPixel.EasyMessages/blob/main/LICENSE) for details.

---

## Status: Beta Release

✅ This is a **beta release** (v0.1.0-beta.1). APIs are **stable** - no breaking changes planned before 1.0.

🎉 Ready for testing and feedback in real projects!

📢 [Give Feedback](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions) | [Report Issues](https://github.com/RecurPixel/RecurPixel.EasyMessages/issues)

---

Built with ❤️ by [RecurPixel](https://github.com/RecurPixel)

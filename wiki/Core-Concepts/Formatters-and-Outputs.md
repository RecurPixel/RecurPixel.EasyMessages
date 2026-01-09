# Formatters and Outputs

Learn how EasyMessages converts messages to different output formats (JSON, XML, Console, PlainText) and how to configure formatting options.

---

## What are Formatters?

**Formatters** are responsible for converting `Message` objects into specific output formats. Each formatter implements the `IMessageFormatter` interface and provides consistent, customizable output.

```csharp
var message = Msg.Auth.LoginFailed();

// Different output formats from the same message
var json = message.ToJson();           // JSON string
var xml = message.ToXml();             // XML string
var text = message.ToPlainText();      // Plain text
message.ToConsole(useColors: true);    // Colored console output
```

**Key Features:**
- ✅ **Multiple formats** - JSON, XML, Console, PlainText
- ✅ **Configurable output** - Control what fields are included
- ✅ **Extensible** - Create custom formatters
- ✅ **Thread-safe** - Safe for concurrent use
- ✅ **Performance optimized** - Minimal allocations

---

## Built-in Formatters

### 1. JSON Formatter

Converts messages to JSON format using System.Text.Json.

**Usage:**
```csharp
var message = Msg.Crud.Created("User")
    .WithData(new { Id = 123, Name = "John" });

var json = message.ToJson();
```

**Default Output:**
```json
{
  "code": "CRUD_001",
  "type": "success",
  "title": "Created Successfully",
  "description": "User has been created successfully.",
  "timestamp": "2026-01-09T14:30:00Z",
  "httpStatusCode": 200,
  "data": {
    "id": 123,
    "name": "John"
  }
}
```

**Features:**
- Uses camelCase for property names
- Excludes null fields by default
- Compact output (not indented)
- Follows RFC 8259 JSON standard

**Custom JSON Options:**
```csharp
var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true,              // Pretty print
    PropertyNamingPolicy = null,       // PascalCase
    DefaultIgnoreCondition = JsonIgnoreCondition.Never // Include nulls
};

var json = message.ToJson(jsonOptions);
```

**Pretty-Printed Output:**
```json
{
  "code": "CRUD_001",
  "type": "success",
  "title": "Created Successfully",
  "description": "User has been created successfully.",
  "timestamp": "2026-01-09T14:30:00Z",
  "httpStatusCode": 200,
  "correlationId": null,
  "hint": null,
  "metadata": {},
  "data": {
    "id": 123,
    "name": "John"
  }
}
```

**Common Use Cases:**
- ✅ API responses
- ✅ Logging to structured logs
- ✅ Storing messages in files
- ✅ Message queues
- ✅ Client-server communication

### 2. XML Formatter

Converts messages to XML format.

**Usage:**
```csharp
var message = Msg.Auth.LoginFailed()
    .WithMetadata("attempt", 3);

var xml = message.ToXml();
```

**Output:**
```xml
<Message>
  <Code>AUTH_001</Code>
  <Type>Error</Type>
  <Title>Authentication Failed</Title>
  <Description>Invalid username or password.</Description>
  <Timestamp>2026-01-09T14:30:00Z</Timestamp>
  <HttpStatusCode>401</HttpStatusCode>
  <Metadata>
    <Entry>
      <Key>attempt</Key>
      <Value>3</Value>
    </Entry>
  </Metadata>
</Message>
```

**Features:**
- Well-formed XML
- Supports nested data structures
- Includes XML declaration
- UTF-8 encoding

**Common Use Cases:**
- ✅ Legacy system integration
- ✅ SOAP services
- ✅ XML-based message queues
- ✅ Configuration files
- ✅ Reports and documents

### 3. PlainText Formatter

Converts messages to human-readable plain text.

**Usage:**
```csharp
var message = Msg.Crud.NotFound("User")
    .WithMetadata("userId", 123);

var text = message.ToPlainText();
```

**Output:**
```
[CRUD_004] Not Found
User was not found.
Timestamp: 2026-01-09T14:30:00Z
HTTP Status: 404

Metadata:
  userId: 123
```

**Features:**
- Human-readable format
- Multiline structure
- Clear section separation
- No special characters needed

**Common Use Cases:**
- ✅ Email notifications
- ✅ Text files/logs
- ✅ SMS messages
- ✅ Terminal output (non-colored)
- ✅ Debug output

### 4. Console Formatter

Converts messages to colored console output with symbols.

**Usage:**
```csharp
var message = Msg.Auth.LoginFailed();

// With colors
message.ToConsole(useColors: true);

// Without colors
message.ToConsole(useColors: false);
```

**Colored Output:**
```
✗ Authentication Failed
  Invalid username or password.
  [2026-01-09 14:30:00] [AUTH_001]
```

**Color Mapping:**
- **Success** → Green ✓
- **Info** → Blue ℹ
- **Warning** → Yellow ⚠
- **Error** → Red ✗
- **Critical** → Red ‼️

**Features:**
- ANSI color codes
- Type-specific symbols
- Compact format
- Optional color support

**Common Use Cases:**
- ✅ Console applications
- ✅ CLI tools
- ✅ Development/debugging
- ✅ Background workers
- ✅ Scripts

---

## Output Methods

### Extension Methods

Every message has these output methods:

```csharp
var message = Msg.Auth.LoginFailed();

// String outputs
string json = message.ToJson();
string xml = message.ToXml();
string text = message.ToPlainText();

// Console output (void)
message.ToConsole(useColors: true);

// Generic format
string custom = message.ToFormat("myformat");

// ASP.NET Core only
IActionResult result = message.ToApiResponse();
```

### ToJson()

```csharp
// Default JSON
var json = message.ToJson();

// Custom JSON options
var json = message.ToJson(new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
});
```

### ToXml()

```csharp
// Standard XML output
var xml = message.ToXml();

// XML is not customizable (uses XmlSerializer defaults)
```

### ToPlainText()

```csharp
// Plain text output
var text = message.ToPlainText();
```

### ToConsole()

```csharp
// With colors (default)
message.ToConsole();
message.ToConsole(useColors: true);

// Without colors
message.ToConsole(useColors: false);
```

### ToFormat()

```csharp
// Use registered custom formatter
FormatterRegistry.Register("csv", () => new CsvFormatter());

var csv = message.ToFormat("csv");
```

### ToApiResponse() (ASP.NET Core)

```csharp
// Convert to IActionResult
[HttpGet]
public IActionResult Get()
{
    return Msg.Crud.Retrieved("Users")
        .WithData(users)
        .ToApiResponse();
}
```

---

## Formatter Configuration

### FormatterOptions

Control what fields are included in formatted output:

```csharp
public class FormatterOptions
{
    public bool IncludeTimestamp { get; set; } = true;
    public bool IncludeCorrelationId { get; set; } = true;
    public bool IncludeHttpStatusCode { get; set; } = true;
    public bool IncludeMetadata { get; set; } = true;
    public bool IncludeData { get; set; } = true;
    public bool IncludeParameters { get; set; } = true;
    public bool IncludeHint { get; set; } = true;
    public bool IncludeNullFields { get; set; } = false;
}
```

### Global Configuration

Configure default formatting for all messages:

```csharp
using RecurPixel.EasyMessages.Configuration;

// Custom configuration
FormatterConfiguration.Configure(options =>
{
    options.IncludeTimestamp = false;
    options.IncludeHttpStatusCode = false;
    options.IncludeMetadata = true;
    options.IncludeData = true;
    options.IncludeNullFields = false;
});
```

### Built-in Presets

Use predefined configurations:

```csharp
// Minimal - Only essential fields
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Minimal);

// Verbose - All fields except nulls
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Verbose);

// Debug - Everything including nulls
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Debug);
```

**Minimal Preset:**
```csharp
// Includes only: code, type, title, description
{
  "code": "AUTH_001",
  "type": "error",
  "title": "Authentication Failed",
  "description": "Invalid username or password."
}
```

**Verbose Preset:**
```csharp
// Includes: all fields except nulls
{
  "code": "AUTH_001",
  "type": "error",
  "title": "Authentication Failed",
  "description": "Invalid username or password.",
  "timestamp": "2026-01-09T14:30:00Z",
  "httpStatusCode": 401,
  "metadata": {},
  "data": null  // Excluded because null
}
```

**Debug Preset:**
```csharp
// Includes: everything, including nulls
{
  "code": "AUTH_001",
  "type": "error",
  "title": "Authentication Failed",
  "description": "Invalid username or password.",
  "timestamp": "2026-01-09T14:30:00Z",
  "correlationId": null,
  "httpStatusCode": 401,
  "hint": null,
  "metadata": {},
  "data": null,
  "parameters": {}
}
```

---

## Output Comparison

### Same Message, Different Formats

**Message:**
```csharp
var message = Msg.Crud.Created("User")
    .WithData(new { Id = 123, Name = "John" })
    .WithMetadata("source", "API");
```

**JSON Output:**
```json
{
  "code": "CRUD_001",
  "type": "success",
  "title": "Created Successfully",
  "description": "User has been created successfully.",
  "timestamp": "2026-01-09T14:30:00Z",
  "httpStatusCode": 200,
  "metadata": {
    "source": "API"
  },
  "data": {
    "id": 123,
    "name": "John"
  }
}
```

**XML Output:**
```xml
<Message>
  <Code>CRUD_001</Code>
  <Type>Success</Type>
  <Title>Created Successfully</Title>
  <Description>User has been created successfully.</Description>
  <Timestamp>2026-01-09T14:30:00Z</Timestamp>
  <HttpStatusCode>200</HttpStatusCode>
  <Metadata>
    <Entry>
      <Key>source</Key>
      <Value>API</Value>
    </Entry>
  </Metadata>
  <Data>
    <Id>123</Id>
    <Name>John</Name>
  </Data>
</Message>
```

**PlainText Output:**
```
[CRUD_001] Created Successfully
User has been created successfully.
Timestamp: 2026-01-09T14:30:00Z
HTTP Status: 200

Metadata:
  source: API

Data:
  {
    "id": 123,
    "name": "John"
  }
```

**Console Output:**
```
✓ Created Successfully
  User has been created successfully.
  [2026-01-09 14:30:00] [CRUD_001]
```

---

## Custom Formatters

### Creating a Custom Formatter

Implement `IMessageFormatter` or extend `MessageFormatterBase`:

```csharp
using RecurPixel.EasyMessages.Formatters;

public class CsvFormatter : MessageFormatterBase
{
    protected override string FormatCore(Message message)
    {
        return $"{message.Code},{message.Type},{message.Title},{message.Description}";
    }

    protected override object FormatAsObject(Message message)
    {
        return new[]
        {
            message.Code,
            message.Type.ToString(),
            message.Title,
            message.Description
        };
    }
}
```

### Registering Custom Formatters

```csharp
// Register the formatter
FormatterRegistry.Register("csv", () => new CsvFormatter());

// Use it
var csv = message.ToFormat("csv");
```

### Example: Markdown Formatter

```csharp
public class MarkdownFormatter : MessageFormatterBase
{
    protected override string FormatCore(Message message)
    {
        var sb = new StringBuilder();

        // Header
        var symbol = message.Type switch
        {
            MessageType.Success => "✅",
            MessageType.Info => "ℹ️",
            MessageType.Warning => "⚠️",
            MessageType.Error => "❌",
            MessageType.Critical => "🔴",
            _ => "•"
        };

        sb.AppendLine($"# {symbol} {message.Title}");
        sb.AppendLine();
        sb.AppendLine($"> {message.Description}");
        sb.AppendLine();

        // Details
        sb.AppendLine("## Details");
        sb.AppendLine($"- **Code:** `{message.Code}`");
        sb.AppendLine($"- **Type:** {message.Type}");
        sb.AppendLine($"- **Timestamp:** {message.Timestamp:yyyy-MM-dd HH:mm:ss}");

        if (message.HttpStatusCode.HasValue)
            sb.AppendLine($"- **HTTP Status:** {message.HttpStatusCode}");

        // Metadata
        if (message.Metadata.Any())
        {
            sb.AppendLine();
            sb.AppendLine("## Metadata");
            foreach (var (key, value) in message.Metadata)
            {
                sb.AppendLine($"- **{key}:** {value}");
            }
        }

        // Data
        if (message.Data != null)
        {
            sb.AppendLine();
            sb.AppendLine("## Data");
            sb.AppendLine("```json");
            sb.AppendLine(JsonSerializer.Serialize(message.Data, new JsonSerializerOptions { WriteIndented = true }));
            sb.AppendLine("```");
        }

        return sb.ToString();
    }

    protected override object FormatAsObject(Message message)
    {
        return FormatCore(message);
    }
}

// Register and use
FormatterRegistry.Register("markdown", () => new MarkdownFormatter());
var md = message.ToFormat("markdown");
```

**Markdown Output:**
```markdown
# ✅ Created Successfully

> User has been created successfully.

## Details
- **Code:** `CRUD_001`
- **Type:** Success
- **Timestamp:** 2026-01-09 14:30:00
- **HTTP Status:** 200

## Metadata
- **source:** API

## Data
```json
{
  "id": 123,
  "name": "John"
}
```
```

---

## Format Selection Guide

| Scenario | Recommended Format | Why |
|----------|-------------------|-----|
| **API Responses** | JSON | Standard, widely supported, compact |
| **Logging** | JSON | Structured logging, searchable |
| **Console Apps** | Console | Colored, human-readable |
| **Email Notifications** | PlainText | Simple, no formatting required |
| **Legacy Systems** | XML | SOAP, enterprise integration |
| **File Storage** | JSON | Compact, easy to parse |
| **Message Queues** | JSON | Standard, platform-agnostic |
| **Reports** | Custom (PDF, HTML) | Create custom formatter |
| **SMS** | PlainText | Character-limited, simple |
| **Debugging** | Console or JSON (pretty) | Human-readable |

---

## Best Practices

### ✅ DO:

**1. Use appropriate format for context**
```csharp
// API - use JSON
return Msg.Crud.Created("User")
    .WithData(user)
    .ToApiResponse();

// Console - use Console formatter
Msg.System.Processing()
    .ToConsole(useColors: true);

// Logging - use JSON for structure
_logger.LogInformation(message.ToJson());
```

**2. Configure formatting globally at startup**
```csharp
// Program.cs or Startup
FormatterConfiguration.SetDefaultOptions(
    Environment.IsDevelopment()
        ? FormatterConfiguration.Debug
        : FormatterConfiguration.Minimal
);
```

**3. Use pretty JSON for debugging**
```csharp
#if DEBUG
var json = message.ToJson(new JsonSerializerOptions
{
    WriteIndented = true
});
Console.WriteLine(json);
#endif
```

**4. Include relevant fields based on environment**
```csharp
// Production - minimal
FormatterConfiguration.Configure(options =>
{
    options.IncludeTimestamp = true;
    options.IncludeMetadata = false;  // May contain sensitive data
    options.IncludeNullFields = false;
});

// Development - verbose
FormatterConfiguration.Configure(options =>
{
    options.IncludeTimestamp = true;
    options.IncludeMetadata = true;
    options.IncludeNullFields = true;  // Debug everything
});
```

**5. Register custom formatters at startup**
```csharp
// Application startup
FormatterRegistry.Register("csv", () => new CsvFormatter());
FormatterRegistry.Register("markdown", () => new MarkdownFormatter());
```

### ❌ DON'T:

**1. Don't mix formats inconsistently**
```csharp
// Bad - inconsistent API responses
public IActionResult Get1()
{
    return Ok(message.ToJson());  // Returns string
}

public IActionResult Get2()
{
    return message.ToApiResponse();  // Returns IActionResult
}

// Good - consistent
public IActionResult Get1()
{
    return message.ToApiResponse();
}

public IActionResult Get2()
{
    return message.ToApiResponse();
}
```

**2. Don't include sensitive data in production**
```csharp
// Bad
FormatterConfiguration.Configure(options =>
{
    options.IncludeMetadata = true;  // May leak sensitive data
});

Msg.Auth.LoginFailed()
    .WithMetadata("password", password)  // ❌ Security risk!
    .ToApiResponse();

// Good
FormatterConfiguration.Configure(options =>
{
    options.IncludeMetadata = Environment.IsDevelopment();
});

Msg.Auth.LoginFailed()
    .WithMetadata("username", username)
    .WithMetadata("ipAddress", ipAddress)
    .ToApiResponse();
```

**3. Don't forget to configure JSON options**
```csharp
// Bad - default options may not match your API
var json = message.ToJson();  // Uses internal defaults

// Good - explicit options
var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = false
};
var json = message.ToJson(jsonOptions);
```

**4. Don't use Console formatter for APIs**
```csharp
// Bad - console output in API
[HttpGet]
public IActionResult Get()
{
    var message = Msg.Crud.Retrieved("Users");
    message.ToConsole();  // ❌ Writes to console, not response
    return Ok();
}

// Good
[HttpGet]
public IActionResult Get()
{
    return Msg.Crud.Retrieved("Users")
        .WithData(users)
        .ToApiResponse();
}
```

**5. Don't create formatters in hot paths**
```csharp
// Bad - creates formatter on every request
public string GetJson()
{
    var formatter = new JsonFormatter();  // ❌ Allocation
    return formatter.Format(message);
}

// Good - use extension method (uses cached formatter)
public string GetJson()
{
    return message.ToJson();  // ✅ Efficient
}
```

---

## Performance Considerations

### Formatter Performance

```
Operation              | Time        | Allocations
-----------------------|-------------|-------------
ToJson()               | ~1-3 μs     | ~560 B
ToXml()                | ~5-10 μs    | ~2 KB
ToPlainText()          | ~2-5 μs     | ~800 B
ToConsole()            | ~3-7 μs     | ~1 KB
```

### Optimization Tips

1. **Cache formatters** - Don't create new instances unnecessarily
2. **Use extension methods** - Built-in caching
3. **Configure once** - Set FormatterOptions at startup
4. **Minimize included fields** - Fewer fields = faster
5. **Use Minimal preset in production** - Smallest output

---

## Troubleshooting

### JSON Output Missing Fields

**Problem:** Some fields are null in JSON output

**Solution:** Check formatter configuration
```csharp
// Include null fields
FormatterConfiguration.Configure(options =>
{
    options.IncludeNullFields = true;
});

// Or use Debug preset
FormatterConfiguration.SetDefaultOptions(FormatterConfiguration.Debug);
```

### Console Colors Not Working

**Problem:** Console output not showing colors

**Solution:** Disable colors or check terminal support
```csharp
// Disable colors
message.ToConsole(useColors: false);

// Or check if terminal supports ANSI
if (Console.IsOutputRedirected)
{
    message.ToConsole(useColors: false);
}
else
{
    message.ToConsole(useColors: true);
}
```

### Custom Formatter Not Found

**Problem:** `ToFormat("myformat")` throws exception

**Solution:** Register formatter first
```csharp
// Register before use
FormatterRegistry.Register("myformat", () => new MyFormatter());

// Then use
var output = message.ToFormat("myformat");
```

---

## Next Steps

- **[Interceptors](Interceptors.md)** - Modify messages before formatting
- **[Build Custom Formatters Guide](../How-To-Guides/Build-Custom-Formatters.md)** - Step-by-step tutorial
- **[Architecture Overview](Architecture-Overview.md)** - Understand the complete system

---

**Questions?** [Ask in Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

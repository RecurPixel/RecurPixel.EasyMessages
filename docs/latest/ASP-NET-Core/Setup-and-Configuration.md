# Setup and Configuration

Complete guide to setting up and configuring EasyMessages in ASP.NET Core applications.

---

## Table of Contents

1. [Installation](#installation)
2. [Basic Setup](#basic-setup)
3. [Configuration Methods](#configuration-methods)
4. [Configuration Options](#configuration-options)
5. [Environment-Specific Configuration](#environment-specific-configuration)
6. [Advanced Setup](#advanced-setup)
7. [Troubleshooting](#troubleshooting)

---

## Installation

### Package Installation

Install the ASP.NET Core package via NuGet:

```bash
# .NET CLI
dotnet add package RecurPixel.EasyMessages.AspNetCore --version 0.1.0-beta.1

# Package Manager Console
Install-Package RecurPixel.EasyMessages.AspNetCore -Version 0.1.0-beta.1
```

**Note:** The AspNetCore package includes the core package as a dependency, so you don't need to install both.

---

### Project File Reference

Your `.csproj` file should include:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="RecurPixel.EasyMessages.AspNetCore" Version="0.1.0-beta.1" />
  </ItemGroup>
</Project>
```

**Supported .NET Versions:** .NET 5.0 - 10.0

---

## Basic Setup

### Minimal Setup (Default Configuration)

**Program.cs:**
```csharp
using RecurPixel.EasyMessages.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add EasyMessages with default settings
builder.Services.AddEasyMessages(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();
```

**What this does:**
- [✓] Registers EasyMessages services in DI
- [✓] Enables correlation ID auto-assignment
- [✓] Loads configuration from appsettings.json (if present)
- [✓] Uses default formatter options
- [ ] Auto-logging disabled (opt-in)
- [ ] Metadata enrichment disabled (opt-in)

---

### Setup with Preset

**Program.cs:**
```csharp
using RecurPixel.EasyMessages.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Use Development preset for verbose logging
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Development
);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();
```

**Available Presets:**
- `EasyMessagesPresets.Development` - Verbose, all features enabled
- `EasyMessagesPresets.Production` - Minimal, performance-optimized
- `EasyMessagesPresets.Testing` - Deterministic, no auto-logging
- `EasyMessagesPresets.Api` - Client-friendly responses
- `EasyMessagesPresets.Staging` - Production-like with more logging

See [Configuration Presets](Configuration-Presets.md) for detailed preset configurations.

---

### Setup with Programmatic Configuration

**Program.cs:**
```csharp
using RecurPixel.EasyMessages.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    // Formatter options
    options.Formatter.IncludeTimestamp = false;
    options.Formatter.IncludeMetadata = true;
    options.Formatter.IncludeCorrelationId = true;

    // Interceptor options
    options.Interceptors.AutoAddCorrelationId = true;
    options.Interceptors.AutoEnrichMetadata = true;
    options.Interceptors.MetadataFields.IncludeRequestPath = true;
    options.Interceptors.MetadataFields.IncludeRequestMethod = true;
    options.Interceptors.MetadataFields.IncludeUserId = true;

    // Logging options
    options.Logging.AutoLog = true;
    options.Logging.MinimumLogLevel = LogLevel.Information;

    // Storage options
    options.Storage.CustomMessagesPath = "messages/custom.json";
});

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();
```

---

## Configuration Methods

EasyMessages supports three configuration methods that can be combined:

### 1. appsettings.json (Recommended)

**appsettings.json:**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": true,
      "IncludeCorrelationId": true,
      "IncludeMetadata": true,
      "IncludeData": true,
      "IncludeNullFields": false
    },
    "Interceptors": {
      "AutoAddCorrelationId": true,
      "AutoEnrichMetadata": true,
      "MetadataFields": {
        "IncludeRequestPath": true,
        "IncludeRequestMethod": true,
        "IncludeUserAgent": false,
        "IncludeIpAddress": false,
        "IncludeUserId": true,
        "IncludeUserName": true
      }
    },
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Information"
    },
    "Storage": {
      "CustomMessagesPath": "messages/custom.json"
    },
    "Localization": {
      "DefaultLocale": "en-US",
      "EnableLocalization": false,
      "FallbackToDefault": true
    }
  }
}
```

**Program.cs:**
```csharp
// Automatically loads from appsettings.json
builder.Services.AddEasyMessages(builder.Configuration);
```

**Benefits:**
- [✓] Environment-specific configuration (appsettings.Development.json, appsettings.Production.json)
- [✓] No code changes needed for different environments
- [✓] Supports configuration reloading (with IOptionsMonitor)
- [✓] Easy to version control

---

### 2. Programmatic Configuration

**Program.cs:**
```csharp
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    // Configure in code
    options.Formatter.IncludeTimestamp = false;
    options.Logging.AutoLog = true;
    options.Storage.CustomMessagesPath = "messages/custom.json";
});
```

**Benefits:**
- [✓] Type-safe configuration
- [✓] IntelliSense support
- [✓] Can use conditional logic
- [✓] Overrides appsettings.json values

---

### 3. Configuration Presets

**Program.cs:**
```csharp
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Development
);
```

**Benefits:**
- [✓] Quick setup for common scenarios
- [✓] Environment-appropriate defaults
- [✓] Can still override via appsettings.json

---

### Configuration Priority (Highest to Lowest)

When multiple configuration methods are used, settings are applied in this order:

```
1. Programmatic configuration (Action<EasyMessagesOptions>)  ← Highest priority
2. Preset configuration (EasyMessagesPresets)
3. appsettings.json configuration
4. Default values                                             ← Lowest priority
```

**Example:**
```csharp
// appsettings.json
{
  "EasyMessages": {
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Warning"
    }
  }
}

// Program.cs
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    // This OVERRIDES appsettings.json
    options.Logging.MinimumLogLevel = LogLevel.Information;
});

// Final result: AutoLog = true (from appsettings), MinimumLogLevel = Information (from code)
```

---

## Configuration Options

### EasyMessagesOptions

Root configuration class with five main sections:

```csharp
public class EasyMessagesOptions
{
    public FormatterOptions Formatter { get; set; }      // Output formatting
    public InterceptorOptions Interceptors { get; set; } // Auto-enrichment
    public LoggingOptions Logging { get; set; }          // Auto-logging
    public StorageOptions Storage { get; set; }          // Custom messages
    public LocalizationOptions Localization { get; set; } // Multi-language
}
```

---

### 1. FormatterOptions

Controls which fields are included in formatted output (JSON, XML, etc.).

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

**Configuration:**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": false,
      "IncludeMetadata": true,
      "IncludeNullFields": false
    }
  }
}
```

**Use Cases:**
- **Production:** Minimal fields (performance)
- **Development:** All fields (debugging)
- **API Clients:** Essential fields only
- **Logging:** Verbose fields

---

### 2. InterceptorOptions

Controls automatic message enrichment via built-in interceptors.

```csharp
public class InterceptorOptions
{
    public bool AutoAddCorrelationId { get; set; } = true;
    public bool AutoEnrichMetadata { get; set; } = false;
    public MetadataEnrichmentFields MetadataFields { get; set; } = new();
}

public class MetadataEnrichmentFields
{
    public bool IncludeRequestPath { get; set; } = true;
    public bool IncludeRequestMethod { get; set; } = true;
    public bool IncludeUserAgent { get; set; } = false;
    public bool IncludeIpAddress { get; set; } = false;
    public bool IncludeUserId { get; set; } = false;
    public bool IncludeUserName { get; set; } = false;
}
```

**Configuration:**
```json
{
  "EasyMessages": {
    "Interceptors": {
      "AutoAddCorrelationId": true,
      "AutoEnrichMetadata": true,
      "MetadataFields": {
        "IncludeRequestPath": true,
        "IncludeRequestMethod": true,
        "IncludeUserId": true,
        "IncludeUserName": true,
        "IncludeUserAgent": false,
        "IncludeIpAddress": false
      }
    }
  }
}
```

**Use Cases:**
- **CorrelationId:** Distributed tracing across microservices
- **Metadata:** Audit logging, security tracking, debugging

---

### 3. LoggingOptions

Controls automatic logging of all messages via ILogger.

```csharp
public class LoggingOptions
{
    public bool AutoLog { get; set; } = false;
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Warning;
}
```

**Configuration:**
```json
{
  "EasyMessages": {
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Information"
    }
  }
}
```

**Log Level Mapping:**

| MessageType | LogLevel |
|-------------|----------|
| Success | Information |
| Info | Information |
| Warning | Warning |
| Error | Error |
| Critical | Critical |

**Use Cases:**
- **Development:** AutoLog=true, MinimumLogLevel=Information
- **Production:** AutoLog=true, MinimumLogLevel=Warning
- **Testing:** AutoLog=false

---

### 4. StorageOptions

Configure custom message sources (JSON files, databases, etc.).

```csharp
public class StorageOptions
{
    public string? CustomMessagesPath { get; set; }
    public List<string>? CustomStorePaths { get; set; }
}
```

**Single Custom File:**
```json
{
  "EasyMessages": {
    "Storage": {
      "CustomMessagesPath": "messages/custom.json"
    }
  }
}
```

**Multiple Custom Files:**
```json
{
  "EasyMessages": {
    "Storage": {
      "CustomStorePaths": [
        "messages/auth.json",
        "messages/payment.json",
        "messages/validation.json"
      ]
    }
  }
}
```

**Loading Priority (last wins):**
1. Built-in messages (embedded)
2. CustomMessagesPath file
3. CustomStorePaths files (in array order)
4. Programmatically registered stores

**Custom Message File Format:**
```json
{
  "CUSTOM_001": {
    "type": "Success",
    "title": "Payment Successful",
    "description": "Payment of ${amount} was processed successfully.",
    "httpStatusCode": 200
  },
  "CUSTOM_002": {
    "type": "Error",
    "title": "Payment Failed",
    "description": "Payment failed: {reason}",
    "httpStatusCode": 402
  }
}
```

---

### 5. LocalizationOptions

Configure multi-language message support.

```csharp
public class LocalizationOptions
{
    public string DefaultLocale { get; set; } = "en-US";
    public bool EnableLocalization { get; set; } = false;
    public bool FallbackToDefault { get; set; } = true;
}
```

**Configuration:**
```json
{
  "EasyMessages": {
    "Localization": {
      "DefaultLocale": "en-US",
      "EnableLocalization": true,
      "FallbackToDefault": true
    }
  }
}
```

**Use Cases:**
- **Single Language:** EnableLocalization=false
- **Multi-Language:** EnableLocalization=true, organize messages by locale

---

## Environment-Specific Configuration

### Development Environment

**appsettings.Development.json:**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": true,
      "IncludeCorrelationId": true,
      "IncludeMetadata": true,
      "IncludeData": true,
      "IncludeParameters": true,
      "IncludeHint": true
    },
    "Interceptors": {
      "AutoAddCorrelationId": true,
      "AutoEnrichMetadata": true,
      "MetadataFields": {
        "IncludeRequestPath": true,
        "IncludeRequestMethod": true,
        "IncludeUserAgent": true,
        "IncludeIpAddress": true,
        "IncludeUserId": true,
        "IncludeUserName": true
      }
    },
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Debug"
    }
  }
}
```

**Characteristics:**
- [✓] Verbose output (all fields)
- [✓] All metadata enrichment
- [✓] Detailed logging (Debug level)

---

### Production Environment

**appsettings.Production.json:**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": false,
      "IncludeCorrelationId": true,
      "IncludeMetadata": false,
      "IncludeData": true,
      "IncludeNullFields": false
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

**Characteristics:**
- **Warning:** Minimal output (performance)
- [✓] Correlation ID only (for tracing)
- **Warning:** Log errors and warnings only

---

### Staging Environment

**appsettings.Staging.json:**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": true,
      "IncludeCorrelationId": true,
      "IncludeMetadata": true,
      "IncludeData": true
    },
    "Interceptors": {
      "AutoAddCorrelationId": true,
      "AutoEnrichMetadata": true,
      "MetadataFields": {
        "IncludeRequestPath": true,
        "IncludeRequestMethod": true,
        "IncludeUserId": true
      }
    },
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Information"
    }
  }
}
```

**Characteristics:**
- [✓] Production-like with more logging
- [✓] Essential metadata for debugging
- [✓] Information-level logging

---

### Testing Environment

**appsettings.Testing.json:**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": false,
      "IncludeCorrelationId": false,
      "IncludeMetadata": false,
      "IncludeData": true
    },
    "Interceptors": {
      "AutoAddCorrelationId": false,
      "AutoEnrichMetadata": false
    },
    "Logging": {
      "AutoLog": false
    }
  }
}
```

**Characteristics:**
- [✓] Deterministic output (no timestamps)
- [ ] No auto-enrichment (predictable)
- [ ] No logging (clean test output)

---

## Advanced Setup

### Custom Stores (Programmatic)

Register custom message stores programmatically:

```csharp
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    options.CustomStores = new List<IMessageStore>
    {
        new DatabaseMessageStore(connectionString),
        new RedisMessageStore(redisConnection)
    };
});
```

---

### Custom Formatters (Programmatic)

Register custom formatters:

```csharp
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    options.CustomFormatters = new Dictionary<string, Func<IMessageFormatter>>
    {
        ["csv"] = () => new CsvFormatter(),
        ["markdown"] = () => new MarkdownFormatter()
    };
});

// Usage
var csv = message.ToFormat("csv");
var markdown = message.ToFormat("markdown");
```

---

### Custom Interceptors (Programmatic)

Register custom interceptors:

```csharp
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    options.Interceptors = new List<IMessageInterceptor>
    {
        new AuditLoggingInterceptor(),
        new PerformanceTrackingInterceptor(),
        new SecurityClassificationInterceptor()
    };
});
```

---

### Combining Configuration Methods

**appsettings.json:**
```json
{
  "EasyMessages": {
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Warning"
    }
  }
}
```

**Program.cs:**
```csharp
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    // Override appsettings.json
    options.Logging.MinimumLogLevel = LogLevel.Information;

    // Add custom stores
    options.CustomStores = new List<IMessageStore>
    {
        new DatabaseMessageStore(connectionString)
    };

    // Add custom interceptors
    options.Interceptors = new List<IMessageInterceptor>
    {
        new AuditInterceptor()
    };
});
```

---

## Troubleshooting

### Issue: Configuration Not Loading from appsettings.json

**Symptoms:** Changes to appsettings.json have no effect

**Causes:**
1. Wrong section name (must be "EasyMessages")
2. appsettings.{Environment}.json not loaded
3. Programmatic configuration overriding values

**Solution:**
```csharp
// Verify configuration is loaded
var config = builder.Configuration.GetSection("EasyMessages");
var autoLog = config.GetValue<bool>("Logging:AutoLog");
Console.WriteLine($"AutoLog: {autoLog}");

// Ensure environment is set correctly
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
```

---

### Issue: Interceptors Not Executing

**Symptoms:** CorrelationId not added, metadata not enriched

**Causes:**
1. Interceptor disabled in configuration
2. IHttpContextAccessor not registered
3. Not in HTTP request context

**Solution:**
```json
{
  "EasyMessages": {
    "Interceptors": {
      "AutoAddCorrelationId": true,  // ← Ensure this is true
      "AutoEnrichMetadata": true
    }
  }
}
```

---

### Issue: Custom Messages Not Loading

**Symptoms:** Custom message file not loaded, still using defaults

**Causes:**
1. File path incorrect (relative to working directory)
2. File format invalid (JSON syntax error)
3. File loaded but message codes wrong

**Solution:**
```csharp
// Debug file loading
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    var path = "messages/custom.json";
    var fullPath = Path.GetFullPath(path);
    Console.WriteLine($"Loading messages from: {fullPath}");
    Console.WriteLine($"File exists: {File.Exists(fullPath)}");

    options.Storage.CustomMessagesPath = path;
});

// Verify loaded messages
var codes = MessageRegistry.GetAllCodes();
Console.WriteLine($"Loaded message codes: {string.Join(", ", codes)}");
```

---

### Issue: Validation Errors on Startup

**Symptoms:** Application crashes with validation error

**Causes:**
1. Invalid configuration values (e.g., invalid locale format)
2. File paths don't exist (when validation is enabled)
3. Required fields missing

**Solution:**
```json
{
  "EasyMessages": {
    "Localization": {
      "DefaultLocale": "en-US"  // ← Must match pattern: xx-XX
    }
  }
}
```

---

### Issue: Auto-Logging Not Working

**Symptoms:** Messages not appearing in logs

**Causes:**
1. AutoLog disabled
2. MinimumLogLevel too high
3. ASP.NET Core logging configuration filtering messages

**Solution:**
```json
{
  "EasyMessages": {
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Information"
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "EasyMessages": "Information"  // ← Ensure EasyMessages category is logged
    }
  }
}
```

---

## Next Steps

- **[Configuration Guide](Configuration-Guide.md)** - Comprehensive configuration reference
- **[Configuration Presets](Configuration-Presets.md)** - Pre-built environment configurations
- **[API Response Patterns](API-Response-Patterns.md)** - REST API response patterns
- **[Logging Integration](Logging-Integration.md)** - Advanced logging strategies

---

**Questions?** [Ask in Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

# EasyMessages Configuration Guide

This comprehensive guide covers all configuration options available in RecurPixel.EasyMessages library.

## 📋 Table of Contents

- [Quick Reference](#quick-reference)
- [Configuration Overview](#configuration-overview)
- [Options Classes](#options-classes)
- [Configuration Methods](#configuration-methods)
- [Environment-Specific Configuration](#environment-specific-configuration)
- [Configuration Presets](#configuration-presets)
- [Validation](#validation)
- [Examples](#examples)
- [Troubleshooting](#troubleshooting)

---

## Quick Reference

### Configuration Hierarchy

```
EasyMessagesOptions (Root)
├── FormatterOptions        - Message formatting behavior
├── InterceptorOptions      - Interceptor configuration
│   └── MetadataEnrichmentFields - Metadata enrichment settings
├── LoggingOptions         - Logging configuration
├── StorageOptions         - Message storage configuration
└── LocalizationOptions    - Localization settings
```

### Quick Configuration Table

| Configuration Section | Options Class | Purpose | When to Use | IOptions Type |
|----------------------|---------------|---------|-------------|---------------|
| `EasyMessages` | `EasyMessagesOptions` | Root configuration | Always | `IOptions<T>` |
| `EasyMessages:Formatter` | `FormatterOptions` | Control message output format | Customize JSON/API responses | `IOptionsSnapshot<T>` |
| `EasyMessages:Interceptor` | `InterceptorOptions` | Auto-enrichment behavior | Add correlation IDs, metadata | `IOptions<T>` |
| `EasyMessages:Logging` | `LoggingOptions` | Automatic logging | Enable message logging | `IOptions<T>` |
| `EasyMessages:Storage` | `StorageOptions` | Custom message sources | Load custom messages | `IOptions<T>` |
| `EasyMessages:Localization` | `LocalizationOptions` | Multi-language support | Internationalization | `IOptions<T>` |

---

## Configuration Overview

EasyMessages supports three configuration approaches:

### 1. **appsettings.json** (Recommended for Production)
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": true,
      "IncludeCorrelationId": true
    }
  }
}
```

### 2. **Programmatic Configuration**
```csharp
services.AddEasyMessages(configuration, options =>
{
    options.Formatter.IncludeTimestamp = true;
    options.Logging.AutoLog = true;
});
```

### 3. **Presets** (Quick Setup)
```csharp
services.AddEasyMessages(configuration, EasyMessagesPresets.Development);
services.AddEasyMessages(configuration, EasyMessagesPresets.Production);
services.AddEasyMessages(configuration, EasyMessagesPresets.Testing);
```

---

## Options Classes

### 1. EasyMessagesOptions (Root)

The root configuration class that contains all sub-configurations.

**Namespace:** `RecurPixel.EasyMessages.AspNetCore.Configuration`

**Properties:**
- `FormatterOptions Formatter` - Message formatting settings
- `InterceptorOptions Interceptor` - Interceptor behavior
- `LoggingOptions Logging` - Logging configuration
- `StorageOptions Storage` - Message storage settings
- `LocalizationOptions Localization` - Localization settings

**Example:**
```csharp
var options = new EasyMessagesOptions
{
    Formatter = new FormatterOptions { IncludeTimestamp = true },
    Logging = new LoggingOptions { AutoLog = true }
};
```

---

### 2. FormatterOptions

Controls what fields are included in formatted message output (JSON, XML, Console).

**Namespace:** `RecurPixel.EasyMessages.Formatters`

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IncludeTimestamp` | `bool` | `true` | Include timestamp in output |
| `IncludeCorrelationId` | `bool` | `true` | Include correlation ID for request tracking |
| `IncludeHttpStatusCode` | `bool` | `true` | Include HTTP status code |
| `IncludeMetadata` | `bool` | `true` | Include metadata dictionary |
| `IncludeData` | `bool` | `true` | Include data payload |
| `IncludeParameters` | `bool` | `true` | Include parameters used in message |
| `IncludeHint` | `bool` | `true` | Include hint/help text |
| `IncludeNullFields` | `bool` | `false` | Include fields with null values |

#### When to Customize

- **API Responses**: Use minimal fields for clean client responses
- **Logging**: Include more context (timestamp, correlation ID, metadata)
- **Debugging**: Include everything (`IncludeNullFields = true`)
- **Production**: Exclude sensitive data (set `IncludeData = false`)

#### Presets Available

```csharp
FormatterConfiguration.Minimal        // Only essential fields
FormatterConfiguration.Verbose        // Everything
FormatterConfiguration.ProductionSafe // No sensitive data
FormatterConfiguration.Debug          // Everything including nulls
FormatterConfiguration.ApiClient      // Clean API responses
FormatterConfiguration.Logging        // Optimized for logs
```

#### Example Configuration

**appsettings.json:**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": true,
      "IncludeCorrelationId": true,
      "IncludeHttpStatusCode": true,
      "IncludeMetadata": false,
      "IncludeData": true,
      "IncludeParameters": false,
      "IncludeHint": true,
      "IncludeNullFields": false
    }
  }
}
```

**Programmatic:**
```csharp
services.AddEasyMessages(configuration, options =>
{
    options.Formatter.IncludeTimestamp = true;
    options.Formatter.IncludeCorrelationId = true;
    options.Formatter.IncludeMetadata = false; // Don't expose internal metadata
});
```

---

### 3. InterceptorOptions

Controls automatic message enrichment through interceptors.

**Namespace:** `RecurPixel.EasyMessages.AspNetCore.Configuration`

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AutoAddCorrelationId` | `bool` | `true` | Automatically add correlation ID from HttpContext.TraceIdentifier |
| `AutoEnrichMetadata` | `bool` | `false` | Automatically enrich metadata with request information |
| `MetadataFields` | `MetadataEnrichmentFields` | `new()` | Configure which metadata fields to auto-enrich |

#### When to Use

- **AutoAddCorrelationId**: Always enable in web applications for request tracking
- **AutoEnrichMetadata**: Enable when you need detailed request context in messages
  - Useful for debugging
  - Useful for audit logging
  - Disable in production if sensitive data is a concern

#### Example Configuration

**appsettings.json:**
```json
{
  "EasyMessages": {
    "Interceptor": {
      "AutoAddCorrelationId": true,
      "AutoEnrichMetadata": true,
      "MetadataFields": {
        "IncludeRequestPath": true,
        "IncludeRequestMethod": true,
        "IncludeUserAgent": false,
        "IncludeIpAddress": false,
        "IncludeUserId": true,
        "IncludeUserName": false
      }
    }
  }
}
```

**Programmatic:**
```csharp
services.AddEasyMessages(configuration, options =>
{
    options.Interceptor.AutoAddCorrelationId = true;
    options.Interceptor.AutoEnrichMetadata = true;
    options.Interceptor.MetadataFields.IncludeRequestPath = true;
    options.Interceptor.MetadataFields.IncludeIpAddress = false;
});
```

---

### 4. MetadataEnrichmentFields

Fine-grained control over which request metadata to auto-enrich.

**Namespace:** `RecurPixel.EasyMessages.AspNetCore.Configuration`

#### Properties

| Property | Type | Default | Description | Privacy Concern |
|----------|------|---------|-------------|-----------------|
| `IncludeRequestPath` | `bool` | `true` | Request path (e.g., /api/users) | Low |
| `IncludeRequestMethod` | `bool` | `true` | HTTP method (GET, POST, etc.) | None |
| `IncludeUserAgent` | `bool` | `false` | User agent string | Medium |
| `IncludeIpAddress` | `bool` | `false` | Client IP address | High (GDPR) |
| `IncludeUserId` | `bool` | `false` | Authenticated user ID | High (GDPR) |
| `IncludeUserName` | `bool` | `false` | Authenticated username | High (GDPR) |

#### Privacy Considerations

⚠️ **GDPR/Privacy Warning**: Be cautious with:
- `IncludeIpAddress` - Personal data under GDPR
- `IncludeUserId` / `IncludeUserName` - Personal identifiable information
- `IncludeUserAgent` - Can be used for fingerprinting

**Recommendation**: Only enable these in internal logging, never in API responses.

#### Example Configuration

**Development (verbose):**
```json
{
  "EasyMessages": {
    "Interceptor": {
      "MetadataFields": {
        "IncludeRequestPath": true,
        "IncludeRequestMethod": true,
        "IncludeUserAgent": true,
        "IncludeIpAddress": true,
        "IncludeUserId": true,
        "IncludeUserName": true
      }
    }
  }
}
```

**Production (privacy-safe):**
```json
{
  "EasyMessages": {
    "Interceptor": {
      "MetadataFields": {
        "IncludeRequestPath": true,
        "IncludeRequestMethod": true,
        "IncludeUserAgent": false,
        "IncludeIpAddress": false,
        "IncludeUserId": false,
        "IncludeUserName": false
      }
    }
  }
}
```

---

### 5. LoggingOptions

Controls automatic logging of messages through ILogger.

**Namespace:** `RecurPixel.EasyMessages.AspNetCore.Configuration`

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AutoLog` | `bool` | `false` | Automatically log all messages |
| `MinimumLogLevel` | `LogLevel` | `Warning` | Minimum severity to log |

#### When to Use

- **Development**: `AutoLog = true` with `LogLevel.Debug` for verbose output
- **Production**: `AutoLog = true` with `LogLevel.Warning` to capture issues
- **Testing**: `AutoLog = false` to avoid log noise

#### Log Level Mapping

EasyMessages automatically maps MessageType to LogLevel:

| MessageType | Default LogLevel |
|-------------|------------------|
| `Success` | `Information` |
| `Info` | `Information` |
| `Warning` | `Warning` |
| `Error` | `Error` |

Only messages at or above `MinimumLogLevel` are logged.

#### Example Configuration

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

**Programmatic:**
```csharp
services.AddEasyMessages(configuration, options =>
{
    options.Logging.AutoLog = true;
    options.Logging.MinimumLogLevel = LogLevel.Warning;
});
```

**Output Example:**
```
[2026-01-09 14:30:00] [Warning] AUTH_001: Authentication Failed
  Invalid username or password.
  CorrelationId: abc-123-def
```

---

### 6. StorageOptions

Configure where EasyMessages loads custom messages from.

**Namespace:** `RecurPixel.EasyMessages.AspNetCore.Configuration`

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `CustomMessagesPath` | `string?` | `null` | Path to custom messages JSON file |
| `CustomStorePaths` | `List<string>?` | `null` | Multiple custom message files |

#### When to Use

- Load custom messages from JSON files
- Override built-in messages
- Load domain-specific messages
- Multi-tenant message customization

#### Message Priority

When multiple stores are configured, messages are loaded in order with **last wins**:

1. Built-in messages (lowest priority)
2. `CustomMessagesPath` file
3. `CustomStorePaths` files (in order)
4. Programmatically registered stores (highest priority)

#### Example Configuration

**appsettings.json:**
```json
{
  "EasyMessages": {
    "Storage": {
      "CustomMessagesPath": "messages/custom.json",
      "CustomStorePaths": [
        "messages/auth.json",
        "messages/payment.json",
        "messages/domain-specific.json"
      ]
    }
  }
}
```

**Programmatic:**
```csharp
services.AddEasyMessages(configuration, options =>
{
    options.Storage.CustomMessagesPath = "messages/custom.json";
    options.Storage.CustomStorePaths = new List<string>
    {
        "messages/auth.json",
        "messages/payment.json"
    };
});
```

**Custom Message File Format:**
```json
{
  "PAYMENT_001": {
    "type": "Success",
    "title": "Payment Processed",
    "description": "Payment of {amount} was successful.",
    "httpStatusCode": 200
  },
  "PAYMENT_002": {
    "type": "Error",
    "title": "Payment Failed",
    "description": "Unable to process payment. Reason: {reason}",
    "httpStatusCode": 400
  }
}
```

---

### 7. LocalizationOptions

Configure multi-language support for messages.

**Namespace:** `RecurPixel.EasyMessages.AspNetCore.Configuration`

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DefaultLocale` | `string` | `"en-US"` | Default locale code |
| `EnableLocalization` | `bool` | `false` | Enable localization feature |
| `FallbackToDefault` | `bool` | `true` | Fallback to default locale if translation missing |

#### Locale Format

Use standard locale codes: `{language}-{COUNTRY}`
- `en-US` - English (United States)
- `en-GB` - English (United Kingdom)
- `es-ES` - Spanish (Spain)
- `fr-FR` - French (France)
- `de-DE` - German (Germany)

#### Example Configuration

**appsettings.json:**
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

**Localized Message Files:**
```
messages/
├── en-US/
│   └── messages.json
├── es-ES/
│   └── messages.json
└── fr-FR/
    └── messages.json
```

---

## Configuration Methods

### Method 1: appsettings.json (Recommended)

**Best for:** Production applications, environment-specific settings

```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": true,
      "IncludeCorrelationId": true
    },
    "Interceptor": {
      "AutoAddCorrelationId": true
    },
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Warning"
    }
  }
}
```

**Startup.cs / Program.cs:**
```csharp
services.AddEasyMessages(configuration);
```

### Method 2: Programmatic Configuration

**Best for:** Dynamic configuration, testing

```csharp
services.AddEasyMessages(configuration, options =>
{
    options.Formatter.IncludeTimestamp = true;
    options.Formatter.IncludeCorrelationId = true;
    options.Logging.AutoLog = true;
    options.Interceptor.AutoAddCorrelationId = true;
});
```

### Method 3: Hybrid (Recommended for Flexibility)

**Best for:** Base configuration in appsettings.json, overrides in code

```csharp
services.AddEasyMessages(configuration, options =>
{
    // appsettings.json provides base config
    // Override specific settings programmatically
    if (env.IsDevelopment())
    {
        options.Formatter = FormatterConfiguration.Debug;
        options.Logging.MinimumLogLevel = LogLevel.Debug;
    }
});
```

### Method 4: Using Presets

**Best for:** Quick setup, common scenarios

```csharp
// Development
services.AddEasyMessages(configuration, EasyMessagesPresets.Development);

// Production
services.AddEasyMessages(configuration, EasyMessagesPresets.Production);

// Testing
services.AddEasyMessages(configuration, EasyMessagesPresets.Testing);
```

---

## Environment-Specific Configuration

### Using appsettings.{Environment}.json

**appsettings.Development.json:**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": true,
      "IncludeCorrelationId": true,
      "IncludeMetadata": true,
      "IncludeData": true
    },
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Debug"
    },
    "Interceptor": {
      "AutoEnrichMetadata": true,
      "MetadataFields": {
        "IncludeRequestPath": true,
        "IncludeUserAgent": true,
        "IncludeIpAddress": true
      }
    }
  }
}
```

**appsettings.Production.json:**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": true,
      "IncludeCorrelationId": true,
      "IncludeMetadata": false,
      "IncludeData": false
    },
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Warning"
    },
    "Interceptor": {
      "AutoEnrichMetadata": false
    }
  }
}
```

### Conditional Configuration in Code

```csharp
services.AddEasyMessages(configuration, options =>
{
    if (env.IsDevelopment())
    {
        options.Formatter = FormatterConfiguration.Debug;
        options.Logging.AutoLog = true;
        options.Logging.MinimumLogLevel = LogLevel.Debug;
        options.Interceptor.AutoEnrichMetadata = true;
    }
    else if (env.IsProduction())
    {
        options.Formatter = FormatterConfiguration.ProductionSafe;
        options.Logging.AutoLog = true;
        options.Logging.MinimumLogLevel = LogLevel.Warning;
        options.Interceptor.AutoEnrichMetadata = false;
    }
    else if (env.IsStaging())
    {
        options.Formatter = FormatterConfiguration.Verbose;
        options.Logging.AutoLog = true;
        options.Logging.MinimumLogLevel = LogLevel.Information;
    }
});
```

---

## Configuration Presets

### EasyMessagesPresets.Development

**Purpose:** Verbose logging and debugging

**Configuration:**
- Formatter: `Debug` (include everything, even nulls)
- Logging: `AutoLog = true`, `MinimumLogLevel = Debug`
- Interceptor: `AutoAddCorrelationId = true`, `AutoEnrichMetadata = true`

**When to use:** Local development, troubleshooting

```csharp
services.AddEasyMessages(configuration, EasyMessagesPresets.Development);
```

### EasyMessagesPresets.Production

**Purpose:** Privacy-safe, optimized for production

**Configuration:**
- Formatter: `ProductionSafe` (no sensitive data)
- Logging: `AutoLog = true`, `MinimumLogLevel = Warning`
- Interceptor: `AutoAddCorrelationId = true`, `AutoEnrichMetadata = false`

**When to use:** Production deployments

```csharp
services.AddEasyMessages(configuration, EasyMessagesPresets.Production);
```

### EasyMessagesPresets.Testing

**Purpose:** Minimal output, no side effects

**Configuration:**
- Formatter: `Minimal` (only essential fields)
- Logging: `AutoLog = false`
- Interceptor: `AutoAddCorrelationId = false`, `AutoEnrichMetadata = false`

**When to use:** Unit tests, integration tests

```csharp
services.AddEasyMessages(configuration, EasyMessagesPresets.Testing);
```

### EasyMessagesPresets.Staging

**Purpose:** Balance between debugging and production

**Configuration:**
- Formatter: `Verbose` (include most fields)
- Logging: `AutoLog = true`, `MinimumLogLevel = Information`
- Interceptor: `AutoAddCorrelationId = true`, `AutoEnrichMetadata = true`

**When to use:** Staging environment, pre-production testing

```csharp
services.AddEasyMessages(configuration, EasyMessagesPresets.Staging);
```

---

## Validation

EasyMessages validates configuration on startup to prevent runtime errors.

### Validation Rules

1. **CustomMessagesPath**: Must point to existing file if specified
2. **CustomStorePaths**: All paths must point to existing files
3. **DefaultLocale**: Must match format `xx-XX` (e.g., en-US)
4. **MinimumLogLevel**: Must be valid LogLevel enum value

### Validation Errors

If validation fails, application startup will fail with descriptive error:

```
Unhandled exception. Microsoft.Extensions.Options.OptionsValidationException:
Invalid EasyMessages configuration: Custom messages file not found: messages/missing.json
```

### Custom Validation

Add custom validation by implementing `IValidateOptions<EasyMessagesOptions>`:

```csharp
public class EasyMessagesOptionsValidator : IValidateOptions<EasyMessagesOptions>
{
    public ValidateOptionsResult Validate(string name, EasyMessagesOptions options)
    {
        var errors = new List<string>();

        // Custom validation logic
        if (options.Storage.CustomMessagesPath != null
            && !File.Exists(options.Storage.CustomMessagesPath))
        {
            errors.Add($"Custom messages file not found: {options.Storage.CustomMessagesPath}");
        }

        if (errors.Any())
        {
            return ValidateOptionsResult.Fail(errors);
        }

        return ValidateOptionsResult.Success;
    }
}

// Register validator
services.AddSingleton<IValidateOptions<EasyMessagesOptions>, EasyMessagesOptionsValidator>();
```

---

## Examples

### Example 1: Development Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Use preset
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Development);

var app = builder.Build();
```

### Example 2: Production with Custom Messages

**appsettings.Production.json:**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": true,
      "IncludeCorrelationId": true,
      "IncludeMetadata": false,
      "IncludeData": false
    },
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Warning"
    },
    "Storage": {
      "CustomMessagesPath": "messages/custom.json"
    }
  }
}
```

```csharp
// Program.cs
builder.Services.AddEasyMessages(builder.Configuration);
```

### Example 3: Fine-Grained Control

```csharp
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    // Formatter
    options.Formatter.IncludeTimestamp = true;
    options.Formatter.IncludeCorrelationId = true;
    options.Formatter.IncludeMetadata = false;
    options.Formatter.IncludeData = true;

    // Logging
    options.Logging.AutoLog = true;
    options.Logging.MinimumLogLevel = LogLevel.Warning;

    // Interceptor
    options.Interceptor.AutoAddCorrelationId = true;
    options.Interceptor.AutoEnrichMetadata = false;

    // Storage
    options.Storage.CustomMessagesPath = "messages/custom.json";
});
```

### Example 4: Using IOptions in Your Code

```csharp
public class MyService
{
    private readonly EasyMessagesOptions _options;

    public MyService(IOptions<EasyMessagesOptions> options)
    {
        _options = options.Value;
    }

    public void DoSomething()
    {
        if (_options.Logging.AutoLog)
        {
            // Logging is enabled
        }
    }
}
```

### Example 5: Hot Reload with IOptionsMonitor

```csharp
public class MyService
{
    private readonly IOptionsMonitor<EasyMessagesOptions> _optionsMonitor;

    public MyService(IOptionsMonitor<EasyMessagesOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;

        // React to configuration changes
        _optionsMonitor.OnChange(options =>
        {
            Console.WriteLine("Configuration changed!");
        });
    }

    public void DoSomething()
    {
        // Always gets current configuration
        var currentOptions = _optionsMonitor.CurrentValue;
    }
}
```

---

## Troubleshooting

### Issue: Configuration not loading from appsettings.json

**Solution:**
```csharp
// Ensure you pass IConfiguration to AddEasyMessages
builder.Services.AddEasyMessages(builder.Configuration);
```

### Issue: Validation errors on startup

**Solution:** Check that all file paths exist and locale format is correct:
```json
{
  "EasyMessages": {
    "Storage": {
      "CustomMessagesPath": "messages/custom.json"  // Verify this file exists
    },
    "Localization": {
      "DefaultLocale": "en-US"  // Must be format: xx-XX
    }
  }
}
```

### Issue: Changes in appsettings.json not reflected

**Solutions:**
1. Use `IOptionsMonitor<T>` for hot reload
2. Ensure `reloadOnChange: true` in configuration builder:
```csharp
builder.Configuration.AddJsonFile("appsettings.json",
    optional: false,
    reloadOnChange: true);
```

### Issue: Custom messages not loading

**Solution:** Check message priority and ensure paths are correct:
```csharp
services.AddEasyMessages(configuration, options =>
{
    // Use absolute path or verify relative path
    var basePath = Directory.GetCurrentDirectory();
    options.Storage.CustomMessagesPath = Path.Combine(basePath, "messages", "custom.json");
});
```

### Issue: Too much/too little information in messages

**Solution:** Use appropriate preset or customize formatter:
```csharp
// Too much info? Use Minimal
options.Formatter = FormatterConfiguration.Minimal;

// Too little info? Use Verbose
options.Formatter = FormatterConfiguration.Verbose;

// Production? Use ProductionSafe
options.Formatter = FormatterConfiguration.ProductionSafe;
```

---

## Best Practices

1. **Use appsettings.json for base configuration**
   - Easier to change without recompilation
   - Environment-specific overrides via appsettings.{Environment}.json

2. **Use presets when possible**
   - `Development`, `Production`, `Testing` cover most scenarios
   - Override specific settings as needed

3. **Validate configuration on startup**
   - Catch configuration errors early
   - Use `ValidateOnStart()` in AddEasyMessages

4. **Be mindful of sensitive data**
   - Use `ProductionSafe` preset in production
   - Disable `IncludeData` and `IncludeMetadata` for API responses
   - Don't log PII (IP addresses, user IDs) in production

5. **Use IOptionsMonitor for runtime changes**
   - When configuration can change without restart
   - For multi-tenant scenarios

6. **Document custom messages**
   - Keep a message catalog
   - Use consistent naming conventions (e.g., DOMAIN_XXX)

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2026-01-09 | Initial configuration guide with IOptions pattern |

---

## See Also

- [Performance Optimizations](PERFORMANCE_OPTIMIZATIONS.md)
- [Architecture Documentation](ARCHITECTURE-FULL.md)
- [API Documentation](../README.md)

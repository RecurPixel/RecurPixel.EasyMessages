# Configuration Migration Guide

## Migrating from MessageConfiguration to EasyMessagesOptions

This guide helps you migrate from the legacy `MessageConfiguration` to the new `EasyMessagesOptions` with IOptions pattern.

---

## Why Migrate?

The new IOptions pattern provides:

[✓] **Configuration from appsettings.json** - No hardcoded settings
[✓] **Validation on startup** - Catch errors early
[✓] **Hot reload support** - Update config without restart
[✓] **Environment-specific config** - Different settings per environment
[✓] **Type safety** - Compile-time checking
[✓] **Better testability** - Easy to mock and test

---

## Migration Steps

### Step 1: Update AddEasyMessages Call

**Before (Legacy):**
```csharp
// Program.cs or Startup.cs
services.AddEasyMessages(options =>
{
    options.FormatterOptions.IncludeTimestamp = true;
    options.AutoLog = true;
    options.MinimumLogLevel = LogLevel.Warning;
});
```

**After (New Pattern):**
```csharp
// Program.cs or Startup.cs
services.AddEasyMessages(configuration, options =>
{
    options.Formatter.IncludeTimestamp = true;
    options.Logging.AutoLog = true;
    options.Logging.MinimumLogLevel = LogLevel.Warning;
});
```

**Key Changes:**
1. Pass `configuration` as first parameter
2. `FormatterOptions` → `Formatter`
3. `AutoLog` → `Logging.AutoLog`
4. `MinimumLogLevel` → `Logging.MinimumLogLevel`
5. `CustomMessagesPath` → `Storage.CustomMessagesPath`
6. `DefaultLocale` → `Localization.DefaultLocale`

---

### Step 2: Move to appsettings.json (Recommended)

Instead of programmatic configuration, use appsettings.json:

**appsettings.json:**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": true,
      "IncludeCorrelationId": true,
      "IncludeHttpStatusCode": true,
      "IncludeMetadata": false,
      "IncludeData": true
    },
    "Interceptor": {
      "AutoAddCorrelationId": true,
      "AutoEnrichMetadata": false
    },
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Warning"
    },
    "Storage": {
      "CustomMessagesPath": "messages/custom.json"
    },
    "Localization": {
      "DefaultLocale": "en-US"
    }
  }
}
```

**Program.cs:**
```csharp
// Simple - uses appsettings.json
services.AddEasyMessages(configuration);

// Or with overrides
services.AddEasyMessages(configuration, options =>
{
    // Override specific settings
    options.Logging.MinimumLogLevel = LogLevel.Debug;
});
```

---

### Step 3: Use Environment-Specific Configuration

**appsettings.Development.json:**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": true,
      "IncludeMetadata": true,
      "IncludeData": true,
      "IncludeNullFields": true
    },
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Debug"
    },
    "Interceptor": {
      "AutoEnrichMetadata": true
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
      "IncludeMetadata": false,
      "IncludeData": false,
      "IncludeNullFields": false
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

---

### Step 4: Or Use Presets (Quick Migration)

**Easiest migration path:**

```csharp
// Development
services.AddEasyMessages(configuration, EasyMessagesPresets.Development);

// Production
services.AddEasyMessages(configuration, EasyMessagesPresets.Production);

// Staging
services.AddEasyMessages(configuration, EasyMessagesPresets.Staging);

// Testing
services.AddEasyMessages(configuration, EasyMessagesPresets.Testing);
```

---

## Complete Property Mapping

| Legacy (MessageConfiguration) | New (EasyMessagesOptions) | Notes |
|------------------------------|---------------------------|-------|
| `FormatterOptions` | `Formatter` | Same type, just renamed |
| `InterceptorOptions` | `Interceptor` | Same type, just renamed |
| `AutoLog` | `Logging.AutoLog` | Moved to LoggingOptions |
| `MinimumLogLevel` | `Logging.MinimumLogLevel` | Moved to LoggingOptions |
| `CustomMessagesPath` | `Storage.CustomMessagesPath` | Moved to StorageOptions |
| `CustomStores` | `CustomStores` | Same property, same level |
| `CustomFormatters` | `CustomFormatters` | Same property, same level |
| `Interceptors` | `Interceptors` | Same property, same level |
| `DefaultLocale` | `Localization.DefaultLocale` | Moved to LocalizationOptions |
| - | `Storage.CustomStorePaths` | **NEW** - Multiple custom files |
| - | `Localization.EnableLocalization` | **NEW** - Enable/disable localization |
| - | `Localization.FallbackToDefault` | **NEW** - Fallback behavior |

---

## Migration Examples

### Example 1: Simple Configuration

**Before:**
```csharp
services.AddEasyMessages(options =>
{
    options.AutoLog = true;
});
```

**After:**
```csharp
services.AddEasyMessages(configuration, options =>
{
    options.Logging.AutoLog = true;
});
```

---

### Example 2: Custom Messages

**Before:**
```csharp
services.AddEasyMessages(options =>
{
    options.CustomMessagesPath = "messages/custom.json";
});
```

**After (Programmatic):**
```csharp
services.AddEasyMessages(configuration, options =>
{
    options.Storage.CustomMessagesPath = "messages/custom.json";
});
```

**After (appsettings.json - Recommended):**
```json
{
  "EasyMessages": {
    "Storage": {
      "CustomMessagesPath": "messages/custom.json"
    }
  }
}
```

---

### Example 3: Full Configuration

**Before:**
```csharp
services.AddEasyMessages(options =>
{
    options.FormatterOptions.IncludeTimestamp = true;
    options.FormatterOptions.IncludeCorrelationId = true;
    options.FormatterOptions.IncludeMetadata = false;

    options.InterceptorOptions.AutoAddCorrelationId = true;
    options.InterceptorOptions.AutoEnrichMetadata = false;

    options.AutoLog = true;
    options.MinimumLogLevel = LogLevel.Warning;

    options.CustomMessagesPath = "messages/custom.json";
    options.DefaultLocale = "en-US";
});
```

**After:**
```csharp
services.AddEasyMessages(configuration, options =>
{
    options.Formatter.IncludeTimestamp = true;
    options.Formatter.IncludeCorrelationId = true;
    options.Formatter.IncludeMetadata = false;

    options.Interceptor.AutoAddCorrelationId = true;
    options.Interceptor.AutoEnrichMetadata = false;

    options.Logging.AutoLog = true;
    options.Logging.MinimumLogLevel = LogLevel.Warning;

    options.Storage.CustomMessagesPath = "messages/custom.json";
    options.Localization.DefaultLocale = "en-US";
});
```

**Or use appsettings.json (Much cleaner):**
```json
{
  "EasyMessages": {
    "Formatter": {
      "IncludeTimestamp": true,
      "IncludeCorrelationId": true,
      "IncludeMetadata": false
    },
    "Interceptor": {
      "AutoAddCorrelationId": true,
      "AutoEnrichMetadata": false
    },
    "Logging": {
      "AutoLog": true,
      "MinimumLogLevel": "Warning"
    },
    "Storage": {
      "CustomMessagesPath": "messages/custom.json"
    },
    "Localization": {
      "DefaultLocale": "en-US"
    }
  }
}
```

```csharp
// Simple!
services.AddEasyMessages(configuration);
```

---

## Handling Deprecation Warnings

The old `MessageConfiguration` is marked with `[Obsolete]`. You'll see warnings like:

```
Warning CS0618: 'MessageConfiguration' is obsolete:
'Use EasyMessagesOptions with IOptions pattern instead.
This class will be removed in version 2.0.0.'
```

**To fix:** Migrate to the new pattern as shown above.

**Temporary suppression (not recommended):**
```csharp
#pragma warning disable CS0618
services.AddEasyMessages(options => { ... });
#pragma warning restore CS0618
```

---

## Testing Migration

### Old Pattern (Legacy):
```csharp
[Test]
public void TestMessageConfiguration()
{
    var services = new ServiceCollection();
    services.AddEasyMessages(options =>
    {
        options.AutoLog = false;
    });

    var provider = services.BuildServiceProvider();
    var config = provider.GetRequiredService<MessageConfiguration>();

    Assert.False(config.AutoLog);
}
```

### New Pattern:
```csharp
[Test]
public void TestEasyMessagesOptions()
{
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            ["EasyMessages:Logging:AutoLog"] = "false"
        })
        .Build();

    var services = new ServiceCollection();
    services.AddEasyMessages(configuration);

    var provider = services.BuildServiceProvider();
    var options = provider.GetRequiredService<IOptions<EasyMessagesOptions>>();

    Assert.False(options.Value.Logging.AutoLog);
}
```

---

## New Features Available After Migration

### 1. Hot Reload
```csharp
public class MyService
{
    private readonly IOptionsMonitor<EasyMessagesOptions> _options;

    public MyService(IOptionsMonitor<EasyMessagesOptions> options)
    {
        _options = options;

        // React to changes
        _options.OnChange(newOptions =>
        {
            Console.WriteLine("Config changed!");
        });
    }

    public void DoWork()
    {
        // Always gets current config
        var current = _options.CurrentValue;
    }
}
```

### 2. Validation
Configuration is validated on startup. Invalid config fails fast with clear errors:

```
Unhandled exception. Microsoft.Extensions.Options.OptionsValidationException:
Invalid EasyMessages configuration:
- Storage.CustomMessagesPath: File not found at 'messages/missing.json'
- Localization.DefaultLocale: Invalid format 'en'. Expected format: xx-XX (e.g., en-US)
```

### 3. Multiple Custom Message Files (NEW)
```json
{
  "EasyMessages": {
    "Storage": {
      "CustomStorePaths": [
        "messages/auth.json",
        "messages/payment.json",
        "messages/domain.json"
      ]
    }
  }
}
```

### 4. Environment-Based Auto-Configuration (NEW)
```csharp
// Automatically selects appropriate preset
var options = EasyMessagesPresets.ForEnvironment(env.EnvironmentName);
services.AddEasyMessages(configuration, options);
```

---

## Troubleshooting

### Issue: "IConfiguration is not available"

**Solution:** Ensure you're passing `builder.Configuration`:
```csharp
var builder = WebApplication.CreateBuilder(args);
services.AddEasyMessages(builder.Configuration); // [✓] Correct
```

### Issue: "Configuration not loading from appsettings.json"

**Solution:** Check section name is exactly `"EasyMessages"`:
```json
{
  "EasyMessages": {  // [✓] Correct - matches EasyMessagesOptions.SectionName
    // ...
  }
}
```

### Issue: "Options validation failed"

**Solution:** Check file paths and locale format:
```json
{
  "EasyMessages": {
    "Storage": {
      "CustomMessagesPath": "messages/custom.json"  // File must exist
    },
    "Localization": {
      "DefaultLocale": "en-US"  // Must be xx-XX format
    }
  }
}
```

---

## Timeline

| Version | Status | Notes |
|---------|--------|-------|
| **v0.1.x-alpha** | Legacy pattern works | New pattern available |
| **v0.2.x-beta** | Deprecation warnings | Both patterns work |
| **v1.0.0** | Deprecation warnings | Both patterns work |
| **v2.0.0** | Legacy removed | Only new pattern |

---

## Need Help?

- [Configuration Guide](CONFIGURATION_GUIDE.md) - Full documentation
- [GitHub Issues](https://github.com/RecurPixel/RecurPixel.EasyMessages/issues)
- [GitHub Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

---

## Quick Reference

### Minimal Migration
```csharp
// Before
services.AddEasyMessages(options => { ... });

// After
services.AddEasyMessages(configuration, options => { ... });
```

### Recommended Migration
```csharp
// Move configuration to appsettings.json
// Then use:
services.AddEasyMessages(configuration);
```

### Fastest Migration (Use Presets)
```csharp
services.AddEasyMessages(configuration, EasyMessagesPresets.Production);
```

---

That's it! You're now using the modern IOptions pattern with all its benefits. 🎉

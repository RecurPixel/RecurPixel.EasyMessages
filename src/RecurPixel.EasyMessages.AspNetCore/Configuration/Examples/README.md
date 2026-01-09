# EasyMessages appsettings.json Examples

This directory contains example configuration files for different scenarios.

## Available Examples

### 1. appsettings.Development.json
**Use Case:** Local development and debugging

**Features:**
- All fields included (verbose output)
- Debug-level logging
- Full metadata enrichment
- Include null fields for inspection

**Copy to your project:**
```bash
# Copy to your appsettings.Development.json
```

---

### 2. appsettings.Production.json
**Use Case:** Production deployments

**Features:**
- Privacy-safe (no sensitive data)
- Warning-level logging only
- No metadata enrichment
- Correlation ID for tracing
- Custom messages support

**Copy to your project:**
```bash
# Copy to your appsettings.Production.json
```

---

### 3. appsettings.Staging.json
**Use Case:** Staging/QA environment

**Features:**
- Balanced verbosity
- Information-level logging
- Basic metadata (path, method only)
- Good for pre-production testing

**Copy to your project:**
```bash
# Copy to your appsettings.Staging.json
```

---

### 4. appsettings.Minimal.json
**Use Case:** Testing or minimal API responses

**Features:**
- Only essential fields
- No logging
- No interceptors
- Clean, minimal output

**Copy to your project:**
```bash
# Use in test projects or minimal APIs
```

---

### 5. appsettings.CustomStores.json
**Use Case:** Loading custom messages from multiple files

**Features:**
- Multiple custom message files
- Organized by domain (auth, payment, etc.)
- Message priority: last file wins

**Directory structure:**
```
messages/
├── custom.json          # General custom messages
├── auth.json            # Authentication messages
├── payment.json         # Payment-related messages
└── domain-specific.json # Your domain messages
```

---

### 6. appsettings.Localized.json
**Use Case:** Multi-language support

**Features:**
- Localization enabled
- Fallback to default locale
- Support for multiple languages

**Directory structure:**
```
messages/
├── en-US/
│   └── messages.json
├── es-ES/
│   └── messages.json
├── fr-FR/
│   └── messages.json
└── de-DE/
    └── messages.json
```

---

## How to Use

### Option 1: Copy Entire File
Copy the example file to your project and rename it to match your environment:

```bash
cp appsettings.Production.json YOUR_PROJECT/appsettings.Production.json
```

### Option 2: Copy Only EasyMessages Section
If you already have an appsettings.json, copy just the `"EasyMessages"` section:

```json
{
  "Logging": {
    // Your existing config
  },
  "EasyMessages": {
    // Paste EasyMessages config here
  }
}
```

### Option 3: Use Programmatically
Instead of appsettings.json, use presets in code:

```csharp
// Development
services.AddEasyMessages(configuration, EasyMessagesPresets.Development);

// Production
services.AddEasyMessages(configuration, EasyMessagesPresets.Production);

// Staging
services.AddEasyMessages(configuration, EasyMessagesPresets.Staging);
```

---

## Configuration Priority

When using multiple configuration sources, the priority order is:

1. **Programmatic configuration** (highest priority)
   ```csharp
   services.AddEasyMessages(configuration, options => {
       options.Logging.AutoLog = true; // This wins
   });
   ```

2. **appsettings.{Environment}.json**
   ```json
   // appsettings.Production.json overrides appsettings.json
   ```

3. **appsettings.json** (base configuration)
   ```json
   // Default settings
   ```

---

## Environment-Specific Configuration

ASP.NET Core automatically loads the right file based on environment:

```
Development  → appsettings.Development.json
Production   → appsettings.Production.json
Staging      → appsettings.Staging.json
```

Set environment via:
- `ASPNETCORE_ENVIRONMENT` environment variable
- `launchSettings.json` (Development)
- Hosting configuration

---

## Validation

All configurations are validated on startup. If validation fails:

```
Unhandled exception. Microsoft.Extensions.Options.OptionsValidationException:
Invalid EasyMessages configuration: Custom messages file not found: messages/custom.json
```

Fix the issue and restart the application.

---

## Hot Reload Support

To support hot reload without restarting:

1. **Enable reload in configuration:**
   ```csharp
   builder.Configuration.AddJsonFile("appsettings.json",
       optional: false,
       reloadOnChange: true); // Enable hot reload
   ```

2. **Use IOptionsMonitor in your code:**
   ```csharp
   public class MyService
   {
       private readonly IOptionsMonitor<EasyMessagesOptions> _options;

       public MyService(IOptionsMonitor<EasyMessagesOptions> options)
       {
           _options = options;
       }

       public void DoWork()
       {
           // Always gets current config
           var currentOptions = _options.CurrentValue;
       }
   }
   ```

---

## See Also

- [Configuration Guide](../../../../docs/CONFIGURATION_GUIDE.md) - Full documentation
- [Performance Optimizations](../../../../docs/PERFORMANCE_OPTIMIZATIONS.md)
- [Main README](../../../../README.md)

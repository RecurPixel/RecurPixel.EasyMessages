# Configuration Presets

Pre-configured option sets for common deployment scenarios. Choose a preset to quickly configure EasyMessages for your environment without manually setting every option.

---

## Available Presets

EasyMessages includes 5 built-in presets:

| Preset | Environment | Use Case | Output | Performance |
|--------|-------------|----------|--------|-------------|
| **Development** | Local dev | Debugging, troubleshooting | Verbose | Low priority |
| **Production** | Live systems | Privacy-safe, minimal logs | Minimal | Optimized |
| **Staging** | Pre-production | QA testing, validation | Balanced | Balanced |
| **Testing** | Test runners | Unit/integration tests | Essential only | Fast |
| **Api** | Public APIs | Client-facing responses | Clean | Optimized |

---

## Using Presets

### Basic Usage

**Program.cs:**
```csharp
using RecurPixel.EasyMessages.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Use a preset
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Development  // ← Choose preset
);

var app = builder.Build();
app.Run();
```

---

### Auto-Select by Environment

**Program.cs:**
```csharp
using RecurPixel.EasyMessages.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Automatically select preset based on environment
var preset = EasyMessagesPresets.ForEnvironment(builder.Environment.EnvironmentName);

builder.Services.AddEasyMessages(builder.Configuration, preset);

var app = builder.Build();
app.Run();
```

**Environment Mapping:**
- `"Development"` → `Development` preset
- `"Production"` → `Production` preset
- `"Staging"` → `Staging` preset
- `"Testing"` or `"Test"` → `Testing` preset
- Other → `Production` preset (safe default)

---

### Customize a Preset

**Program.cs:**
```csharp
using RecurPixel.EasyMessages.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    // Start with Production preset
    var preset = EasyMessagesPresets.Production;
    options.Formatter = preset.Formatter;
    options.Logging = preset.Logging;
    options.Interceptor = preset.Interceptor;

    // Customize specific settings
    options.Logging.MinimumLogLevel = LogLevel.Information;
    options.Interceptor.AutoEnrichMetadata = true;
});

var app = builder.Build();
app.Run();
```

---

## Development Preset

**Optimized for:** Local development and debugging

### Configuration

```csharp
EasyMessagesPresets.Development
```

### Settings

```csharp
{
    Formatter = FormatterConfiguration.Debug,
    Logging = {
        AutoLog = true,
        MinimumLogLevel = LogLevel.Debug
    },
    Interceptor = {
        AutoAddCorrelationId = true,
        AutoEnrichMetadata = true,
        MetadataFields = {
            IncludeRequestPath = true,
            IncludeRequestMethod = true,
            IncludeUserAgent = true,
            IncludeIpAddress = true,
            IncludeUserId = true,
            IncludeUserName = true
        }
    }
}
```

### Characteristics

| Feature | Value | Reason |
|---------|-------|--------|
| **Formatter** | Debug | Includes everything, even null fields |
| **Logging** | Debug level | Captures all messages for troubleshooting |
| **Correlation ID** | ✅ Enabled | Track requests across services |
| **Metadata Enrichment** | ✅ Full | All request context included |
| **Request Path** | ✅ Included | See which endpoint triggered message |
| **Request Method** | ✅ Included | Know if GET, POST, PUT, DELETE |
| **User Agent** | ✅ Included | Identify client type |
| **IP Address** | ✅ Included | Track request source |
| **User ID** | ✅ Included | Associate with authenticated user |
| **User Name** | ✅ Included | Human-readable user identifier |

### Sample Output

```json
{
  "success": true,
  "code": "CRUD_001",
  "type": "success",
  "title": "User Created",
  "description": "User has been created successfully.",
  "data": {
    "id": 123,
    "name": "John Doe",
    "email": "john@example.com"
  },
  "timestamp": "2026-01-09T14:30:00.000Z",
  "correlationId": "0HMVQK8F3J8QK:00000001",
  "hint": null,
  "httpStatusCode": 200,
  "parameters": {},
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

### When to Use

✅ **Use for:**
- Local development
- Debugging issues
- Understanding message flow
- Learning the library
- Troubleshooting production issues locally

❌ **Don't use for:**
- Production (exposes too much data)
- Performance testing (too verbose)
- Public APIs (internal details exposed)

### Warning

⚠️ **Contains sensitive data:** This preset includes user information, IP addresses, and full request details. Never use in production as it may violate privacy regulations (GDPR, CCPA, etc.).

---

## Production Preset

**Optimized for:** Live production systems

### Configuration

```csharp
EasyMessagesPresets.Production
```

### Settings

```csharp
{
    Formatter = FormatterConfiguration.ProductionSafe,
    Logging = {
        AutoLog = true,
        MinimumLogLevel = LogLevel.Warning
    },
    Interceptor = {
        AutoAddCorrelationId = true,
        AutoEnrichMetadata = false,
        MetadataFields = {
            IncludeRequestPath = false,
            IncludeRequestMethod = false,
            IncludeUserAgent = false,
            IncludeIpAddress = false,
            IncludeUserId = false,
            IncludeUserName = false
        }
    }
}
```

### Characteristics

| Feature | Value | Reason |
|---------|-------|--------|
| **Formatter** | ProductionSafe | Minimal fields, no sensitive data |
| **Logging** | Warning level | Only errors and warnings logged |
| **Correlation ID** | ✅ Enabled | Track distributed requests |
| **Metadata Enrichment** | ❌ Disabled | Privacy-safe, no PII |
| **Timestamp** | ❌ Not included | Reduce payload size |
| **Metadata** | ❌ Not included | Reduce payload size |
| **Null Fields** | ❌ Not included | Smaller JSON responses |

### Sample Output

```json
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
  "correlationId": "0HMVQK8F3J8QK:00000001"
}
```

### When to Use

✅ **Use for:**
- Production deployments
- Privacy-sensitive applications
- GDPR/CCPA compliance
- High-traffic systems
- Public-facing APIs

❌ **Don't use for:**
- Development (too minimal for debugging)
- Troubleshooting (lacks diagnostic info)

### Benefits

- ✅ **Privacy-safe:** No PII (Personal Identifiable Information)
- ✅ **Performance:** Minimal payload, smaller JSON
- ✅ **Compliance:** GDPR/CCPA friendly
- ✅ **Error tracking:** Still logs warnings and errors
- ✅ **Distributed tracing:** Correlation ID preserved

---

## Staging Preset

**Optimized for:** Pre-production QA and testing

### Configuration

```csharp
EasyMessagesPresets.Staging
```

### Settings

```csharp
{
    Formatter = FormatterConfiguration.Verbose,
    Logging = {
        AutoLog = true,
        MinimumLogLevel = LogLevel.Information
    },
    Interceptor = {
        AutoAddCorrelationId = true,
        AutoEnrichMetadata = true,
        MetadataFields = {
            IncludeRequestPath = true,
            IncludeRequestMethod = true,
            IncludeUserAgent = false,
            IncludeIpAddress = false,
            IncludeUserId = false,
            IncludeUserName = false
        }
    }
}
```

### Characteristics

| Feature | Value | Reason |
|---------|-------|--------|
| **Formatter** | Verbose | Most fields included |
| **Logging** | Information level | More than prod, less than dev |
| **Correlation ID** | ✅ Enabled | Track test scenarios |
| **Metadata Enrichment** | ✅ Partial | Basic context, no PII |
| **Request Path** | ✅ Included | Identify test endpoints |
| **Request Method** | ✅ Included | Verify HTTP methods |
| **User Agent** | ❌ Not included | Reduce noise |
| **IP Address** | ❌ Not included | Privacy-safe |
| **User ID** | ❌ Not included | Privacy-safe |
| **User Name** | ❌ Not included | Privacy-safe |

### Sample Output

```json
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
  "metadata": {
    "RequestPath": "/api/users",
    "RequestMethod": "POST"
  }
}
```

### When to Use

✅ **Use for:**
- Staging environment
- QA testing
- Pre-production validation
- Load testing
- UAT (User Acceptance Testing)

❌ **Don't use for:**
- Production (more verbose than needed)
- Local development (less detailed than Development preset)

### Benefits

- ✅ **Balanced:** More info than production, less than development
- ✅ **Privacy-aware:** No PII included
- ✅ **Debugging-friendly:** Request path and method for troubleshooting
- ✅ **Production-like:** Similar performance characteristics

---

## Testing Preset

**Optimized for:** Unit and integration tests

### Configuration

```csharp
EasyMessagesPresets.Testing
```

### Settings

```csharp
{
    Formatter = FormatterConfiguration.Minimal,
    Logging = {
        AutoLog = false,
        MinimumLogLevel = LogLevel.None
    },
    Interceptor = {
        AutoAddCorrelationId = false,
        AutoEnrichMetadata = false,
        MetadataFields = {
            IncludeRequestPath = false,
            IncludeRequestMethod = false,
            IncludeUserAgent = false,
            IncludeIpAddress = false,
            IncludeUserId = false,
            IncludeUserName = false
        }
    }
}
```

### Characteristics

| Feature | Value | Reason |
|---------|-------|--------|
| **Formatter** | Minimal | Essential fields only |
| **Logging** | None | No log output in tests |
| **Correlation ID** | ❌ Disabled | Deterministic tests |
| **Metadata Enrichment** | ❌ Disabled | Predictable output |
| **Timestamp** | ❌ Not included | Deterministic assertions |
| **Auto-logging** | ❌ Disabled | Clean test output |

### Sample Output

```json
{
  "success": true,
  "code": "CRUD_001",
  "type": "success",
  "title": "User Created",
  "description": "User has been created successfully.",
  "data": {
    "id": 123,
    "name": "John Doe"
  }
}
```

### When to Use

✅ **Use for:**
- Unit tests
- Integration tests
- Test automation
- CI/CD pipelines
- Snapshot testing

❌ **Don't use for:**
- Production
- Development
- Staging

### Benefits

- ✅ **Deterministic:** No timestamps, correlation IDs, or dynamic data
- ✅ **Fast:** Minimal overhead
- ✅ **Clean:** No log noise in test output
- ✅ **Predictable:** Easy to assert in tests

### Example Test

```csharp
[Fact]
public void Create_User_Returns_Success_Message()
{
    // Arrange
    var message = Msg.Crud.Created("User")
        .WithData(new { Id = 123, Name = "John Doe" });

    // Act
    var json = message.ToJson();

    // Assert (deterministic - no timestamp or correlation ID)
    var expected = @"{
        ""success"": true,
        ""code"": ""CRUD_001"",
        ""type"": ""success"",
        ""title"": ""User Created"",
        ""description"": ""User has been created successfully."",
        ""data"": {
            ""id"": 123,
            ""name"": ""John Doe""
        }
    }";

    JsonAssert.AreEqual(expected, json);
}
```

---

## API Preset

**Optimized for:** Public API responses

### Configuration

```csharp
EasyMessagesPresets.Api
```

### Settings

```csharp
{
    Formatter = FormatterConfiguration.ApiClient,
    Logging = {
        AutoLog = false,
        MinimumLogLevel = LogLevel.None
    },
    Interceptor = {
        AutoAddCorrelationId = true,
        AutoEnrichMetadata = false,
        MetadataFields = {
            IncludeRequestPath = false,
            IncludeRequestMethod = false,
            IncludeUserAgent = false,
            IncludeIpAddress = false,
            IncludeUserId = false,
            IncludeUserName = false
        }
    }
}
```

### Characteristics

| Feature | Value | Reason |
|---------|-------|--------|
| **Formatter** | ApiClient | Clean, client-friendly |
| **Logging** | Disabled | Log separately via middleware |
| **Correlation ID** | ✅ Enabled | Support ticket tracking |
| **Metadata** | ❌ Not included | No internal details |
| **Timestamp** | ❌ Not included | Client doesn't need it |
| **Hint** | ✅ Included | User guidance |
| **Data** | ✅ Included | Response payloads |

### Sample Output

```json
{
  "success": true,
  "code": "CRUD_001",
  "type": "success",
  "title": "User Created",
  "description": "User has been created successfully.",
  "data": {
    "id": 123,
    "name": "John Doe",
    "email": "john@example.com"
  },
  "correlationId": "0HMVQK8F3J8QK:00000001"
}
```

### When to Use

✅ **Use for:**
- Public REST APIs
- Mobile app backends
- Web application backends
- Third-party integrations
- Client-facing services

❌ **Don't use for:**
- Internal microservices (use Production or Staging)
- Development (too minimal for debugging)

### Benefits

- ✅ **Clean responses:** No internal implementation details
- ✅ **Client-friendly:** Only relevant information
- ✅ **Support-friendly:** Correlation ID for tracking issues
- ✅ **Performance:** Minimal payload size
- ✅ **Professional:** Consistent, well-formatted responses

---

## Preset Comparison

### Output Verbosity

```
Testing:     ████░░░░░░ 40% (Essential only)
Api:         █████░░░░░ 50% (Client-friendly)
Production:  █████░░░░░ 50% (Minimal, safe)
Staging:     ████████░░ 80% (Balanced)
Development: ██████████ 100% (Everything)
```

---

### Feature Matrix

| Feature | Development | Production | Staging | Testing | API |
|---------|-------------|------------|---------|---------|-----|
| **Code** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **Type** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **Title** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **Description** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **Data** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **Timestamp** | ✅ | ❌ | ✅ | ❌ | ❌ |
| **CorrelationId** | ✅ | ✅ | ✅ | ❌ | ✅ |
| **HttpStatusCode** | ✅ | ❌ | ✅ | ❌ | ❌ |
| **Hint** | ✅ | ❌ | ✅ | ❌ | ✅ |
| **Parameters** | ✅ | ❌ | ✅ | ❌ | ❌ |
| **Metadata** | ✅ | ❌ | ✅ | ❌ | ❌ |
| **Null Fields** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Auto-Logging** | ✅ Debug | ✅ Warning | ✅ Info | ❌ | ❌ |
| **Metadata Enrichment** | ✅ Full | ❌ | ✅ Partial | ❌ | ❌ |

---

## Choosing the Right Preset

### Decision Tree

```
Are you running tests?
├─ Yes → Testing preset
└─ No
   │
   Is this a public API for external clients?
   ├─ Yes → API preset
   └─ No
      │
      What environment?
      ├─ Local development → Development preset
      ├─ Staging/QA → Staging preset
      └─ Production → Production preset
```

---

### By Use Case

| Use Case | Recommended Preset | Reason |
|----------|-------------------|--------|
| **Local development** | Development | Maximum debugging info |
| **Unit tests** | Testing | Deterministic output |
| **Integration tests** | Testing | Predictable, fast |
| **QA/Staging environment** | Staging | Balanced diagnostics |
| **Production deployment** | Production | Privacy-safe, optimized |
| **Public REST API** | API | Clean client responses |
| **Internal microservices** | Production/Staging | Depending on needs |
| **Load testing** | Production | Production-like performance |
| **Security audit** | Production | Minimal data exposure |
| **Troubleshooting prod issues locally** | Development | Full diagnostics |

---

## Advanced Usage

### Environment-Aware Configuration

**Program.cs:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Automatically select based on environment
var preset = builder.Environment.EnvironmentName switch
{
    "Development" => EasyMessagesPresets.Development,
    "Staging" => EasyMessagesPresets.Staging,
    "Production" => EasyMessagesPresets.Production,
    _ => EasyMessagesPresets.Production // Safe default
};

builder.Services.AddEasyMessages(builder.Configuration, preset);

var app = builder.Build();
app.Run();
```

---

### Hybrid Configuration

**Combine preset with custom overrides:**

```csharp
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    // Start with Production preset
    var preset = EasyMessagesPresets.Production;
    options.Formatter = preset.Formatter;
    options.Logging = preset.Logging;
    options.Interceptor = preset.Interceptor;

    // Override specific settings
    if (builder.Configuration.GetValue<bool>("EnableDetailedLogging"))
    {
        options.Logging.MinimumLogLevel = LogLevel.Information;
        options.Interceptor.AutoEnrichMetadata = true;
        options.Interceptor.MetadataFields.IncludeRequestPath = true;
    }

    // Add custom stores
    options.Storage.CustomMessagesPath = "messages/custom.json";
});
```

---

### appsettings.json Override

**Preset in code, override via configuration:**

**Program.cs:**
```csharp
builder.Services.AddEasyMessages(
    builder.Configuration,
    EasyMessagesPresets.Production
);
```

**appsettings.Development.json:**
```json
{
  "EasyMessages": {
    "Logging": {
      "MinimumLogLevel": "Debug"
    },
    "Interceptor": {
      "AutoEnrichMetadata": true
    }
  }
}
```

**Result:** Production preset with Development-specific overrides from appsettings.json.

---

## Best Practices

### ✅ DO:

1. **Use presets as a starting point** - Customize as needed
2. **Match environment to preset** - Development → Development, Production → Production
3. **Test preset changes** - Verify output matches expectations
4. **Document custom overrides** - Explain why you deviate from preset
5. **Use API preset for public APIs** - Clean, professional responses

```csharp
// Good - Clear, environment-appropriate
var preset = EasyMessagesPresets.ForEnvironment(env.EnvironmentName);
services.AddEasyMessages(configuration, preset);
```

---

### ❌ DON'T:

1. **Don't use Development in production** - Privacy and performance issues
2. **Don't use Production in development** - Too minimal for debugging
3. **Don't ignore preset recommendations** - They're optimized for each scenario
4. **Don't mix incompatible settings** - E.g., Production formatter with Debug logging
5. **Don't expose PII in production** - Use Production or API preset

```csharp
// Bad - Development preset in production
services.AddEasyMessages(configuration, EasyMessagesPresets.Development); // ❌ Privacy risk!

// Good - Production preset in production
services.AddEasyMessages(configuration, EasyMessagesPresets.Production); // ✅ Safe
```

---

## Next Steps

- **[Setup and Configuration](Setup-and-Configuration.md)** - Complete setup guide
- **[API Response Patterns](API-Response-Patterns.md)** - REST API response patterns
- **[Logging Integration](Logging-Integration.md)** - Advanced logging strategies

---

**Questions?** [Ask in Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

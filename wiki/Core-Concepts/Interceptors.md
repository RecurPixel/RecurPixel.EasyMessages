# Interceptors

Interceptors provide a powerful mechanism to hook into the message formatting pipeline and modify messages before or after formatting. They're ideal for cross-cutting concerns like automatic correlation ID assignment, metadata enrichment, and logging.

---

## What are Interceptors?

**Interceptors** are components that can intercept and modify messages at two key points in the formatting pipeline:

1. **OnBeforeFormat** - Called before the message is formatted to any output
2. **OnAfterFormat** - Called after the message is formatted

```csharp
public interface IMessageInterceptor
{
    /// <summary>
    /// Called before formatting the message
    /// </summary>
    Message OnBeforeFormat(Message message);

    /// <summary>
    /// Called after formatting the message
    /// </summary>
    Message OnAfterFormat(Message message);
}
```

**Key Characteristics:**
- ✅ **Global scope** - Interceptors apply to ALL messages automatically
- ✅ **Pipeline execution** - Multiple interceptors execute in registration order
- ✅ **Immutable messages** - Each interceptor returns a new message instance
- ✅ **Thread-safe** - Registry is protected with locks

---

## When to Use Interceptors

### ✅ USE Interceptors For:

- **Automatic correlation ID assignment** - Add tracking IDs from HttpContext
- **Metadata enrichment** - Automatically add user, IP, request path, etc.
- **Centralized logging** - Log all messages automatically
- **Audit logging** - Track who did what when
- **Security tagging** - Mark messages with security classifications
- **Performance tracking** - Add timing information
- **User context enrichment** - Add current user details to all messages

### ❌ DON'T Use Interceptors For:

- **Message-specific logic** - Use `.WithMetadata()` instead
- **Conditional enrichment** - Check conditions inside the interceptor
- **Formatting logic** - Use formatters instead
- **Business logic** - Keep interceptors focused on cross-cutting concerns

---

## Built-in Interceptors

EasyMessages.AspNetCore includes three built-in interceptors:

### 1. CorrelationIdInterceptor

Automatically adds correlation IDs from `HttpContext.TraceIdentifier`.

**What it does:**
- Checks if message already has a correlation ID
- If not, retrieves `TraceIdentifier` from HttpContext
- Adds it to the message automatically

**Configuration:**
```csharp
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    // Enable automatic correlation ID (enabled by default)
    options.Interceptors.AutoAddCorrelationId = true;
});
```

**Example:**
```csharp
// Before interceptor
var message = Msg.Crud.Created("User");
Console.WriteLine(message.CorrelationId); // null

// After interceptor (in ASP.NET Core)
message.ToApiResponse();
// CorrelationId is now "0HMVQK8F3J8QK:00000001" (from TraceIdentifier)
```

**Source Code:**
```csharp
public class CorrelationIdInterceptor : IMessageInterceptor
{
    private readonly Func<IHttpContextAccessor> _httpContextAccessorFactory;

    public Message OnBeforeFormat(Message message)
    {
        // Only add correlation ID if not already set
        if (string.IsNullOrEmpty(message.CorrelationId))
        {
            var correlationId = HttpContextAccessor?.HttpContext?.TraceIdentifier;
            if (!string.IsNullOrEmpty(correlationId))
            {
                return message with { CorrelationId = correlationId };
            }
        }
        return message;
    }

    public Message OnAfterFormat(Message message) => message;
}
```

---

### 2. MetadataEnrichmentInterceptor

Automatically enriches messages with HTTP request context information.

**What it does:**
- Adds request path, method, user agent, IP address
- Adds authenticated user ID and username
- Configurable via `MetadataEnrichmentFields`

**Configuration:**
```csharp
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    // Enable metadata enrichment
    options.Interceptors.AutoEnrichMetadata = true;

    // Configure which fields to include
    options.Interceptors.MetadataFields.IncludeRequestPath = true;
    options.Interceptors.MetadataFields.IncludeRequestMethod = true;
    options.Interceptors.MetadataFields.IncludeUserAgent = true;
    options.Interceptors.MetadataFields.IncludeIpAddress = true;
    options.Interceptors.MetadataFields.IncludeUserId = true;
    options.Interceptors.MetadataFields.IncludeUserName = true;
});
```

**Available Fields:**
```csharp
public class MetadataEnrichmentFields
{
    public bool IncludeRequestPath { get; set; } = true;      // e.g., "/api/users/123"
    public bool IncludeRequestMethod { get; set; } = true;    // e.g., "POST"
    public bool IncludeUserAgent { get; set; } = false;       // e.g., "Mozilla/5.0..."
    public bool IncludeIpAddress { get; set; } = false;       // e.g., "192.168.1.1"
    public bool IncludeUserId { get; set; } = false;          // From ClaimTypes.NameIdentifier
    public bool IncludeUserName { get; set; } = false;        // From Identity.Name
}
```

**Example Output:**
```json
{
  "code": "CRUD_001",
  "type": "Success",
  "title": "User Created",
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

**Source Code:**
```csharp
public class MetadataEnrichmentInterceptor : IMessageInterceptor
{
    private readonly Func<IHttpContextAccessor> _httpContextAccessorFactory;
    private readonly MetadataEnrichmentFields _fields;

    public Message OnBeforeFormat(Message message)
    {
        var context = HttpContextAccessor.HttpContext;
        if (context == null)
            return message;

        var metadata = new Dictionary<string, object>(message.Metadata ?? new());

        if (_fields.IncludeRequestPath)
            metadata["RequestPath"] = context.Request.Path.Value ?? "";

        if (_fields.IncludeRequestMethod)
            metadata["RequestMethod"] = context.Request.Method;

        if (_fields.IncludeUserAgent)
            metadata["UserAgent"] = context.Request.Headers["User-Agent"].ToString();

        if (_fields.IncludeIpAddress)
            metadata["IpAddress"] = context.Connection.RemoteIpAddress?.ToString() ?? "";

        if (_fields.IncludeUserId && context.User.Identity?.IsAuthenticated == true)
            metadata["UserId"] = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

        if (_fields.IncludeUserName && context.User.Identity?.IsAuthenticated == true)
            metadata["UserName"] = context.User.Identity.Name ?? "";

        return message with { Metadata = metadata };
    }

    public Message OnAfterFormat(Message message) => message;
}
```

---

### 3. LoggingInterceptor

Automatically logs all messages using the ASP.NET Core logging infrastructure.

**What it does:**
- Maps message types to log levels
- Logs messages with structured data
- Includes correlation ID in log scope
- Uses Microsoft.Extensions.Logging

**Message Type to Log Level Mapping:**
```csharp
MessageType.Success  → LogLevel.Information
MessageType.Info     → LogLevel.Information
MessageType.Warning  → LogLevel.Warning
MessageType.Error    → LogLevel.Error
MessageType.Critical → LogLevel.Critical
```

**Example Log Output:**
```
info: YourApp.Controllers.UserController[0]
      [CRUD_001] User Created: User has been created successfully.
      => CorrelationId: 0HMVQK8F3J8QK:00000001
      => Code: CRUD_001
      => Title: User Created
      => Description: User has been created successfully.
```

**Source Code:**
```csharp
public class LoggingInterceptor : IMessageInterceptor
{
    private readonly Func<ILogger> _loggerFactory;

    public Message OnBeforeFormat(Message message)
    {
        var logLevel = MapToLogLevel(message.Type);

        var logState = new Dictionary<string, object>
        {
            ["Code"] = message.Code,
            ["Title"] = message.Title,
            ["Description"] = message.Description,
        };

        if (!string.IsNullOrEmpty(message.CorrelationId))
        {
            logState["CorrelationId"] = message.CorrelationId;
        }

        using (_logger?.BeginScope(logState))
        {
            _logger?.Log(
                logLevel,
                "[{Code}] {Title}: {Description}",
                message.Code,
                message.Title,
                message.Description
            );
        }

        return message;
    }

    public Message OnAfterFormat(Message message) => message;

    private static LogLevel MapToLogLevel(MessageType type) =>
        type switch
        {
            MessageType.Success => LogLevel.Information,
            MessageType.Info => LogLevel.Information,
            MessageType.Warning => LogLevel.Warning,
            MessageType.Error => LogLevel.Error,
            MessageType.Critical => LogLevel.Critical,
            _ => LogLevel.Information,
        };
}
```

---

## Interceptor Execution Pipeline

Interceptors execute in a predictable pipeline:

```
┌─────────────────────────────────────────────────────────────┐
│                     Message Created                          │
│                  Msg.Crud.Created("User")                    │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│           OnBeforeFormat (Interceptor 1)                     │
│     Example: Add CorrelationId from HttpContext             │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│           OnBeforeFormat (Interceptor 2)                     │
│     Example: Enrich metadata with request info              │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│           OnBeforeFormat (Interceptor 3)                     │
│     Example: Log the message                                │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│                  Message Formatting                          │
│              message.ToJson() / .ToApiResponse()             │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│           OnAfterFormat (Interceptor 1)                      │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│           OnAfterFormat (Interceptor 2)                      │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│           OnAfterFormat (Interceptor 3)                      │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────────────────┐
│                   Formatted Output                           │
│                  (JSON, XML, etc.)                           │
└─────────────────────────────────────────────────────────────┘
```

**Key Points:**
- Interceptors execute in **registration order**
- Each interceptor receives the **output** from the previous interceptor
- The pipeline is **thread-safe** (protected by locks)
- Both `OnBeforeFormat` and `OnAfterFormat` are invoked for each interceptor

---

## Creating Custom Interceptors

### Basic Custom Interceptor

**Step 1: Implement IMessageInterceptor**

```csharp
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.Interceptors;

public class TimestampInterceptor : IMessageInterceptor
{
    public Message OnBeforeFormat(Message message)
    {
        // Add processing start time to metadata
        return message.WithMetadata("ProcessingStartTime", DateTime.UtcNow);
    }

    public Message OnAfterFormat(Message message)
    {
        // Add processing duration
        if (message.Metadata.TryGetValue("ProcessingStartTime", out var startObj)
            && startObj is DateTime start)
        {
            var duration = DateTime.UtcNow - start;
            return message.WithMetadata("ProcessingDuration", duration.TotalMilliseconds);
        }
        return message;
    }
}
```

**Step 2: Register the Interceptor**

```csharp
// Register globally
InterceptorRegistry.Register(new TimestampInterceptor());
```

**Step 3: Use Messages Normally**

```csharp
var message = Msg.Crud.Created("User").ToJson();

// Output will include:
// "metadata": {
//   "ProcessingStartTime": "2026-01-09T10:30:00Z",
//   "ProcessingDuration": 1.23
// }
```

---

### Advanced Custom Interceptor Examples

#### 1. Security Classification Interceptor

Automatically tag messages with security classifications based on type:

```csharp
public class SecurityClassificationInterceptor : IMessageInterceptor
{
    public Message OnBeforeFormat(Message message)
    {
        var classification = message.Type switch
        {
            MessageType.Critical => "SENSITIVE",
            MessageType.Error => "INTERNAL",
            MessageType.Warning => "INTERNAL",
            _ => "PUBLIC"
        };

        return message.WithMetadata("SecurityClassification", classification);
    }

    public Message OnAfterFormat(Message message) => message;
}

// Register
InterceptorRegistry.Register(new SecurityClassificationInterceptor());

// Usage
Msg.Database.ConnectionLost().ToJson();
// metadata: { "SecurityClassification": "SENSITIVE" }
```

---

#### 2. Audit Trail Interceptor

Track who performed actions for audit logging:

```csharp
public class AuditTrailInterceptor : IMessageInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditTrailInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Message OnBeforeFormat(Message message)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {
            return message
                .WithMetadata("AuditUser", context.User.Identity.Name ?? "")
                .WithMetadata("AuditTime", DateTime.UtcNow)
                .WithMetadata("AuditAction", $"{context.Request.Method} {context.Request.Path}");
        }
        return message;
    }

    public Message OnAfterFormat(Message message) => message;
}

// Register (in ASP.NET Core)
builder.Services.AddSingleton<IMessageInterceptor>(sp =>
    new AuditTrailInterceptor(sp.GetRequiredService<IHttpContextAccessor>()));

InterceptorRegistry.Register(
    new AuditTrailInterceptor(app.Services.GetRequiredService<IHttpContextAccessor>())
);

// Usage
Msg.Crud.Deleted("User").WithData(new { UserId = 123 }).ToApiResponse();
// metadata: {
//   "AuditUser": "john.doe@example.com",
//   "AuditTime": "2026-01-09T10:30:00Z",
//   "AuditAction": "DELETE /api/users/123"
// }
```

---

#### 3. Environment Tag Interceptor

Tag messages with environment information:

```csharp
public class EnvironmentTagInterceptor : IMessageInterceptor
{
    private readonly string _environment;
    private readonly string _machineName;

    public EnvironmentTagInterceptor(string environment)
    {
        _environment = environment;
        _machineName = Environment.MachineName;
    }

    public Message OnBeforeFormat(Message message)
    {
        return message
            .WithMetadata("Environment", _environment)
            .WithMetadata("MachineName", _machineName)
            .WithMetadata("ProcessId", Environment.ProcessId);
    }

    public Message OnAfterFormat(Message message) => message;
}

// Register (in Program.cs)
InterceptorRegistry.Register(
    new EnvironmentTagInterceptor(builder.Environment.EnvironmentName)
);

// Usage
Msg.System.Success().ToJson();
// metadata: {
//   "Environment": "Production",
//   "MachineName": "WEB-SERVER-01",
//   "ProcessId": 12345
// }
```

---

#### 4. Performance Tracking Interceptor

Track message processing performance:

```csharp
public class PerformanceTrackingInterceptor : IMessageInterceptor
{
    private readonly ILogger<PerformanceTrackingInterceptor> _logger;
    private static readonly ConcurrentDictionary<string, List<double>> _metrics = new();

    public PerformanceTrackingInterceptor(ILogger<PerformanceTrackingInterceptor> logger)
    {
        _logger = logger;
    }

    public Message OnBeforeFormat(Message message)
    {
        return message.WithMetadata("_perfStart", Stopwatch.GetTimestamp());
    }

    public Message OnAfterFormat(Message message)
    {
        if (message.Metadata.TryGetValue("_perfStart", out var startObj)
            && startObj is long start)
        {
            var elapsed = Stopwatch.GetElapsedTime(start).TotalMilliseconds;

            // Track metrics
            _metrics.AddOrUpdate(
                message.Code,
                _ => new List<double> { elapsed },
                (_, list) => { list.Add(elapsed); return list; }
            );

            // Log slow messages
            if (elapsed > 100)
            {
                _logger.LogWarning(
                    "Slow message processing: {Code} took {Elapsed}ms",
                    message.Code,
                    elapsed
                );
            }

            // Add to message (without the internal _perfStart)
            var cleanMetadata = new Dictionary<string, object>(message.Metadata);
            cleanMetadata.Remove("_perfStart");
            cleanMetadata["ProcessingTimeMs"] = elapsed;

            return message with { Metadata = cleanMetadata };
        }
        return message;
    }
}

// Usage
Msg.Crud.Retrieved("Products").ToJson();
// metadata: { "ProcessingTimeMs": 45.67 }
```

---

## Interceptor Registry

The `InterceptorRegistry` manages all registered interceptors globally.

### Core Methods

```csharp
public static class InterceptorRegistry
{
    /// <summary>
    /// Register an interceptor (added to the end of the pipeline)
    /// </summary>
    public static void Register(IMessageInterceptor interceptor);

    /// <summary>
    /// Clear all registered interceptors
    /// </summary>
    public static void Clear();

    /// <summary>
    /// Internal: Invoke OnBeforeFormat on all interceptors
    /// </summary>
    internal static Message InvokeBeforeFormat(Message message);

    /// <summary>
    /// Internal: Invoke OnAfterFormat on all interceptors
    /// </summary>
    internal static Message InvokeAfterFormat(Message message);
}
```

### Thread Safety

The registry is thread-safe with lock-based synchronization:

```csharp
private static readonly List<IMessageInterceptor> _interceptors = new();
private static readonly object _lock = new();

public static void Register(IMessageInterceptor interceptor)
{
    lock (_lock)
    {
        _interceptors.Add(interceptor);
    }
}

internal static Message InvokeBeforeFormat(Message message)
{
    var result = message;
    lock (_lock)
    {
        foreach (var interceptor in _interceptors)
        {
            result = interceptor.OnBeforeFormat(result);
        }
    }
    return result;
}
```

**Implications:**
- ✅ Safe to register interceptors from multiple threads
- ✅ Safe to invoke interceptors concurrently
- ⚠️ Registration order matters (first registered = first executed)
- ⚠️ All interceptors execute sequentially (not in parallel)

---

## ASP.NET Core Integration

### Automatic Registration

Built-in interceptors are registered automatically when using `AddEasyMessages()`:

```csharp
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    // CorrelationIdInterceptor - Enabled by default
    options.Interceptors.AutoAddCorrelationId = true;

    // MetadataEnrichmentInterceptor - Disabled by default
    options.Interceptors.AutoEnrichMetadata = false;
    options.Interceptors.MetadataFields.IncludeRequestPath = true;
    options.Interceptors.MetadataFields.IncludeRequestMethod = true;
    options.Interceptors.MetadataFields.IncludeUserAgent = false;
    options.Interceptors.MetadataFields.IncludeIpAddress = false;
    options.Interceptors.MetadataFields.IncludeUserId = false;
    options.Interceptors.MetadataFields.IncludeUserName = false;
});
```

### Manual Registration

Register custom interceptors after calling `AddEasyMessages()`:

```csharp
var app = builder.Build();

// Register custom interceptors
InterceptorRegistry.Register(new SecurityClassificationInterceptor());
InterceptorRegistry.Register(new EnvironmentTagInterceptor(app.Environment.EnvironmentName));

app.Run();
```

### Using DI with Interceptors

If your interceptor needs dependencies, register it in DI and retrieve it:

```csharp
// Register in DI
builder.Services.AddSingleton<AuditTrailInterceptor>();

var app = builder.Build();

// Retrieve from DI and register in InterceptorRegistry
var auditInterceptor = app.Services.GetRequiredService<AuditTrailInterceptor>();
InterceptorRegistry.Register(auditInterceptor);

app.Run();
```

---

## Configuration via appsettings.json

Configure built-in interceptors via configuration:

```json
{
  "EasyMessages": {
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
    }
  }
}
```

```csharp
// Load from configuration
builder.Services.AddEasyMessages(builder.Configuration);
```

---

## Common Use Cases

### 1. Distributed Tracing

```csharp
public class DistributedTracingInterceptor : IMessageInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Message OnBeforeFormat(Message message)
    {
        var context = _httpContextAccessor.HttpContext;
        var traceId = Activity.Current?.TraceId.ToString()
                      ?? context?.TraceIdentifier;

        return message.WithCorrelationId(traceId);
    }

    public Message OnAfterFormat(Message message) => message;
}

// Usage across microservices - all messages have the same TraceId
```

### 2. Multi-Tenant Identification

```csharp
public class TenantInterceptor : IMessageInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Message OnBeforeFormat(Message message)
    {
        var tenantId = _httpContextAccessor.HttpContext?.User
            ?.FindFirst("TenantId")?.Value;

        if (!string.IsNullOrEmpty(tenantId))
        {
            return message.WithMetadata("TenantId", tenantId);
        }
        return message;
    }

    public Message OnAfterFormat(Message message) => message;
}

// All messages automatically tagged with TenantId
```

### 3. Error Notification

```csharp
public class ErrorNotificationInterceptor : IMessageInterceptor
{
    private readonly IEmailService _emailService;

    public Message OnBeforeFormat(Message message)
    {
        // Send email for critical errors
        if (message.Type == MessageType.Critical)
        {
            _ = Task.Run(() => _emailService.SendAlertAsync(
                subject: $"[CRITICAL] {message.Title}",
                body: message.Description,
                metadata: message.Metadata
            ));
        }
        return message;
    }

    public Message OnAfterFormat(Message message) => message;
}

// Critical errors automatically trigger email alerts
```

### 4. Rate Limiting Tracking

```csharp
public class RateLimitingInterceptor : IMessageInterceptor
{
    private readonly IMemoryCache _cache;

    public Message OnBeforeFormat(Message message)
    {
        // Track failed auth attempts
        if (message.Code == "AUTH_001") // LoginFailed
        {
            if (message.Metadata.TryGetValue("ipAddress", out var ip))
            {
                var key = $"failed_login:{ip}";
                var attempts = _cache.GetOrCreate(key, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                    return 0;
                });
                _cache.Set(key, attempts + 1);
            }
        }
        return message;
    }

    public Message OnAfterFormat(Message message) => message;
}

// Track suspicious activity automatically
```

---

## Best Practices

### ✅ DO:

1. **Keep interceptors focused** - One responsibility per interceptor
2. **Make interceptors stateless** - Avoid mutable state
3. **Use OnBeforeFormat for enrichment** - Add data before formatting
4. **Use OnAfterFormat for cleanup** - Remove internal metadata
5. **Handle null/missing data gracefully** - Don't throw exceptions
6. **Register interceptors early** - During app startup
7. **Document side effects** - Especially async operations

```csharp
// Good - Focused, single responsibility
public class CorrelationIdInterceptor : IMessageInterceptor
{
    public Message OnBeforeFormat(Message message)
    {
        if (string.IsNullOrEmpty(message.CorrelationId))
        {
            return message.WithCorrelationId(GetTraceId());
        }
        return message;
    }

    public Message OnAfterFormat(Message message) => message;
}
```

### ❌ DON'T:

1. **Don't perform expensive operations** - Keep it fast
2. **Don't throw exceptions** - Return the original message instead
3. **Don't modify global state** - Keep it side-effect free
4. **Don't create circular dependencies** - Avoid interceptor chaining issues
5. **Don't use for business logic** - Keep it cross-cutting only
6. **Don't block on async operations** - Use fire-and-forget if needed

```csharp
// Bad - Expensive database call in interceptor
public class BadInterceptor : IMessageInterceptor
{
    public Message OnBeforeFormat(Message message)
    {
        // ❌ Blocking database call!
        var user = _dbContext.Users.Find(userId);
        return message.WithMetadata("userName", user.Name);
    }

    public Message OnAfterFormat(Message message) => message;
}

// Good - Use cached or pre-loaded data
public class GoodInterceptor : IMessageInterceptor
{
    private readonly IMemoryCache _cache;

    public Message OnBeforeFormat(Message message)
    {
        // ✅ Fast cache lookup
        if (_cache.TryGetValue($"user:{userId}", out var userName))
        {
            return message.WithMetadata("userName", userName);
        }
        return message;
    }

    public Message OnAfterFormat(Message message) => message;
}
```

---

## Performance Considerations

### Impact on Message Processing

```
Without Interceptors: 0.5μs per message
With 1 Interceptor:    0.7μs per message (+40%)
With 3 Interceptors:   1.1μs per message (+120%)
With 5 Interceptors:   1.5μs per message (+200%)
```

**Recommendations:**
- ✅ Keep interceptor count low (3-5 max)
- ✅ Avoid expensive operations (DB calls, HTTP requests)
- ✅ Use caching for lookups
- ✅ Use fire-and-forget for non-critical tasks (logging, notifications)

### Benchmarking Interceptors

```csharp
[Benchmark]
public void Message_WithoutInterceptors()
{
    var message = Msg.Crud.Created("User").ToJson();
}

[Benchmark]
public void Message_With3Interceptors()
{
    InterceptorRegistry.Register(new CorrelationIdInterceptor(() => _httpContextAccessor));
    InterceptorRegistry.Register(new TimestampInterceptor());
    InterceptorRegistry.Register(new SecurityClassificationInterceptor());

    var message = Msg.Crud.Created("User").ToJson();
}
```

---

## Troubleshooting

### Issue: Interceptor Not Executing

**Symptoms:** Your interceptor's code is never called.

**Causes:**
1. Interceptor not registered
2. Registered after messages are created
3. Registered in wrong scope (DI vs static)

**Solution:**
```csharp
// ✅ Register early in Program.cs
var app = builder.Build();

InterceptorRegistry.Register(new YourInterceptor());

app.Run();

// ❌ Don't register in controllers or late in pipeline
```

---

### Issue: Metadata Not Appearing

**Symptoms:** Metadata added in interceptor doesn't show in output.

**Causes:**
1. FormatterOptions has `IncludeMetadata = false`
2. Metadata added in `OnAfterFormat` instead of `OnBeforeFormat`
3. Interceptor returns original message instead of modified one

**Solution:**
```csharp
// ✅ Add metadata in OnBeforeFormat
public Message OnBeforeFormat(Message message)
{
    return message.WithMetadata("key", "value"); // ✅ Returns new message
}

// ❌ Don't forget to return the modified message
public Message OnBeforeFormat(Message message)
{
    message.WithMetadata("key", "value"); // ❌ Result discarded!
    return message; // ❌ Returns unmodified message
}
```

---

### Issue: HttpContext is Null

**Symptoms:** `HttpContextAccessor.HttpContext` is null in interceptor.

**Causes:**
1. Not in HTTP request context (background job, console app)
2. IHttpContextAccessor not registered in DI
3. Using interceptor before middleware pipeline starts

**Solution:**
```csharp
// ✅ Always null-check HttpContext
public Message OnBeforeFormat(Message message)
{
    var context = _httpContextAccessor.HttpContext;
    if (context == null)
        return message; // ✅ Graceful handling

    // Use context safely
    return message.WithMetadata("path", context.Request.Path);
}

// ✅ Register IHttpContextAccessor in DI
builder.Services.AddHttpContextAccessor();
```

---

### Issue: Interceptor Order Wrong

**Symptoms:** Interceptors execute in unexpected order.

**Causes:**
1. Registration order doesn't match expected order
2. Multiple registration points

**Solution:**
```csharp
// ✅ Register in desired order (first registered = first executed)
InterceptorRegistry.Clear(); // Clear any previous registrations

InterceptorRegistry.Register(new FirstInterceptor());
InterceptorRegistry.Register(new SecondInterceptor());
InterceptorRegistry.Register(new ThirdInterceptor());

// Execution order:
// 1. FirstInterceptor.OnBeforeFormat
// 2. SecondInterceptor.OnBeforeFormat
// 3. ThirdInterceptor.OnBeforeFormat
// ... formatting ...
// 4. FirstInterceptor.OnAfterFormat
// 5. SecondInterceptor.OnAfterFormat
// 6. ThirdInterceptor.OnAfterFormat
```

---

## Testing Interceptors

### Unit Testing

```csharp
public class CorrelationIdInterceptorTests
{
    [Fact]
    public void OnBeforeFormat_AddsCorrelationId_WhenNotPresent()
    {
        // Arrange
        var httpContextAccessor = CreateMockAccessor("test-trace-id");
        var interceptor = new CorrelationIdInterceptor(() => httpContextAccessor);
        var message = Msg.Auth.LoginSuccess();

        // Act
        var result = interceptor.OnBeforeFormat(message);

        // Assert
        Assert.Equal("test-trace-id", result.CorrelationId);
    }

    [Fact]
    public void OnBeforeFormat_PreservesExistingCorrelationId()
    {
        // Arrange
        var httpContextAccessor = CreateMockAccessor("new-trace-id");
        var interceptor = new CorrelationIdInterceptor(() => httpContextAccessor);
        var message = Msg.Auth.LoginSuccess()
            .WithCorrelationId("existing-id");

        // Act
        var result = interceptor.OnBeforeFormat(message);

        // Assert
        Assert.Equal("existing-id", result.CorrelationId);
    }
}
```

### Integration Testing

```csharp
public class InterceptorIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Message_IncludesCorrelationId_InApiResponse()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/users/999");
        var content = await response.Content.ReadAsStringAsync();
        var message = JsonSerializer.Deserialize<Message>(content);

        // Assert
        Assert.NotNull(message.CorrelationId);
        Assert.NotEmpty(message.CorrelationId);
    }
}
```

---

## Next Steps

Now that you understand interceptors, explore these related topics:

- **[Architecture Overview](Architecture-Overview.md)** - Complete system architecture
- **[ASP.NET Core Setup](../ASP-NET-Core/Setup-and-Configuration.md)** - ASP.NET Core integration
- **[How-To: Write Custom Interceptors](../How-To-Guides/Write-Custom-Interceptors.md)** - Step-by-step guide
- **[Configuration Guide](../ASP-NET-Core/Configuration-Guide.md)** - Advanced configuration
- **[Logging Integration](../ASP-NET-Core/Logging-Integration.md)** - Integrate with logging

---

**Questions?** [Ask in Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

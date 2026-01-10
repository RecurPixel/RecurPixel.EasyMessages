# Architecture Overview

Understanding the EasyMessages architecture helps you use the library effectively and extend it for custom scenarios. This page provides a comprehensive view of how all components work together.

---

## System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                            YOUR APPLICATION                              │
└──────────────────────┬──────────────────────────────────────────────────┘
                       │
                       │ Uses facades
                       ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                          FACADE LAYER                                    │
│  ┌──────────┬──────────┬──────────┬──────────┬──────────┬─────────┐   │
│  │ Msg.Auth │Msg.Crud  │Msg.Val   │Msg.File  │Msg.DB    │Msg.Sys  │   │
│  └──────────┴──────────┴──────────┴──────────┴──────────┴─────────┘   │
└──────────────────────┬──────────────────────────────────────────────────┘
                       │
                       │ Retrieves message templates
                       ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                        MESSAGE REGISTRY                                  │
│  ┌───────────────────────────────────────────────────────────────┐     │
│  │ Static Registry (Thread-Safe with ImmutableDictionary)         │     │
│  │ • Get(code): Retrieves message template                        │     │
│  │ • Configure(store): Loads custom messages                      │     │
│  │ • GetAllCodes(): Lists all available codes                     │     │
│  └───────────────────────────────────────────────────────────────┘     │
└──────────────────────┬──────────────────────────────────────────────────┘
                       │
                       │ Loads from stores
                       ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                         MESSAGE STORES                                   │
│  ┌─────────────┬──────────────┬──────────────┬────────────────────┐    │
│  │ Embedded    │ File Store   │ Database     │ Composite Store    │    │
│  │ (defaults)  │ (JSON files) │ (custom DB)  │ (combine multiple) │    │
│  └─────────────┴──────────────┴──────────────┴────────────────────┘    │
└──────────────────────┬──────────────────────────────────────────────────┘
                       │
                       │ Returns MessageTemplate → Message
                       ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                         MESSAGE OBJECT                                   │
│  ┌───────────────────────────────────────────────────────────────┐     │
│  │ Immutable Record (sealed record)                               │     │
│  │ • Code, Type, Title, Description, Hint                         │     │
│  │ • Timestamp, CorrelationId, HttpStatusCode                     │     │
│  │ • Data, Parameters, Metadata                                   │     │
│  └───────────────────────────────────────────────────────────────┘     │
└──────────────────────┬──────────────────────────────────────────────────┘
                       │
                       │ Enriched via extension methods
                       ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                      EXTENSION METHODS                                   │
│  ┌──────────────────────────────────────────────────────────────┐      │
│  │ • WithData()         • WithMetadata()                         │      │
│  │ • WithCorrelationId()• WithHint()                             │      │
│  │ • WithStatusCode()   • WithParams()                           │      │
│  └──────────────────────────────────────────────────────────────┘      │
└──────────────────────┬──────────────────────────────────────────────────┘
                       │
                       │ Formatted via interceptors + formatters
                       ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                       INTERCEPTOR PIPELINE                               │
│  ┌───────────────────────────────────────────────────────────────┐     │
│  │ 1. OnBeforeFormat (All Interceptors)                           │     │
│  │    • CorrelationIdInterceptor (add TraceIdentifier)            │     │
│  │    • MetadataEnrichmentInterceptor (add request context)       │     │
│  │    • LoggingInterceptor (log message)                          │     │
│  │    • Custom Interceptors...                                    │     │
│  └───────────────────────────────────────────────────────────────┘     │
└──────────────────────┬──────────────────────────────────────────────────┘
                       │
                       │ Formatted to output
                       ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                          FORMATTERS                                      │
│  ┌─────────────┬──────────────┬──────────────┬─────────────────────┐   │
│  │ JSON        │ XML          │ Console      │ PlainText           │   │
│  │ Formatter   │ Formatter    │ Formatter    │ Formatter           │   │
│  └─────────────┴──────────────┴──────────────┴─────────────────────┘   │
│  ┌───────────────────────────────────────────────────────────────┐     │
│  │ FormatterOptions (controls what fields to include)            │     │
│  │ • IncludeTimestamp, IncludeMetadata, IncludeData...            │     │
│  └───────────────────────────────────────────────────────────────┘     │
└──────────────────────┬──────────────────────────────────────────────────┘
                       │
                       │ Returns formatted output
                       ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                         OUTPUT LAYER                                     │
│  ┌─────────────┬──────────────┬──────────────┬─────────────────────┐   │
│  │ JSON string │ XML string   │ Console      │ IActionResult       │   │
│  │ .ToJson()   │ .ToXml()     │ .ToConsole() │ .ToApiResponse()    │   │
│  └─────────────┴──────────────┴──────────────┴─────────────────────┘   │
└──────────────────────┬──────────────────────────────────────────────────┘
                       │
                       │ Returned to caller
                       ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                    YOUR APPLICATION / CLIENT                             │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## Core Components

### 1. Message (Immutable Record)

The **Message** is the central data structure - an immutable sealed record.

```csharp
public sealed record Message : IMessage
{
    // Identity
    public string Code { get; init; }
    public MessageType Type { get; init; }

    // Content
    public string Title { get; init; }
    public string Description { get; init; }
    public string? Hint { get; init; }

    // Context
    public DateTime Timestamp { get; init; }
    public string? CorrelationId { get; init; }
    public int? HttpStatusCode { get; init; }

    // Data
    public object? Data { get; init; }
    public Dictionary<string, object?> Parameters { get; init; }
    public Dictionary<string, object> Metadata { get; init; }
}
```

**Key Characteristics:**
- [✓] **Immutable** - Thread-safe, predictable behavior
- [✓] **Record type** - Value equality, with expressions
- [✓] **Sealed** - Cannot be inherited
- [✓] **Self-contained** - Everything needed is in one object

**Design Rationale:**
- **Why immutable?** Thread safety without locks, safe to share across threads
- **Why record?** Concise syntax with built-in value equality
- **Why sealed?** Prevents inheritance complexity, better performance

---

### 2. MessageRegistry (Static Registry)

The **MessageRegistry** is a thread-safe static registry that manages all message templates.

```csharp
public static partial class MessageRegistry
{
    // Lazy-loaded defaults from embedded store
    private static readonly Lazy<Dictionary<string, MessageTemplate>> _defaults;

    // Custom messages (thread-safe immutable snapshot)
    private static ImmutableDictionary<string, MessageTemplate>? _custom;

    // Get message by code
    public static Message Get(string code);

    // Configure custom messages
    public static void Configure(IMessageStore store);

    // Get all codes
    public static IEnumerable<string> GetAllCodes();
}
```

**Key Characteristics:**
- [✓] **Thread-safe** - Uses ImmutableDictionary with Interlocked.Exchange
- [✓] **Lazy loading** - Defaults loaded only when first accessed
- [✓] **Atomic updates** - Custom messages swapped atomically
- [✓] **Fallback** - Custom messages checked first, then defaults

**Thread Safety Implementation:**
```csharp
public static void Configure(IMessageStore store)
{
    var customMessages = store.LoadAsync().GetAwaiter().GetResult();
    var merged = MessageMergeHelper.MergeWithDefaults(_defaults.Value, customMessages);

    // Atomic swap - thread-safe
    var immutable = merged.ToImmutableDictionary();
    Interlocked.Exchange(ref _custom, immutable);
}
```

**Design Rationale:**
- **Why static?** Single source of truth, globally accessible
- **Why ImmutableDictionary?** Thread-safe reads without locks
- **Why Lazy<T>?** Defer expensive initialization until needed

---

### 3. Message Stores (Storage Layer)

**Message Stores** define how messages are loaded from various sources.

```csharp
public interface IMessageStore
{
    Task<Dictionary<string, MessageTemplate>> LoadAsync();
}
```

**Built-in Stores:**

| Store | Description | Use Case |
|-------|-------------|----------|
| **EmbeddedMessageStore** | Loads from embedded JSON resource | Default messages (100+ built-in) |
| **FileMessageStore** | Loads from JSON file | Custom messages, environment-specific overrides |
| **DatabaseMessageStore** | Loads from database | Dynamic messages, multi-tenant scenarios |
| **DictionaryMessageStore** | Loads from in-memory dictionary | Testing, temporary messages |
| **CompositeMessageStore** | Combines multiple stores with priority | Layered configuration (defaults + custom) |

**Design Rationale:**
- **Why interface?** Extensibility - users can create custom stores
- **Why async?** Support async I/O (database, HTTP, etc.)
- **Why dictionary return?** Simple, predictable contract

---

### 4. Facades (Discoverability Layer)

**Facades** provide a discoverable API for creating messages.

```csharp
public static partial class Msg
{
    public static class Auth
    {
        public static Message LoginFailed() => MessageRegistry.Get("AUTH_001");
        public static Message Unauthorized() => MessageRegistry.Get("AUTH_002");
        // ... 10 total messages
    }

    public static class Crud
    {
        public static Message Created(string? resource = null)
        {
            var message = MessageRegistry.Get("CRUD_001");
            return resource != null ? message.WithParams(new { resource }) : message;
        }
        // ... 10 total messages
    }

    // ... 13 total facade categories
}
```

**Key Characteristics:**
- [✓] **IntelliSense-friendly** - Type `Msg.` to discover all categories
- [✓] **Partial classes** - Easy to extend with custom facades
- [✓] **Optional parameters** - Some messages accept parameters, some don't
- [✓] **Lightweight** - Just thin wrappers around MessageRegistry.Get()

**Design Rationale:**
- **Why static classes?** No instantiation needed, cleaner API
- **Why nested classes?** Logical grouping (Msg.Auth, Msg.Crud, etc.)
- **Why partial?** Users can extend with custom facades

---

### 5. Extension Methods (Fluent API)

**Extension methods** enable fluent message enrichment and formatting.

```csharp
// Data enrichment extensions
public static partial class MessageExtensions
{
    public static Message WithData(this Message message, object data);
    public static Message WithMetadata(this Message message, string key, object value);
    public static Message WithCorrelationId(this Message message, string correlationId);
    public static Message WithHint(this Message message, string hint);
    public static Message WithStatusCode(this Message message, int statusCode);
    public static Message WithParams(this Message message, object parameters);
}

// Output extensions
public static partial class MessageExtensions
{
    public static string ToJson(this Message message, FormatterOptions? options = null);
    public static string ToXml(this Message message, FormatterOptions? options = null);
    public static string ToPlainText(this Message message, FormatterOptions? options = null);
    public static void ToConsole(this Message message, bool useColors = true);
}

// ASP.NET Core extensions
public static class MessageResultExtensions
{
    public static IActionResult ToApiResponse(this Message message);
    public static IActionResult ToCreated(this Message message, string location);
    public static IResult ToMinimalApiResult(this Message message);
}

// Logging extensions
public static class MessageLoggingExtensions
{
    public static Message Log(this Message message, ILogger logger);
    public static Message LogIf(this Message message, ILogger logger, bool condition);
}
```

**Key Characteristics:**
- [✓] **Fluent API** - Chain multiple methods
- [✓] **Immutability preserved** - Each method returns a new instance
- [✓] **Partial classes** - Split by concern (Data, Output, Logging)
- [✓] **Optional parameters** - Sensible defaults

**Design Rationale:**
- **Why extension methods?** Non-intrusive, discoverable, chainable
- **Why partial classes?** Organize extensions by concern
- **Why return new instances?** Preserve immutability

---

### 6. Interceptors (Cross-Cutting Concerns)

**Interceptors** hook into the formatting pipeline to modify messages globally.

```csharp
public interface IMessageInterceptor
{
    Message OnBeforeFormat(Message message);
    Message OnAfterFormat(Message message);
}

public static class InterceptorRegistry
{
    public static void Register(IMessageInterceptor interceptor);
    public static void Clear();

    internal static Message InvokeBeforeFormat(Message message);
    internal static Message InvokeAfterFormat(Message message);
}
```

**Built-in Interceptors (ASP.NET Core):**
- **CorrelationIdInterceptor** - Adds TraceIdentifier from HttpContext
- **MetadataEnrichmentInterceptor** - Adds request path, method, user info
- **LoggingInterceptor** - Logs all messages automatically

**Design Rationale:**
- **Why global?** Cross-cutting concerns apply to all messages
- **Why two hooks?** OnBeforeFormat for enrichment, OnAfterFormat for cleanup
- **Why sequential execution?** Predictable, debuggable pipeline

---

### 7. Formatters (Output Layer)

**Formatters** convert messages to different output formats.

```csharp
public interface IMessageFormatter
{
    string Format(Message message);
    object FormatAsObject(Message message);
}

public abstract class MessageFormatterBase : IMessageFormatter
{
    protected FormatterOptions Options { get; }

    protected abstract string FormatCore(Message message);

    public string Format(Message message)
    {
        // Apply interceptors
        var enriched = InterceptorRegistry.InvokeBeforeFormat(message);

        // Format
        var result = FormatCore(enriched);

        // Post-processing
        InterceptorRegistry.InvokeAfterFormat(enriched);

        return result;
    }
}
```

**Built-in Formatters:**
- **JsonFormatter** - Converts to JSON string
- **XmlFormatter** - Converts to XML string
- **ConsoleFormatter** - Writes colored output to console
- **PlainTextFormatter** - Converts to plain text

**FormatterOptions:**
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

**Design Rationale:**
- **Why base class?** Share interceptor invocation logic
- **Why FormatterOptions?** Control output verbosity
- **Why separate FormatAsObject?** Support XDocument, JsonElement, etc.

---

## Data Flow

### 1. Message Creation Flow

```
User Code
    │
    ├─→ Msg.Crud.Created("User")
    │       │
    │       ├─→ MessageRegistry.Get("CRUD_001")
    │       │       │
    │       │       ├─→ Check _custom dictionary
    │       │       ├─→ Fallback to _defaults
    │       │       └─→ Return MessageTemplate
    │       │
    │       └─→ MessageTemplate.ToMessage("CRUD_001")
    │               │
    │               └─→ Create Message record
    │
    └─→ Message { Code = "CRUD_001", Type = Success, ... }
```

---

### 2. Message Enrichment Flow

```
Message Created
    │
    ├─→ .WithData(new { Id = 123 })
    │       │
    │       └─→ message with { Data = ... }  [New instance!]
    │
    ├─→ .WithMetadata("userId", 456)
    │       │
    │       └─→ message with { Metadata = ... }  [New instance!]
    │
    ├─→ .WithCorrelationId("abc-123")
    │       │
    │       └─→ message with { CorrelationId = ... }  [New instance!]
    │
    └─→ Enriched Message
```

---

### 3. Message Formatting Flow

```
Enriched Message
    │
    ├─→ .ToJson()
    │       │
    │       ├─→ InterceptorRegistry.InvokeBeforeFormat(message)
    │       │       │
    │       │       ├─→ CorrelationIdInterceptor.OnBeforeFormat(message)
    │       │       │       └─→ Add TraceIdentifier → message'
    │       │       │
    │       │       ├─→ MetadataEnrichmentInterceptor.OnBeforeFormat(message')
    │       │       │       └─→ Add request context → message''
    │       │       │
    │       │       └─→ LoggingInterceptor.OnBeforeFormat(message'')
    │       │               └─→ Log message → message'''
    │       │
    │       ├─→ JsonFormatter.FormatCore(message''')
    │       │       │
    │       │       └─→ Serialize to JSON string
    │       │
    │       ├─→ InterceptorRegistry.InvokeAfterFormat(message''')
    │       │       └─→ (Post-processing if needed)
    │       │
    │       └─→ Return JSON string
    │
    └─→ JSON Output
```

---

### 4. ASP.NET Core API Response Flow

```
Enriched Message
    │
    ├─→ .ToApiResponse()
    │       │
    │       ├─→ Invoke interceptor pipeline (same as above)
    │       │
    │       ├─→ Convert to ApiResponse model
    │       │       {
    │       │         "success": true,
    │       │         "code": "CRUD_001",
    │       │         "type": "success",
    │       │         "title": "User Created",
    │       │         "description": "User has been created successfully.",
    │       │         "data": { "id": 123 },
    │       │         "metadata": { "userId": 456 }
    │       │       }
    │       │
    │       ├─→ Map HttpStatusCode to IActionResult
    │       │       │
    │       │       ├─→ 200 → OkObjectResult
    │       │       ├─→ 201 → CreatedResult
    │       │       ├─→ 404 → NotFoundObjectResult
    │       │       └─→ ... (other status codes)
    │       │
    │       └─→ Return IActionResult
    │
    └─→ HTTP Response (200 OK with JSON body)
```

---

## Design Patterns Used

### 1. Facade Pattern

**Where:** `Msg.Auth`, `Msg.Crud`, etc.

**Why:** Simplifies API, provides discoverability via IntelliSense

**Example:**
```csharp
// Instead of:
MessageRegistry.Get("AUTH_001")

// Users write:
Msg.Auth.LoginFailed()
```

---

### 2. Registry Pattern

**Where:** `MessageRegistry`, `InterceptorRegistry`, `FormatterRegistry`

**Why:** Centralized management, global access, extensibility

**Example:**
```csharp
// Register custom formatter
FormatterRegistry.Register("csv", () => new CsvFormatter());

// Use it
var csv = message.ToFormat("csv");
```

---

### 3. Strategy Pattern

**Where:** `IMessageFormatter`, `IMessageStore`

**Why:** Swap implementations at runtime, easy to extend

**Example:**
```csharp
// Different formatting strategies
IMessageFormatter formatter = useJson
    ? new JsonFormatter()
    : new XmlFormatter();

var output = formatter.Format(message);
```

---

### 4. Template Method Pattern

**Where:** `MessageFormatterBase`

**Why:** Share common logic (interceptor invocation), customize formatting

**Example:**
```csharp
public abstract class MessageFormatterBase : IMessageFormatter
{
    // Template method
    public string Format(Message message)
    {
        var enriched = InterceptorRegistry.InvokeBeforeFormat(message);
        var result = FormatCore(enriched);  // ← Subclass implements this
        InterceptorRegistry.InvokeAfterFormat(enriched);
        return result;
    }

    protected abstract string FormatCore(Message message);
}
```

---

### 5. Interceptor Pattern (Chain of Responsibility)

**Where:** `IMessageInterceptor`, `InterceptorRegistry`

**Why:** Add cross-cutting concerns without modifying core logic

**Example:**
```csharp
// Register interceptors (chain)
InterceptorRegistry.Register(new CorrelationIdInterceptor(...));
InterceptorRegistry.Register(new MetadataEnrichmentInterceptor(...));
InterceptorRegistry.Register(new LoggingInterceptor(...));

// They all execute in order
var formatted = message.ToJson();
```

---

### 6. Builder Pattern (Fluent API)

**Where:** Extension methods (`WithData()`, `WithMetadata()`, etc.)

**Why:** Readable, chainable message construction

**Example:**
```csharp
var message = Msg.Crud.Created("User")
    .WithData(new { Id = 123 })
    .WithMetadata("source", "API")
    .WithCorrelationId("abc-123")
    .Log(_logger);
```

---

### 7. Factory Pattern

**Where:** Facade methods, `MessageRegistry.Get()`

**Why:** Encapsulate message creation logic

**Example:**
```csharp
public static Message Created(string? resource = null)
{
    var message = MessageRegistry.Get("CRUD_001");
    return resource != null
        ? message.WithParams(new { resource })
        : message;
}
```

---

### 8. Options Pattern (ASP.NET Core)

**Where:** `EasyMessagesOptions`, `FormatterOptions`, `InterceptorOptions`

**Why:** Configuration via DI, strongly-typed settings

**Example:**
```csharp
builder.Services.AddEasyMessages(builder.Configuration, options =>
{
    options.Formatter.IncludeTimestamp = false;
    options.Interceptors.AutoAddCorrelationId = true;
});
```

---

## Thread Safety

### Immutable Message

**Problem:** Multiple threads accessing/modifying the same message

**Solution:** Message is an immutable record

```csharp
// Thread A
var message1 = Msg.Auth.LoginFailed();

// Thread B (different instance!)
var message2 = message1.WithData(new { UserId = 123 });

// message1 is unchanged - safe!
```

---

### MessageRegistry

**Problem:** Concurrent Configure() and Get() calls

**Solution:** ImmutableDictionary with atomic swap

```csharp
public static void Configure(IMessageStore store)
{
    var merged = MessageMergeHelper.MergeWithDefaults(_defaults.Value, customMessages);
    var immutable = merged.ToImmutableDictionary();

    // Atomic swap - thread-safe!
    Interlocked.Exchange(ref _custom, immutable);
}

public static Message Get(string code)
{
    // Read from immutable snapshot - no locks needed
    var snapshot = _custom;
    if (snapshot != null && snapshot.TryGetValue(code, out var template))
        return template.ToMessage(code);

    // ...
}
```

---

### InterceptorRegistry

**Problem:** Concurrent Register() and InvokeBeforeFormat() calls

**Solution:** Lock-based synchronization

```csharp
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

---

## Performance Optimizations

### 1. Lazy Loading (Defaults)

```csharp
private static readonly Lazy<Dictionary<string, MessageTemplate>> _defaults = new(() =>
    LoadDefaultsSync()
);
```

**Benefit:** Defaults only loaded when first accessed, not at startup

---

### 2. Immutable Snapshots (Custom Messages)

```csharp
var snapshot = _custom;  // O(1) read, no lock!
```

**Benefit:** Lock-free reads, only Configure() needs locking

---

### 3. String Interning (MessageType)

```csharp
private static readonly Dictionary<MessageType, string> Cache = new()
{
    [MessageType.Success] = "success",
    [MessageType.Error] = "error",
    // ...
};
```

**Benefit:** Avoid repeated string allocations

---

### 4. Pre-allocated Dictionaries

```csharp
private static readonly Dictionary<string, object> EmptyMetadata = new(0);

var metadata = message.Metadata?.Count > 0
    ? message.Metadata
    : null;  // Don't serialize empty dictionaries
```

**Benefit:** Reduce allocations, smaller JSON payloads

---

### 5. Optimized String Replacement (Parameters)

```csharp
// Only replace if placeholder exists
if (title.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
{
    title = title.Replace(placeholder, replacement, StringComparison.OrdinalIgnoreCase);
}
```

**Benefit:** Avoid unnecessary string allocations

---

## Extensibility Points

### 1. Custom Message Stores

```csharp
public class RedisMessageStore : IMessageStore
{
    public async Task<Dictionary<string, MessageTemplate>> LoadAsync()
    {
        // Load from Redis
    }
}

MessageRegistry.Configure(new RedisMessageStore());
```

---

### 2. Custom Formatters

```csharp
public class CsvFormatter : MessageFormatterBase
{
    protected override string FormatCore(Message message)
    {
        return $"{message.Code},{message.Type},{message.Title}";
    }
}

FormatterRegistry.Register("csv", () => new CsvFormatter());
message.ToFormat("csv");
```

---

### 3. Custom Interceptors

```csharp
public class AuditInterceptor : IMessageInterceptor
{
    public Message OnBeforeFormat(Message message)
    {
        // Add audit trail
        return message.WithMetadata("auditTime", DateTime.UtcNow);
    }

    public Message OnAfterFormat(Message message) => message;
}

InterceptorRegistry.Register(new AuditInterceptor());
```

---

### 4. Custom Facades

```csharp
public static partial class Msg
{
    public static class Custom
    {
        public static Message PaymentFailed()
            => MessageRegistry.Get("CUSTOM_001");
    }
}

Msg.Custom.PaymentFailed().ToApiResponse();
```

---

## Package Structure

### RecurPixel.EasyMessages (Core)

```
RecurPixel.EasyMessages/
├── Core/
│   ├── Message.cs                     // Immutable message record
│   ├── MessageType.cs                 // Enum (Success, Error, etc.)
│   ├── MessageTemplate.cs             // Template for creating messages
│   └── MessageRegistry.cs             // Static registry
├── Storage/
│   ├── IMessageStore.cs               // Store interface
│   ├── EmbeddedMessageStore.cs        // Built-in defaults
│   ├── FileMessageStore.cs            // JSON file loading
│   ├── DatabaseMessageStore.cs        // Database loading
│   ├── DictionaryMessageStore.cs      // In-memory
│   └── CompositeMessageStore.cs       // Multiple stores
├── Facades/
│   ├── Msg.cs                         // Root facade
│   ├── AuthMessages.cs                // Msg.Auth
│   ├── CrudMessages.cs                // Msg.Crud
│   └── ... (10 more)
├── Formatters/
│   ├── IMessageFormatter.cs           // Formatter interface
│   ├── MessageFormatterBase.cs        // Base class
│   ├── JsonFormatter.cs               // JSON output
│   ├── XmlFormatter.cs                // XML output
│   ├── ConsoleFormatter.cs            // Console output
│   ├── PlainTextFormatter.cs          // Plain text
│   ├── FormatterOptions.cs            // Configuration
│   └── FormatterRegistry.cs           // Registry
├── Interceptors/
│   ├── IMessageInterceptor.cs         // Interceptor interface
│   └── InterceptorRegistry.cs         // Registry
└── Extensions/
    ├── MessageExtensions.Data.cs      // WithData, WithMetadata, etc.
    └── MessageExtensions.Output.cs    // ToJson, ToXml, etc.
```

---

### RecurPixel.EasyMessages.AspNetCore

```
RecurPixel.EasyMessages.AspNetCore/
├── Configuration/
│   ├── EasyMessagesOptions.cs         // DI options
│   ├── InterceptorOptions.cs          // Interceptor config
│   ├── MetadataEnrichmentFields.cs    // Metadata config
│   └── EasyMessagesPresets.cs         // Dev, Prod, Test, Api
├── Interceptors/
│   ├── CorrelationIdInterceptor.cs    // Add TraceIdentifier
│   ├── MetadataEnrichmentInterceptor.cs // Add request context
│   └── LoggingInterceptor.cs          // Auto-logging
├── Extensions/
│   ├── ServiceCollectionExtensions.cs // AddEasyMessages()
│   ├── MessageResultExtensions.cs     // ToApiResponse()
│   └── MessageLoggingExtensions.cs    // Log()
└── Models/
    └── ApiResponse.cs                 // Standard API response
```

---

## Best Practices

### [✓] DO:

1. **Use facades** - `Msg.Auth.LoginFailed()` instead of `MessageRegistry.Get("AUTH_001")`
2. **Chain extension methods** - Fluent API for readability
3. **Use immutability** - Messages return new instances
4. **Configure early** - Call `MessageRegistry.Configure()` at startup
5. **Register interceptors once** - During app initialization
6. **Use IOptions** - For ASP.NET Core configuration
7. **Trust default HTTP status codes** - Override only when necessary

```csharp
// Good
var message = Msg.Crud.Created("User")
    .WithData(new { Id = 123 })
    .WithMetadata("source", "API")
    .Log(_logger)
    .ToApiResponse();
```

---

### [ ] DON'T:

1. **Don't mutate messages** - They're immutable (it won't work anyway)
2. **Don't ignore return values** - Extensions return new instances
3. **Don't register interceptors per-request** - Register once at startup
4. **Don't use Message codes directly** - Use facades for discoverability
5. **Don't call Configure() multiple times** - It overwrites previous configuration
6. **Don't perform expensive operations in interceptors** - Keep them fast

```csharp
// Bad
var message = Msg.Crud.Created("User");
message.WithData(new { Id = 123 });  // [ ] Result discarded!
return message.ToApiResponse();      // [ ] No data!

// Good
var message = Msg.Crud.Created("User")
    .WithData(new { Id = 123 });     // [✓] Returns new instance
return message.ToApiResponse();      // [✓] Has data
```

---

## Next Steps

Now that you understand the architecture, explore:

- **[ASP.NET Core Setup](../ASP-NET-Core/Setup-and-Configuration.md)** - Integrate with ASP.NET Core
- **[Configuration Guide](../ASP-NET-Core/Configuration-Guide.md)** - Advanced configuration
- **[How-To Guides](../How-To-Guides/)** - Practical recipes
- **[Examples](../Examples/)** - Complete working examples

---

**Questions?** [Ask in Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

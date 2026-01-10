# Message Registry and Stores

Learn how EasyMessages stores, retrieves, and manages message templates using the Registry pattern and pluggable storage backends.

---

## What is the Message Registry?

The **MessageRegistry** is a central, thread-safe repository that manages all message templates in your application. It provides fast lookup of message templates by their code and supports custom message storage backends.

```csharp
// Get a message from the registry
var message = MessageRegistry.Get("AUTH_001");

// The registry automatically loads from the embedded store
// and caches messages for fast retrieval
```

**Key Features:**
- [✓] **Thread-safe** - Concurrent access without locks
- [✓] **Fast lookups** - In-memory caching with O(1) retrieval
- [✓] **Lazy loading** - Default messages loaded on first access
- [✓] **Pluggable storage** - Multiple storage backends
- [✓] **Merge support** - Combine multiple message sources
- [✓] **Immutable snapshots** - Atomic updates without race conditions

---

## How Messages Are Stored

### Message Templates

Messages are stored as **templates** that contain the structure and placeholders:

```csharp
public class MessageTemplate
{
    public MessageType Type { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Hint { get; set; }
    public int? HttpStatusCode { get; set; }
}
```

**Example JSON:**
```json
{
  "AUTH_001": {
    "type": "Error",
    "title": "Authentication Failed",
    "description": "Invalid username or password.",
    "httpStatusCode": 401
  }
}
```

### From Template to Message

When you request a message, the registry converts the template:

```csharp
// Template stored in registry
{
  "code": "CRUD_001",
  "type": "Success",
  "title": "Created Successfully",
  "description": "{resource} has been created successfully."
}

// Converted to Message with parameters
var message = MessageRegistry.Get("CRUD_001");
var finalMessage = message.WithParams(new { resource = "User" });

// Result:
// Code: "CRUD_001"
// Title: "Created Successfully"
// Description: "User has been created successfully."
// Timestamp: DateTime.UtcNow
// Metadata: {} (empty)
// Data: null
```

---

## Built-in Message Stores

EasyMessages provides several built-in storage implementations:

### 1. EmbeddedMessageStore (Default)

Loads messages from an embedded JSON resource in the assembly.

**Location:** `RecurPixel.EasyMessages/Resources/messages.json`

**Usage:**
```csharp
// Automatically used - no configuration needed
var message = Msg.Auth.LoginFailed(); // Loads from embedded store
```

**Characteristics:**
- [✓] Zero configuration
- [✓] Fast - no I/O
- [✓] 100+ pre-built messages
- [✓] Always available
- [ ] Cannot be modified at runtime
- [ ] Fixed message set

**Use for:**
- Quick start without configuration
- Standard scenarios covered by built-in messages
- When custom messages aren't needed

### 2. FileMessageStore

Loads messages from external JSON files.

**Usage:**
```csharp
// Single file
MessageRegistry.Configure(new FileMessageStore("messages/custom.json"));

// Or use the convenience method
MessageRegistry.LoadCustomMessages("messages/custom.json");
```

**File Format:**
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

**Characteristics:**
- [✓] Easy to edit (just a JSON file)
- [✓] No rebuild required
- [✓] Can be version controlled
- [✓] Environment-specific messages
- **Warning:** Requires file system access
- **Warning:** Must exist at runtime

**Use for:**
- Custom business messages
- Localization files
- Environment-specific messages
- Tenant-specific messages

### 3. DictionaryMessageStore

Loads messages from an in-memory dictionary (programmatic).

**Usage:**
```csharp
var messages = new Dictionary<string, MessageTemplate>
{
    ["CUSTOM_001"] = new MessageTemplate
    {
        Type = MessageType.Success,
        Title = "Custom Success",
        Description = "Operation completed successfully.",
        HttpStatusCode = 200
    },
    ["CUSTOM_002"] = new MessageTemplate
    {
        Type = MessageType.Error,
        Title = "Custom Error",
        Description = "Operation failed: {reason}",
        HttpStatusCode = 500
    }
};

MessageRegistry.Configure(new DictionaryMessageStore(messages));
```

**Characteristics:**
- [✓] Programmatic control
- [✓] No file system needed
- [✓] Dynamic message generation
- [✓] Perfect for testing
- [ ] Lost on restart (not persisted)
- [ ] More code to maintain

**Use for:**
- Unit tests
- Dynamic message generation
- In-memory caching
- Prototype/development

### 4. DatabaseMessageStore (Abstract Base)

Base class for loading messages from databases.

**Usage:**
```csharp
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
        await connection.OpenAsync();

        var command = new SqlCommand(
            "SELECT Code, Type, Title, Description, HttpStatusCode FROM Messages",
            connection
        );

        var messages = new Dictionary<string, MessageTemplate>();

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var code = reader.GetString(0);
            messages[code] = new MessageTemplate
            {
                Type = Enum.Parse<MessageType>(reader.GetString(1)),
                Title = reader.GetString(2),
                Description = reader.GetString(3),
                HttpStatusCode = reader.IsDBNull(4) ? null : reader.GetInt32(4)
            };
        }

        return messages;
    }
}

// Use it
MessageRegistry.Configure(new SqlServerMessageStore(connectionString));
```

**Characteristics:**
- [✓] Centralized message management
- [✓] Dynamic updates without deployment
- [✓] Multi-tenant support
- [✓] Versioning and auditing
- **Warning:** Requires database connection
- **Warning:** More complex setup
- **Warning:** Network latency on load

**Use for:**
- Enterprise applications
- Multi-tenant systems
- CMS-style message management
- Centralized configuration

### 5. CompositeMessageStore

Combines multiple stores with priority order (last store wins).

**Usage:**
```csharp
MessageRegistry.Configure(new CompositeMessageStore(
    new EmbeddedMessageStore(),        // Base defaults (lowest priority)
    new FileMessageStore("base.json"), // Common messages
    new FileMessageStore("custom.json"), // Custom overrides (highest priority)
    new SqlServerMessageStore(connStr)  // Database overrides (highest priority)
));
```

**How it works:**
1. Loads messages from each store in order
2. Later stores override earlier ones
3. Missing properties fall back to defaults
4. Partial overrides supported

**Example:**
```json
// base.json
{
  "CUSTOM_001": {
    "type": "Success",
    "title": "Operation Complete",
    "description": "The operation completed successfully.",
    "httpStatusCode": 200
  }
}

// custom.json (overrides only description)
{
  "CUSTOM_001": {
    "description": "Your custom operation completed successfully!"
  }
}

// Result: Merged template
{
  "type": "Success",
  "title": "Operation Complete",
  "description": "Your custom operation completed successfully!",
  "httpStatusCode": 200
}
```

**Characteristics:**
- [✓] Flexible layering
- [✓] Partial overrides
- [✓] Environment-specific overrides
- [✓] Incremental customization
- **Warning:** Load order matters
- **Warning:** Can be complex to debug

**Use for:**
- Multi-environment deployments
- Tenant-specific overrides
- Gradual customization
- Layered configuration

---

## Message Registry API

### Getting Messages

```csharp
// Get by code
var message = MessageRegistry.Get("AUTH_001");

// Throws MessageNotFoundException if not found
try
{
    var message = MessageRegistry.Get("UNKNOWN_001");
}
catch (MessageNotFoundException ex)
{
    Console.WriteLine(ex.Message);
    // "Message code 'UNKNOWN_001' not found. Available: AUTH_001, AUTH_002, ..."
}
```

### Listing Available Messages

```csharp
// Get all message codes
var codes = MessageRegistry.GetAllCodes();

foreach (var code in codes)
{
    Console.WriteLine(code);
}
// Output: AUTH_001, AUTH_002, CRUD_001, CRUD_002, ...

// Count messages
var count = MessageRegistry.GetAllCodes().Count();
Console.WriteLine($"Total messages: {count}");
```

### Configuring Custom Stores

```csharp
// Single store
MessageRegistry.Configure(new FileMessageStore("custom.json"));

// Composite store
MessageRegistry.Configure(new CompositeMessageStore(
    new EmbeddedMessageStore(),
    new FileMessageStore("custom.json")
));

// Convenience method for file store
MessageRegistry.LoadCustomMessages("custom.json");
```

### Resetting (Testing Only)

```csharp
// Reset to default state (removes custom messages)
MessageRegistry.Reset();

// Useful in unit tests
[TestCleanup]
public void Cleanup()
{
    MessageRegistry.Reset();
}
```

---

## How Message Loading Works

### Loading Flow

```
1. Application starts
   ↓
2. First message requested (e.g., Msg.Auth.LoginFailed())
   ↓
3. MessageRegistry lazy-loads embedded messages
   ↓
4. Message template retrieved from cache (O(1) lookup)
   ↓
5. Template converted to Message instance
   ↓
6. Message returned to caller
```

### With Custom Store

```
1. Call MessageRegistry.Configure(store)
   ↓
2. Store.LoadAsync() executed
   ↓
3. Templates merged with defaults
   ↓
4. Immutable snapshot created
   ↓
5. Atomic swap (thread-safe)
   ↓
6. Custom messages available immediately
```

### Thread Safety

```csharp
// Registry uses immutable snapshots for thread safety
// Multiple threads can safely:

// Thread 1
var message1 = MessageRegistry.Get("AUTH_001");

// Thread 2 (concurrent)
var message2 = MessageRegistry.Get("CRUD_001");

// Thread 3 (concurrent configuration)
MessageRegistry.Configure(new FileMessageStore("custom.json"));

// No locks needed - immutable snapshots ensure safety
```

---

## Custom Messages

### Creating Custom Messages

**Step 1: Create JSON file**

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
  },
  "REFUND_001": {
    "type": "Success",
    "title": "Refund Processed",
    "description": "Refund of {amount} has been processed to {method}.",
    "httpStatusCode": 200
  }
}
```

**Step 2: Load the file**

```csharp
// Console application
MessageRegistry.LoadCustomMessages("messages/custom.json");

// Or with FileMessageStore
MessageRegistry.Configure(new FileMessageStore("messages/custom.json"));
```

**Step 3: Use custom messages**

```csharp
// Use via Msg.Custom()
var message = Msg.Custom("PAYMENT_001")
    .WithParams(new
    {
        amount = "$50.00",
        transactionId = "TXN-123456"
    });

Console.WriteLine(message.Description);
// Output: "Payment of $50.00 was successful. Transaction ID: TXN-123456"
```

### Partial Overrides

You can override only specific properties:

```json
// Override just the description of built-in message
{
  "AUTH_001": {
    "description": "Your custom authentication failed message."
  }
}
```

Result:
- **Type:** Error (from default)
- **Title:** "Authentication Failed" (from default)
- **Description:** "Your custom authentication failed message." (overridden)
- **HttpStatusCode:** 401 (from default)

---

## Best Practices

### [✓] DO:

**1. Load custom messages at startup**

```csharp
// ASP.NET Core - Program.cs
var builder = WebApplication.CreateBuilder(args);

// Load custom messages before building app
var customPath = builder.Configuration["EasyMessages:CustomMessagesPath"];
if (!string.IsNullOrEmpty(customPath))
{
    MessageRegistry.LoadCustomMessages(customPath);
}

var app = builder.Build();
```

**2. Use CompositeStore for layering**

```csharp
// Layer: Embedded → Common → Environment-specific
MessageRegistry.Configure(new CompositeMessageStore(
    new EmbeddedMessageStore(),                           // Base
    new FileMessageStore("messages/common.json"),         // Shared
    new FileMessageStore($"messages/{env}.json")          // Dev/Prod overrides
));
```

**3. Organize custom messages by category**

```json
// messages/payment.json
{
  "PAYMENT_001": { ... },
  "PAYMENT_002": { ... }
}

// messages/notifications.json
{
  "NOTIFY_001": { ... },
  "NOTIFY_002": { ... }
}
```

**4. Include all required properties in custom messages**

```json
{
  "CUSTOM_001": {
    "type": "Success",          // Required
    "title": "Success",         // Required
    "description": "Done.",     // Required
    "httpStatusCode": 200       // Optional but recommended
  }
}
```

**5. Use consistent code prefixes**

```json
{
  "PAY_001": { ... },   // Payment messages
  "PAY_002": { ... },
  "NOTIF_001": { ... }, // Notification messages
  "NOTIF_002": { ... }
}
```

### [ ] DON'T:

**1. Don't configure the registry multiple times**

```csharp
// Bad - overwrites previous configuration
MessageRegistry.Configure(new FileMessageStore("file1.json"));
MessageRegistry.Configure(new FileMessageStore("file2.json")); // Replaces file1

// Good - use CompositeStore
MessageRegistry.Configure(new CompositeMessageStore(
    new FileMessageStore("file1.json"),
    new FileMessageStore("file2.json")
));
```

**2. Don't load messages in hot paths**

```csharp
// Bad - loads on every request
public IActionResult Get()
{
    MessageRegistry.LoadCustomMessages("custom.json"); // [ ] Slow!
    return Msg.Crud.Retrieved("User").ToApiResponse();
}

// Good - load at startup
public void Configure()
{
    MessageRegistry.LoadCustomMessages("custom.json"); // [✓] Once at startup
}
```

**3. Don't use MessageRegistry.Reset() in production**

```csharp
// Only for testing
[TestCleanup]
public void Cleanup()
{
    MessageRegistry.Reset(); // [✓] OK in tests
}

// Never in production
public void ProductionCode()
{
    MessageRegistry.Reset(); // [ ] Removes all custom messages!
}
```

**4. Don't hardcode file paths**

```csharp
// Bad
MessageRegistry.LoadCustomMessages("C:\\MyApp\\messages.json");

// Good - use configuration
var path = configuration["EasyMessages:CustomMessagesPath"];
MessageRegistry.LoadCustomMessages(path);
```

**5. Don't forget error handling**

```csharp
// Bad
MessageRegistry.Configure(new FileMessageStore("missing.json")); // Throws!

// Good
try
{
    var path = configuration["EasyMessages:CustomMessagesPath"];
    if (File.Exists(path))
    {
        MessageRegistry.LoadCustomMessages(path);
        logger.LogInformation("Loaded custom messages from {Path}", path);
    }
    else
    {
        logger.LogWarning("Custom messages file not found: {Path}", path);
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to load custom messages");
    // Application continues with default messages
}
```

---

## Advanced Scenarios

### Scenario 1: Multi-Tenant Messages

```csharp
public class TenantMessageStore : IMessageStore
{
    private readonly string _tenantId;
    private readonly IDbConnection _connection;

    public async Task<Dictionary<string, MessageTemplate>> LoadAsync()
    {
        var messages = await _connection.QueryAsync<MessageTemplate>(
            "SELECT * FROM TenantMessages WHERE TenantId = @TenantId",
            new { TenantId = _tenantId }
        );

        return messages.ToDictionary(m => m.Code);
    }
}

// Configure per-tenant at request time (if needed)
// Or cache per tenant
```

### Scenario 2: Localized Messages

```csharp
// Load locale-specific messages
var locale = CultureInfo.CurrentCulture.Name; // "en-US", "es-ES", etc.

MessageRegistry.Configure(new CompositeMessageStore(
    new EmbeddedMessageStore(),                          // Default (en-US)
    new FileMessageStore($"messages/{locale}.json")      // Localized
));
```

```json
// messages/es-ES.json
{
  "AUTH_001": {
    "title": "Autenticación Fallida",
    "description": "Usuario o contraseña inválidos."
  }
}
```

### Scenario 3: Hot Reload

```csharp
// Watch for file changes and reload
var watcher = new FileSystemWatcher("messages");
watcher.Changed += (s, e) =>
{
    MessageRegistry.LoadCustomMessages(e.FullPath);
    Console.WriteLine($"Reloaded messages from {e.FullPath}");
};
watcher.EnableRaisingEvents = true;
```

### Scenario 4: Versioned Messages

```csharp
public class VersionedMessageStore : IMessageStore
{
    private readonly string _version;

    public async Task<Dictionary<string, MessageTemplate>> LoadAsync()
    {
        // Load messages for specific API version
        var path = $"messages/v{_version}.json";
        return await File.ReadAllTextAsync(path)
            .ContinueWith(t => JsonSerializer.Deserialize<Dictionary<string, MessageTemplate>>(t.Result));
    }
}
```

---

## Performance Considerations

### Registry Performance

```
Operation              | Time       | Notes
-----------------------|------------|---------------------------
First message access   | ~100-500ms | Lazy loading embedded store
Subsequent access      | ~100-200ns | In-memory O(1) lookup
Configure new store    | ~10-50ms   | One-time merge operation
Get all codes          | ~1-5μs     | Iterate immutable collection
```

### Optimization Tips

1. **Load custom messages at startup** - Not per request
2. **Use CompositeStore** - Better than multiple Configure calls
3. **Cache message instances** - If used repeatedly
4. **Keep custom message files small** - < 1000 messages recommended
5. **Use embedded store for defaults** - Fastest option

---

## Troubleshooting

### Message Not Found

```csharp
// Error: MessageNotFoundException: Message code 'CUSTOM_001' not found
// Cause: Message not loaded or wrong code

// Solution 1: Check if file is loaded
var codes = MessageRegistry.GetAllCodes();
Console.WriteLine(string.Join(", ", codes));

// Solution 2: Verify file path
if (!File.Exists("messages/custom.json"))
    Console.WriteLine("File not found!");

// Solution 3: Check JSON format
// Ensure code matches exactly (case-sensitive)
```

### Custom Messages Not Working

```csharp
// Cause: Loaded before messages are used
// Solution: Load at startup, before any Msg.* calls

// Wrong order
Msg.Custom("PAYMENT_001"); // [ ] Not loaded yet!
MessageRegistry.LoadCustomMessages("custom.json");

// Correct order
MessageRegistry.LoadCustomMessages("custom.json");
Msg.Custom("PAYMENT_001"); // [✓] Works!
```

### File Not Found at Runtime

```csharp
// Use relative paths from app root
MessageRegistry.LoadCustomMessages("messages/custom.json");

// Or absolute paths
var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "messages", "custom.json");
MessageRegistry.LoadCustomMessages(path);
```

---

## Next Steps

- **[Facades (Msg.Auth, Msg.Crud, etc.)](Facades.md)** - Learn about message facades
- **[Create Custom Messages Guide](../How-To-Guides/Create-Custom-Messages.md)** - Step-by-step tutorial
- **[Implement Custom Stores Guide](../How-To-Guides/Implement-Custom-Stores.md)** - Build your own store
- **[Message Codes Reference](../API-Reference/Message-Codes-Reference.md)** - All built-in messages

---

**Questions?** [Ask in Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

# Messages and Message Types

Understanding the Message structure and types is fundamental to using EasyMessages effectively.

---

## What is a Message?

A **Message** in EasyMessages is an immutable record that represents a standardized communication about an operation, event, or state in your application.

```csharp
var message = Msg.Auth.LoginFailed();

// Message is a sealed record - immutable and thread-safe
Console.WriteLine(message.Code);        // "AUTH_001"
Console.WriteLine(message.Type);        // MessageType.Error
Console.WriteLine(message.Title);       // "Authentication Failed"
Console.WriteLine(message.Description); // "Invalid username or password."
```

---

## Message Structure

Every message consists of three main parts: **Identity**, **Content**, and **Context**.

### 1. Identity Properties

These uniquely identify the message:

```csharp
public sealed record Message
{
    public string Code { get; init; }         // e.g., "AUTH_001"
    public MessageType Type { get; init; }    // Success, Info, Warning, Error, Critical
}
```

**Code Format:** `PREFIX_NNN`
- **PREFIX:** Category identifier (3-6 uppercase letters)
  - `AUTH` - Authentication
  - `CRUD` - CRUD operations
  - `VAL` - Validation
  - `SYS` - System
  - `DB` - Database
  - `FILE` - File operations
  - `NET` - Network
  - And more...
- **NNN:** Sequential number (001-999)

### 2. Content Properties

These describe what the message is about:

```csharp
public string Title { get; init; }        // Short summary
public string Description { get; init; }  // Detailed explanation
public string? Hint { get; init; }        // Optional suggestion/guidance
```

**Example:**
```csharp
var message = Msg.Validation.RequiredField("Email");

// Title: "Required Field Missing"
// Description: "Email is required."
// Hint: null (or could be "Please provide a valid email address")
```

### 3. Context Properties

These provide additional information about when and where:

```csharp
public DateTime Timestamp { get; init; }      // When created (UTC)
public string? CorrelationId { get; init; }   // For distributed tracing
public int? HttpStatusCode { get; init; }     // Suggested HTTP status
```

### 4. Data Properties

These carry custom information:

```csharp
public object? Data { get; init; }                        // Payload data
public Dictionary<string, object?> Parameters { get; init; } // Template parameters
public Dictionary<string, object> Metadata { get; init; }    // Additional context
```

**Example:**
```csharp
var message = Msg.Crud.Created("User")
    .WithData(new { Id = 123, Name = "John" })
    .WithMetadata("source", "API")
    .WithMetadata("ipAddress", "192.168.1.1")
    .WithCorrelationId("abc-123");
```

---

## Message Types

EasyMessages defines 5 message types that indicate severity and purpose:

```csharp
public enum MessageType
{
    Success = 0,    // Operation succeeded
    Info = 1,       // Informational message
    Warning = 2,    // Warning - something needs attention
    Error = 3,      // Error - operation failed
    Critical = 4    // Critical - system integrity affected
}
```

### Success (MessageType.Success)

**Purpose:** Indicate successful operations

**Characteristics:**
- [✓] Operation completed successfully
- [✓] Positive outcome
- [✓] User action worked as intended
- [✓] Typically logged at Information level

**HTTP Status Codes:** Usually 200-299 (2xx Success)

**Examples:**
```csharp
Msg.Auth.LoginSuccess()          // Type: Success, HTTP: 200
Msg.Crud.Created("Product")      // Type: Success, HTTP: 200
Msg.Crud.Updated("User")         // Type: Success, HTTP: 200
Msg.System.Success()             // Type: Success, HTTP: 200
```

**Console Output:** Green ✓

**When to Use:**
- User successfully logs in
- Resource created/updated/deleted
- File uploaded successfully
- Payment processed
- Data imported

### Info (MessageType.Info)

**Purpose:** Provide informational messages without implying success or failure

**Characteristics:**
- ℹ️ Neutral information
- ℹ️ State updates
- ℹ️ Progress notifications
- ℹ️ Typically logged at Information level

**HTTP Status Codes:** Usually 200, 202 (Accepted), or custom

**Examples:**
```csharp
Msg.System.Processing()          // Type: Info, HTTP: 200
Msg.Search.InProgress()          // Type: Info, HTTP: 200
Msg.Import.Processing()          // Type: Info, HTTP: 202
```

**Console Output:** Blue ℹ

**When to Use:**
- Long-running operation in progress
- Queue item received
- Background job started
- Status updates
- Non-error notifications

### Warning (MessageType.Warning)

**Purpose:** Alert about potential issues that don't prevent operation completion

**Characteristics:**
- **Warning:** Something needs attention
- **Warning:** Operation completed but with caveats
- **Warning:** Potential problem detected
- **Warning:** Typically logged at Warning level

**HTTP Status Codes:** Usually 200, 207 (Multi-Status), or 4xx

**Examples:**
```csharp
Msg.Crud.NoChangesDetected()     // Type: Warning, HTTP: 200
Msg.File.SizeWarning()           // Type: Warning, HTTP: 200
Msg.System.Deprecated()          // Type: Warning, HTTP: 200
```

**Console Output:** Yellow ⚠

**When to Use:**
- Update succeeded but no changes made
- Resource approaching limits
- Deprecated feature used
- Partial success
- Data quality issues

### Error (MessageType.Error)

**Purpose:** Indicate operation failures

**Characteristics:**
- [ ] Operation failed
- [ ] Expected error condition
- [ ] Recoverable failure
- [ ] Typically logged at Error level

**HTTP Status Codes:** Usually 400-499 (4xx Client Error) or 500-599 (5xx Server Error)

**Examples:**
```csharp
Msg.Auth.LoginFailed()           // Type: Error, HTTP: 401
Msg.Crud.NotFound("User")        // Type: Error, HTTP: 404
Msg.Validation.Failed()          // Type: Error, HTTP: 422
Msg.System.Error()               // Type: Error, HTTP: 500
```

**Console Output:** Red ✗

**When to Use:**
- Authentication/authorization failed
- Resource not found
- Validation failed
- Business rule violation
- Database error
- External service failure

### Critical (MessageType.Critical)

**Purpose:** Indicate severe system failures requiring immediate attention

**Characteristics:**
- 🔴 System integrity compromised
- 🔴 Immediate action required
- 🔴 Data corruption possible
- 🔴 Typically logged at Critical level

**HTTP Status Codes:** Usually 500-599 (5xx Server Error)

**Examples:**
```csharp
Msg.Database.ConnectionLost()    // Type: Critical, HTTP: 503
Msg.System.OutOfMemory()         // Type: Critical, HTTP: 500
Msg.System.DataCorruption()      // Type: Critical, HTTP: 500
```

**Console Output:** Red ‼️

**When to Use:**
- Database unavailable
- Out of memory
- Disk full
- Configuration corruption
- Security breach detected
- Data integrity violated

---

## Message Type Selection Guide

| Scenario | Type | Example |
|----------|------|---------|
| [✓] User logged in successfully | Success | `Msg.Auth.LoginSuccess()` |
| ℹ️ Processing file | Info | `Msg.System.Processing()` |
| **Warning:** Update with no changes | Warning | `Msg.Crud.NoChangesDetected()` |
| [ ] Invalid email format | Error | `Msg.Validation.InvalidEmail()` |
| 🔴 Database connection lost | Critical | `Msg.Database.ConnectionLost()` |
| [✓] Product created | Success | `Msg.Crud.Created("Product")` |
| ℹ️ Search started | Info | `Msg.Search.InProgress()` |
| **Warning:** File size near limit | Warning | `Msg.File.SizeWarning()` |
| [ ] Product not found | Error | `Msg.Crud.NotFound("Product")` |
| 🔴 Out of disk space | Critical | `Msg.System.DiskFull()` |

---

## HTTP Status Code Mapping

EasyMessages automatically maps message types to appropriate HTTP status codes:

### Success Messages → 2xx

```csharp
Msg.Crud.Created("User")         // HTTP 200 OK (could be 201 Created)
Msg.Crud.Updated("User")         // HTTP 200 OK
Msg.Crud.Deleted("User")         // HTTP 200 OK (could be 204 No Content)
Msg.Auth.LoginSuccess()          // HTTP 200 OK
```

### Info Messages → 2xx

```csharp
Msg.System.Processing()          // HTTP 200 OK
Msg.Import.Processing()          // HTTP 202 Accepted
```

### Warning Messages → 2xx or 4xx

```csharp
Msg.Crud.NoChangesDetected()     // HTTP 200 OK
Msg.File.SizeWarning()           // HTTP 200 OK
```

### Error Messages → 4xx or 5xx

**Client Errors (4xx):**
```csharp
Msg.Auth.Unauthorized()          // HTTP 403 Forbidden
Msg.Crud.NotFound("User")        // HTTP 404 Not Found
Msg.Validation.Failed()          // HTTP 422 Unprocessable Entity
Msg.Auth.LoginFailed()           // HTTP 401 Unauthorized
```

**Server Errors (5xx):**
```csharp
Msg.System.Error()               // HTTP 500 Internal Server Error
Msg.Database.ConnectionFailed()  // HTTP 503 Service Unavailable
Msg.Network.Timeout()            // HTTP 504 Gateway Timeout
```

### Critical Messages → 5xx

```csharp
Msg.System.OutOfMemory()         // HTTP 500 Internal Server Error
Msg.Database.ConnectionLost()    // HTTP 503 Service Unavailable
Msg.System.DataCorruption()      // HTTP 500 Internal Server Error
```

### Overriding HTTP Status Codes

```csharp
// Use the automatic mapping
return Msg.Crud.NotFound("User").ToApiResponse(); // Returns 404

// Override if needed
return Msg.Auth.LoginFailed()
    .WithStatusCode(429) // Too Many Requests
    .ToApiResponse();
```

---

## Message Immutability

Messages are **immutable records** - once created, they cannot be changed:

```csharp
// This creates a NEW message, doesn't modify the original
var message1 = Msg.Auth.LoginFailed();
var message2 = message1.WithData(new { UserId = 123 });

// message1 is unchanged
Console.WriteLine(message1.Data == null);  // true

// message2 is a new instance with data
Console.WriteLine(message2.Data != null);  // true
```

**Benefits:**
- [✓] **Thread-safe** - Can be safely shared across threads
- [✓] **Predictable** - No side effects
- [✓] **Testable** - Easy to verify in tests
- [✓] **Cacheable** - Safe to cache without cloning

---

## Message Categories

EasyMessages organizes messages into logical categories:

### Authentication & Authorization (AUTH)
```csharp
Msg.Auth.LoginFailed()
Msg.Auth.LoginSuccess()
Msg.Auth.Unauthorized()
Msg.Auth.TokenExpired()
// 10 total messages
```

### CRUD Operations (CRUD)
```csharp
Msg.Crud.Created("Entity")
Msg.Crud.Updated("Entity")
Msg.Crud.Deleted("Entity")
Msg.Crud.NotFound("Entity")
Msg.Crud.Retrieved("Entity")
// 10 total messages
```

### Validation (VAL)
```csharp
Msg.Validation.Failed()
Msg.Validation.RequiredField("Field")
Msg.Validation.InvalidFormat("Field")
Msg.Validation.InvalidEmail()
// 15 total messages
```

### System (SYS)
```csharp
Msg.System.Error()
Msg.System.Processing()
Msg.System.Success()
// 10 total messages
```

### Database (DB)
```csharp
Msg.Database.ConnectionFailed()
Msg.Database.QueryTimeout()
Msg.Database.ConnectionLost()
// 8 total messages
```

### File Operations (FILE)
```csharp
Msg.File.Uploaded()
Msg.File.InvalidType("pdf", "docx")
Msg.File.SizeExceeded()
// 12 total messages
```

### Network (NET)
```csharp
Msg.Network.Timeout()
Msg.Network.ServiceUnavailable()
// 10 total messages
```

**Total: 100+ built-in messages**

See [Message Codes Reference](../API-Reference/Message-Codes-Reference.md) for the complete list.

---

## Creating Messages

### Using Facades (Recommended)

```csharp
// Simplest way - use pre-built messages
var message = Msg.Auth.LoginFailed();
```

### With Parameters

```csharp
// Messages with placeholders
var message = Msg.Crud.Created("User");
// Description: "User has been created successfully."

var message = Msg.Validation.RequiredField("Email");
// Description: "Email is required."
```

### With Data

```csharp
// Add payload data
var message = Msg.Crud.Created("Product")
    .WithData(new
    {
        Id = 123,
        Name = "Widget",
        Price = 19.99
    });
```

### With Metadata

```csharp
// Add contextual information
var message = Msg.Auth.LoginFailed()
    .WithMetadata("ipAddress", "192.168.1.1")
    .WithMetadata("attempt", 3)
    .WithMetadata("userAgent", "Mozilla/5.0...");
```

### With Correlation ID

```csharp
// For distributed tracing
var message = Msg.System.Processing()
    .WithCorrelationId("abc-123-def-456");
```

### Chaining Methods

```csharp
// Chain multiple methods
var message = Msg.Crud.Created("Order")
    .WithData(new { OrderId = 789, Total = 99.99 })
    .WithMetadata("source", "WebAPI")
    .WithMetadata("userId", 123)
    .WithCorrelationId(Guid.NewGuid().ToString())
    .WithStatusCode(201);
```

---

## Message Inspection

```csharp
var message = Msg.Auth.LoginFailed()
    .WithData(new { UserId = 123 })
    .WithMetadata("attempt", 3);

// Check message type
if (message.Type == MessageType.Error)
{
    Console.WriteLine("Operation failed");
}

// Access properties
Console.WriteLine($"Code: {message.Code}");
Console.WriteLine($"Title: {message.Title}");
Console.WriteLine($"Description: {message.Description}");
Console.WriteLine($"Timestamp: {message.Timestamp}");
Console.WriteLine($"HTTP Status: {message.HttpStatusCode}");

// Access data
var data = message.Data as dynamic;
Console.WriteLine($"User ID: {data?.UserId}");

// Access metadata
if (message.Metadata.TryGetValue("attempt", out var attempt))
{
    Console.WriteLine($"Attempt: {attempt}");
}
```

---

## Console Output by Type

Messages display differently in the console based on their type:

```csharp
// Success (Green ✓)
Msg.Crud.Created("User").ToConsole(useColors: true);
// ✓ User Created
//   User has been created successfully.
//   [2026-01-09 14:30:00] [CRUD_001]

// Info (Blue ℹ)
Msg.System.Processing().ToConsole(useColors: true);
// ℹ Processing
//   Your request is being processed.
//   [2026-01-09 14:30:00] [SYS_002]

// Warning (Yellow ⚠)
Msg.Crud.NoChangesDetected().ToConsole(useColors: true);
// ⚠ No Changes Detected
//   No changes were detected to update.
//   [2026-01-09 14:30:00] [CRUD_009]

// Error (Red ✗)
Msg.Auth.LoginFailed().ToConsole(useColors: true);
// ✗ Authentication Failed
//   Invalid username or password.
//   [2026-01-09 14:30:00] [AUTH_001]

// Critical (Red ‼️)
Msg.Database.ConnectionLost().ToConsole(useColors: true);
// ‼️ Database Connection Lost
//   Connection to database was lost.
//   [2026-01-09 14:30:00] [DB_008]
```

---

## Best Practices

### [✓] DO:

- **Use specific message types** - Choose the right type for the situation
- **Include relevant data** - Add data and metadata for debugging
- **Use correlation IDs** - Especially in distributed systems
- **Let HTTP status codes be automatic** - Override only when necessary
- **Chain methods for clarity** - Build messages fluently

```csharp
// Good
return Msg.Crud.NotFound("Product")
    .WithMetadata("productId", id)
    .WithMetadata("requestedBy", userId)
    .WithCorrelationId(correlationId)
    .Log(_logger)
    .ToApiResponse();
```

### [ ] DON'T:

- **Misuse message types** - Don't use Error for warnings
- **Ignore immutability** - Remember messages create new instances
- **Override HTTP codes unnecessarily** - Trust the defaults
- **Include sensitive data** - Be careful with PII in metadata
- **Create messages without context** - Add metadata for debugging

```csharp
// Bad - including password in metadata
Msg.Auth.LoginFailed()
    .WithMetadata("password", password) // [ ] Security risk!
    .ToApiResponse();

// Good
Msg.Auth.LoginFailed()
    .WithMetadata("username", username)
    .WithMetadata("ipAddress", ipAddress)
    .ToApiResponse();
```

---

## Next Steps

- **[Message Registry and Stores](Message-Registry-and-Stores.md)** - Learn how messages are stored and loaded
- **[Facades (Msg.Auth, Msg.Crud, etc.)](Facades.md)** - Understand the facade pattern
- **[Formatters and Outputs](Formatters-and-Outputs.md)** - Convert messages to different formats
- **[Message Codes Reference](../API-Reference/Message-Codes-Reference.md)** - Complete list of all messages

---

**Questions?** [Ask in Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

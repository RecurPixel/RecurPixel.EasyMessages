# Facades (Msg.Auth, Msg.Crud, etc.)

Learn how EasyMessages uses the Facade pattern to provide a clean, discoverable API for creating messages.

---

## What are Facades?

**Facades** are static classes that provide simple, intuitive methods for creating messages without needing to know message codes or templates.

```csharp
// Instead of this:
var message = MessageRegistry.Get("AUTH_001");

// Use this:
var message = Msg.Auth.LoginFailed();
```

**Benefits:**
- [✓] **IntelliSense-driven** - Discover messages as you type
- [✓] **Type-safe** - Compile-time validation
- [✓] **Self-documenting** - Method names explain what they do
- [✓] **No magic strings** - No need to memorize message codes
- [✓] **Refactoring-friendly** - IDEs can find all usages

---

## The Msg Class

All facades are accessed through the static `Msg` class:

```csharp
Msg.Auth.*         // Authentication messages
Msg.Crud.*         // CRUD operation messages
Msg.Validation.*   // Validation messages
Msg.System.*       // System messages
Msg.Database.*     // Database messages
Msg.File.*         // File operation messages
Msg.Network.*      // Network messages
Msg.Payment.*      // Payment messages
Msg.Email.*        // Email messages
Msg.Search.*       // Search messages
Msg.Import.*       // Import messages
Msg.Export.*       // Export messages
Msg.Custom(code)   // Custom messages
```

---

## Available Facades

### 1. Msg.Auth - Authentication & Authorization

**Purpose:** User authentication, authorization, and session management

```csharp
// Login scenarios
Msg.Auth.LoginFailed()              // AUTH_001 - Invalid credentials
Msg.Auth.LoginSuccessful()          // AUTH_003 - Login succeeded
Msg.Auth.LogoutSuccessful()         // AUTH_007 - Logout succeeded

// Authorization
Msg.Auth.Unauthorized()             // AUTH_002 - No permission
Msg.Auth.Forbidden()                // AUTH_002 - Alias for Unauthorized

// Tokens & sessions
Msg.Auth.TokenExpired()             // AUTH_004 - Token/session timeout
Msg.Auth.InvalidToken()             // AUTH_005 - Invalid JWT
Msg.Auth.InvalidRefreshToken()      // AUTH_009 - Cannot refresh
Msg.Auth.SessionExpired()           // AUTH_004 - Session timeout

// Account status
Msg.Auth.AccountLocked()            // AUTH_006 - Too many attempts
Msg.Auth.PasswordResetRequired()    // AUTH_008 - Force password change
Msg.Auth.MfaRequired()              // AUTH_010 - Need MFA
```

**Common Usage:**
```csharp
// Login endpoint
[HttpPost("login")]
public IActionResult Login(LoginDto dto)
{
    if (!_authService.Validate(dto))
        return Msg.Auth.LoginFailed().ToApiResponse();

    var token = _authService.GenerateToken(dto);
    return Msg.Auth.LoginSuccessful()
        .WithData(new { token })
        .ToApiResponse();
}

// Protected resource
[HttpGet("protected")]
public IActionResult GetProtected()
{
    if (!User.Identity.IsAuthenticated)
        return Msg.Auth.Unauthorized().ToApiResponse();

    // Return protected data
}
```

### 2. Msg.Crud - CRUD Operations

**Purpose:** Create, Read, Update, Delete operations

```csharp
// Success operations
Msg.Crud.Created("User")            // CRUD_001 - Resource created
Msg.Crud.Updated("Product")         // CRUD_002 - Resource updated
Msg.Crud.Deleted("Order")           // CRUD_003 - Resource deleted
Msg.Crud.Retrieved("Customer")      // CRUD_005 - Resource retrieved

// Failures
Msg.Crud.NotFound("User")           // CRUD_004 - 404 Not Found
Msg.Crud.CreationFailed()           // CRUD_006 - POST failed
Msg.Crud.UpdateFailed()             // CRUD_007 - PUT/PATCH failed
Msg.Crud.DeletionFailed()           // CRUD_008 - DELETE failed

// Special cases
Msg.Crud.NoChangesDetected()        // CRUD_009 - Update with no delta
Msg.Crud.ConflictDetected()         // CRUD_010 - Optimistic concurrency
```

**Parameter Support:**
```csharp
// With resource name
Msg.Crud.Created("User")
// Description: "User has been created successfully."

// Without resource name (generic)
Msg.Crud.Created()
// Description: "Resource has been created successfully."

// Multiple resources
Msg.Crud.Retrieved("Users")
// Description: "Users has been retrieved successfully."
```

**Common Usage:**
```csharp
// REST API CRUD
[HttpGet("{id}")]
public IActionResult GetById(int id)
{
    var user = _service.GetById(id);
    if (user == null)
        return Msg.Crud.NotFound("User")
            .WithMetadata("userId", id)
            .ToApiResponse();

    return Msg.Crud.Retrieved("User")
        .WithData(user)
        .ToApiResponse();
}

[HttpPost]
public IActionResult Create(CreateUserDto dto)
{
    var user = _service.Create(dto);
    return Msg.Crud.Created("User")
        .WithData(user)
        .ToApiResponse();
}

[HttpPut("{id}")]
public IActionResult Update(int id, UpdateUserDto dto)
{
    var user = _service.Update(id, dto);
    if (user == null)
        return Msg.Crud.UpdateFailed().ToApiResponse();

    return Msg.Crud.Updated("User")
        .WithData(user)
        .ToApiResponse();
}
```

### 3. Msg.Validation - Input Validation

**Purpose:** Data validation errors

```csharp
// General validation
Msg.Validation.Failed()                     // VAL_001 - General failure

// Field-specific
Msg.Validation.RequiredField("Email")       // VAL_002 - Missing required
Msg.Validation.InvalidFormat("Phone")       // VAL_003 - Format mismatch
Msg.Validation.OutOfRange("Age")            // VAL_004 - Min/max violation
Msg.Validation.TooShort("Password")         // VAL_010 - MinLength
Msg.Validation.TooLong("Description")       // VAL_011 - MaxLength

// Type-specific
Msg.Validation.InvalidEmail()               // VAL_005 - Email format
Msg.Validation.InvalidPhone()               // VAL_006 - Phone format
Msg.Validation.InvalidUrl()                 // VAL_012 - URL format
Msg.Validation.InvalidDate()                // VAL_009 - Date format
Msg.Validation.InvalidFileExtension()       // VAL_013 - File type
Msg.Validation.InvalidCharacters()          // VAL_015 - Character whitelist

// Password validation
Msg.Validation.PasswordTooWeak()            // VAL_007 - Weak password
Msg.Validation.PasswordsMismatch()          // VAL_008 - Confirmation fail

// Uniqueness
Msg.Validation.DuplicateValue("Email")      // VAL_014 - Already exists
```

**Common Usage:**
```csharp
// Model validation
[HttpPost]
public IActionResult Create(CreateUserDto dto)
{
    if (!ModelState.IsValid)
    {
        return Msg.Validation.Failed()
            .WithData(ModelState)
            .ToApiResponse();
    }

    // Custom validation
    if (_service.EmailExists(dto.Email))
    {
        return Msg.Validation.DuplicateValue("Email")
            .WithMetadata("email", dto.Email)
            .ToApiResponse();
    }

    var user = _service.Create(dto);
    return Msg.Crud.Created("User")
        .WithData(user)
        .ToApiResponse();
}

// Field-level validation
public IActionResult ValidateField(string field, string value)
{
    if (string.IsNullOrEmpty(value))
        return Msg.Validation.RequiredField(field).ToApiResponse();

    if (field == "Email" && !IsValidEmail(value))
        return Msg.Validation.InvalidEmail().ToApiResponse();

    return Ok();
}
```

### 4. Msg.System - System Messages

**Purpose:** General system status and errors

```csharp
// Status messages
Msg.System.Processing()             // SYS_002 - In progress
Msg.System.Success()                // SYS_001 - General success

// Errors
Msg.System.Error()                  // SYS_003 - General error
Msg.System.Maintenance()            // SYS_004 - Under maintenance
Msg.System.ServiceUnavailable()     // SYS_005 - Temporarily down

// Resource issues
Msg.System.OutOfMemory()            // SYS_006 - Memory exhausted
Msg.System.DiskFull()               // SYS_007 - Storage full
Msg.System.ConfigurationError()     // SYS_008 - Invalid config

// Deprecation
Msg.System.Deprecated()             // SYS_009 - Feature deprecated
Msg.System.NotImplemented()         // SYS_010 - Feature not ready
```

**Common Usage:**
```csharp
// Long-running operations
[HttpPost("import")]
public async Task<IActionResult> Import(ImportDto dto)
{
    // Start processing
    Msg.System.Processing()
        .WithData(new { FileName = dto.FileName })
        .Log(_logger)
        .ToConsole();

    try
    {
        await _service.ImportAsync(dto);
        return Msg.System.Success()
            .WithData(new { Imported = dto.RecordCount })
            .ToApiResponse();
    }
    catch (Exception ex)
    {
        return Msg.System.Error()
            .WithMetadata("error", ex.Message)
            .Log(_logger)
            .ToApiResponse();
    }
}

// Health check
[HttpGet("health")]
public IActionResult Health()
{
    if (_service.IsUnderMaintenance())
        return Msg.System.Maintenance().ToApiResponse();

    if (!_service.IsHealthy())
        return Msg.System.ServiceUnavailable().ToApiResponse();

    return Ok(new { status = "healthy" });
}
```

### 5. Msg.Database - Database Operations

**Purpose:** Database connectivity and query errors

```csharp
Msg.Database.ConnectionFailed()     // DB_001 - Cannot connect
Msg.Database.QueryTimeout()         // DB_002 - Query too slow
Msg.Database.ConstraintViolation()  // DB_003 - FK/UK violation
Msg.Database.DeadlockDetected()     // DB_004 - Deadlock
Msg.Database.ConnectionLost()       // DB_008 - Connection dropped
```

**Common Usage:**
```csharp
public async Task<IActionResult> GetData()
{
    try
    {
        var data = await _repository.GetAllAsync();
        return Msg.Crud.Retrieved("Data")
            .WithData(data)
            .ToApiResponse();
    }
    catch (SqlException ex) when (ex.Number == -2) // Timeout
    {
        return Msg.Database.QueryTimeout()
            .WithMetadata("query", "GetAll")
            .Log(_logger)
            .ToApiResponse();
    }
    catch (SqlException ex) when (ex.Number == 1205) // Deadlock
    {
        return Msg.Database.DeadlockDetected()
            .Log(_logger)
            .ToApiResponse();
    }
}
```

### 6. Msg.File - File Operations

**Purpose:** File upload, download, and validation

```csharp
// Success
Msg.File.Uploaded()                         // FILE_001 - Upload success
Msg.File.Downloaded()                       // FILE_002 - Download success

// Validation
Msg.File.InvalidType("pdf", "docx")         // FILE_003 - Wrong type
Msg.File.SizeExceeded()                     // FILE_004 - Too large
Msg.File.SizeWarning()                      // FILE_005 - Near limit

// Errors
Msg.File.NotFound()                         // FILE_006 - File missing
Msg.File.ReadError()                        // FILE_007 - Cannot read
Msg.File.WriteError()                       // FILE_008 - Cannot write
Msg.File.CorruptFile()                      // FILE_009 - Corrupted
```

**Common Usage:**
```csharp
[HttpPost("upload")]
public async Task<IActionResult> Upload(IFormFile file)
{
    // Validate type
    var allowedTypes = new[] { "pdf", "docx", "txt" };
    var extension = Path.GetExtension(file.FileName).TrimStart('.');

    if (!allowedTypes.Contains(extension.ToLower()))
    {
        return Msg.File.InvalidType(allowedTypes)
            .WithMetadata("actualType", extension)
            .ToApiResponse();
    }

    // Validate size
    const long maxSize = 10 * 1024 * 1024; // 10 MB
    if (file.Length > maxSize)
    {
        return Msg.File.SizeExceeded()
            .WithMetadata("maxSize", "10 MB")
            .WithMetadata("actualSize", $"{file.Length / 1024 / 1024} MB")
            .ToApiResponse();
    }

    // Upload
    await _service.UploadAsync(file);
    return Msg.File.Uploaded()
        .WithData(new { FileName = file.FileName, Size = file.Length })
        .ToApiResponse();
}
```

### 7. Msg.Network - Network & API Calls

**Purpose:** Network connectivity and external API errors

```csharp
Msg.Network.Timeout()               // NET_001 - Request timeout
Msg.Network.ServiceUnavailable()    // NET_002 - Service down
Msg.Network.BadGateway()            // NET_003 - Upstream error
Msg.Network.ConnectionRefused()     // NET_004 - Cannot connect
Msg.Network.DnsError()              // NET_005 - DNS resolution
```

**Common Usage:**
```csharp
public async Task<IActionResult> CallExternalApi()
{
    try
    {
        var result = await _httpClient.GetAsync("https://api.example.com");
        return Ok(result);
    }
    catch (TaskCanceledException)
    {
        return Msg.Network.Timeout()
            .WithMetadata("endpoint", "https://api.example.com")
            .Log(_logger)
            .ToApiResponse();
    }
    catch (HttpRequestException ex)
    {
        return Msg.Network.ServiceUnavailable()
            .WithMetadata("error", ex.Message)
            .Log(_logger)
            .ToApiResponse();
    }
}
```

### 8. Msg.Payment - Payment Operations

**Purpose:** Payment processing messages

```csharp
Msg.Payment.Success()               // PAY_001 - Payment succeeded
Msg.Payment.Failed()                // PAY_002 - Payment failed
Msg.Payment.Pending()               // PAY_003 - Awaiting processing
Msg.Payment.InsufficientFunds()     // PAY_004 - Not enough balance
Msg.Payment.InvalidCard()           // PAY_005 - Card validation
Msg.Payment.Declined()              // PAY_006 - Bank declined
Msg.Payment.Refunded()              // PAY_007 - Refund processed
```

### 9. Msg.Custom - Custom Messages

**Purpose:** Access custom messages loaded from files or databases

```csharp
// Load custom messages
MessageRegistry.LoadCustomMessages("messages/custom.json");

// Use custom messages
Msg.Custom("PAYMENT_001")
    .WithParams(new { amount = "$50.00" })
    .ToApiResponse();

Msg.Custom("NOTIFY_001")
    .WithParams(new { userName = "John", action = "logged in" })
    .ToConsole();
```

**Common Usage:**
```csharp
// Domain-specific messages
public IActionResult ProcessOrder(OrderDto dto)
{
    try
    {
        _service.ProcessOrder(dto);
        return Msg.Custom("ORDER_PROCESSED")
            .WithParams(new { orderId = dto.Id, total = dto.Total })
            .ToApiResponse();
    }
    catch (InventoryException)
    {
        return Msg.Custom("OUT_OF_STOCK")
            .WithParams(new { productId = dto.ProductId })
            .ToApiResponse();
    }
}
```

---

## Facade Method Signatures

### No Parameters

```csharp
// Simple, no parameters
public static Message LoginFailed()
{
    return MessageRegistry.Get("AUTH_001");
}

// Usage
Msg.Auth.LoginFailed()
Msg.System.Error()
Msg.Validation.Failed()
```

### With Optional Parameters

```csharp
// Optional parameter for customization
public static Message Created(string? resource = null)
{
    var message = MessageRegistry.Get("CRUD_001");
    return resource != null
        ? message.WithParams(new { resource })
        : message;
}

// Usage
Msg.Crud.Created()          // "Resource has been created successfully."
Msg.Crud.Created("User")    // "User has been created successfully."
Msg.Crud.Created("Product") // "Product has been created successfully."
```

### With Required Parameters

```csharp
// Required parameter
public static Message RequiredField(string fieldName)
{
    return MessageRegistry.Get("VAL_002")
        .WithParams(new { fieldName });
}

// Usage
Msg.Validation.RequiredField("Email")     // "Email is required."
Msg.Validation.RequiredField("Password")  // "Password is required."
```

### With Multiple Parameters

```csharp
// Multiple required parameters
public static Message InvalidType(params string[] allowedTypes)
{
    return MessageRegistry.Get("FILE_003")
        .WithParams(new { types = string.Join(", ", allowedTypes) });
}

// Usage
Msg.File.InvalidType("pdf", "docx")
// "Only pdf, docx files are allowed."

Msg.File.InvalidType("jpg", "png", "gif")
// "Only jpg, png, gif files are allowed."
```

---

## IntelliSense Discovery

Facades are designed for discovery through IntelliSense:

### Step 1: Type `Msg.`

```csharp
Msg. // IntelliSense shows all facades:
     // - Auth
     // - Crud
     // - Validation
     // - System
     // - Database
     // - File
     // - Network
     // - Payment
     // - Custom(code)
```

### Step 2: Select a facade (e.g., `Auth`)

```csharp
Msg.Auth. // IntelliSense shows all methods:
          // - LoginFailed()
          // - Unauthorized()
          // - LoginSuccessful()
          // - SessionExpired()
          // - etc.
```

### Step 3: View documentation

Each method includes XML documentation:

```csharp
Msg.Auth.LoginFailed() // Hovering shows:
/// <summary>
/// Returns AUTH_001: Invalid username or password.
/// </summary>
```

---

## Creating Custom Facades

You can extend the `Msg` class with your own facades:

```csharp
// Create a partial class in your application
namespace MyApp;

public static partial class Msg
{
    public static class Order
    {
        public static Message Placed()
            => MessageRegistry.Get("ORDER_001");

        public static Message Shipped()
            => MessageRegistry.Get("ORDER_002");

        public static Message Delivered()
            => MessageRegistry.Get("ORDER_003");

        public static Message Cancelled(string reason)
            => MessageRegistry.Get("ORDER_004")
                .WithParams(new { reason });
    }
}
```

**Usage:**
```csharp
// Your custom facade works just like built-in ones
return Msg.Order.Placed()
    .WithData(new { OrderId = 123 })
    .ToApiResponse();

return Msg.Order.Cancelled("Customer request")
    .WithMetadata("orderId", 123)
    .ToApiResponse();
```

---

## Best Practices

### [✓] DO:

**1. Use facades instead of MessageRegistry directly**
```csharp
// Good
Msg.Auth.LoginFailed()

// Bad
MessageRegistry.Get("AUTH_001")
```

**2. Include resource names for clarity**
```csharp
// Good - specific
Msg.Crud.NotFound("User")
Msg.Crud.Created("Order")

// OK - generic
Msg.Crud.NotFound()
Msg.Crud.Created()
```

**3. Chain methods for complete messages**
```csharp
// Good - complete context
return Msg.Crud.NotFound("User")
    .WithMetadata("userId", id)
    .WithMetadata("searchedBy", currentUser)
    .Log(_logger)
    .ToApiResponse();
```

**4. Create custom facades for domain-specific messages**
```csharp
// Good - domain-specific facade
public static class Msg
{
    public static class Inventory
    {
        public static Message StockLow(string product, int current, int threshold)
            => Msg.Custom("INV_LOW")
                .WithParams(new { product, current, threshold });
    }
}
```

### [ ] DON'T:

**1. Don't bypass facades**
```csharp
// Bad - loses type safety and discoverability
var message = MessageRegistry.Get("AUTH_001");

// Good
var message = Msg.Auth.LoginFailed();
```

**2. Don't forget to pass parameters**
```csharp
// Bad - missing resource name
Msg.Crud.Created()  // "Resource has been created successfully."

// Good - specific resource
Msg.Crud.Created("User")  // "User has been created successfully."
```

**3. Don't create facades for every message code**
```csharp
// Bad - one-off usage
public static class Msg
{
    public static class MyFeature
    {
        public static Message OnlyUsedOnce() => ...  // [ ] Overkill
    }
}

// Good - use Msg.Custom() for rare messages
Msg.Custom("MY_FEATURE_001")
```

---

## Quick Reference

### Most Used Facades

```csharp
// Authentication
Msg.Auth.LoginFailed()
Msg.Auth.Unauthorized()
Msg.Auth.LoginSuccessful()

// CRUD
Msg.Crud.Created("Entity")
Msg.Crud.Updated("Entity")
Msg.Crud.Deleted("Entity")
Msg.Crud.NotFound("Entity")
Msg.Crud.Retrieved("Entity")

// Validation
Msg.Validation.Failed()
Msg.Validation.RequiredField("Field")
Msg.Validation.InvalidFormat("Field")

// System
Msg.System.Error()
Msg.System.Processing()
Msg.System.Success()

// Files
Msg.File.Uploaded()
Msg.File.InvalidType("pdf", "docx")
Msg.File.SizeExceeded()

// Custom
Msg.Custom("YOUR_CODE")
```

---

## Next Steps

- **[Formatters and Outputs](Formatters-and-Outputs.md)** - Convert messages to different formats
- **[Message Codes Reference](../API-Reference/Message-Codes-Reference.md)** - Complete list of all codes
- **[Create Custom Messages](../How-To-Guides/Create-Custom-Messages.md)** - Add your own messages

---

**Questions?** [Ask in Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

# EasyMessages - Architecture & Design Decisions

## 1. API Pattern Analysis

###  Pattern Propossed

```
Msg.{Category}.{Action}(params) → Message → .With{X}()* → .Log()? → .To{Output}()
```

**Reason To Use:**
1. **Natural reading order** - "Message CRUD Created user with data, log it, return as API response"
2. **Fluent and discoverable** - IntelliSense guides you through
3. **Optional steps are optional** - Minimum: `Msg.Crud.Created("user").ToApiResponse()`
4. **Terminal operation pattern** - `.To*()` methods end the chain and return the output

### Pattern Breakdown & Evaluation

| Step | Purpose | Required? | Notes |
|------|---------|-----------|-------|
| `Msg` | Entry point | ✅ Yes | Static facade |
| `.Crud` | Category | ✅ Yes | Groups related messages |
| `.Created("user")` | What happened | ✅ Yes | Returns `Message` object |
| `.WithData(newUser)` | Additional context | ❌ Optional | Chainable |
| `.Log()` | Side effect | ❌ Optional | Chainable |
| `.ToApiResponse()` | Output format | ✅ Yes | Terminal operation |


---

## 2. Message Code Mapping Strategy

### The Core Challenge
**Problem:** How do we map `Msg.Auth.LoginFailed()` → `AUTH_001` → Message content?

### Solution: Multi-Layer Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    User Code Layer                       │
│  Msg.Auth.LoginFailed() ──────────────────────┐         │
└───────────────────────────────────────────────┼─────────┘
                                                │
┌───────────────────────────────────────────────▼─────────┐
│               Facade/Method Layer                        │
│  LoginFailed() method knows its code = "AUTH_001"       │
└───────────────────────────────────────────────┬─────────┘
                                                │
┌───────────────────────────────────────────────▼─────────┐
│              Message Registry Layer                      │
│  Looks up "AUTH_001" in:                                │
│    1. Custom messages (user override)                   │
│    2. Default messages (built-in)                       │
└───────────────────────────────────────────────┬─────────┘
                                                │
┌───────────────────────────────────────────────▼─────────┐
│               Message Object (Record Type)              │  
│  - Immutable by default (C# record with init properties)│
│  - All With*() methods return NEW instances             │
│  - Thread-safe due to immutability                      │
└─────────────────────────────────────────────────────────┘
```

### Implementation Strategy

#### Hardcoded Method-to-Code Mapping

```csharp
// In AuthMessages.cs facade
public static class AuthMessages
{
    public static Message LoginFailed()
    {
        // Method knows its own code - hardcoded mapping
        return MessageRegistry.Get("AUTH_001");
    }
    
    public static Message LoginSuccess()
    {
        return MessageRegistry.Get("AUTH_003");
    }
    
    public static Message Unauthorized()
    {
        return MessageRegistry.Get("AUTH_002");
    }
}
```

**Pros:**
- ✅ Type-safe and discoverable
- ✅ Method name documents the intent
- ✅ Users can't break the mapping
- ✅ Easy to maintain

**Cons:**
- ❌ Adding new methods requires code changes (but that's expected for a library)

---

### Message Override Strategy

#### defaults.json (Built-in - Embedded Resource)
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
#### custom.json (User Override - Optional File)
```json
{
  "AUTH_001": {
    "type": "Error",
    "title": "Login Failed",  // ← User customized title
    "description": "The credentials you provided are incorrect. Please try again.",
    "httpStatusCode": 401
  },
  "MYAPP_001": {  // ← User's own message
    "type": "Success",
    "title": "Payment Processed",
    "description": "Your payment has been processed successfully."
  }
}
```

#### MessageRegistry Logic
```csharp
public class MessageRegistry
{
    private static Dictionary<string, MessageTemplate> _defaults;
    private static Dictionary<string, MessageTemplate> _custom;
    
    public static Message Get(string code)
    {
        // Priority: Custom → Default → Exception
        MessageTemplate template;
        
        // 1. Check custom messages first
        if (_custom.TryGetValue(code, out template))
            return template.ToMessage(code);
        
        // 2. Fall back to defaults
        if (_defaults.TryGetValue(code, out template))
            return template.ToMessage(code);
        
        // 3. Code not found
        throw new MessageNotFoundException($"Message code '{code}' not found");
    }
}
```

#### **defaults.json Loading Mechanism**

```csharp
// Priority order:
1. Check custom messages (file-based or DI-registered)
2. Fall back to embedded defaults.json
3. Throw MessageNotFoundException if code not found

// Loading strategy:
- Embedded: Load once at startup (static Lazy<T>)
- Custom: Load on AddEasyMessages() call or first access
- Hot reload: NOT supported in v1.0 (document this limitation)
```

#### Custom Message Merge Behavior

**Specification:**
- Custom messages support **partial overrides** - users can override only specific properties
- Missing properties in custom messages fall back to defaults
- Merge strategy: `{ ...defaults[code], ...custom[code] }`

**Example:**
```json
// defaults.json
{
  "AUTH_001": {
    "type": "Error",
    "title": "Authentication Failed",
    "description": "Invalid username or password.",
    "httpStatusCode": 401
  }
}

// custom.json (partial override)
{
  "AUTH_001": {
    "title": "Login Failed"
    // type, description, httpStatusCode inherited from defaults
  }
}
```

**Implementation:** MessageRegistry performs shallow merge during `LoadCustomMessages()`.

---

### User Custom Messages - Two Approaches

#### Approach 1: Override Built-in Codes (Customization)
```csharp
// User wants different text for AUTH_001
// In custom.json
{
  "AUTH_001": {
    "title": "Oops! Login Failed",  // Their brand voice
    "description": "Hmm, those credentials didn't work. Try again?"
  }
}

// Code still works the same
Msg.Auth.LoginFailed(); // ← Uses custom text now
```

#### Approach 2: Add New Codes (Extension)
```csharp
// User adds their own business logic messages
// In custom.json
{
  "PAYMENT_001": {
    "type": "Success",
    "title": "Payment Complete",
    "description": "Transaction ID: {transactionId}"
  }
}

// Access via Custom facade
Msg.Custom("PAYMENT_001")
    .WithParams(new { transactionId = "TXN123" })
    .ToApiResponse();
```

---

## 3. Extensibility Architecture

### Core Principle: Open/Closed
**Open for extension, closed for modification**

### Extension Points

#### 1. Custom Formatters (IMessageFormatter)
```csharp
public interface IMessageFormatter
{
    string Format(Message message);
    object FormatAsObject(Message message);
}

// Built-in formatters
- JsonFormatter
- ConsoleFormatter
- LogFormatter
- XmlFormatter

// User can add their own
public class SlackFormatter : IMessageFormatter
{
    public string Format(Message message)
    {
        return $":warning: *{message.Title}* - {message.Description}";
    }
}

// Register it
MessageConfig.RegisterFormatter<SlackFormatter>();

// Use it
Msg.System.Error().ToFormat<SlackFormatter>();
```

#### 2. Custom Output Targets (IMessageOutput)
```csharp
public interface IMessageOutput
{
    Task SendAsync(Message message);
}

// User creates custom output
public class EmailOutput : IMessageOutput
{
    public async Task SendAsync(Message message)
    {
        await _emailService.SendAsync(message.Title, message.Description);
    }
}

// Use it
await Msg.System.Critical()
    .ToAsync<EmailOutput>();
```

#### 3. Custom Categories (User-Defined Facades)
```csharp
// User creates their own category
public static class PaymentMessages
{
    public static Message ProcessingStarted()
    {
        return MessageRegistry.Get("PAYMENT_001");
    }
    
    public static Message InsufficientFunds()
    {
        return MessageRegistry.Get("PAYMENT_002");
    }
}

// Use alongside built-in categories
Msg.Custom.Payment.ProcessingStarted(); // ← User's own

// Or simpler:
Msg.Custom("PAYMENT_001");
```

#### 4. Middleware/Interceptors
```csharp
public interface IMessageInterceptor
{
    Message OnBeforeFormat(Message message);
    Message OnAfterFormat(Message message);
}

// Example: Add user context to all messages
public class UserContextInterceptor : IMessageInterceptor
{
    public Message OnBeforeFormat(Message message)
    {
        message.Metadata["userId"] = _currentUser.Id;
        return message;
    }
}

// Register globally
MessageConfig.AddInterceptor<UserContextInterceptor>();
```

#### 5. Custom Message Stores
```csharp
public interface IMessageStore
{
    Task<Dictionary<string, MessageTemplate>> LoadAsync();
    Task<bool> IsAvailableAsync() => Task.FromResult(true);
}

// User can load from database, API, etc.
public class DatabaseMessageStore : IMessageStore
{
    public async Task<Dictionary<string, MessageTemplate>> LoadAsync()
    {
        return await _db.Messages.ToDictionaryAsync();
    }
}

// Register it
MessageConfig.UseStore<DatabaseMessageStore>();
```

---

## 4. Configuration Architecture

### Zero-Config by Default
```csharp
// Works immediately - no setup
Msg.Auth.LoginFailed().ToApiResponse();
```

### Optional Configuration
```csharp
// Program.cs (ASP.NET Core)
builder.Services.AddEasyMessages(options =>
{
    // Custom messages
    options.LoadCustomMessages("messages/custom.json");
    
    // Or from database
    options.UseStore<DatabaseMessageStore>();
    
    // Localization
    options.DefaultLocale = "en-US";
    options.SupportedLocales = new[] { "en-US", "es-ES", "fr-FR" };
    
    // Formatting
    options.IncludeStackTrace = builder.Environment.IsDevelopment();
    options.IncludeTimestamp = true;
    options.IncludeCorrelationId = true;
    
    // Auto-logging
    options.AutoLog = true;
    options.LogLevel = LogLevel.Warning; // Only log warnings and errors
    
    // Custom formatters
    options.RegisterFormatter<SlackFormatter>();
    
    // Interceptors
    options.AddInterceptor<UserContextInterceptor>();
});
```

---

## 5. Code Organization Strategy

### Namespace Structure
```
RecurPixel.EasyMessages
├── Core
│   ├── Message.cs
│   ├── MessageType.cs
│   ├── MessageTemplate.cs
│   └── MessageRegistry.cs
├── Facades
│   ├── Msg.cs (main entry point)
│   ├── AuthMessages.cs
│   ├── CrudMessages.cs
│   ├── ValidationMessages.cs
│   ├── SystemMessages.cs
│   └── CustomMessages.cs
├── Formatters
│   ├── IMessageFormatter.cs
│   ├── JsonFormatter.cs
│   ├── ConsoleFormatter.cs
│   └── LogFormatter.cs
├── Messages
│   ├── defaults.json (default)
│   ├── message-schema.json
│   └── custom.json
├── Outputs
│   ├── IMessageOutput.cs
│   ├── ApiOutput.cs
│   └── ConsoleOutput.cs
├── Configuration
│   └── MessageConfiguration.cs // shift to ASP.NET part of DI
├── Storage
│   ├── CompositeMessageStore.cs
│   ├── DatabaseMessageStore.cs
│   ├── DatabaseMessageStore.cs
│   └── FileMessageStore.cs
├── Interceptors
│   ├── IMessageInterceptor.cs
│   └── LoggingInterceptor.cs // shift to ASP.NET
└── Extensions
    ├── MessageExtensions.cs
    └── ServiceCollectionExtensions.cs // shirt to ASP.NET part of DI
```

---

## 6. Key Design Decisions

### Decision 1: Static vs Dependency Injection

**Support Both**

```csharp
// Static (Quick & Easy)
Msg.Auth.LoginFailed().ToApiResponse();

// DI (Testable & Configurable)
public class AuthController : ControllerBase
{
    private readonly IMessageService _messages;
    
    public AuthController(IMessageService messages)
    {
        _messages = messages;
    }
    
    public IActionResult Login()
    {
        return _messages.Auth.LoginFailed().ToApiResponse();
    }
}
```

**Why both?**
- Static for simplicity (Console apps, scripts)
- DI for testability (Enterprise apps, unit tests)

---

### Decision 2: Code Naming Convention

**CATEGORY_NNN**

```
AUTH_001, AUTH_002, ...      Authentication
CRUD_001, CRUD_002, ...      CRUD operations
VAL_001, VAL_002, ...        Validation
SYS_001, SYS_002, ...        System messages
DB_001, DB_002, ...          Database
FILE_001, FILE_002, ...      File operations
NET_001, NET_002, ...        Network/API calls
PAY_001, PAY_002, ...        Payment (if applicable)
```

**User codes:** `MYAPP_001`, `COMPANY_001`, etc.

**Why this convention?**
- Easy to scan and group
- Clear ownership (built-in vs custom)
- Alphabetically sortable
- Room for growth (001-999)

---

### Decision 3: Message Mutability

**Immutable by Default, Builder Pattern for Changes**

```csharp
// Message is immutable once created (use records)
var message = Msg.Auth.LoginFailed(); // Immutable

// Use extension methods that return new instances
var enriched = message
    .WithData(user)           // Returns new Message
    .WithCorrelationId(id);   // Returns new Message

// Original unchanged
Assert.Null(message.Data);
Assert.NotNull(enriched.Data);
```

**Why immutable?**
- Thread-safe
- Predictable behavior
- Easier to test

---

### Decision 4: HTTP Status Code Handling

**Smart Defaults + Override Option**

```csharp
// defaults.json
{
  "AUTH_001": {
    "type": "Error",
    "httpStatusCode": 401  // ← Default status
  }
}

// Override per use
Msg.Auth.LoginFailed()
    .WithStatusCode(403)  // ← Override to Forbidden
    .ToApiResponse();

```
---

### Decision 5: Correlation ID Strategy

**Opt-In, Not Auto-Generated**
```csharp
// NO automatic generation - keep it explicit
var message = Msg.Auth.LoginFailed(); 
Assert.Null(message.CorrelationId); // ✅ Null by default

// User sets explicitly when needed
var enriched = message.WithCorrelationId(HttpContext.TraceIdentifier);
Assert.NotNull(enriched.CorrelationId); // ✅ Now set
```

**Why opt-in?**
- Avoid magic behavior
- Users control when/how correlation IDs are generated
- Simpler to understand and test
- No dependencies on HttpContext or external services

**Future:** May add helper method `WithAutoCorrelationId()` that generates GUID if needed.

---

### Decision 6: Thread Safety Guarantees

**Read-Only After Startup**
```csharp
// ✅ Thread-safe operations:
- MessageRegistry.Get(code)           // Always safe (immutable read)
- Message instances                   // Immutable records - inherently safe
- Extension methods                   // Return new instances - safe

// ⚠️ NOT thread-safe:
- MessageRegistry.LoadCustomMessages() // Call ONCE at startup only
- Re-loading custom messages at runtime // NOT supported
```

**Rules:**
1. **Load phase** (startup): Single-threaded - call `LoadCustomMessages()` in `Program.cs` before handling requests
2. **Runtime phase**: Multi-threaded - unlimited concurrent `Get()` calls are safe
3. **No hot-reload**: Custom messages cannot be reloaded at runtime in v1.0

**Documentation:** XML comments on `LoadCustomMessages()` must include thread-safety warning.

---

## 7. Integration Strategy

### Goal: Work WITH Everything, Replace NOTHING

#### Works With:
- **Logging**: Serilog, NLog, Microsoft.Extensions.Logging
- **API**: ASP.NET Core, Minimal APIs, Web API
- **Validation**: FluentValidation
- **Error Handling**: ProblemDetails, Hellang.Middleware
- **UI**: Blazor, Razor Pages, MVC

#### How?
```csharp
// Example: FluentValidation integration
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(Msg.Validation.RequiredField("Email").Description);
    }
}

// Example: Serilog integration
Msg.System.Error()
    .Log(_logger);  // Uses ILogger under the hood

// Example: ProblemDetails integration
Msg.Auth.Unauthorized()
    .ToProblemDetails(); // Returns RFC 7807 format
```

---

## 8. Testing Strategy

### Unit Testing
```csharp
[Fact]
public void LoginFailed_ShouldReturnCorrectCode()
{
    var message = Msg.Auth.LoginFailed();
    
    Assert.Equal("AUTH_001", message.Code);
    Assert.Equal(MessageType.Error, message.Type);
    Assert.Equal(401, message.HttpStatusCode);
}
```

### Integration Testing
```csharp
[Fact]
public async Task LoginFailed_ShouldReturnUnauthorized()
{
    var response = await _client.PostAsync("/auth/login", content);
    
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    
    var result = await response.Content.ReadAsAsync<ApiResponse>();
    Assert.Equal("AUTH_001", result.Code);
}
```

### Mocking Support
```csharp
// Create test messages
var mockMessage = MessageFactory.Create(MessageType.Success)
    .WithCode("TEST_001")
    .WithTitle("Test Message");
```

---

## 9. Performance Considerations

### Message Registry Caching
```csharp
// Load once at startup, cache forever
private static readonly Lazy<Dictionary<string, MessageTemplate>> _cache =
    new Lazy<Dictionary<string, MessageTemplate>>(LoadMessages);

// O(1) lookup
public static Message Get(string code) => _cache.Value[code].ToMessage();
```

### Zero Allocations for Common Paths
```csharp
// Reuse common objects
private static readonly Message _loginFailed = MessageRegistry.Get("AUTH_001");

public static Message LoginFailed() => _loginFailed.Clone();
```

### Async for IO Operations Only
```csharp
// Sync by default (no unnecessary async)
Msg.Auth.LoginFailed().ToApiResponse();

// Async only when needed
await Msg.Auth.LoginFailed().ToEmailAsync();
```

---

## 10. Additional Considerations

### Versioning Strategy

**Semantic Versioning: MAJOR.MINOR.PATCH**
```markdown
MAJOR: Breaking changes
MINOR: New features (backward compatible)
PATCH: Bug fixes
```

**Breaking Change Definition:**
```csharp
// ✅ NOT BREAKING (safe to do):
- Adding new message codes
- Adding new facade methods
- Adding new extension methods
- Adding new properties to Message (with default values)
- Changing message text (title/description) - MINOR version bump

// ⚠️ POTENTIALLY BREAKING (minor version with deprecation):
- Marking methods as [Obsolete]
- Renaming parameters (may break named arguments)

// 🔴 BREAKING (requires MAJOR version bump):
- Removing message codes
- Removing facade methods
- Changing method signatures
- Changing Message class structure (removing properties)
- Changing default httpStatusCode mappings
- Changing parameter substitution behavior
```

**Deprecation Policy:**
1. Mark with `[Obsolete("Message", false)]` for 1 MAJOR version
2. Change to `[Obsolete("Message", true)]` (compiler error) next MAJOR version
3. Remove in following MAJOR version

**Example:**
```csharp
// v1.0
public static Message LoginFailed() => ...

// v2.0 - Deprecate
[Obsolete("Use Auth.AuthenticationFailed() instead", false)]
public static Message LoginFailed() => ...

// v3.0 - Compiler error
[Obsolete("Use Auth.AuthenticationFailed() instead", true)]
public static Message LoginFailed() => ...

// v4.0 - Removed entirely
```

### Documentation Strategy
- **XML comments** on all public APIs
- **Code samples** in documentation
- **Migration guides** for major versions
- **Changelog** for every release

### Security Considerations
- **Never expose stack traces** in production by default
- **Sanitize error messages** (no SQL queries, file paths, etc.)
- **Rate limiting** for custom message loading
- **Input validation** for user-provided codes

### Backward Compatibility
- **Never remove built-in codes** (mark as obsolete instead)
- **Keep method signatures stable**
- **Add, don't change** existing messages

---

## Summary: Answers to Your Questions

### 1. ✅ API Pattern
**Final pattern:** 
```
Msg.{Category}.{Action}() → .With{X}()* → .Log()? → .To{Output}()
```

### 2. ✅ Code Mapping
- **Hardcode method-to-code mapping** in facade classes
- **Users can override message content** via custom.json
- **Users can add new codes** via custom.json + Msg.Custom()

### 3. ✅ Extensibility
- **Interfaces provided**: IMessageFormatter, IMessageOutput, , IMessageInterceptor
- **Users can extend** without modifying library code
- **Plugin architecture** for advanced scenarios

### 4. ✅ Philosophy
- **Work WITH everything** (Serilog, FluentValidation, etc.)
- **Replace nothing** (complement, don't compete)
- **Zero config required** (but configurable when needed)

---

## Future Considerations

1. **Localization** - Multi-language support from day one
2. **Correlation IDs** - Distributed tracing support
3. **Immutability** - Thread-safe by design
4. **Testing support** - Easy to mock and test
5. **Performance** - Caching and zero-allocation paths
6. **Security** - Safe error messages by default
7. **Migration path** - Easy to adopt, hard to break

---

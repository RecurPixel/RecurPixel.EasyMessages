# EasyMessages - Architecture (Alpha)

## Core Concepts

### Message (Immutable Record)
```csharp
public sealed record Message
{
    public string Code { get; init; }          // e.g., "AUTH_001"
    public MessageType Type { get; init; }     // Success, Error, etc.
    public string Title { get; init; }         // "Login Failed"
    public string Description { get; init; }   // Details
    public int HttpStatusCode { get; init; }   // 401, 404, etc.
    public DateTime Timestamp { get; init; }   // Auto-generated
}
```

### Message Registry
Loads messages from `defaults.json` (embedded) and optional custom files.
```csharp
MessageRegistry.Get("AUTH_001");              // Load message
MessageRegistry.LoadCustomMessages("custom.json"); // Override
```

### Fluent API
```csharp
Msg.Auth.LoginFailed()                // Get message
   .WithData(user)                    // Add data
   .ToJson();                         // Output
```

## Project Structure (Alpha)
```
RecurPixel.EasyMessages/
├── Core/
│   ├── Message.cs
│   ├── MessageRegistry.cs
│   └── MessageType.cs
├── Facades/
│   └── Msg.cs
├── Formatters/
│   ├── JsonFormatter.cs
│   └── ConsoleFormatter.cs
├── Extensions/
│   └── MessageExtensions.cs
└── Messages/
    └── defaults.json (50 messages)
```

## What's Not Here Yet (Coming in Beta)

- AspNetCore integration (.ToApiResponse())
- XML/PlainText formatters
- Interceptor system
- Advanced stores
- 200+ messages

---

**Full architecture:** See [ARCHITECTURE-FULL.md](docs/ARCHITECTURE-FULL.md)
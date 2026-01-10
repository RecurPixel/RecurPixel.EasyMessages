---
layout: default
title: Core Concepts
parent: Latest Documentation
nav_order: 4
has_children: true
---

# Core Concepts
{: .no_toc }

Understand the fundamental concepts and architecture behind EasyMessages.
{: .fs-6 .fw-300 }

---

## In This Section

- [**Messages and Message Types**](Messages-and-Message-Types) - Understand the Message structure and types
- [**Message Registry and Stores**](Message-Registry-and-Stores) - How messages are stored and retrieved
- [**Facades**](Facades) - Static facades (Msg.Auth, Msg.Crud, etc.) for easy access
- [**Formatters and Outputs**](Formatters-and-Outputs) - Converting messages to different formats
- [**Interceptors**](Interceptors) - Intercept and modify messages
- [**Architecture Overview**](Architecture-Overview) - High-level system design

---

## Key Concepts

### Immutability
All messages are immutable records, ensuring thread-safety and predictable behavior.

### Fluent API
Methods can be chained together for clean, readable code:

```csharp
Msg.Auth.LoginFailed()
    .WithData(new { email })
    .Log(logger)
    .ToApiResponse();
```

### Zero Configuration
EasyMessages works immediately without any setup required.

---
layout: default
title: Getting Started
parent: Latest Documentation
nav_order: 3
has_children: true
---

# Getting Started
{: .no_toc }

Step-by-step tutorials to get you up and running quickly with EasyMessages.
{: .fs-6 .fw-300 }

---

## In This Section

- [**Installation**](Installation) - Install packages and set up your environment
- [**Your First Message**](Your-First-Message) - Create and use your first message in 5 minutes
- [**Console vs Web Applications**](Console-vs-Web-Applications) - Choose the right approach for your app type
- [**Next Steps**](Next-Steps) - Learn about advanced features and best practices

---

## Quick Example

```csharp
using RecurPixel.EasyMessages;

// Works immediately - no setup needed!
var message = Msg.Auth.LoginFailed();
Console.WriteLine(message.ToJson());
```

Ready to get started? Head to the [Installation](Installation) guide!

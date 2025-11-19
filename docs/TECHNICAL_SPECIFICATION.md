# Technical Specifications

## 1. Message Immutability
**Decision:** 
-  Message Immutability vs Fluent API

**Rationale:**

- Architecture says "immutable by default" but fluent API suggests chaining that modifies the message

**Implementation:**

- Make Message a record type (immutable by default)
- All With*() methods return NEW instances

## 2. Design Decision
**Decision:** 
-  Using IMessageStore instead of IMessageLoader or Both

**Rationale:**

- For **RecurPixel.EasyMessages**, this separation is **over-engineered**. Here's why:
- **only support JSON** (and will for foreseeable future)
- **Users don't need format flexibility** (JSON is the standard)
- **YAGNI principle** (You Aren't Gonna Need It)

**Implementation:**

- Remove Loader Folder
- Implement logic inside store(store represent. Where data comming from)


## 3. New Centralized Message Formater

**Rationale:**

- There are many parts in the message, different application. And to format each message for each type is difficult.

**Implementation:**

- Make Central Formater with default/minimal/custom options for users to use.

**Example Usage(non-exhaustive)**

```charp

var msg = Msg.Crud.Created("User")
    .WithParams(new { username = "john" })
    .WithData(new { id = 123 })
    .WithCorrelationId("abc-123");

// Full output
var json = msg.ToJson();

// Minimal output
var minimal = msg.ToJson(FormatterOptions.Minimal);

// Custom
var custom = msg.ToJson(new FormatterOptions 
{ 
    IncludeMetadata = false,
    IncludeTimestamp = false 
});

```
> TODO: Add these changes in architecute specification.
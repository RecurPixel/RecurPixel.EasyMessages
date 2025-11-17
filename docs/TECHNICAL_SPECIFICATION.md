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
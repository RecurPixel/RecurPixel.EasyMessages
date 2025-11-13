# Technical Specifications

## 1. Message Immutability
**Decision:** 
-  Message Immutability vs Fluent API

**Rationale:**

- Architecture says "immutable by default" but fluent API suggests chaining that modifies the message

**Implementation:**

- Make Message a record type (immutable by default)
- All With*() methods return NEW instances

## 2. HTTP Status Code Precedence
...
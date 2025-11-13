# Message Code Reference

Complete reference for all built-in message codes in RecurPixel.EasyMessages.

## Message Code Format

```
PREFIX_NNN
├── PREFIX: Category identifier (3-6 uppercase letters)
└── NNN: Sequential number (001-999)
```

## Message Categories

### 📋 Quick Index

| Prefix | Category | Range | Count |
|--------|----------|-------|-------|
| `AUTH` | Authentication & Authorization | 001-010 | 10 |
| `CRUD` | CRUD Operations | 001-010 | 10 |
| `VAL` | Validation | 001-015 | 15 |
| `SYS` | System Messages | 001-010 | 10 |
| `DB` | Database Operations | 001-008 | 8 |
| `FILE` | File Operations | 001-012 | 12 |
| `NET` | Network & API | 001-010 | 10 |
| `PAY` | Payment & Transactions | 001-014 | 14 |
| `EMAIL` | Email & Notifications | 001-005 | 5 |
| `SEARCH` | Search & Filter | 001-004 | 4 |
| `IMPORT` | Import/Export | 001-003 + 2 | 5 |

---

## 🔐 AUTH - Authentication & Authorization

**Range:** `AUTH_001` to `AUTH_010`

| Code | Type | Title | Use Case |
|------|------|-------|----------|
| `AUTH_001` | Error | Authentication Failed | Invalid credentials |
| `AUTH_002` | Error | Unauthorized Access | Permission denied |
| `AUTH_003` | Success | Login Successful | Successful login |
| `AUTH_004` | Error | Session Expired | Token/session timeout |
| `AUTH_005` | Error | Invalid Token | JWT/token validation failed |
| `AUTH_006` | Error | Account Locked | Too many failed attempts |
| `AUTH_007` | Success | Logout Successful | User logged out |
| `AUTH_008` | Error | Password Reset Required | Force password change |
| `AUTH_009` | Error | Invalid Refresh Token | Cannot refresh session |
| `AUTH_010` | Error | Multi-Factor Authentication Required | MFA challenge |

**Placeholders:**
- `{username}` - User's display name
- `{lockoutMinutes}` - Account lockout duration

---

## 📝 CRUD - Create, Read, Update, Delete

**Range:** `CRUD_001` to `CRUD_010`

| Code | Type | Title | Use Case |
|------|------|-------|----------|
| `CRUD_001` | Success | Created Successfully | Resource creation |
| `CRUD_002` | Success | Updated Successfully | Resource update |
| `CRUD_003` | Success | Deleted Successfully | Resource deletion |
| `CRUD_004` | Error | Resource Not Found | 404 scenarios |
| `CRUD_005` | Success | Retrieved Successfully | GET operations |
| `CRUD_006` | Error | Creation Failed | POST failures |
| `CRUD_007` | Error | Update Failed | PUT/PATCH failures |
| `CRUD_008` | Error | Deletion Failed | DELETE failures |
| `CRUD_009` | Warning | No Changes Detected | Update with no delta |
| `CRUD_010` | Error | Conflict Detected | Optimistic concurrency |

**Placeholders:**
- `{resource}` - Resource type (e.g., "User", "Product")

---

## ✅ VAL - Validation

**Range:** `VAL_001` to `VAL_015`

| Code | Type | Title | Use Case |
|------|------|-------|----------|
| `VAL_001` | Error | Validation Failed | General validation error |
| `VAL_002` | Error | Required Field Missing | Missing required field |
| `VAL_003` | Error | Invalid Format | Format mismatch |
| `VAL_004` | Error | Value Out of Range | Min/max violations |
| `VAL_005` | Error | Invalid Email Address | Email validation |
| `VAL_006` | Error | Invalid Phone Number | Phone validation |
| `VAL_007` | Error | Password Too Weak | Password strength |
| `VAL_008` | Error | Passwords Don't Match | Confirmation mismatch |
| `VAL_009` | Error | Invalid Date | Date validation |
| `VAL_010` | Error | Value Too Short | MinLength violation |
| `VAL_011` | Error | Value Too Long | MaxLength violation |
| `VAL_012` | Error | Invalid URL | URL validation |
| `VAL_013` | Error | Invalid File Extension | File type validation |
| `VAL_014` | Error | Duplicate Value | Uniqueness constraint |
| `VAL_015` | Error | Invalid Characters | Character whitelist |

**Placeholders:**
- `{field}` - Field name
- `{format}` - Expected format
- `{min}` / `{max}` - Range bounds
- `{minLength}` / `{maxLength}` - Length bounds
- `{type}` - Expected type
- `{value}` - The duplicate value
- `{allowed}` - Allowed characters

---

## ⚙️ SYS - System Messages

**Range:** `SYS_001` to `SYS_010`

| Code | Type | Title | Use Case |
|------|------|-------|----------|
| `SYS_001` | Critical | System Error | Unhandled exceptions |
| `SYS_002` | Info | Processing Request | Long-running operations |
| `SYS_003` | Warning | Service Degraded | Partial outage |
| `SYS_004` | Info | Maintenance Mode | Scheduled maintenance |
| `SYS_005` | Success | Operation Completed | Generic success |
| `SYS_006` | Warning | Rate Limit Exceeded | Throttling |
| `SYS_007` | Error | Service Unavailable | 503 scenarios |
| `SYS_008` | Info | Request Queued | Async processing |
| `SYS_009` | Error | Timeout | Operation timeout |
| `SYS_010` | Critical | Configuration Error | Misconfiguration |

**Placeholders:**
- `{estimatedTime}` - Maintenance ETA
- `{retryAfter}` - Seconds until retry

---

## 🗄️ DB - Database Operations

**Range:** `DB_001` to `DB_008`

| Code | Type | Title | Use Case |
|------|------|-------|----------|
| `DB_001` | Critical | Database Connection Failed | Connection errors |
| `DB_002` | Error | Duplicate Entry | Unique constraint violation |
| `DB_003` | Error | Foreign Key Constraint | FK violation |
| `DB_004` | Error | Transaction Failed | Transaction rollback |
| `DB_005` | Critical | Data Integrity Error | Data corruption |
| `DB_006` | Error | Query Timeout | Slow queries |
| `DB_007` | Error | Deadlock Detected | Concurrent access |
| `DB_008` | Warning | Migration Pending | Outdated schema |

**Placeholders:**
- `{resource}` - Entity type
- `{field}` - Field causing duplicate
- `{relatedResource}` - FK target
- `{errorCode}` - Technical error code

---

## 📁 FILE - File Operations

**Range:** `FILE_001` to `FILE_012`

| Code | Type | Title | Use Case |
|------|------|-------|----------|
| `FILE_001` | Success | File Uploaded Successfully | Upload success |
| `FILE_002` | Error | Invalid File Type | Extension validation |
| `FILE_003` | Error | File Too Large | Size limit exceeded |
| `FILE_004` | Error | File Upload Failed | Upload errors |
| `FILE_005` | Success | File Downloaded Successfully | Download success |
| `FILE_006` | Error | File Not Found | Missing file |
| `FILE_007` | Error | File Access Denied | Permission denied |
| `FILE_008` | Success | File Deleted Successfully | Deletion success |
| `FILE_009` | Error | Corrupted File | Integrity check failed |
| `FILE_010` | Error | Storage Quota Exceeded | Quota limit |
| `FILE_011` | Warning | File Already Exists | Overwrite prompt |
| `FILE_012` | Error | Virus Detected | Malware scan failure |

**Placeholders:**
- `{fileName}` - File name
- `{allowedTypes}` - Permitted extensions
- `{maxSize}` - Maximum file size
- `{actualSize}` - Current file size

---

## 🌐 NET - Network & API

**Range:** `NET_001` to `NET_010`

| Code | Type | Title | Use Case |
|------|------|-------|----------|
| `NET_001` | Error | Network Error | Connectivity issues |
| `NET_002` | Error | Request Timeout | HTTP timeout |
| `NET_003` | Error | Bad Request | 400 errors |
| `NET_004` | Error | Server Error | 500 errors |
| `NET_005` | Error | API Rate Limit Exceeded | API throttling |
| `NET_006` | Error | Connection Refused | Service unavailable |
| `NET_007` | Error | SSL Certificate Error | TLS/SSL issues |
| `NET_008` | Warning | Slow Connection | Performance warning |
| `NET_009` | Error | Gateway Timeout | 504 errors |
| `NET_010` | Success | Connection Established | Connection success |

**Placeholders:**
- `{retryAfter}` - Retry delay (seconds)
- `{service}` - Service name

---

## 💳 PAY - Payment & Transactions

**Range:** `PAY_001` to `PAY_014`

| Code | Type | Title | Use Case |
|------|------|-------|----------|
| `PAY_001` | Success | Payment Successful | Payment completed |
| `PAY_002` | Error | Payment Failed | Payment error |
| `PAY_003` | Error | Insufficient Funds | NSF |
| `PAY_004` | Error | Card Declined | Bank declined |
| `PAY_005` | Error | Invalid Card Details | Validation error |
| `PAY_006` | Error | Card Expired | Expired card |
| `PAY_007` | Success | Refund Processed | Refund success |
| `PAY_008` | Error | Refund Failed | Refund error |
| `PAY_009` | Warning | Payment Pending | Processing payment |
| `PAY_010` | Error | Transaction Limit Exceeded | Limit violation |
| `PAY_011` | Error | Payment Gateway Error | Gateway unavailable |
| `PAY_012` | Success | Subscription Activated | Sub created |
| `PAY_013` | Warning | Subscription Expiring Soon | Renewal reminder |
| `PAY_014` | Error | Subscription Cancelled | Sub terminated |

**Placeholders:**
- `{amount}` - Transaction amount
- `{transactionId}` - Transaction ID
- `{limit}` - Transaction limit
- `{nextBillingDate}` - Next billing date
- `{expiryDate}` - Expiration date
- `{accessUntil}` - Access end date

---

## 📧 EMAIL - Email & Notifications

**Range:** `EMAIL_001` to `EMAIL_005`

| Code | Type | Title | Use Case |
|------|------|-------|----------|
| `EMAIL_001` | Success | Email Sent Successfully | Send success |
| `EMAIL_002` | Error | Email Delivery Failed | Send failure |
| `EMAIL_003` | Success | Email Verified | Verification success |
| `EMAIL_004` | Error | Invalid Verification Link | Bad token |
| `EMAIL_005` | Info | Verification Email Sent | Check inbox |

**Placeholders:**
- `{recipient}` - Recipient email
- `{email}` - Email address

---

## 🔍 SEARCH - Search & Filter

**Range:** `SEARCH_001` to `SEARCH_004`

| Code | Type | Title | Use Case |
|------|------|-------|----------|
| `SEARCH_001` | Info | No Results Found | Empty results |
| `SEARCH_002` | Success | Search Completed | Results found |
| `SEARCH_003` | Warning | Too Many Results | Refinement needed |
| `SEARCH_004` | Error | Invalid Search Query | Query syntax error |

**Placeholders:**
- `{query}` - Search query
- `{count}` - Result count

---

## 📊 IMPORT/EXPORT - Data Operations

**Import Range:** `IMPORT_001` to `IMPORT_003`  
**Export Range:** `EXPORT_001` to `EXPORT_002`

| Code | Type | Title | Use Case |
|------|------|-------|----------|
| `IMPORT_001` | Success | Import Completed | Full import success |
| `IMPORT_002` | Error | Import Failed | Import error |
| `IMPORT_003` | Warning | Partial Import | Some records failed |
| `EXPORT_001` | Success | Export Completed | Export success |
| `EXPORT_002` | Error | Export Failed | Export error |

**Placeholders:**
- `{count}` - Total records
- `{fileName}` - File name
- `{successCount}` - Successful records
- `{totalCount}` - Total records
- `{failedCount}` - Failed records

---

## 🎨 Message Structure

Each message follows this structure:

```json
{
  "CODE_NNN": {
    "type": "Success|Error|Warning|Info|Critical",
    "title": "Short Title",
    "description": "Detailed message with {placeholders}",
    "hint": "Optional helpful suggestion"
  }
}
```

### Message Types

| Type | Color | HTTP Range | Use Case |
|------|-------|------------|----------|
| **Success** | Green | 200-299 | Operations completed successfully |
| **Info** | Blue | 100-199 | Informational messages |
| **Warning** | Yellow | - | Non-critical issues |
| **Error** | Red | 400-499 | Client errors, validation failures |
| **Critical** | Dark Red | 500-599 | Server errors, system failures |

---

## 📚 Best Practices

### 1. **Choosing the Right Code**
```csharp
// ✅ Good - Specific code
return Msg.Get("VAL_002", new { field = "Email" });

// ❌ Bad - Generic code when specific exists
return Msg.Get("VAL_001");
```

### 2. **Using Placeholders**
```csharp
// ✅ Good - All placeholders provided
Msg.Get("CRUD_001", new { resource = "User" });

// ⚠️ Warning - Missing placeholder shows {resource}
Msg.Get("CRUD_001");
```

### 3. **Consistent Naming**
```csharp
// ✅ Good - Consistent resource naming
Msg.Get("CRUD_001", new { resource = "Product" });
Msg.Get("CRUD_004", new { resource = "Product" });

// ❌ Bad - Inconsistent naming
Msg.Get("CRUD_001", new { resource = "Product" });
Msg.Get("CRUD_004", new { resource = "product" }); // lowercase
```

### 4. **Error Stacking**
```csharp
// ✅ Good - Combine related messages
var errors = new[] {
    Msg.Get("VAL_002", new { field = "Email" }),
    Msg.Get("VAL_002", new { field = "Password" })
};
```

---

## 🔮 Future Categories (Reserved)

| Prefix | Category | Status |
|--------|----------|--------|
| `CACHE` | Caching Operations | Planned |
| `QUEUE` | Queue/Background Jobs | Planned |
| `REPORT` | Reporting | Planned |
| `AUDIT` | Audit Logging | Planned |
| `NOTIF` | Push Notifications | Planned |
| `SMS` | SMS Operations | Planned |
| `WEBHOOK` | Webhook Events | Planned |

---

## 📝 Contributing New Messages

When adding new messages:

1. **Follow the naming convention** (`PREFIX_NNN`)
2. **Use appropriate type** (Success, Error, Warning, Info, Critical)
3. **Keep titles short** (< 100 chars)
4. **Provide helpful hints** when applicable
5. **Document placeholders** in this README
6. **Add to the appropriate category**
7. **Update the count** in the Quick Index

---

## 📄 License

MIT License - Part of RecurPixel.EasyMessages

---

**Last Updated:** v1.0.0  
**Total Messages:** 98  
**Categories:** 11
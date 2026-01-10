# API Response Patterns

Comprehensive guide to implementing standard REST API response patterns using EasyMessages in ASP.NET Core.

---

## Table of Contents

1. [Standard REST API Patterns](#standard-rest-api-patterns)
2. [CRUD Operations](#crud-operations)
3. [Validation Patterns](#validation-patterns)
4. [Error Handling](#error-handling)
5. [Authentication & Authorization](#authentication--authorization)
6. [Pagination & Filtering](#pagination--filtering)
7. [Best Practices](#best-practices)

---

## Standard REST API Patterns

### ApiResponse Format

All EasyMessages API responses follow this standardized format:

```json
{
  "success": true,
  "code": "CRUD_001",
  "type": "success",
  "title": "User Created",
  "description": "User has been created successfully.",
  "data": { ... },
  "timestamp": "2026-01-09T14:30:00.000Z",
  "correlationId": "0HMVQK8F3J8QK:00000001",
  "metadata": { ... }
}
```

**Fields:**
- `success` - Boolean indicating success/failure
- `code` - Unique message code (e.g., "CRUD_001")
- `type` - Message type ("success", "error", "warning", "info")
- `title` - Short summary
- `description` - Detailed explanation
- `data` - Response payload (entity, list, etc.)
- `timestamp` - When the message was created (optional)
- `correlationId` - Request tracking ID (optional)
- `metadata` - Additional context (optional)

---

## CRUD Operations

### Create (POST)

**Pattern:** Return created entity with 201 Created status and Location header

**Controller:**
```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpPost]
public IActionResult Create([FromBody] CreateUserDto dto)
{
    // Validate
    if (!ModelState.IsValid)
    {
        return Msg.Validation.Failed()
            .WithData(new { errors = ModelState })
            .ToApiResponse(); // 422 Unprocessable Entity
    }

    // Create entity
    var user = _userService.Create(dto);

    // Return with Location header
    return Msg.Crud.Created("User")
        .WithData(user)
        .Log(_logger)
        .ToCreated($"/api/users/{user.Id}"); // 201 Created with Location
}
```

**Response:**
```http
HTTP/1.1 201 Created
Location: /api/users/123
Content-Type: application/json

{
  "success": true,
  "code": "CRUD_001",
  "type": "success",
  "title": "User Created",
  "description": "User has been created successfully.",
  "data": {
    "id": 123,
    "name": "John Doe",
    "email": "john@example.com",
    "createdAt": "2026-01-09T14:30:00.000Z"
  },
  "correlationId": "0HMVQK8F3J8QK:00000001"
}
```

---

### Read - Single (GET by ID)

**Pattern:** Return entity or 404 Not Found

**Controller:**
```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpGet("{id}")]
public IActionResult GetById(int id)
{
    var user = _userService.FindById(id);

    if (user == null)
    {
        return Msg.Crud.NotFound("User")
            .WithMetadata("userId", id)
            .Log(_logger)
            .ToApiResponse(); // 404 Not Found
    }

    return Msg.Crud.Retrieved("User")
        .WithData(user)
        .ToApiResponse(); // 200 OK
}
```

**Success Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "success": true,
  "code": "CRUD_003",
  "type": "success",
  "title": "User Retrieved",
  "description": "User has been retrieved successfully.",
  "data": {
    "id": 123,
    "name": "John Doe",
    "email": "john@example.com"
  }
}
```

**Not Found Response:**
```http
HTTP/1.1 404 Not Found
Content-Type: application/json

{
  "success": false,
  "code": "CRUD_004",
  "type": "error",
  "title": "User Not Found",
  "description": "The requested User could not be found.",
  "metadata": {
    "userId": 999
  }
}
```

---

### Read - Collection (GET all)

**Pattern:** Return list of entities

**Controller:**
```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpGet]
public IActionResult GetAll()
{
    var users = _userService.GetAll();

    return Msg.Crud.Retrieved("Users")
        .WithData(users)
        .WithMetadata("count", users.Count)
        .ToApiResponse(); // 200 OK
}
```

**Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "success": true,
  "code": "CRUD_003",
  "type": "success",
  "title": "Users Retrieved",
  "description": "Users has been retrieved successfully.",
  "data": [
    { "id": 1, "name": "John Doe" },
    { "id": 2, "name": "Jane Smith" }
  ],
  "metadata": {
    "count": 2
  }
}
```

---

### Update (PUT)

**Pattern:** Return updated entity or 404 if not found

**Controller:**
```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpPut("{id}")]
public IActionResult Update(int id, [FromBody] UpdateUserDto dto)
{
    // Validate
    if (!ModelState.IsValid)
    {
        return Msg.Validation.Failed()
            .WithData(new { errors = ModelState })
            .ToApiResponse(); // 422
    }

    // Check exists
    if (!_userService.Exists(id))
    {
        return Msg.Crud.NotFound("User")
            .WithMetadata("userId", id)
            .ToApiResponse(); // 404
    }

    // Update
    var user = _userService.Update(id, dto);

    return Msg.Crud.Updated("User")
        .WithData(user)
        .Log(_logger)
        .ToApiResponse(); // 200 OK
}
```

**Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "success": true,
  "code": "CRUD_002",
  "type": "success",
  "title": "User Updated",
  "description": "User has been updated successfully.",
  "data": {
    "id": 123,
    "name": "John Doe Updated",
    "email": "john.updated@example.com",
    "updatedAt": "2026-01-09T14:35:00.000Z"
  }
}
```

---

### Delete (DELETE)

**Pattern:** Return 204 No Content or 200 OK with confirmation

**Option 1: 204 No Content (Recommended)**
```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpDelete("{id}")]
public IActionResult Delete(int id)
{
    if (!_userService.Exists(id))
    {
        return Msg.Crud.NotFound("User")
            .WithMetadata("userId", id)
            .ToApiResponse(); // 404
    }

    _userService.Delete(id);

    return Msg.Crud.Deleted("User")
        .Log(_logger)
        .ToNoContent(); // 204 No Content
}
```

**Response:**
```http
HTTP/1.1 204 No Content
```

**Option 2: 200 OK with Confirmation**
```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpDelete("{id}")]
public IActionResult Delete(int id)
{
    if (!_userService.Exists(id))
    {
        return Msg.Crud.NotFound("User")
            .WithMetadata("userId", id)
            .ToApiResponse(); // 404
    }

    _userService.Delete(id);

    return Msg.Crud.Deleted("User")
        .WithData(new { id, deletedAt = DateTime.UtcNow })
        .Log(_logger)
        .ToApiResponse(); // 200 OK
}
```

**Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "success": true,
  "code": "CRUD_005",
  "type": "success",
  "title": "User Deleted",
  "description": "User has been deleted successfully.",
  "data": {
    "id": 123,
    "deletedAt": "2026-01-09T14:40:00.000Z"
  }
}
```

---

## Validation Patterns

### Model Validation

**Pattern:** Return 422 Unprocessable Entity with validation errors

**Controller:**
```csharp
[HttpPost]
public IActionResult Create([FromBody] CreateUserDto dto)
{
    if (!ModelState.IsValid)
    {
        var errors = ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .ToDictionary(
                e => e.Key,
                e => e.Value.Errors.Select(x => x.ErrorMessage).ToArray()
            );

        return Msg.Validation.Failed()
            .WithData(new { errors })
            .ToApiResponse(); // 422 Unprocessable Entity
    }

    // ... create user
}
```

**Response:**
```http
HTTP/1.1 422 Unprocessable Entity
Content-Type: application/json

{
  "success": false,
  "code": "VAL_001",
  "type": "error",
  "title": "Validation Failed",
  "description": "One or more validation errors occurred.",
  "data": {
    "errors": {
      "Email": ["Invalid email format"],
      "Password": ["Password must be at least 8 characters"]
    }
  }
}
```

---

### FluentValidation Integration

**Validator:**
```csharp
using FluentValidation;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}
```

**Controller:**
```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpPost]
public IActionResult Create([FromBody] CreateUserDto dto)
{
    var validationResult = _validator.Validate(dto);

    if (!validationResult.IsValid)
    {
        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        return Msg.Validation.Failed()
            .WithData(new { errors })
            .ToApiResponse(); // 422
    }

    // ... create user
}
```

---

### Business Rule Validation

**Pattern:** Validate business rules and return appropriate error

**Controller:**
```csharp
[HttpPost]
public IActionResult Create([FromBody] CreateUserDto dto)
{
    // Check if email already exists
    if (_userService.EmailExists(dto.Email))
    {
        return Msg.Validation.Duplicate("Email")
            .WithMetadata("email", dto.Email)
            .ToApiResponse(); // 422
    }

    // Check if username is reserved
    if (_userService.IsReservedUsername(dto.Username))
    {
        return Msg.Validation.InvalidFormat("Username")
            .WithHint("This username is reserved. Please choose another.")
            .ToApiResponse(); // 422
    }

    // ... create user
}
```

**Response:**
```http
HTTP/1.1 422 Unprocessable Entity
Content-Type: application/json

{
  "success": false,
  "code": "VAL_014",
  "type": "error",
  "title": "Duplicate Value",
  "description": "The email 'john@example.com' already exists.",
  "metadata": {
    "email": "john@example.com"
  }
}
```

---

## Error Handling

### Global Exception Handling

**Middleware:**
```csharp
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await HandleExceptionAsync(context, ex, Msg.Crud.NotFound(ex.ResourceName));
        }
        catch (ValidationException ex)
        {
            await HandleExceptionAsync(context, ex, Msg.Validation.Failed()
                .WithData(new { errors = ex.Errors }));
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleExceptionAsync(context, ex, Msg.Auth.Unauthorized());
        }
        catch (DbUpdateException ex)
        {
            await HandleExceptionAsync(context, ex, Msg.Database.TransactionFailed()
                .WithMetadata("error", ex.Message));
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, Msg.System.Error()
                .WithMetadata("error", ex.Message));
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex, Message message)
    {
        _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

        var response = message
            .WithCorrelationId(context.TraceIdentifier)
            .ToApiResponse();

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = message.HttpStatusCode ?? 500;

        await context.Response.WriteAsJsonAsync(((ObjectResult)response).Value);
    }
}

// Register in Program.cs
app.UseMiddleware<GlobalExceptionHandler>();
```

---

### Database Errors

**Pattern:** Handle database-specific errors

**Controller:**
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpPost]
public IActionResult Create([FromBody] CreateUserDto dto)
{
    try
    {
        var user = _userService.Create(dto);
        return Msg.Crud.Created("User")
            .WithData(user)
            .ToCreated($"/api/users/{user.Id}");
    }
    catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
    {
        // Unique constraint violation
        return Msg.Validation.Duplicate("User")
            .WithMetadata("error", "A user with this email already exists")
            .ToApiResponse(); // 422
    }
    catch (DbUpdateException ex)
    {
        return Msg.Database.TransactionFailed()
            .WithMetadata("error", ex.Message)
            .Log(_logger)
            .ToApiResponse(); // 500
    }
}
```

**Response:**
```http
HTTP/1.1 500 Internal Server Error
Content-Type: application/json

{
  "success": false,
  "code": "DB_004",
  "type": "error",
  "title": "Transaction Failed",
  "description": "The database transaction failed and has been rolled back.",
  "metadata": {
    "error": "Connection timeout occurred"
  }
}
```

---

### External Service Errors

**Pattern:** Handle third-party service failures

**Controller:**
```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpPost("send-email")]
public async Task<IActionResult> SendEmail([FromBody] EmailDto dto)
{
    try
    {
        await _emailService.SendAsync(dto);
        return Msg.System.OperationCompleted()
            .WithData(new { sent = true })
            .ToApiResponse(); // 200 OK
    }
    catch (HttpRequestException ex)
    {
        return Msg.System.Unavailable()
            .WithMetadata("service", "EmailService")
            .WithMetadata("error", ex.Message)
            .Log(_logger)
            .ToApiResponse(); // 503 Service Unavailable
    }
    catch (TimeoutException ex)
    {
        return Msg.Network.Timeout()
            .WithMetadata("service", "EmailService")
            .Log(_logger)
            .ToApiResponse(); // 504 Gateway Timeout
    }
}
```

---

## Authentication & Authorization

### Login Success

**Pattern:** Return user info with auth token

**Controller:**
```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpPost("login")]
public IActionResult Login([FromBody] LoginDto dto)
{
    var user = _authService.Authenticate(dto.Email, dto.Password);

    if (user == null)
    {
        return Msg.Auth.LoginFailed()
            .WithMetadata("email", dto.Email)
            .Log(_logger)
            .ToApiResponse(); // 401 Unauthorized
    }

    var token = _tokenService.GenerateToken(user);

    return Msg.Auth.LoginSuccessful()
        .WithData(new
        {
            user = new { user.Id, user.Name, user.Email },
            token,
            expiresAt = DateTime.UtcNow.AddHours(24)
        })
        .Log(_logger)
        .ToApiResponse(); // 200 OK
}
```

**Success Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "success": true,
  "code": "AUTH_003",
  "type": "success",
  "title": "Login Successful",
  "description": "Welcome back, john!",
  "data": {
    "user": {
      "id": 123,
      "name": "John Doe",
      "email": "john@example.com"
    },
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2026-01-10T14:30:00.000Z"
  }
}
```

**Failure Response:**
```http
HTTP/1.1 401 Unauthorized
Content-Type: application/json

{
  "success": false,
  "code": "AUTH_001",
  "type": "error",
  "title": "Authentication Failed",
  "description": "Invalid username or password.",
  "metadata": {
    "email": "john@example.com"
  }
}
```

---

### Unauthorized Access

**Pattern:** Return 403 Forbidden for insufficient permissions

**Controller:**
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpDelete("{id}")]
[Authorize(Roles = "Admin")]
public IActionResult Delete(int id)
{
    // Check if user has permission
    if (!User.IsInRole("Admin"))
    {
        return Msg.Auth.Unauthorized()
            .WithMetadata("requiredRole", "Admin")
            .Log(_logger)
            .ToApiResponse(); // 403 Forbidden
    }

    _userService.Delete(id);

    return Msg.Crud.Deleted("User")
        .ToNoContent(); // 204 No Content
}
```

**Response:**
```http
HTTP/1.1 403 Forbidden
Content-Type: application/json

{
  "success": false,
  "code": "AUTH_002",
  "type": "error",
  "title": "Unauthorized Access",
  "description": "You do not have permission to perform this action.",
  "metadata": {
    "requiredRole": "Admin"
  }
}
```

---

### Expired Token

**Pattern:** Return 401 with token expired message

**Controller:**
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpGet("profile")]
[Authorize]
public IActionResult GetProfile()
{
    // Token validation happens in middleware/filter
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    var user = _userService.FindById(userId);

    return Msg.Crud.Retrieved("Profile")
        .WithData(user)
        .ToApiResponse();
}

// In JWT middleware/filter
if (tokenExpired)
{
    return Msg.Auth.SessionExpired()
        .ToApiResponse(); // 401 Unauthorized
}
```

**Response:**
```http
HTTP/1.1 401 Unauthorized
Content-Type: application/json

{
  "success": false,
  "code": "AUTH_004",
  "type": "error",
  "title": "Session Expired",
  "description": "Your session has expired."
}
```

---

## Pagination & Filtering

### Paginated Response

**Pattern:** Return page of results with pagination metadata

**Controller:**
```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpGet]
public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
{
    var result = _userService.GetPaged(page, pageSize);

    return Msg.Crud.Retrieved("Users")
        .WithData(result.Items)
        .WithMetadata("pagination", new
        {
            page,
            pageSize,
            totalCount = result.TotalCount,
            totalPages = result.TotalPages,
            hasNext = result.HasNext,
            hasPrevious = result.HasPrevious
        })
        .ToApiResponse();
}
```

**Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "success": true,
  "code": "CRUD_003",
  "type": "success",
  "title": "Users Retrieved",
  "description": "Users has been retrieved successfully.",
  "data": [
    { "id": 1, "name": "User 1" },
    { "id": 2, "name": "User 2" },
    { "id": 3, "name": "User 3" }
  ],
  "metadata": {
    "pagination": {
      "page": 1,
      "pageSize": 10,
      "totalCount": 25,
      "totalPages": 3,
      "hasNext": true,
      "hasPrevious": false
    }
  }
}
```

---

### Filtered/Searched Response

**Pattern:** Return filtered results with search criteria

**Controller:**
```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpGet("search")]
public IActionResult Search([FromQuery] string query, [FromQuery] string role = null)
{
    var users = _userService.Search(query, role);

    return Msg.Search.Completed(users.Count, query)
        .WithData(users)
        .WithMetadata("role", role)
        .ToApiResponse();
}
```

**Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "success": true,
  "code": "SEARCH_002",
  "type": "success",
  "title": "Search Completed",
  "description": "Found 2 result(s) for 'john'.",
  "data": [
    { "id": 5, "name": "John Doe", "role": "Admin" },
    { "id": 12, "name": "John Smith", "role": "User" }
  ],
  "metadata": {
    "query": "john",
    "role": "Admin",
    "count": 2
  }
}
```

---

### No Results Found

**Pattern:** Return empty array with informative message

**Controller:**
```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[HttpGet("search")]
public IActionResult Search([FromQuery] string query)
{
    var users = _userService.Search(query);

    if (!users.Any())
    {
        return Msg.Search.NoResults(query)
            .WithData(new List<object>())
            .ToApiResponse(); // 200 OK with empty array
    }

    return Msg.Search.Completed(users.Count, query)
        .WithData(users)
        .ToApiResponse();
}
```

**Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "success": true,
  "code": "SEARCH_001",
  "type": "info",
  "title": "No Results Found",
  "description": "No results found for 'xyz123'.",
  "data": [],
  "metadata": {
    "query": "xyz123"
  }
}
```

---

## Best Practices

### [✓] DO:

1. **Use standard HTTP status codes**
   ```csharp
   // Good - Appropriate status codes
   return Msg.Crud.Created("User").ToCreated("/api/users/123"); // 201
   return Msg.Crud.NotFound("User").ToApiResponse();            // 404
   return Msg.Validation.Failed().ToApiResponse();              // 422
   ```

2. **Include relevant data in responses**
   ```csharp
   // Good - Return created entity
   return Msg.Crud.Created("User")
       .WithData(user)
       .ToCreated($"/api/users/{user.Id}");
   ```

3. **Add metadata for debugging**
   ```csharp
   // Good - Helpful metadata
   return Msg.Crud.NotFound("User")
       .WithMetadata("userId", id)
       .WithMetadata("requestedBy", User.Identity.Name)
       .ToApiResponse();
   ```

4. **Log errors and warnings**
   ```csharp
   // Good - Log before returning
   return Msg.Auth.LoginFailed()
       .WithMetadata("email", dto.Email)
       .Log(_logger)
       .ToApiResponse();
   ```

5. **Use correlation IDs for tracing**
   ```csharp
   // Good - Automatic with interceptor enabled
   return Msg.System.Error()
       .Log(_logger)
       .ToApiResponse();
   // CorrelationId added automatically from HttpContext.TraceIdentifier
   ```

---

### [ ] DON'T:

1. **Don't return inconsistent formats**
   ```csharp
   // Bad - Mixing formats
   return Ok(new { message = "User created" });  // [ ] Custom format
   return Msg.Crud.Created("User").ToApiResponse(); // [✓] Standard format
   ```

2. **Don't expose sensitive data**
   ```csharp
   // Bad - Exposing password hash
   return Msg.Crud.Retrieved("User")
       .WithData(user) // [ ] Includes passwordHash
       .ToApiResponse();

   // Good - DTO without sensitive fields
   return Msg.Crud.Retrieved("User")
       .WithData(new UserDto(user)) // [✓] Safe DTO
       .ToApiResponse();
   ```

3. **Don't ignore validation**
   ```csharp
   // Bad - No validation
   var user = _userService.Create(dto); // [ ] What if dto is invalid?

   // Good - Validate first
   if (!ModelState.IsValid)
   {
       return Msg.Validation.Failed()
           .WithData(new { errors = ModelState })
           .ToApiResponse();
   }
   ```

4. **Don't swallow exceptions silently**
   ```csharp
   // Bad - Silent failure
   try
   {
       _userService.Delete(id);
   }
   catch { } // [ ] Error hidden!

   // Good - Log and return error
   try
   {
       _userService.Delete(id);
   }
   catch (Exception ex)
   {
       return Msg.System.Error()
           .WithMetadata("error", ex.Message)
           .Log(_logger)
           .ToApiResponse();
   }
   ```

5. **Don't use wrong HTTP methods**
   ```csharp
   // Bad - Wrong HTTP method
   [HttpGet("delete/{id}")] // [ ] Should be DELETE
   public IActionResult DeleteUser(int id) { ... }

   // Good - Correct HTTP method
   [HttpDelete("{id}")] // [✓] Proper RESTful design
   public IActionResult Delete(int id) { ... }
   ```

---

## Next Steps

- **[Logging Integration](Logging-Integration.md)** - Advanced logging strategies
- **[How-To Guides](../How-To-Guides/)** - Practical recipes
- **[Examples](../Examples/)** - Complete working examples

---

**Questions?** [Ask in Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

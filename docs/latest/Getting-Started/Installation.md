# Installation

Learn how to install EasyMessages packages for your .NET applications.

---

## Prerequisites

### Supported Platforms

**Current Beta Release (v0.1.0-beta.1):**
- .NET 8.0 or later
- .NET 9.0
- .NET 10.0

**Coming in Stable:**
- .NET 6.0
- .NET 7.0
- .NET Standard 2.1 (covers .NET 5+)

### Development Environment

- **IDE:** Visual Studio 2022, JetBrains Rider, or VS Code
- **SDK:** .NET 8.0 SDK or later
- **Package Manager:** NuGet (comes with .NET SDK)

---

## Choosing the Right Package

EasyMessages provides two packages. Choose based on your application type:

### Core Package: `RecurPixel.EasyMessages`

**Use for:**
- Console applications
- Background workers and services
- Class libraries
- Desktop applications
- Any non-web .NET application

**Includes:**
- 100+ pre-built messages
- Multiple output formats (JSON, XML, Console, PlainText)
- Custom messages support
- Custom formatters and interceptors
- Message registry and stores
- Parameter substitution
- Metadata and correlation IDs

### AspNetCore Package: `RecurPixel.EasyMessages.AspNetCore`

**Use for:**
- ASP.NET Core Web APIs
- MVC Applications
- Minimal APIs
- Blazor Server applications

**Includes:**
- Everything from Core package (automatically referenced)
- `.ToApiResponse()` - Convert to IActionResult
- `.Log(ILogger)` - Logging integration
- Dependency injection support
- IOptions configuration pattern
- Configuration presets (Development, Production, Testing, Staging, Api)
- Built-in interceptors (CorrelationId, MetadataEnrichment, Logging)
- HTTP context integration

---

## Installation Steps

### Option 1: Using .NET CLI (Recommended)

#### For Console Applications, Workers, Class Libraries:

```bash
dotnet add package RecurPixel.EasyMessages --version 0.1.0-beta.1
```

#### For ASP.NET Core Applications:

```bash
dotnet add package RecurPixel.EasyMessages.AspNetCore --version 0.1.0-beta.1
```

**Note:** **Note:** The AspNetCore package automatically includes the Core package as a dependency.

### Option 2: Using Visual Studio Package Manager

1. Right-click on your project in Solution Explorer
2. Select "Manage NuGet Packages..."
3. Click the "Browse" tab
4. Check "Include prerelease"
5. Search for:
   - `RecurPixel.EasyMessages` (for console apps)
   - `RecurPixel.EasyMessages.AspNetCore` (for web apps)
6. Click "Install"

### Option 3: Package Manager Console

```powershell
# For console applications
Install-Package RecurPixel.EasyMessages -Version 0.1.0-beta.1 -IncludePrerelease

# For ASP.NET Core applications
Install-Package RecurPixel.EasyMessages.AspNetCore -Version 0.1.0-beta.1 -IncludePrerelease
```

### Option 4: Edit .csproj Directly

Add the package reference to your `.csproj` file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <!-- For console applications -->
    <PackageReference Include="RecurPixel.EasyMessages" Version="0.1.0-beta.1" />

    <!-- OR for ASP.NET Core applications -->
    <PackageReference Include="RecurPixel.EasyMessages.AspNetCore" Version="0.1.0-beta.1" />
  </ItemGroup>
</Project>
```

Then restore packages:
```bash
dotnet restore
```

---

## Verify Installation

### For Console Applications

Create a simple test file:

```csharp
using RecurPixel.EasyMessages;

Console.WriteLine("Testing EasyMessages installation...");

// Test basic message creation
var message = Msg.Auth.LoginFailed();
Console.WriteLine($"✓ Message created: {message.Code}");

// Test console output
message.ToConsole(useColors: true);

// Test JSON output
var json = Msg.Crud.Created("User").ToJson();
Console.WriteLine($"✓ JSON output: {json}");

Console.WriteLine("\n✓ EasyMessages installed successfully!");
```

Run the application:
```bash
dotnet run
```

**Expected output:**
```
Testing EasyMessages installation...
✓ Message created: AUTH_001
✗ Authentication Failed
  Invalid username or password.
  [2026-01-09 14:30:00] [AUTH_001]
✓ JSON output: {"code":"CRUD_001","type":"success","title":"Created Successfully",...}

✓ EasyMessages installed successfully!
```

### For ASP.NET Core Applications

1. **Program.cs** - Add service registration:

```csharp
using RecurPixel.EasyMessages.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Register EasyMessages
builder.Services.AddEasyMessages(builder.Configuration);

builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();
```

2. **Create a test controller**:

```csharp
using Microsoft.AspNetCore.Mvc;
using RecurPixel.EasyMessages;
using RecurPixel.EasyMessages.AspNetCore;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Test()
    {
        return Msg.System.Processing()
            .WithData(new { message = "EasyMessages is working!" })
            .ToApiResponse();
    }
}
```

3. **Run and test**:
```bash
dotnet run
```

Navigate to: `https://localhost:5001/api/test`

**Expected JSON response:**
```json
{
  "success": true,
  "code": "SYS_002",
  "type": "info",
  "title": "Processing",
  "description": "Your request is being processed.",
  "timestamp": "2026-01-09T14:30:00Z",
  "data": {
    "message": "EasyMessages is working!"
  }
}
```

---

## Troubleshooting

### Issue: Package not found

**Error:** `Unable to find package 'RecurPixel.EasyMessages'`

**Solution:**
1. Ensure you have the latest NuGet sources:
   ```bash
   dotnet nuget list source
   ```
2. Add NuGet.org if missing:
   ```bash
   dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
   ```
3. Clear NuGet cache:
   ```bash
   dotnet nuget locals all --clear
   ```

### Issue: Version conflict

**Error:** `Package 'RecurPixel.EasyMessages' is not compatible with...`

**Solution:**
- Ensure your project targets .NET 8.0 or later
- Check your `.csproj` file:
  ```xml
  <TargetFramework>net8.0</TargetFramework>
  ```
- For multi-targeting, use:
  ```xml
  <TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>
  ```

### Issue: Beta version not appearing

**Solution:**
- Include prerelease packages:
  ```bash
  dotnet add package RecurPixel.EasyMessages --version 0.1.0-beta.1 --prerelease
  ```
- In Visual Studio, check "Include prerelease" in NuGet Package Manager

### Issue: IntelliSense not showing

**Solution:**
1. Rebuild your project:
   ```bash
   dotnet build
   ```
2. Restart your IDE
3. Check that the package is properly restored:
   ```bash
   dotnet restore
   ```

---

## Next Steps

Now that you have EasyMessages installed:

1. **[Your First Message (5 minutes)](Your-First-Message.md)** - Quick tutorial to create your first message
2. **[Console vs Web Applications](Console-vs-Web-Applications.md)** - Understand the differences
3. **Note:** **[Core Concepts](../Core-Concepts/Messages-and-Message-Types.md)** - Learn the fundamentals
4. **[Examples](../Examples/Console-Application.md)** - See real-world code examples

---

## Upgrading

### Version History

Check the [CHANGELOG](../../CHANGELOG.md) for version history and breaking changes.

### Staying Current

To always get the latest beta version, use:
```xml
<PackageReference Include="RecurPixel.EasyMessages" Version="0.1.0-beta.*" />
```

The `*` wildcard will automatically pull the latest patch version within the beta series.

---

## Package Contents

### What's Included

Both packages include:
- [✓] Pre-compiled assemblies for .NET 8, 9, 10
- [✓] XML documentation for IntelliSense
- [✓] Source link support for debugging
- [✓] NuGet package symbols

### Package Dependencies

**RecurPixel.EasyMessages (Core):**
- No external dependencies
- Uses only .NET BCL (Base Class Library)

**RecurPixel.EasyMessages.AspNetCore:**
- RecurPixel.EasyMessages (Core) - automatically installed
- Microsoft.AspNetCore.Http.Abstractions ≥ 2.2.0
- Microsoft.Extensions.DependencyInjection.Abstractions ≥ 8.0.0
- Microsoft.Extensions.Logging.Abstractions ≥ 8.0.0
- Microsoft.Extensions.Options ≥ 8.0.0
- Microsoft.Extensions.Options.ConfigurationExtensions ≥ 8.0.0

---

## Support & Resources

- [Complete Documentation](../Home.md)
- [Report Issues](https://github.com/RecurPixel/RecurPixel.EasyMessages/issues)
- **Note:** [Request Features](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)
- 📧 [Contact Support](https://github.com/RecurPixel)

---

**Ready to create your first message?** → [Your First Message (5 minutes)](Your-First-Message.md)

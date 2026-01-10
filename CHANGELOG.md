# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [0.1.0-beta.1] - 2026-01-10

### 🎉 Beta Release

First beta release with comprehensive documentation and stable APIs.

### Added

#### Documentation (Major Update)
- ✅ **15 comprehensive wiki pages** (30,000+ lines)
  - Getting Started section (4 pages) - Installation, tutorials, learning paths
  - Core Concepts section (6 pages) - Messages, Registry, Facades, Formatters, Interceptors, Architecture
  - ASP.NET Core section (5 pages) - Overview, Setup, Presets, API Patterns, Logging
- ✅ 200+ working code examples
- ✅ Complete API documentation
- ✅ Troubleshooting guides
- ✅ Best practices throughout
- ✅ REMAINING_DOCUMENTATION.md for future work planning
- ✅ DOCUMENTATION_SUMMARY.md for status tracking

#### Features
- All features from alpha releases remain stable and working
- APIs are now considered **stable**
- Full ASP.NET Core integration
- 100+ built-in message codes
- Multiple output formatters
- Interceptor system
- IOptions configuration pattern

### Changed

#### Package Metadata
- Version updated from `0.1.0-alpha.3` to `0.1.0-beta.1`
- Package descriptions updated to reflect beta status
- Added comprehensive documentation links to NuGet metadata
- Updated NuGet tags (removed `alpha`, added `beta`)
- Added package icons to both packages

#### Documentation Organization
- Moved redundant docs to `docs/archive/`:
  - ARCHITECTURE-FULL.md (superseded by wiki/Core-Concepts/Architecture-Overview.md)
  - CONFIGURATION_GUIDE.md (superseded by wiki/ASP-NET-Core/Setup-and-Configuration.md)
  - CONFIGURATION_MIGRATION_GUIDE.md (superseded by wiki documentation)
  - DOCS-Plan.md (planning complete, documentation created)
  - DESIGN_DOCUMENT.md (superseded by wiki documentation)
- Cleaned up docs folder to contain only essential technical documents

#### Status
- APIs are now considered **stable**
- Documentation is **production-ready**
- Ready for **testing and feedback**
- No breaking changes planned before 1.0

### Maintained Files

#### Essential Documentation (kept in docs/)
- ✅ MESSAGECODES.md - Complete message code reference (100+ codes)
- ✅ TECHNICAL_SPECIFICATION.md - Design decisions and rationale
- ✅ CONFIGURATION_IMPLEMENTATION_SUMMARY.md - Implementation details for contributors
- ✅ CONFIGURATION_TESTS_SUMMARY.md - Test documentation for contributors
- ✅ PERFORMANCE_OPTIMIZATIONS.md - Performance details and benchmarks
- ✅ DOCUMENTATION_SUMMARY.md - Documentation status and metrics
- ✅ REMAINING_DOCUMENTATION.md - Future documentation plans and context

---

## [0.1.0-alpha.3] - 2026-01-09

### Added
- IOptions pattern implementation for ASP.NET Core
- Configuration presets (Development, Production, Staging, Testing, API)
- FormatterOptions for controlling output verbosity
- Configuration validation with DataAnnotations
- Comprehensive test suite (237 tests passing)
- EasyMessagesStartupService for initialization
- MetadataEnrichmentFields configuration

### Changed
- Moved to IOptions pattern for ASP.NET Core configuration
- Improved performance with .NET 5-10 optimizations
- Enhanced thread safety with ImmutableDictionary
- Refined interceptor registration

### Deprecated
- Legacy MessageConfiguration pattern (marked obsolete, will be removed in 2.0)
- Old-style AddEasyMessages(Action<MessageConfiguration>) method

---

## [0.1.0-alpha.2] - 2026-01-08

### Added
- ASP.NET Core interceptors:
  - CorrelationIdInterceptor - Auto-add correlation IDs from HttpContext
  - MetadataEnrichmentInterceptor - Auto-enrich with request context
  - LoggingInterceptor - Auto-log messages via ILogger
- Configuration via appsettings.json binding
- Auto-logging support with configurable minimum log level
- FormatterRegistry for custom formatter registration
- InterceptorRegistry for managing interceptors
- FormatterConfiguration for global formatter options

### Changed
- Improved formatter architecture with base class
- Enhanced MessageRegistry with composite store support
- Better thread safety throughout

---

## [0.1.0-alpha.1] - 2026-01-07

### Added

#### Core Features
- Message system with immutable sealed record
- 100+ built-in message codes across 10 categories:
  - AUTH (10 codes) - Authentication & Authorization
  - CRUD (10 codes) - CRUD Operations
  - VAL (15 codes) - Validation
  - SYS (10 codes) - System Messages
  - DB (8 codes) - Database
  - FILE (12 codes) - File Operations
  - NET (10 codes) - Network
  - SEARCH (5 codes) - Search Operations
  - IMPORT (10 codes) - Import/Export
  - CONFIG (10 codes) - Configuration

#### Message Facades
- Msg.Auth - Authentication messages
- Msg.Crud - CRUD operation messages
- Msg.Validation - Validation messages
- Msg.System - System messages
- Msg.Database - Database messages
- Msg.File - File operation messages
- Msg.Network - Network messages
- Msg.Search - Search messages
- Msg.Import - Import/Export messages
- Msg.Config - Configuration messages

#### Output Formatters
- JSON formatter with customizable options
- XML formatter
- Console formatter with ANSI colors
- PlainText formatter
- Custom formatter support via IMessageFormatter

#### Message Stores
- EmbeddedMessageStore (built-in defaults)
- FileMessageStore (load from JSON files)
- DatabaseMessageStore (custom database implementation)
- DictionaryMessageStore (in-memory for testing)
- CompositeMessageStore (combine multiple stores with priority)

#### Extension Methods
- `.WithData(object)` - Add payload data
- `.WithMetadata(string, object)` - Add metadata
- `.WithCorrelationId(string)` - Add correlation ID
- `.WithHint(string)` - Add user hint
- `.WithStatusCode(int)` - Override HTTP status
- `.WithParams(object)` - Parameter substitution

#### ASP.NET Core Integration
- `.ToApiResponse()` - Convert to IActionResult (controllers)
- `.ToMinimalApiResult()` - Convert to IResult (Minimal APIs)
- `.ToCreated(string)` - 201 Created with Location header
- `.ToCreatedAtAction()` - 201 with action routing
- `.ToNoContent()` - 204 No Content
- `.Log(ILogger)` - Manual logging integration
- `AddEasyMessages()` - DI registration

#### Core Architecture
- Thread-safe MessageRegistry with ImmutableDictionary
- Lazy loading of default messages
- Atomic updates with Interlocked.Exchange
- Interceptor pipeline (OnBeforeFormat, OnAfterFormat)
- Custom message support with partial overrides
- Parameter substitution in title and description

### Technical Details
- ✅ .NET 8, 9, 10 support
- ✅ Immutable message records
- ✅ Fluent API design
- ✅ HTTP status code mapping
- ✅ Correlation ID support
- ✅ Structured logging support
- ✅ Thread-safe operations
- ✅ Performance optimized

---

## Versioning Strategy

### Pre-1.0 Releases

- **Alpha (0.1.0-alpha.x)** - Core features, APIs subject to change
- **Beta (0.1.0-beta.x)** - Feature complete, stable APIs, comprehensive documentation **(Current)**
- **RC (0.1.0-rc.x)** - Release candidate, production testing

### Post-1.0 Releases

- **Patch (1.0.x)** - Bug fixes, documentation updates
- **Minor (1.x.0)** - New features, backward compatible
- **Major (x.0.0)** - Breaking changes

---

## Links

- [Documentation](https://recurpixel.github.io/RecurPixel.EasyMessages/)
- [GitHub Repository](https://github.com/RecurPixel/RecurPixel.EasyMessages)
- [NuGet Package (Core)](https://www.nuget.org/packages/RecurPixel.EasyMessages)
- [NuGet Package (AspNetCore)](https://www.nuget.org/packages/RecurPixel.EasyMessages.AspNetCore)
- [Issue Tracker](https://github.com/RecurPixel/RecurPixel.EasyMessages/issues)
- [Discussions](https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions)

---

**Note:** This project follows [Semantic Versioning](https://semver.org/) starting from version 1.0.0.

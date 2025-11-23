# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [0.1.0-alpha.1] - 2024-01-XX

### 🎉 First Alpha Release

This is the first public preview of EasyMessages!

### Added
- Core message system with immutable Message record
- MessageRegistry for loading messages from JSON
- 50 essential built-in messages (AUTH, CRUD, VAL, SYS)
- Msg facade API (Msg.Auth.LoginFailed(), etc.)
- Parameter substitution ({field} placeholders)
- JSON formatter
- Console formatter (with colors)
- Custom message loading from JSON files
- Thread-safe registries

### Known Issues
- AspNetCore integration not available yet (coming in beta)
- Limited formatter options
- No interceptor system
- Documentation in progress

### Breaking Changes
- None (initial release)

---

## [Unreleased]

### Planned for Beta (v0.2.0-beta.1)
- AspNetCore package
- .ToApiResponse() extension
- Interceptor system
- XML & PlainText formatters
- Expand to 100+ messages

---

[0.1.0-alpha.1]: https://github.com/RecurPixel/EasyMessages/releases/tag/v0.1.0-alpha.1
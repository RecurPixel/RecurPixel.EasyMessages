## Roadmap

### Phase 1: Alpha (Current - v0.1.0-alpha.1)
**Goal:** Validate core design with early adopters

**Supported Versions:** .NET 8, 9, 10

**Features:**
- ✅ Core message system
- ✅ 100+ essential messages
- ✅ JSON, XML, Console, PlainText formatters
- ✅ Parameter substitution
- ✅ Custom message loading

**What's Missing:**
- AspNetCore package
- Advanced interceptors
- Broader .NET version support

**Timeline:** 2 weeks for feedback

---

### Phase 2: Beta (v0.2.0-beta.1)
**Target:** 2 weeks after alpha

**Supported Versions:** .NET 6, 7, 8, 9, 10

**New Features:**
- AspNetCore package (.ToApiResponse())
- Logging integration
- Interceptor system
- Expand to 150+ messages
- **Add .NET 6, 7 support** ✨

**Focus:** ASP.NET Core integration + broader compatibility

---

### Phase 3: Release Candidate (v1.0.0-rc.1)
**Target:** 4 weeks after alpha

**Supported Versions:** .NET Standard 2.1, .NET 6-10

**Focus:**
- Bug fixes only
- Documentation completion
- Performance testing
- **Add .NET Standard 2.1 support** ✨

---

### Phase 4: Stable (v1.0.0)
**Target:** 6 weeks after alpha

**Supported Versions:** .NET Standard 2.1, .NET 6-10

**Commitment:**
- Production-ready
- Semantic versioning
- Long-term support
- **Full cross-platform compatibility** ✨

---

## Comparison Table

| Phase | Alpha | Beta | RC/1.0 |
|-------|-------|------|--------|
| **.NET Version** | 8, 9, 10 | 6, 7, 8, 9, 10 | Standard 2.1 + 6-10 |
| **Message Count** | 100+ | 150+ | 200+ |
| **AspNetCore** | ❌ | ✅ | ✅ |
| **Testing** | Minimal | Moderate | Full |
| **Stability** | ⚠️ Experimental | 🔧 Testing | ✅ Production |
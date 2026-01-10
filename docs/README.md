# EasyMessages Documentation

This folder contains the complete documentation for RecurPixel.EasyMessages, organized for GitHub Pages deployment with version support.

## 📁 Structure

```
docs/
├── index.md                  # Root landing page with version selector
├── _config.yml              # Jekyll/GitHub Pages configuration
│
├── latest/                   # Latest version documentation (always current)
│   ├── index.md             # Main documentation home
│   ├── Getting-Started/     # Tutorial content
│   ├── Core-Concepts/       # Conceptual documentation
│   ├── ASP-NET-Core/        # ASP.NET Core integration guide
│   ├── API-Reference/       # API reference (planned)
│   ├── Examples/            # Code examples (planned)
│   ├── How-To-Guides/       # Task-oriented guides (planned)
│   ├── Advanced-Topics/     # Advanced topics (planned)
│   └── Migration-Guides/    # Migration guides (planned)
│
├── v0.1.0-beta.1/           # Tagged version (frozen at release)
│   └── (same structure as latest/)
│
├── dev/                      # Developer/contributor documentation
│   ├── CONFIGURATION_IMPLEMENTATION_SUMMARY.md
│   ├── CONFIGURATION_TESTS_SUMMARY.md
│   ├── TECHNICAL_SPECIFICATION.md
│   ├── PERFORMANCE_OPTIMIZATIONS.md
│   └── REMAINING_DOCUMENTATION.md
│
├── reference/                # Reference documentation
│   ├── MESSAGECODES.md      # Complete message code reference
│   └── DOCUMENTATION_SUMMARY.md
│
└── archive/                  # Archived documentation
    ├── ARCHITECTURE-FULL.md
    ├── CONFIGURATION_GUIDE.md
    ├── CONFIGURATION_MIGRATION_GUIDE.md
    ├── DOCS-Plan.md
    └── DESIGN_DOCUMENT.md
```

## GitHub Pages Deployment

### URLs

After deployment, documentation will be available at:

- **Root:** https://recurpixel.github.io/RecurPixel.EasyMessages/
- **Latest:** https://recurpixel.github.io/RecurPixel.EasyMessages/latest/
- **Beta 1:** https://recurpixel.github.io/RecurPixel.EasyMessages/v0.1.0-beta.1/
- **Developer Docs:** https://recurpixel.github.io/RecurPixel.EasyMessages/dev/
- **Reference:** https://recurpixel.github.io/RecurPixel.EasyMessages/reference/

### Deployment Steps

1. **Enable GitHub Pages:**
   - Go to repository Settings → Pages
   - Source: Deploy from a branch
   - Branch: `main` → `/docs` folder
   - Save

2. **Wait for deployment** (2-5 minutes)
   - GitHub Actions will automatically build and deploy
   - Check deployment status in Actions tab

3. **Verify deployment:**
   - Visit https://recurpixel.github.io/RecurPixel.EasyMessages/
   - Check that all pages load correctly
   - Verify internal links work

## Version Management

### Creating a New Version

When releasing a new version (e.g., v0.2.0-beta.1):

```bash
# 1. Copy latest to new version folder
cp -r docs/latest docs/v0.2.0-beta.1

# 2. Update docs/latest with new content
# (edit files in docs/latest/)

# 3. Update docs/index.md to include new version in list

# 4. Commit and push
git add docs/
git commit -m "Documentation for v0.2.0-beta.1"
git push
```

### Version URLs

Users can access specific versions:
- Latest (always current): `/latest/`
- Specific version: `/v0.1.0-beta.1/`, `/v0.2.0-beta.1/`, etc.

This allows users to:
- View docs matching their installed version
- Compare changes between versions
- Access old docs if needed

## Documentation Categories

### User Documentation (`latest/` and versioned folders)

**Getting Started** - Tutorials for new users
- Installation instructions
- Quick start guides
- Basic concepts

**Core Concepts** - In-depth explanations
- Architecture overview
- Key concepts
- Design patterns

**ASP.NET Core** - Integration guide
- Setup and configuration
- API response patterns
- Logging integration

**How-To Guides** - Task-oriented instructions (planned)
**Examples** - Complete code examples (planned)
**API Reference** - Detailed API documentation (planned)
**Advanced Topics** - Deep dives (planned)
**Migration Guides** - Upgrade guides (planned)

### Developer Documentation (`dev/`)

Technical documentation for contributors:
- Implementation summaries
- Test documentation
- Technical specifications
- Performance analysis
- Future work planning

### Reference Documentation (`reference/`)

Quick reference materials:
- Message codes reference (all 100+ built-in codes)
- Documentation status and metrics

### Archived Documentation (`archive/`)

Superseded documentation kept for historical reference:
- Old architecture documents
- Legacy configuration guides
- Original design documents

## Maintenance

### Updating Latest Documentation

Always update `docs/latest/` for current development:

```bash
# Edit files in docs/latest/
vim docs/latest/Core-Concepts/Messages-and-Message-Types.md

# Commit changes
git add docs/latest/
git commit -m "Update message types documentation"
git push
```

### Adding New Pages

1. Create markdown file in appropriate folder under `docs/latest/`
2. Update navigation in `docs/latest/index.md`
3. Add cross-references from related pages
4. Commit and push

### Fixing Broken Links

Links should use relative paths:
```markdown
<!-- Good - relative links -->
[Installation](Getting-Started/Installation.md)
[Core Concepts](../Core-Concepts/Messages-and-Message-Types.md)

<!-- Avoid - absolute links -->
[Installation](/docs/latest/Getting-Started/Installation.md)
```

## Jekyll Theme

We use the **Cayman theme** which provides:
- Clean, modern design
- Responsive layout (mobile-friendly)
- Syntax highlighting for code blocks
- GitHub integration

Theme customization in `_config.yml`:
- Site title and description
- Navigation links
- Base URL configuration
- Markdown processor settings

## Writing Guidelines

### Markdown Format

- Use GitHub-Flavored Markdown (GFM)
- Include syntax highlighting for code blocks
- Use relative links for internal pages
- Add front matter for Jekyll pages (optional)

### Code Examples

Always use language-specific syntax highlighting:

````markdown
```csharp
using RecurPixel.EasyMessages;

Msg.Auth.LoginFailed().ToConsole();
```
````

### Cross-References

Link to related topics generously:
- Link to prerequisite concepts
- Link to related guides
- Link to API reference
- Provide "Next Steps" at the end

### Consistency

- Follow existing page structure
- Use consistent heading levels
- Match tone and style of existing docs
- Use emoji sparingly (status indicators only)

## 🧪 Testing Locally

### Option 1: Jekyll (Full simulation)

```bash
# Install Jekyll
gem install bundler jekyll

# Create Gemfile
echo "source 'https://rubygems.org'" > Gemfile
echo "gem 'github-pages', group: :jekyll_plugins" >> Gemfile

# Install dependencies
bundle install

# Serve locally
bundle exec jekyll serve --source docs

# Visit http://localhost:4000/RecurPixel.EasyMessages/
```

### Option 2: Simple HTTP server

```bash
# Python 3
python -m http.server 8000 --directory docs

# Visit http://localhost:8000/latest/
```

### Option 3: VS Code Extension

Install "Live Server" extension:
- Right-click `docs/index.md` → "Open with Live Server"
- May need markdown preview extension

## 🔍 Quality Checklist

Before committing documentation changes:

- [ ] All code examples compile
- [ ] All internal links work
- [ ] Markdown renders correctly
- [ ] Images display (if any)
- [ ] Consistent with existing style
- [ ] No typos or grammar errors
- [ ] Version information is accurate
- [ ] Navigation is updated

## 📞 Questions?

For documentation questions:
- **Issues:** https://github.com/RecurPixel/RecurPixel.EasyMessages/issues
- **Discussions:** https://github.com/RecurPixel/RecurPixel.EasyMessages/discussions
- **Developer Docs:** See `dev/REMAINING_DOCUMENTATION.md` for guidelines

---

**Documentation Status:** 15 pages complete, 30,000+ lines
**Last Updated:** 2026-01-10
**Current Version:** v0.1.0-beta.1

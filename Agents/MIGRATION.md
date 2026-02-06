# Migration Notes

## Reorganization Complete

The Agents directory has been reorganized into a clear structure for use by both Cursor and Claude.

## Old Files → New Location

| Old File | New Location | Notes |
|----------|-------------|-------|
| `agent-instructions.md` | Split into `rules/documentation.md` and `rules/coding-standards.md` | Documentation rules separated from coding standards |
| `vibe-instructions.md` | Merged into `context/project-overview.md` | Project vibe merged with overview |
| `project-context.md` | Merged into `context/project-overview.md` | Consolidated project context |
| `eccfin-identity.md` | Consolidated into `context/branding.md` | Branding information unified |
| `branding-assets.md` | Consolidated into `context/branding.md` | Branding information unified |
| `persistence-rule.md` | Moved to `guides/daily-blessing-rule.md` | Renamed for clarity |
| `backend-setup.md` | Moved to `guides/backend-setup.md` | Enhanced with more details |
| `frontend-setup.md` | Moved to `guides/frontend-setup.md` | Enhanced with more details |
| `pwa-implementation.md` | Moved to `guides/pwa-implementation.md` | Kept as guide |
| `admin-qr.md` | Moved to `guides/admin-qr-generator.md` | Renamed for clarity |

## New Structure

```
Agents/
├── README.md                    # Main index
├── MIGRATION.md                 # This file
├── rules/                       # Coding standards
│   ├── coding-standards.md
│   ├── documentation.md
│   └── unit-testing.md
├── guides/                      # Implementation guides
│   ├── admin-qr-generator.md
│   ├── backend-setup.md
│   ├── daily-blessing-rule.md
│   ├── frontend-setup.md
│   └── pwa-implementation.md
└── context/                     # Project context
    ├── branding.md
    └── project-overview.md
```

## Next Steps

1. **Review** the new structure and files
2. **Test** that Cursor and Claude can access the instructions properly
3. **Remove** old files from root `Agents/` directory once confirmed working:
   - `agent-instructions.md`
   - `vibe-instructions.md`
   - `project-context.md`
   - `eccfin-identity.md`
   - `branding-assets.md`
   - `persistence-rule.md`
   - `backend-setup.md`
   - `frontend-setup.md`
   - `pwa-implementation.md`
   - `admin-qr.md`

## Benefits

- **Clear organization** by purpose (rules, guides, context)
- **Consolidated information** (no duplicate branding files)
- **Better discoverability** with README index
- **Compatible** with both Cursor (.cursorrules) and Claude (markdown)
- **Easier maintenance** with logical grouping

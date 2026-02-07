# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Encourager is a scripture verse encouragement PWA that serves random Bible verses in English, Amharic, and Finnish. It enforces a "one blessing per day" rule — users receive one verse and must click "Amen" to lock it in; subsequent visits that day show a reflection view with countdown to midnight.

## Build & Dev Commands

### Frontend (React 19 + Vite + Tailwind CSS 4)
```bash
cd frontend
npm install          # install dependencies
npm run dev          # dev server on :5173, proxies /api to backend
npm run build        # tsc -b && vite build
npm run lint         # eslint (flat config)
```

### Backend (.NET 10 Minimal APIs)
```bash
cd backend
dotnet run           # Kestrel on :5226
dotnet build --configuration Release
dotnet test          # runs from solution root or tests/ dir
```

### Full Stack (Docker Compose)
```bash
docker compose up    # backend :8080 (internal), frontend :4000
```

### E2E Tests (Playwright)
```bash
cd e2e
npx playwright install --with-deps chromium   # first time setup
npx playwright test                           # headless
npm run test:headed                           # with browser UI
```
E2E tests expect the app running at `http://localhost:4000` (Docker Compose).

### Backend Unit Tests
```bash
dotnet test                                  # run all tests
dotnet test tests/Encourager.Api.Tests/     # run specific project tests
dotnet test --filter "FullyQualifiedName~GetRandomVerse"  # run single test
dotnet test --verbosity normal              # verbose output
```

## Architecture

**Monorepo** with three top-level directories: `backend/`, `frontend/`, `infrastructure/`.

### Backend (.NET 10 Minimal APIs)
- **Dual entry points** with shared configuration:
  - `Program.cs` — Kestrel web server for local development
  - `LambdaEntryPoint.cs` — AWS Lambda container entry point
  - `AppConfiguration.cs` — shared DI service registration and endpoint mapping used by both
- **Multi-Agent Service Layer** (all singletons):
  - `IVerseCoordinatorService` — orchestrates Lookup → Language → Formatter sub-services
  - `IVerseLookupService` — random or ID-based verse selection, returns `VerseMetadata`
  - `IVerseLanguageService` — language-specific text retrieval with English fallback, returns `VerseText`
  - `IVerseFormatterService` — assembles `VerseResponse` from metadata + text
- **Data Layer**:
  - `VerseRepository` — static indexed data from `EnglishVerses`, `AmharicVerses`, `FinnishVerses` arrays
  - `ReferenceParser` — parses Bible references ("1 Peter 5:7") into structured metadata (Book, Chapter, VerseNumber)
  - Each data class contains ~50 verses with `Text` and `Reference` properties
- **API Endpoints**:
  - `GET /api/verse/random?lang={en|am|fi}&verseId={1-50}` — random or specific verse
  - `GET /api/health` — health check with timestamp
- **API Response**: `{ verseId, book, chapter, verseNumber, text, language }`
- **CORS**: Controlled by `ALLOWED_ORIGIN` env var (defaults to `*`, restrict in production)
- **VerseScraper Tool** (`tools/VerseScraper/`): Development tool that scrapes English verses from biblestudytools.com, fetches Finnish/Amharic translations via free Bible APIs, deduplicates, and updates the static data files

### Frontend (React 19 + Vite + Tailwind CSS 4)
- **TypeScript**: Strict mode enabled, no `any` types allowed
- **State Management**:
  - `LanguageContext` — global language state (en/am/fi), persisted to `localStorage.lang`
  - Daily blessing state in `localStorage.last_blessing_data`: `{ timestamp: ISO8601, verse: { verseId, book, chapter, verseNumber, text, language } }`
  - Legacy localStorage migration: auto-converts old `{ text, reference, index }` format to new format
  - Blessing locks after "Amen" click until midnight; date comparison checks year-month-day match
- **Routing**: React Router v7 with two routes:
  - `/` — Home page (verse display, Amen button, reflection view if already blessed today)
  - `/admin` — QR code generator (not linked in UI, admin access only)
- **Animations**: Framer Motion for verse transitions, canvas-confetti for celebration on "Amen"
- **PWA**: vite-plugin-pwa with Workbox strategies:
  - Network-first for `/api/*` routes (fresh data, fallback to cache)
  - Cache-first for static assets (CSS, JS, images)

### Infrastructure (AWS)
- SAM template at `infrastructure/template.yaml`
- CloudFront → S3 (frontend) + API Gateway → Lambda (backend)
- Deployment scripts: `scripts/deploy-backend.sh`, `scripts/deploy-frontend.sh`
- CI/CD: `.github/workflows/ci.yml` (build+test), `.github/workflows/deploy.yml`

## Coding Conventions

### C# / .NET
- C# 14, nullable reference types enabled, minimal APIs
- Test naming: `[MethodName]_Should[ExpectedBehavior]_When[Condition]`
- xUnit + NSubstitute for mocking, Test Data Builders for domain objects
- No comments in unit tests; no blank lines between Arrange statements; only blank lines between Arrange/Act/Assert sections
- Assign results to `actual` variable

### TypeScript / React
- Strict TypeScript — no `any` types
- Functional components with hooks only
- Let React Compiler handle optimizations (avoid manual useMemo/useCallback)
- Context API for shared state, avoid prop drilling
- Tailwind CSS utility classes, mobile-first responsive design
- E2E selectors: prefer `data-testid` or role-based

### Branding Colors
- Primary (Navy): `#1a374f` — headings, buttons, logos
- Secondary (Green): `#6f9078` — borders, secondary icons
- Accent (Terracotta): `#d06450` — highlights
- Background (Cream): `#fdfbca` — main background
- See `Agents/context/branding.md` for full guidelines

## Key Business Rules

- **One blessing per day**:
  - Verse locks after user clicks "Amen" (not on page load — allows reading without commitment)
  - Stored in `localStorage.last_blessing_data` with ISO timestamp and verse data
  - Date comparison checks year-month-day equality with current date
  - If already blessed today: show "reflection view" with saved verse and countdown to midnight
  - Resets automatically at midnight (local time)
- **Three languages**: English (en), Amharic (am), Finnish (fi). Always test all three when modifying verse display or API.
- **PWA Offline Support**:
  - Service worker caches API responses (5 min TTL) and static assets
  - App shell loads immediately, verses load from cache if offline
  - Install prompt available on supported browsers

## Agent Documentation

Detailed guides live in `Agents/` — see `Agents/README.md` for index:
- `Agents/rules/coding-standards.md` — code style
- `Agents/rules/unit-testing.md` — test patterns and builder usage
- `Agents/rules/auto-documentation.md` ⭐ **MANDATORY** — documentation standards and automatic updates with code changes
- `Agents/context/branding.md` — ECCFIN brand colors and UI guidelines
- `Agents/guides/daily-blessing-rule.md` — one-blessing-per-day implementation details
- `Agents/guides/backend-setup.md` — .NET 10 API setup and structure
- `Agents/guides/frontend-setup.md` — React 19 setup and component structure
- `Agents/guides/pwa-implementation.md` — Progressive Web App setup
- `Agents/guides/admin-qr-generator.md` — QR code generation for admin
- `Agents/guides/bible_verse_multiagent_design.md` — multi-agent service layer design and ScraperAgent

### Important: Auto-Documentation Rule

**When making ANY code changes, you MUST automatically update relevant documentation files and include those updates in the SAME commit.** See `Agents/rules/auto-documentation.md` for:
- Code-to-documentation mapping tables
- Step-by-step update instructions
- Mermaid diagram update guidelines
- Commit message format requirements

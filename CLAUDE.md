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
dotnet test tests/Encourager.Api.Tests/
```

## Architecture

**Monorepo** with three top-level directories: `backend/`, `frontend/`, `infrastructure/`.

### Backend
- Dual entry points: `Program.cs` (local dev) and `LambdaEntryPoint.cs` (AWS Lambda)
- `AppConfiguration.cs` is the shared setup — registers DI services and maps endpoints, used by both entry points
- `VerseService` (singleton) serves verses from static in-memory data classes (`EnglishVerses`, `AmharicVerses`, `FinnishVerses`)
- API: `GET /api/verse/random?lang=en&index=0`, `GET /api/health`
- CORS origin controlled by `ALLOWED_ORIGIN` env var (defaults to `*`)

### Frontend
- React 19 + TypeScript strict mode, Vite 7, Tailwind CSS 4
- `LanguageContext` (React Context) manages global language state, persisted to localStorage
- React Router v7 with two routes: `/` (Home) and `/admin` (QR code generator)
- Framer Motion for animations, canvas-confetti for celebration effects
- PWA via vite-plugin-pwa with Workbox (network-first for API, cache-first for static assets)
- Daily blessing state tracked in localStorage key `last_blessing_data`

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

- **One blessing per day**: Verse locks after user clicks "Amen". Stored in localStorage with timestamp. Next day resets automatically.
- **Three languages**: English (en), Amharic (am), Finnish (fi). Always test all three when modifying verse display or API.
- **PWA**: App works offline. Service worker uses network-first for `/api/*`, cache-first for static assets.

## Agent Documentation

Detailed guides live in `Agents/` — see `Agents/README.md` for index:
- `Agents/rules/coding-standards.md` — code style
- `Agents/rules/unit-testing.md` — test patterns and builder usage
- `Agents/context/branding.md` — ECCFIN brand colors and UI guidelines
- `Agents/guides/daily-blessing-rule.md` — one-blessing-per-day implementation details

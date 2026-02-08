---
# Bible Verse Multi-Agent Design

This document describes the **multi-agent, in-memory Bible verse system** using VerseId as a key, without a database. It covers the implemented service layer architecture, data flow, and the ScraperAgent tool.

---

## 1. Data Structure

Verse data is stored in static C# arrays (`EnglishVerses`, `AmharicVerses`, `FinnishVerses`) with ~50 verses each. At construction, `VerseRepository` (implementing `IVerseRepository`, registered as singleton via DI) validates that all three arrays have equal length, then indexes them into dictionaries keyed by VerseId (1-based):

```
VerseRepository.Metadata[verseId] → VerseMetadata(VerseId, Book, Chapter, VerseNumber)
VerseRepository.Translations[verseId]["en"] → "For I know the plans..."
VerseRepository.Translations[verseId]["fi"] → "Sillä minä tiedän..."
VerseRepository.Translations[verseId]["am"] → "እኔ ስለ እናንተ..."
```

- **Outer key**: VerseId (int, 1-based)
- **Inner keys**: Language codes (en, fi, am)
- **Lookup**: O(1) dictionary access

---

## 2. Multi-Agent Service Layer (Implemented)

### Services:

1. **VerseCoordinatorService** (`IVerseCoordinatorService`):
   - Orchestrates requests
   - Maintains VerseId consistency
   - Calls sub-services in sequence: Lookup → Language → Formatter

2. **VerseLookupService** (`IVerseLookupService`):
   - Injected with `IVerseRepository`
   - `GetRandom()` → returns `VerseMetadata` with random VerseId
   - `GetByVerseId(int verseId)` → returns `VerseMetadata` (clamped to valid range — single clamping responsibility)

3. **VerseLanguageService** (`IVerseLanguageService`):
   - Injected with `IVerseRepository`
   - `GetText(int verseId, string language)` → returns `VerseText` (expects already-valid verseId from LookupService)
   - Falls back to English for unsupported languages

4. **VerseFormatterService** (`IVerseFormatterService`):
   - `Format(VerseMetadata, VerseText)` → returns `VerseResponse`
   - Assembles complete response DTO

### Architecture Diagram

```text
                    +-----------------+
                    |  Client / UI    |
                    +-----------------+
                              |
                              v
                    +-----------------+
                    |  API Endpoint   |
                    +-----------------+
                              |
                              v
                    +-----------------+
                    |  Coordinator    |
                    +-----------------+
                     /        |        \
                    /         |         \
           +-------------+ +-------------+ +-------------+
           | LookupSvc  | | LanguageSvc | | FormatterSvc|
           +-------------+ +-------------+ +-------------+
                |                |                |
                v                v                v
         VerseMetadata    VerseText         VerseResponse
                |                |
                v                v
           VerseRepository (in-memory dictionaries)
```

### Response Format

```json
{
  "verseId": 1,
  "book": "Jeremiah",
  "chapter": 29,
  "verseNumber": "11",
  "text": "For I know the plans I have for you...",
  "language": "en"
}
```

---

## 3. ReferenceParser

Parses Bible reference strings into structured metadata using regex:

```
"Psalm 23:1"       → (Book: "Psalm",       Chapter: 23, VerseNumber: "1")
"1 Peter 5:7"      → (Book: "1 Peter",     Chapter: 5,  VerseNumber: "7")
"Numbers 6:24-25"  → (Book: "Numbers",     Chapter: 6,  VerseNumber: "24-25")
"2 Corinthians 12:9" → (Book: "2 Corinthians", Chapter: 12, VerseNumber: "9")
```

---

## 4. ScraperAgent (Development Tool)

The `tools/VerseScraper` console application scrapes new verses and adds them to the data files:

### Usage
```bash
cd tools/VerseScraper
dotnet run -- --url "https://www.biblestudytools.com/topical-verses/inspirational-bible-verses/"
```

### Pipeline
1. **EnglishVerseScraper**: Scrapes biblestudytools.com page with AngleSharp HTML parser
2. **VerseDeduplicator**: Compares scraped references against existing verses, removes duplicates
3. **TranslationFetcher**: Fetches Finnish (bible-api.com) and Amharic (bible-api-kappa.vercel.app) translations
4. **DataFileGenerator**: Appends new verses to `EnglishVerses.cs`, `AmharicVerses.cs`, `FinnishVerses.cs`

### Dependencies
- **AngleSharp** — HTML parsing for web scraping
- **System.Net.Http.Json** — HTTP client for translation APIs
- References `Encourager.Api` project for access to existing verse data

---

## 5. DI Registration

All services are registered as singletons in `AppConfiguration.cs`:

```csharp
services.AddSingleton<IVerseRepository, VerseRepository>();
services.AddSingleton<IVerseLookupService, VerseLookupService>();
services.AddSingleton<IVerseLanguageService, VerseLanguageService>();
services.AddSingleton<IVerseFormatterService, VerseFormatterService>();
services.AddSingleton<IVerseCoordinatorService, VerseCoordinatorService>();
```

---

## 6. Key Notes for Implementation

- **VerseId is the central key** → ensures consistency across languages
- **Randomize once per request** → cache VerseId for language switching
- **All data stays in memory** → `VerseRepository` instance (singleton via DI)
- **DI-injectable** → `IVerseRepository` interface enables mocking in tests
- **Single clamping responsibility** → only `VerseLookupService` clamps verseId to valid range
- **Multi-agent architecture** allows modular, testable services
- **NSubstitute** mocking for unit testing all services independently
- **Cloud caching** (CloudFront / Cloud CDN) reduces backend load

---

## 7. OIDC Architecture Notes (Optional)

- **AWS OIDC + GitHub Actions**:
  - CloudFormation creates IAM Role with OIDC trust
  - GitHub Actions workflow uses `aws-actions/configure-aws-credentials` to assume role
- No AWS keys stored in GitHub
- Temporary credentials expire automatically (~15 mins)

- **GCP Equivalent**: Workload Identity Federation
  - GitHub OIDC → GCP Service Account
  - Use `google-github-actions/auth` in workflow
  - Short-lived OAuth tokens

---

End of design document.

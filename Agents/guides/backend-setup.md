# Backend Setup Guide

## Overview
The backend is a .NET 10 Web API using Minimal APIs pattern, deployed as an AWS Lambda function. It uses a multi-agent service layer where a Coordinator orchestrates Lookup, Language, and Formatter sub-services.

## Initial Setup

### Prerequisites
- .NET SDK 10.0+
- Docker Desktop (for Lambda container builds)
- AWS CLI configured

### Project Structure
```
backend/
├── Program.cs              # Local development entry point
├── LambdaEntryPoint.cs     # AWS Lambda entry point
├── AppConfiguration.cs     # Shared service & endpoint registration
├── Services/
│   ├── IVerseCoordinatorService.cs   # Coordinator interface
│   ├── VerseCoordinatorService.cs    # Orchestrates sub-services
│   ├── IVerseLookupService.cs        # Lookup interface
│   ├── VerseLookupService.cs         # Random/ID-based verse selection
│   ├── IVerseLanguageService.cs      # Language interface
│   ├── VerseLanguageService.cs       # Language-specific text retrieval
│   ├── IVerseFormatterService.cs     # Formatter interface
│   ├── VerseFormatterService.cs      # Response assembly
│   └── ReferenceParser.cs            # Bible reference → metadata parser
├── Data/
│   ├── IVerseRepository.cs  # Repository interface (Count, Metadata, Translations)
│   ├── VerseRepository.cs   # Indexed verse data (VerseId → metadata + translations), DI singleton
│   ├── EnglishVerses.cs     # ~50 English verses
│   ├── AmharicVerses.cs     # ~50 Amharic verses
│   └── FinnishVerses.cs     # ~50 Finnish verses
└── Models/
    └── Verse.cs             # Verse, VerseMetadata, VerseText, VerseResponse records
```

## Implementation Details

### Models
```csharp
public record Verse(string Text, string Reference);
public record VerseMetadata(int VerseId, string Book, int Chapter, string VerseNumber);
public record VerseText(string Text, string Language);
public record VerseResponse(int VerseId, string Book, int Chapter, string VerseNumber, string Text, string Language);
```

### Multi-Agent Service Layer

| Service | Interface | Role |
|---------|-----------|------|
| **VerseCoordinatorService** | `IVerseCoordinatorService` | Orchestrates Lookup → Language → Formatter |
| **VerseLookupService** | `IVerseLookupService` | `GetRandom()` / `GetByVerseId(id)` → `VerseMetadata` |
| **VerseLanguageService** | `IVerseLanguageService` | `GetText(verseId, lang)` → `VerseText` (falls back to English) |
| **VerseFormatterService** | `IVerseFormatterService` | `Format(metadata, text)` → `VerseResponse` |

### Data Layer

- **IVerseRepository** — interface exposing `Count`, `Metadata`, and `Translations` dictionaries
- **VerseRepository** — implements `IVerseRepository`, registered as DI singleton; indexes `EnglishVerses`, `AmharicVerses`, `FinnishVerses` arrays by VerseId (1-based); validates array lengths match at construction
- **ReferenceParser** — regex-based parser: `"1 Peter 5:7"` → `(Book: "1 Peter", Chapter: 5, VerseNumber: "7")`

### API Endpoints

#### GET /api/verse/random
Returns a random or specific verse.

**Query Parameters:**
- `lang` (optional): Language code (en, am, fi). Defaults to English.
- `verseId` (optional): Specific verse ID (1-50). If provided, returns that verse instead of random.

**Response:**
```json
{
  "verseId": 1,
  "book": "Jeremiah",
  "chapter": 29,
  "verseNumber": "11",
  "text": "For God so loved the world...",
  "language": "en"
}
```

#### GET /api/health
Health check endpoint.

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2026-02-06T12:00:00Z"
}
```

## CORS Configuration
- Enabled for frontend access
- Configured in `AppConfiguration.cs`
- Allows all origins in development, restricted in production

## Local Development

### Running Locally
```bash
cd backend
dotnet restore
dotnet run
```

API will be available at `http://localhost:5226`

### Testing
```bash
dotnet test
```

## VerseScraper Tool

A development tool for scraping new verses and adding them to the data files:

```bash
cd tools/VerseScraper
dotnet run -- --url "https://www.biblestudytools.com/topical-verses/inspirational-bible-verses/"
```

The scraper:
1. Scrapes English verses from the given URL
2. Deduplicates against existing verse data
3. Fetches Finnish and Amharic translations via free Bible APIs
4. Appends new verses to `EnglishVerses.cs`, `AmharicVerses.cs`, `FinnishVerses.cs`

## Deployment
See main README.md for deployment instructions using AWS SAM.

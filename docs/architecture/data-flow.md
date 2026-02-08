# Data Flow

## Request Flow: Getting a Verse

```mermaid
sequenceDiagram
    participant User
    participant Browser
    participant CloudFront
    participant API Gateway
    participant Lambda
    participant Coordinator
    participant Lookup
    participant Language
    participant Formatter

    User->>Browser: Scan QR / Visit URL
    Browser->>Browser: Check localStorage
    alt No Blessing Today (useVerse hook)
        Browser->>CloudFront: GET /api/verse/random?lang=en
        CloudFront->>API Gateway: Forward request
        API Gateway->>Lambda: Invoke function
        Lambda->>Coordinator: GetRandomVerse("en")
        Coordinator->>Lookup: GetRandom()
        Lookup-->>Coordinator: VerseMetadata{VerseId, Book, Chapter, VerseNumber}
        Coordinator->>Language: GetText(verseId, "en")
        Language-->>Coordinator: VerseText{Text, Language}
        Coordinator->>Formatter: Format(metadata, text)
        Formatter-->>Coordinator: VerseResponse
        Coordinator-->>Lambda: VerseResponse
        Lambda-->>API Gateway: JSON response
        API Gateway-->>CloudFront: HTTP 200
        CloudFront-->>Browser: Response
        Browser->>Browser: Display verse
        Browser->>Browser: Store in localStorage on "Amen"
    else Already Blessed Today (useDailyBlessing hook)
        Browser->>Browser: Read from localStorage
        Browser->>Browser: Show ReflectionView
    end
```

## Daily Blessing State Flow

```mermaid
graph TD
    Start[User Visits App] --> Check{localStorage<br/>last_blessing_data<br/>exists?}

    Check -->|No| FetchNew[Fetch Random Verse<br/>from API]
    Check -->|Yes| ParseDate[Parse timestamp<br/>from stored data]

    ParseDate --> Compare{Is timestamp<br/>same day as<br/>today?}

    Compare -->|Yes| ShowReflection[Show ReflectionView<br/>with saved verse<br/>+ countdown]
    Compare -->|No| FetchNew

    FetchNew --> DisplayVerse[Display Verse<br/>with Amen Button]
    DisplayVerse --> UserClicks{User Clicks<br/>Amen?}

    UserClicks -->|Yes| SaveState[Save to localStorage:<br/>timestamp: ISO8601<br/>verse: verseId, book, chapter,<br/>verseNumber, text, language]
    UserClicks -->|No| DisplayVerse

    SaveState --> ShowSuccess[Show SuccessView<br/>with confetti]
    ShowSuccess --> Locked[Lock blessing<br/>for today]

    ShowReflection --> WaitMidnight[Wait until<br/>midnight]
    WaitMidnight --> Reset[Reset state<br/>next day]
    Reset --> FetchNew

    style Compare fill:#1a374f,color:#fff
    style SaveState fill:#6f9078,color:#fff
    style ShowReflection fill:#d06450,color:#fff
```

## Language Selection Flow

```mermaid
graph LR
    Start[App Loads] --> CheckLang{localStorage.lang<br/>exists?}

    CheckLang -->|Yes| LoadLang[Load saved language]
    CheckLang -->|No| DefaultLang[Default: English]

    LoadLang --> InitContext[Initialize LanguageContext]
    DefaultLang --> InitContext

    InitContext --> UpdateUI[Update UI translations]
    UpdateUI --> UserChanges{User Changes<br/>Language?}

    UserChanges -->|Yes| SaveLang[Save to localStorage.lang]
    UserChanges -->|No| KeepCurrent[Keep current language]

    SaveLang --> FetchVerse[Fetch verse in<br/>new language<br/>using same verseId]
    FetchVerse --> UpdateUI

    KeepCurrent --> Display[Display current verse]
    FetchVerse --> Display

    style InitContext fill:#1a374f,color:#fff
    style SaveLang fill:#6f9078,color:#fff
```

## PWA Offline Flow

```mermaid
graph TD
    Request[User Request] --> Online{Is Browser<br/>Online?}

    Online -->|Yes| NetworkFirst[Network-First Strategy<br/>for /api/* routes]
    Online -->|No| CacheFirst[Cache-First Strategy<br/>for static assets]

    NetworkFirst --> FetchAPI[Fetch from API]
    FetchAPI --> Success{Success?}

    Success -->|Yes| ReturnFresh[Return fresh data<br/>+ update cache]
    Success -->|No| CheckCache{Check cache}

    CheckCache -->|Hit| ReturnCached[Return cached data]
    CheckCache -->|Miss| ReturnFallback[Return fallback verse]

    CacheFirst --> CheckStaticCache{Static asset<br/>in cache?}
    CheckStaticCache -->|Yes| ReturnStatic[Return from cache]
    CheckStaticCache -->|No| ReturnOffline[Return offline page]

    ReturnFresh --> Display[Display Content]
    ReturnCached --> Display
    ReturnFallback --> Display
    ReturnStatic --> Display
    ReturnOffline --> Display

    style NetworkFirst fill:#1a374f,color:#fff
    style CacheFirst fill:#6f9078,color:#fff
```

## API Response Structure

### Verse Endpoint Response
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

### Health Check Response
```json
{
  "status": "healthy",
  "timestamp": "2026-02-06T12:34:56.789Z"
}
```

## localStorage Schema

### `last_blessing_data`
```typescript
{
  timestamp: "2026-02-06T08:30:00.000Z",  // ISO8601
  verse: {
    verseId: 1,
    book: "Jeremiah",
    chapter: 29,
    verseNumber: "11",
    text: "For I know the plans...",
    language: "en"
  }
}
```

### `lang`
```typescript
"en" | "am" | "fi"  // Language preference
```

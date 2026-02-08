# Component Relationships

## Backend Components

```mermaid
graph TB
    subgraph "Entry Points"
        Program[Program.cs<br/>Kestrel Server]
        LambdaEntry[LambdaEntryPoint.cs<br/>Lambda Handler]
    end

    subgraph "Configuration"
        AppConfig[AppConfiguration.cs<br/>Shared Config]
    end

    subgraph "Service Layer (Multi-Agent)"
        Coordinator[IVerseCoordinatorService<br/>Orchestrator]
        Lookup[IVerseLookupService<br/>Verse Selection]
        Language[IVerseLanguageService<br/>Translation Lookup]
        Formatter[IVerseFormatterService<br/>Response Assembly]
    end

    subgraph "Data Layer"
        IVerseRepo[IVerseRepository<br/>Repository Interface]
        VerseRepo[VerseRepository<br/>DI Singleton]
        RefParser[ReferenceParser<br/>Reference → Metadata]
        EnglishVerses[EnglishVerses<br/>~50 verses]
        AmharicVerses[AmharicVerses<br/>~50 verses]
        FinnishVerses[FinnishVerses<br/>~50 verses]
    end

    subgraph "Models"
        VerseMetadata[VerseMetadata<br/>VerseId, Book, Chapter, VerseNumber]
        VerseText[VerseText<br/>Text, Language]
        VerseResponse[VerseResponse<br/>Full Response DTO]
    end

    subgraph "Endpoints"
        VerseEndpoint[GET /api/verse/random]
        HealthEndpoint[GET /api/health]
    end

    Program --> AppConfig
    LambdaEntry --> AppConfig
    AppConfig --> IVerseRepo
    AppConfig --> Coordinator
    AppConfig --> Lookup
    AppConfig --> Language
    AppConfig --> Formatter
    AppConfig --> VerseEndpoint
    AppConfig --> HealthEndpoint
    VerseEndpoint --> Coordinator
    Coordinator --> Lookup
    Coordinator --> Language
    Coordinator --> Formatter
    Lookup --> IVerseRepo
    Language --> IVerseRepo
    IVerseRepo --> VerseRepo
    VerseRepo --> RefParser
    VerseRepo --> EnglishVerses
    VerseRepo --> AmharicVerses
    VerseRepo --> FinnishVerses
    Lookup -.-> VerseMetadata
    Language -.-> VerseText
    Formatter -.-> VerseResponse

    style Coordinator fill:#1a374f,color:#fff
    style AppConfig fill:#6f9078,color:#fff
    style VerseRepo fill:#d06450,color:#fff
```

## Frontend Components

```mermaid
graph TB
    subgraph "Entry Point"
        Main[main.tsx<br/>App Bootstrap]
        ErrorBoundary[ErrorBoundary<br/>Error Fallback UI]
    end

    subgraph "Routing"
        Router[React Router v7]
        HomeRoute[/ Route]
        AdminRoute[/admin Route]
    end

    subgraph "Context"
        LanguageContext[LanguageContext<br/>Global Language State]
    end

    subgraph "Pages"
        Home[Home.tsx<br/>Main Verse Display]
        Admin[Admin.tsx<br/>QR Generator]
    end

    subgraph "Components"
        VerseDisplay[VerseDisplay.tsx]
        ReflectionView[ReflectionView.tsx]
        SuccessView[SuccessView.tsx]
        Celebration[Celebration.tsx<br/>Confetti]
        InstallPrompt[InstallPrompt.tsx<br/>i18n-enabled]
    end

    subgraph "Hooks"
        UseLanguage[useLanguage<br/>Language Hook]
        UseDailyBlessing[useDailyBlessing<br/>Blessing State & Lock]
        UseVerse[useVerse<br/>Verse Fetch & Cache]
    end

    subgraph "Services"
        ApiService[api.ts<br/>fetchVerse & ApiError]
    end

    subgraph "Types"
        VerseTypes[verse.ts<br/>Verse, BlessingData]
    end

    subgraph "i18n"
        Translations[translations.ts<br/>Multi-language Strings]
    end

    subgraph "PWA"
        ServiceWorker[Service Worker<br/>Workbox]
        Manifest[PWA Manifest]
    end

    Main --> ErrorBoundary
    ErrorBoundary --> LanguageContext
    Main --> ServiceWorker
    LanguageContext --> Router
    Router --> HomeRoute
    Router --> AdminRoute
    HomeRoute --> Home
    AdminRoute --> Admin
    Home --> VerseDisplay
    Home --> ReflectionView
    Home --> SuccessView
    Home --> Celebration
    Home --> InstallPrompt
    Home --> UseLanguage
    Home --> UseDailyBlessing
    Home --> UseVerse
    UseVerse --> ApiService
    UseVerse --> UseLanguage
    UseDailyBlessing --> VerseTypes
    ApiService --> VerseTypes
    UseLanguage --> LanguageContext
    InstallPrompt --> UseLanguage
    LanguageContext --> Translations
    ServiceWorker --> Manifest

    style LanguageContext fill:#1a374f,color:#fff
    style Home fill:#6f9078,color:#fff
    style ServiceWorker fill:#d06450,color:#fff
    style ErrorBoundary fill:#d06450,color:#fff
```

## Data Flow: Daily Blessing Rule

```mermaid
graph LR
    subgraph "User Actions"
        Load[Page Load]
        Amen[Amen Click]
        Refresh[Page Refresh]
    end

    subgraph "State Check"
        CheckStorage{Check localStorage<br/>last_blessing_data}
        CheckDate{Is Same Day?}
    end

    subgraph "States"
        NewBlessing[New Blessing<br/>Show Verse]
        Locked[Locked Today<br/>Show Reflection]
    end

    subgraph "Storage"
        Save[Save to localStorage<br/>timestamp + verse]
        Read[Read from localStorage]
    end

    Load --> CheckStorage
    CheckStorage -->|No Data| NewBlessing
    CheckStorage -->|Has Data| CheckDate
    CheckDate -->|Same Day| Locked
    CheckDate -->|Different Day| NewBlessing
    Amen --> Save
    Save --> Locked
    Refresh --> CheckStorage
    Locked --> Read

    style CheckDate fill:#1a374f,color:#fff
    style Save fill:#6f9078,color:#fff
```

## Component Interaction: Verse Fetching

```mermaid
sequenceDiagram
    participant User
    participant Home
    participant API
    participant Coordinator
    participant Lookup
    participant Language
    participant Formatter
    participant Storage

    User->>Home: Load Page
    Home->>Storage: Check last_blessing_data
    alt Already Blessed Today
        Storage-->>Home: Return saved verse
        Home->>User: Show ReflectionView
    else New Blessing
        Home->>API: GET /api/verse/random?lang=en
        API->>Coordinator: GetRandomVerse("en")
        Coordinator->>Lookup: GetRandom()
        Lookup-->>Coordinator: VerseMetadata
        Coordinator->>Language: GetText(verseId, "en")
        Language-->>Coordinator: VerseText
        Coordinator->>Formatter: Format(metadata, text)
        Formatter-->>Coordinator: VerseResponse
        Coordinator-->>API: VerseResponse
        API-->>Home: JSON response
        Home->>User: Display verse
        User->>Home: Click "Amen"
        Home->>Storage: Save timestamp + verse
        Home->>User: Show SuccessView
    end
```

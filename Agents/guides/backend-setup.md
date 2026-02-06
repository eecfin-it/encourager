# Backend Setup Guide

## Overview
The backend is a .NET 10 Web API using Minimal APIs pattern, deployed as an AWS Lambda function.

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
│   └── VerseService.cs    # Verse retrieval logic
├── Data/                  # Verse data (English, Amharic, Finnish)
└── Models/
    └── Verse.cs           # Verse model
```

## Implementation Details

### Verse Model
```csharp
public class Verse
{
    public string Text { get; set; }
    public string Reference { get; set; }
    public int Index { get; set; }
}
```

### Verse Service
- Contains hardcoded list of encouraging verses (50+ per language)
- Supports multiple languages: English (en), Amharic (am), Finnish (fi)
- Provides random verse selection
- Falls back to English for unknown languages

### API Endpoints

#### GET /api/verse/random
Returns a random verse.

**Query Parameters:**
- `lang` (optional): Language code (en, am, fi). Defaults to English.
- `index` (optional): Specific verse index. If provided, returns that verse instead of random.

**Response:**
```json
{
  "text": "For God so loved the world...",
  "reference": "John 3:16",
  "index": 0
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

## Deployment
See main README.md for deployment instructions using AWS SAM.

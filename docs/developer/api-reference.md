# API Reference

## Base URL

**Local Development:** `http://localhost:5226`
**Production:** `https://<api-gateway-url>/Prod`

## Endpoints

### Get Random Verse

Returns a random Bible verse in the specified language.

**Endpoint:** `GET /api/verse/random`

**Query Parameters:**

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `lang` | string | No | `en` | Language code: `en`, `am`, or `fi` |
| `verseId` | integer | No | random | Specific verse ID (1-50) |

**Request Examples:**

```bash
# Random verse in English
GET /api/verse/random?lang=en

# Random verse in Amharic
GET /api/verse/random?lang=am

# Specific verse by ID
GET /api/verse/random?lang=en&verseId=24

# Random verse (defaults to English)
GET /api/verse/random
```

**Response:**

```json
{
  "verseId": 1,
  "book": "Jeremiah",
  "chapter": 29,
  "verseNumber": "11",
  "text": "For I know the plans I have for you, declares the Lord, plans to prosper you and not to harm you, plans to give you hope and a future.",
  "language": "en"
}
```

**Response Fields:**

| Field | Type | Description |
|-------|------|-------------|
| `verseId` | integer | Verse ID (1-50) for consistent retrieval across languages |
| `book` | string | Bible book name (e.g., "Jeremiah", "1 Peter") |
| `chapter` | integer | Chapter number |
| `verseNumber` | string | Verse number or range (e.g., "11", "24-25") |
| `text` | string | The verse text in the requested language |
| `language` | string | Language code of the returned text (en, am, or fi) |

**Status Codes:**

- `200 OK`: Success
- `400 Bad Request`: Invalid language or verseId parameter
- `500 Internal Server Error`: Server error

**Example Usage:**

```typescript
// Fetch random verse
const response = await fetch('/api/verse/random?lang=en');
const verse = await response.json();
console.log(verse.text);        // Verse text
console.log(verse.book);        // Bible book name
console.log(verse.chapter);     // Chapter number
console.log(verse.verseNumber); // Verse number
console.log(verse.verseId);     // Verse ID
console.log(verse.language);    // Language code

// Fetch specific verse by ID
const specificVerse = await fetch('/api/verse/random?lang=en&verseId=24');
const data = await specificVerse.json();
```

### Health Check

Returns API health status and current timestamp.

**Endpoint:** `GET /api/health`

**Request:**

```bash
GET /api/health
```

**Response:**

```json
{
  "status": "healthy",
  "timestamp": "2026-02-06T12:34:56.789Z"
}
```

**Response Fields:**

| Field | Type | Description |
|-------|------|-------------|
| `status` | string | Always returns `"healthy"` |
| `timestamp` | string | Current UTC timestamp in ISO8601 format |

**Status Codes:**

- `200 OK`: Service is healthy

**Example Usage:**

```typescript
const response = await fetch('/api/health');
const health = await response.json();
console.log(health.status);    // "healthy"
console.log(health.timestamp); // ISO8601 timestamp
```

## Language Codes

| Code | Language | Script |
|------|----------|--------|
| `en` | English | Latin |
| `am` | Amharic | Ge'ez (አማርኛ) |
| `fi` | Finnish | Latin |

## Error Responses

### Unknown Language

Unknown languages fall back to English silently (returns `"language": "en"`).

### Invalid VerseId

Out-of-range verse IDs are clamped to valid range (1 to max). Negative values clamp to 1, overflow values clamp to the maximum verse ID.

### Server Error

**Response:** `500 Internal Server Error`

```json
{
  "error": "Internal server error"
}
```

## CORS Configuration

The API supports Cross-Origin Resource Sharing (CORS) for browser requests.

**Configuration:**
- Controlled by `ALLOWED_ORIGIN` environment variable
- Default: `*` (allows all origins)
- Production: Should be set to CloudFront URL

**Headers:**
- `Access-Control-Allow-Origin`: Configured origin
- `Access-Control-Allow-Methods`: `GET, OPTIONS`
- `Access-Control-Allow-Headers`: `Content-Type, Authorization`

## Rate Limiting

Currently, no rate limiting is implemented. Consider implementing rate limiting for production use.

## Caching

**CloudFront Caching:**
- API routes (`/api/*`) use network-first strategy
- Default TTL: 5 minutes (300 seconds)
- Max TTL: 1 hour (3600 seconds)
- Query strings are forwarded to origin

**Client-Side Caching:**
- Service worker caches API responses
- Network-first strategy with 5-minute cache
- Falls back to cache if network fails

## Versioning

Currently, no API versioning is implemented. All endpoints are under `/api/`.

Future versions may use:
- `/api/v1/verse/random`
- `/api/v2/verse/random`

## Testing Endpoints

### Using curl

```bash
# Random verse
curl http://localhost:5226/api/verse/random?lang=en

# Specific verse
curl http://localhost:5226/api/verse/random?lang=en&verseId=24

# Health check
curl http://localhost:5226/api/health
```

### Using Postman/Insomnia

1. Create new GET request
2. Set URL: `http://localhost:5226/api/verse/random`
3. Add query parameters:
   - `lang`: `en`
   - `verseId`: `24` (optional)
4. Send request

### Using Browser

```javascript
// In browser console
fetch('/api/verse/random?lang=en')
  .then(res => res.json())
  .then(data => console.log(data));
```

## Implementation Details

### Backend Implementation

**Service Layer:** Multi-agent architecture with four services:

| Service | Interface | Role |
|---------|-----------|------|
| **VerseCoordinatorService** | `IVerseCoordinatorService` | Orchestrates sub-services for verse requests |
| **VerseLookupService** | `IVerseLookupService` | Selects random or specific verse by ID |
| **VerseLanguageService** | `IVerseLanguageService` | Returns verse text in requested language |
| **VerseFormatterService** | `IVerseFormatterService` | Assembles final response from metadata + text |

**Data Layer:**
- `VerseRepository` — static class that indexes existing verse data by VerseId
- `ReferenceParser` — parses "Book Chapter:Verse" references into structured metadata
- `EnglishVerses`, `AmharicVerses`, `FinnishVerses` — static verse arrays (~50 verses each)

**Request Flow:**
1. Endpoint receives request → calls `IVerseCoordinatorService`
2. Coordinator → `IVerseLookupService.GetRandom()` or `.GetByVerseId(id)` → returns `VerseMetadata`
3. Coordinator → `IVerseLanguageService.GetText(verseId, lang)` → returns `VerseText`
4. Coordinator → `IVerseFormatterService.Format(metadata, text)` → returns `VerseResponse`

### Frontend Integration

**Fetch Pattern:**
```typescript
const fetchVerse = async (lang: string, verseId?: number) => {
  const url = verseId !== undefined
    ? `/api/verse/random?lang=${lang}&verseId=${verseId}`
    : `/api/verse/random?lang=${lang}`;

  const response = await fetch(url);
  if (!response.ok) {
    throw new Error('Failed to fetch verse');
  }
  return response.json();
};
```

**Error Handling:**
```typescript
try {
  const verse = await fetchVerse('en');
  setVerse(verse);
} catch (error) {
  // Fallback to default verse
  setVerse({
    verseId: 0,
    book: '',
    chapter: 0,
    verseNumber: '0',
    text: 'Fallback verse text',
    language: 'en'
  });
}
```

## Best Practices

1. **Always Handle Errors**: API calls can fail, implement error handling
2. **Use VerseId for Consistency**: When switching languages, use verseId to get same verse
3. **Cache Responses**: Use service worker caching for offline support
4. **Respect Daily Limit**: Don't fetch new verses if user already blessed today
5. **Language Persistence**: Save language preference in localStorage

## Future Enhancements

Potential API improvements:

- Pagination for verse lists
- Verse search/filtering
- Verse categories/themes
- User favorites
- Verse sharing endpoints
- Analytics endpoints

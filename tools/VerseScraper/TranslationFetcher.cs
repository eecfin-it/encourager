using System.Net.Http.Json;
using System.Text.Json;

namespace VerseScraper;

public class TranslationFetcher : IDisposable
{
    private readonly HttpClient _httpClient = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<string?> FetchFinnishAsync(string reference)
    {
        try
        {
            var apiRef = NormalizeReferenceForBibleApi(reference);
            var url = $"https://bible-api.com/{Uri.EscapeDataString(apiRef)}?translation=finnish";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadFromJsonAsync<BibleApiResponse>(JsonOptions);
            return json?.Text?.Trim();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Finnish translation failed for '{reference}': {ex.Message}");
            return null;
        }
    }

    public async Task<string?> FetchAmharicAsync(string reference)
    {
        try
        {
            var (book, chapter, verse) = ParseReference(reference);
            var url = $"https://bible-api-kappa.vercel.app/api/{Uri.EscapeDataString(book)}/{chapter}/{verse}?lang=amh";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
            if (json.TryGetProperty("text", out var textProp))
                return textProp.GetString()?.Trim();
            if (json.TryGetProperty("verse", out var verseProp))
                return verseProp.GetString()?.Trim();

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Amharic translation failed for '{reference}': {ex.Message}");
            return null;
        }
    }

    private static string NormalizeReferenceForBibleApi(string reference)
    {
        // bible-api.com expects format like "John 3:16" or "Psalm 23:1"
        return reference
            .Replace("–", "-")
            .Replace("—", "-")
            .Trim();
    }

    private static (string Book, int Chapter, string Verse) ParseReference(string reference)
    {
        var match = System.Text.RegularExpressions.Regex.Match(reference, @"^(.+?)\s+(\d+):(.+)$");
        if (!match.Success)
            return (reference, 1, "1");

        return (match.Groups[1].Value, int.Parse(match.Groups[2].Value), match.Groups[3].Value);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }

    private record BibleApiResponse(string? Text, string? Reference);
}

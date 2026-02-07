using AngleSharp;
using AngleSharp.Dom;

namespace VerseScraper;

public static class EnglishVerseScraper
{
    public static async Task<List<(string Text, string Reference)>> ScrapeAsync(string url)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(url);

        var verses = new List<(string Text, string Reference)>();

        // biblestudytools.com typically uses <p> or <div> elements containing verse text and references
        // Try multiple selectors to handle different page layouts
        var verseElements = document.QuerySelectorAll("div.verse-box, div.single-verse, blockquote");

        if (verseElements.Length > 0)
        {
            foreach (var element in verseElements)
            {
                var text = ExtractVerseText(element);
                var reference = ExtractReference(element);
                if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(reference))
                    verses.Add((text.Trim(), reference.Trim()));
            }
        }

        // Fallback: look for common patterns in list items or paragraphs
        if (verses.Count == 0)
        {
            var paragraphs = document.QuerySelectorAll("p, li");
            foreach (var p in paragraphs)
            {
                var parsed = TryParseVerseFromText(p.TextContent);
                if (parsed.HasValue)
                    verses.Add(parsed.Value);
            }
        }

        Console.WriteLine($"Scraped {verses.Count} verses from {url}");
        return verses;
    }

    private static string ExtractVerseText(IElement element)
    {
        var textEl = element.QuerySelector(".verse-text, .text, p");
        return textEl?.TextContent?.Trim() ?? element.TextContent?.Trim() ?? "";
    }

    private static string ExtractReference(IElement element)
    {
        var refEl = element.QuerySelector(".verse-reference, .reference, cite, strong, a[href*='bible']");
        return refEl?.TextContent?.Trim() ?? "";
    }

    private static (string Text, string Reference)? TryParseVerseFromText(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return null;

        // Match patterns like: "Verse text" - Reference or "Verse text" (Reference)
        // Also: Reference - "Verse text"
        var dashPattern = System.Text.RegularExpressions.Regex.Match(
            content.Trim(),
            "^[\"\u201C\u201D](.+?)[\"\u201C\u201D][\\s]*[-\u2013\u2014][\\s]*(.+\\d+:\\d+.*)$");

        if (dashPattern.Success)
            return (dashPattern.Groups[1].Value.Trim(), dashPattern.Groups[2].Value.Trim());

        // Try: Reference - Verse text
        var refFirstPattern = System.Text.RegularExpressions.Regex.Match(
            content.Trim(),
            "^(.+\\d+:\\d+[^\"\u201C\u201D]*?)[\\s]*[-\u2013\u2014][\\s]*[\"\u201C\u201D]?(.+?)[\"\u201C\u201D]?$");

        if (refFirstPattern.Success)
            return (refFirstPattern.Groups[2].Value.Trim(), refFirstPattern.Groups[1].Value.Trim());

        return null;
    }
}

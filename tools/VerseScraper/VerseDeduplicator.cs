using Encourager.Api.Data;

namespace VerseScraper;

public static class VerseDeduplicator
{
    public static List<(string Text, string Reference)> RemoveDuplicates(
        List<(string Text, string Reference)> scraped)
    {
        var existingRefs = new HashSet<string>(
            EnglishVerses.Verses.Select(v => NormalizeReference(v.Reference)),
            StringComparer.OrdinalIgnoreCase);

        var unique = new List<(string Text, string Reference)>();
        var seenRefs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (text, reference) in scraped)
        {
            var normalized = NormalizeReference(reference);
            if (existingRefs.Contains(normalized))
            {
                Console.WriteLine($"  Skipping duplicate: {reference}");
                continue;
            }
            if (!seenRefs.Add(normalized))
            {
                Console.WriteLine($"  Skipping duplicate within batch: {reference}");
                continue;
            }
            unique.Add((text, reference));
        }

        Console.WriteLine($"Found {unique.Count} new verses (skipped {scraped.Count - unique.Count} duplicates)");
        return unique;
    }

    private static string NormalizeReference(string reference)
    {
        return reference
            .Replace("–", "-")
            .Replace("—", "-")
            .Trim()
            .ToLowerInvariant();
    }
}

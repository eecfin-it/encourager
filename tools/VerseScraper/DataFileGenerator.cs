using System.Text;

namespace VerseScraper;

public static class DataFileGenerator
{
    public static void UpdateDataFiles(
        List<(string English, string Finnish, string Amharic, string Reference)> newVerses,
        string backendDataDir)
    {
        if (newVerses.Count == 0)
        {
            Console.WriteLine("No new verses to add.");
            return;
        }

        UpdateFile(
            Path.Combine(backendDataDir, "EnglishVerses.cs"),
            "EnglishVerses",
            newVerses.Select(v => (v.English, v.Reference)).ToList());

        UpdateFile(
            Path.Combine(backendDataDir, "FinnishVerses.cs"),
            "FinnishVerses",
            newVerses.Select(v => (v.Finnish, v.Reference)).ToList());

        UpdateFile(
            Path.Combine(backendDataDir, "AmharicVerses.cs"),
            "AmharicVerses",
            newVerses.Select(v => (v.Amharic, v.Reference)).ToList());

        Console.WriteLine($"Successfully added {newVerses.Count} verses to all three data files.");
    }

    private static void UpdateFile(
        string filePath,
        string className,
        List<(string Text, string Reference)> newVerses)
    {
        var content = File.ReadAllText(filePath);

        // Find the closing "];" of the array and insert new entries before it
        var closingIndex = content.LastIndexOf("];", StringComparison.Ordinal);
        if (closingIndex < 0)
        {
            Console.WriteLine($"  ERROR: Could not find array closing in {filePath}");
            return;
        }

        var sb = new StringBuilder();
        foreach (var (text, reference) in newVerses)
        {
            var escapedText = text.Replace("\"", "\\\"");
            var escapedRef = reference.Replace("\"", "\\\"");
            sb.AppendLine($"        new(\"{escapedText}\", \"{escapedRef}\"),");
        }

        var updated = content.Insert(closingIndex, sb.ToString());
        File.WriteAllText(filePath, updated);
        Console.WriteLine($"  Updated {filePath} (+{newVerses.Count} verses)");
    }
}

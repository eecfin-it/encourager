using VerseScraper;

if (args.Length < 2 || args[0] != "--url")
{
    Console.WriteLine("Usage: dotnet run -- --url <biblestudytools-url>");
    Console.WriteLine("Example: dotnet run -- --url \"https://www.biblestudytools.com/topical-verses/inspirational-bible-verses/\"");
    return 1;
}

var url = args[1];
var backendDataDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "backend", "Data"));

if (!Directory.Exists(backendDataDir))
{
    // Try relative from working directory
    backendDataDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "backend", "Data"));
}

if (!Directory.Exists(backendDataDir))
{
    Console.WriteLine($"ERROR: Could not find backend/Data directory. Tried: {backendDataDir}");
    return 1;
}

Console.WriteLine($"Backend data directory: {backendDataDir}");
Console.WriteLine();

// Step 1: Scrape English verses
Console.WriteLine("=== Step 1: Scraping English verses ===");
var scraped = await EnglishVerseScraper.ScrapeAsync(url);
if (scraped.Count == 0)
{
    Console.WriteLine("No verses found. Check the URL and page structure.");
    return 1;
}

// Step 2: Deduplicate
Console.WriteLine();
Console.WriteLine("=== Step 2: Deduplicating ===");
var unique = VerseDeduplicator.RemoveDuplicates(scraped);
if (unique.Count == 0)
{
    Console.WriteLine("All scraped verses already exist in the data files.");
    return 0;
}

// Step 3: Fetch translations
Console.WriteLine();
Console.WriteLine("=== Step 3: Fetching translations ===");
using var fetcher = new TranslationFetcher();
var completeVerses = new List<(string English, string Finnish, string Amharic, string Reference)>();

foreach (var (text, reference) in unique)
{
    Console.WriteLine($"  Translating: {reference}");

    var finnish = await fetcher.FetchFinnishAsync(reference);
    if (finnish == null)
    {
        Console.WriteLine($"    Skipping (no Finnish translation)");
        continue;
    }

    var amharic = await fetcher.FetchAmharicAsync(reference);
    if (amharic == null)
    {
        Console.WriteLine($"    Skipping (no Amharic translation)");
        continue;
    }

    completeVerses.Add((text, finnish, amharic, reference));
    Console.WriteLine($"    OK (en/fi/am)");

    // Rate limiting
    await Task.Delay(500);
}

Console.WriteLine($"Got complete translations for {completeVerses.Count}/{unique.Count} verses");

// Step 4: Update data files
Console.WriteLine();
Console.WriteLine("=== Step 4: Updating data files ===");
DataFileGenerator.UpdateDataFiles(completeVerses, backendDataDir);

Console.WriteLine();
Console.WriteLine("Done!");
return 0;

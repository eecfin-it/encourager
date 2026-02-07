namespace Encourager.Api.Models;

public record Verse(string Text, string Reference);

public record VerseMetadata(int VerseId, string Book, int Chapter, string VerseNumber);

public record VerseText(string Text, string Language);

public record VerseResponse(
    int VerseId,
    string Book,
    int Chapter,
    string VerseNumber,
    string Text,
    string Language,
    IReadOnlyDictionary<string, string> Translations);

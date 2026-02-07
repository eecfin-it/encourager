using Encourager.Api.Models;
using Encourager.Api.Services;

namespace Encourager.Api.Data;

public static class VerseRepository
{
    public static int Count { get; }
    public static IReadOnlyDictionary<int, VerseMetadata> Metadata { get; }
    public static IReadOnlyDictionary<int, IReadOnlyDictionary<string, string>> Translations { get; }

    static VerseRepository()
    {
        Count = EnglishVerses.Verses.Length;

        var metadata = new Dictionary<int, VerseMetadata>(Count);
        var translations = new Dictionary<int, IReadOnlyDictionary<string, string>>(Count);

        for (var i = 0; i < Count; i++)
        {
            var verseId = i + 1;
            var englishVerse = EnglishVerses.Verses[i];
            var (book, chapter, verseNumber) = ReferenceParser.Parse(englishVerse.Reference);

            metadata[verseId] = new VerseMetadata(verseId, book, chapter, verseNumber);

            translations[verseId] = new Dictionary<string, string>
            {
                ["en"] = EnglishVerses.Verses[i].Text,
                ["am"] = AmharicVerses.Verses[i].Text,
                ["fi"] = FinnishVerses.Verses[i].Text
            };
        }

        Metadata = metadata;
        Translations = translations;
    }
}

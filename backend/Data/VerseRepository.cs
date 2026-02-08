using Encourager.Api.Models;
using Encourager.Api.Services;

namespace Encourager.Api.Data;

public class VerseRepository : IVerseRepository
{
    public int Count { get; }
    public IReadOnlyDictionary<int, VerseMetadata> Metadata { get; }
    public IReadOnlyDictionary<int, IReadOnlyDictionary<string, string>> Translations { get; }

    public VerseRepository()
    {
        if (EnglishVerses.Verses.Length != AmharicVerses.Verses.Length ||
            EnglishVerses.Verses.Length != FinnishVerses.Verses.Length)
        {
            throw new InvalidOperationException(
                $"Verse array lengths must match: English={EnglishVerses.Verses.Length}, " +
                $"Amharic={AmharicVerses.Verses.Length}, Finnish={FinnishVerses.Verses.Length}");
        }

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

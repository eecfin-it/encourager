using Encourager.Api.Models;

namespace Encourager.Api.Services;

public class VerseFormatterService : IVerseFormatterService
{
    public VerseResponse Format(VerseMetadata metadata, VerseText verseText, IReadOnlyDictionary<string, string> translations)
    {
        return new VerseResponse(
            metadata.VerseId,
            metadata.Book,
            metadata.Chapter,
            metadata.VerseNumber,
            verseText.Text,
            verseText.Language,
            translations
        );
    }
}

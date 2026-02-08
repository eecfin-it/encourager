using Encourager.Api.Models;
using Microsoft.Extensions.Logging;

namespace Encourager.Api.Services;

public class VerseFormatterService(ILogger<VerseFormatterService> logger) : IVerseFormatterService
{
    public VerseResponse Format(VerseMetadata metadata, VerseText verseText, IReadOnlyDictionary<string, string> translations)
    {
        logger.LogDebug("Formatting verse {VerseId} ({Book} {Chapter}:{VerseNumber}) in language {Language}",
            metadata.VerseId, metadata.Book, metadata.Chapter, metadata.VerseNumber, verseText.Language);
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

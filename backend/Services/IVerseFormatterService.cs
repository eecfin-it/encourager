using Encourager.Api.Models;

namespace Encourager.Api.Services;

public interface IVerseFormatterService
{
    VerseResponse Format(VerseMetadata metadata, VerseText verseText, IReadOnlyDictionary<string, string> translations);
}

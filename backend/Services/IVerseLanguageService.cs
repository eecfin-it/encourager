using Encourager.Api.Models;

namespace Encourager.Api.Services;

public interface IVerseLanguageService
{
    VerseText GetText(int verseId, string language);
    IReadOnlyDictionary<string, string> GetAllTexts(int verseId);
}

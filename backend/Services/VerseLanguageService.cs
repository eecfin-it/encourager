using Encourager.Api.Data;
using Encourager.Api.Models;

namespace Encourager.Api.Services;

public class VerseLanguageService : IVerseLanguageService
{
    private static readonly HashSet<string> SupportedLanguages = ["en", "am", "fi"];

    public VerseText GetText(int verseId, string language)
    {
        var safeId = Math.Clamp(verseId, 1, VerseRepository.Count);
        var lang = SupportedLanguages.Contains(language) ? language : "en";
        var text = VerseRepository.Translations[safeId][lang];
        return new VerseText(text, lang);
    }

    public IReadOnlyDictionary<string, string> GetAllTexts(int verseId)
    {
        var safeId = Math.Clamp(verseId, 1, VerseRepository.Count);
        return VerseRepository.Translations[safeId];
    }
}

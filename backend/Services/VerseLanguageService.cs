using Encourager.Api.Data;
using Encourager.Api.Models;

namespace Encourager.Api.Services;

public class VerseLanguageService(IVerseRepository repository) : IVerseLanguageService
{
    private static readonly HashSet<string> SupportedLanguages = ["en", "am", "fi"];

    public VerseText GetText(int verseId, string language)
    {
        var lang = SupportedLanguages.Contains(language) ? language : "en";
        var text = repository.Translations[verseId][lang];
        return new VerseText(text, lang);
    }

    public IReadOnlyDictionary<string, string> GetAllTexts(int verseId)
    {
        return repository.Translations[verseId];
    }
}

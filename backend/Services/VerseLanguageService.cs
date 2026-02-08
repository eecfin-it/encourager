using Encourager.Api.Data;
using Encourager.Api.Models;
using Microsoft.Extensions.Logging;

namespace Encourager.Api.Services;

public class VerseLanguageService(IVerseRepository repository, ILogger<VerseLanguageService> logger) : IVerseLanguageService
{
    private static readonly HashSet<string> SupportedLanguages = ["en", "am", "fi"];

    public VerseText GetText(int verseId, string language)
    {
        var lang = SupportedLanguages.Contains(language) ? language : "en";
        if (lang != language)
        {
            logger.LogWarning("Unsupported language '{RequestedLanguage}' requested for verse {VerseId}, falling back to English", language, verseId);
        }
        logger.LogDebug("Retrieving text for verse {VerseId} in language {Language}", verseId, lang);
        var text = repository.Translations[verseId][lang];
        return new VerseText(text, lang);
    }

    public IReadOnlyDictionary<string, string> GetAllTexts(int verseId)
    {
        logger.LogDebug("Retrieving all translations for verse {VerseId}", verseId);
        return repository.Translations[verseId];
    }
}

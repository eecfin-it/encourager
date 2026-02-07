using Encourager.Api.Models;

namespace Encourager.Api.Services;

public class VerseCoordinatorService(
    IVerseLookupService lookupService,
    IVerseLanguageService languageService,
    IVerseFormatterService formatterService) : IVerseCoordinatorService
{
    public VerseResponse GetRandomVerse(string language)
    {
        var metadata = lookupService.GetRandom();
        var verseText = languageService.GetText(metadata.VerseId, language);
        var translations = languageService.GetAllTexts(metadata.VerseId);
        return formatterService.Format(metadata, verseText, translations);
    }

    public VerseResponse GetVerseByVerseId(int verseId, string language)
    {
        var metadata = lookupService.GetByVerseId(verseId);
        var verseText = languageService.GetText(metadata.VerseId, language);
        var translations = languageService.GetAllTexts(metadata.VerseId);
        return formatterService.Format(metadata, verseText, translations);
    }
}

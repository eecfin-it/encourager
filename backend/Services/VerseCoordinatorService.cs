using Encourager.Api.Models;
using Microsoft.Extensions.Logging;

namespace Encourager.Api.Services;

public class VerseCoordinatorService(
    IVerseLookupService lookupService,
    IVerseLanguageService languageService,
    IVerseFormatterService formatterService,
    ILogger<VerseCoordinatorService> logger) : IVerseCoordinatorService
{
    public VerseResponse GetRandomVerse(string language)
    {
        logger.LogInformation("Getting random verse for language: {Language}", language);
        var metadata = lookupService.GetRandom();
        var verseText = languageService.GetText(metadata.VerseId, language);
        var translations = languageService.GetAllTexts(metadata.VerseId);
        var response = formatterService.Format(metadata, verseText, translations);
        logger.LogInformation("Retrieved random verse {VerseId} ({Book} {Chapter}:{VerseNumber}) in language {Language}",
            response.VerseId, response.Book, response.Chapter, response.VerseNumber, response.Language);
        return response;
    }

    public VerseResponse GetVerseByVerseId(int verseId, string language)
    {
        logger.LogInformation("Getting verse by ID: {VerseId} for language: {Language}", verseId, language);
        var metadata = lookupService.GetByVerseId(verseId);
        var verseText = languageService.GetText(metadata.VerseId, language);
        var translations = languageService.GetAllTexts(metadata.VerseId);
        var response = formatterService.Format(metadata, verseText, translations);
        logger.LogInformation("Retrieved verse {VerseId} ({Book} {Chapter}:{VerseNumber}) in language {Language}",
            response.VerseId, response.Book, response.Chapter, response.VerseNumber, response.Language);
        return response;
    }
}

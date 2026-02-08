using Encourager.Api.Models;
using Encourager.Api.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Encourager.Api.Tests;

public class VerseCoordinatorServiceTests
{
    private readonly IVerseLookupService _lookupService = Substitute.For<IVerseLookupService>();
    private readonly IVerseLanguageService _languageService = Substitute.For<IVerseLanguageService>();
    private readonly IVerseFormatterService _formatterService = Substitute.For<IVerseFormatterService>();
    private readonly VerseCoordinatorService _sut;

    private static readonly IReadOnlyDictionary<string, string> TestTranslations =
        new Dictionary<string, string> { ["en"] = "Text", ["fi"] = "Teksti", ["am"] = "ጽሑፍ" };

    public VerseCoordinatorServiceTests()
    {
        _sut = new VerseCoordinatorService(_lookupService, _languageService, _formatterService, Substitute.For<ILogger<VerseCoordinatorService>>());
    }

    [Fact]
    public void GetRandomVerse_ShouldOrchestrateLookupLanguageAndFormat_WhenCalled()
    {
        var metadata = new VerseMetadata(1, "Psalm", 23, "1");
        var verseText = new VerseText("The Lord is my shepherd.", "en");
        var expected = new VerseResponse(1, "Psalm", 23, "1", "The Lord is my shepherd.", "en", TestTranslations);
        _lookupService.GetRandom().Returns(metadata);
        _languageService.GetText(1, "en").Returns(verseText);
        _languageService.GetAllTexts(1).Returns(TestTranslations);
        _formatterService.Format(metadata, verseText, TestTranslations).Returns(expected);

        var actual = _sut.GetRandomVerse("en");

        Assert.Equal(expected, actual);
        _lookupService.Received(1).GetRandom();
        _languageService.Received(1).GetText(1, "en");
        _languageService.Received(1).GetAllTexts(1);
        _formatterService.Received(1).Format(metadata, verseText, TestTranslations);
    }

    [Fact]
    public void GetRandomVerse_ShouldPassLanguageToLanguageService_WhenCalledWithFinnish()
    {
        var metadata = new VerseMetadata(5, "Proverbs", 3, "5");
        var verseText = new VerseText("Turvaa Herraan.", "fi");
        var expected = new VerseResponse(5, "Proverbs", 3, "5", "Turvaa Herraan.", "fi", TestTranslations);
        _lookupService.GetRandom().Returns(metadata);
        _languageService.GetText(5, "fi").Returns(verseText);
        _languageService.GetAllTexts(5).Returns(TestTranslations);
        _formatterService.Format(metadata, verseText, TestTranslations).Returns(expected);

        var actual = _sut.GetRandomVerse("fi");

        Assert.Equal("fi", actual.Language);
        _languageService.Received(1).GetText(5, "fi");
    }

    [Fact]
    public void GetVerseByVerseId_ShouldOrchestrateLookupLanguageAndFormat_WhenCalled()
    {
        var metadata = new VerseMetadata(10, "Psalm", 34, "18");
        var verseText = new VerseText("The Lord is close.", "en");
        var expected = new VerseResponse(10, "Psalm", 34, "18", "The Lord is close.", "en", TestTranslations);
        _lookupService.GetByVerseId(10).Returns(metadata);
        _languageService.GetText(10, "en").Returns(verseText);
        _languageService.GetAllTexts(10).Returns(TestTranslations);
        _formatterService.Format(metadata, verseText, TestTranslations).Returns(expected);

        var actual = _sut.GetVerseByVerseId(10, "en");

        Assert.Equal(expected, actual);
        _lookupService.Received(1).GetByVerseId(10);
    }

    [Fact]
    public void GetVerseByVerseId_ShouldReturnFormattedOutput_WhenCalled()
    {
        var metadata = new VerseMetadata(1, "Jeremiah", 29, "11");
        var verseText = new VerseText("For I know the plans.", "en");
        var expected = new VerseResponse(1, "Jeremiah", 29, "11", "For I know the plans.", "en", TestTranslations);
        _lookupService.GetByVerseId(1).Returns(metadata);
        _languageService.GetText(1, "en").Returns(verseText);
        _languageService.GetAllTexts(1).Returns(TestTranslations);
        _formatterService.Format(metadata, verseText, TestTranslations).Returns(expected);

        var actual = _sut.GetVerseByVerseId(1, "en");

        Assert.Equal(1, actual.VerseId);
        Assert.Equal("Jeremiah", actual.Book);
        Assert.Equal(29, actual.Chapter);
        Assert.Equal("11", actual.VerseNumber);
    }
}

using Encourager.Api.Data;
using Encourager.Api.Services;
using NSubstitute;
using Xunit;

namespace Encourager.Api.Tests;

public class VerseLanguageServiceTests
{
    private readonly IVerseRepository _repository = Substitute.For<IVerseRepository>();
    private readonly VerseLanguageService _sut;

    public VerseLanguageServiceTests()
    {
        var translations = new Dictionary<int, IReadOnlyDictionary<string, string>>
        {
            [1] = new Dictionary<string, string>
            {
                ["en"] = "In the beginning",
                ["am"] = "በመጀመሪያ",
                ["fi"] = "Alussa"
            },
            [2] = new Dictionary<string, string>
            {
                ["en"] = "The Lord is my shepherd",
                ["am"] = "እግዚአብሔር እረኛዬ ነው",
                ["fi"] = "Herra on minun paimeneni"
            }
        };
        _repository.Count.Returns(2);
        _repository.Translations.Returns(translations);
        _sut = new VerseLanguageService(_repository);
    }

    [Fact]
    public void GetText_ShouldReturnEnglishText_WhenLanguageIsEn()
    {
        var actual = _sut.GetText(1, "en");

        Assert.Equal("en", actual.Language);
        Assert.Equal("In the beginning", actual.Text);
    }

    [Fact]
    public void GetText_ShouldReturnAmharicText_WhenLanguageIsAm()
    {
        var actual = _sut.GetText(1, "am");

        Assert.Equal("am", actual.Language);
        Assert.Equal("በመጀመሪያ", actual.Text);
    }

    [Fact]
    public void GetText_ShouldReturnFinnishText_WhenLanguageIsFi()
    {
        var actual = _sut.GetText(1, "fi");

        Assert.Equal("fi", actual.Language);
        Assert.Equal("Alussa", actual.Text);
    }

    [Fact]
    public void GetText_ShouldFallBackToEnglish_WhenLanguageIsUnknown()
    {
        var actual = _sut.GetText(1, "xx");

        Assert.Equal("en", actual.Language);
        Assert.Equal("In the beginning", actual.Text);
    }

    [Fact]
    public void GetAllTexts_ShouldReturnAllThreeLanguages_WhenValidId()
    {
        var actual = _sut.GetAllTexts(1);

        Assert.Equal(3, actual.Count);
        Assert.Equal("In the beginning", actual["en"]);
        Assert.Equal("በመጀመሪያ", actual["am"]);
        Assert.Equal("Alussa", actual["fi"]);
    }
}

using Encourager.Api.Services;
using Xunit;

namespace Encourager.Api.Tests;

public class VerseLanguageServiceTests
{
    private readonly VerseLanguageService _sut = new();

    [Fact]
    public void GetText_ShouldReturnEnglishText_WhenLanguageIsEn()
    {
        var actual = _sut.GetText(1, "en");

        Assert.Equal("en", actual.Language);
        Assert.Contains("plans", actual.Text);
    }

    [Fact]
    public void GetText_ShouldReturnAmharicText_WhenLanguageIsAm()
    {
        var actual = _sut.GetText(1, "am");

        Assert.Equal("am", actual.Language);
        Assert.False(string.IsNullOrWhiteSpace(actual.Text));
        Assert.NotEqual(_sut.GetText(1, "en").Text, actual.Text);
    }

    [Fact]
    public void GetText_ShouldReturnFinnishText_WhenLanguageIsFi()
    {
        var actual = _sut.GetText(1, "fi");

        Assert.Equal("fi", actual.Language);
        Assert.False(string.IsNullOrWhiteSpace(actual.Text));
        Assert.NotEqual(_sut.GetText(1, "en").Text, actual.Text);
    }

    [Fact]
    public void GetText_ShouldFallBackToEnglish_WhenLanguageIsUnknown()
    {
        var actual = _sut.GetText(1, "xx");

        Assert.Equal("en", actual.Language);
        Assert.Equal(_sut.GetText(1, "en").Text, actual.Text);
    }

    [Fact]
    public void GetText_ShouldClampVerseId_WhenIdOutOfRange()
    {
        var actual = _sut.GetText(9999, "en");

        Assert.Equal("en", actual.Language);
        Assert.False(string.IsNullOrWhiteSpace(actual.Text));
    }

    [Fact]
    public void GetAllTexts_ShouldReturnAllThreeLanguages_WhenValidId()
    {
        var actual = _sut.GetAllTexts(1);

        Assert.Equal(3, actual.Count);
        Assert.True(actual.ContainsKey("en"));
        Assert.True(actual.ContainsKey("fi"));
        Assert.True(actual.ContainsKey("am"));
        Assert.False(string.IsNullOrWhiteSpace(actual["en"]));
        Assert.False(string.IsNullOrWhiteSpace(actual["fi"]));
        Assert.False(string.IsNullOrWhiteSpace(actual["am"]));
    }

    [Fact]
    public void GetAllTexts_ShouldClampVerseId_WhenIdOutOfRange()
    {
        var actual = _sut.GetAllTexts(9999);

        Assert.Equal(3, actual.Count);
        Assert.False(string.IsNullOrWhiteSpace(actual["en"]));
    }
}

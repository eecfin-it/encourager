using Encourager.Api.Models;
using Encourager.Api.Services;
using Xunit;

namespace Encourager.Api.Tests;

public class VerseFormatterServiceTests
{
    private readonly VerseFormatterService _sut = new();

    [Fact]
    public void Format_ShouldReturnCompleteResponse_WhenGivenMetadataAndText()
    {
        var metadata = new VerseMetadata(1, "Psalm", 23, "1");
        var verseText = new VerseText("The Lord is my shepherd.", "en");
        var translations = new Dictionary<string, string>
        {
            ["en"] = "The Lord is my shepherd.",
            ["fi"] = "Herra on minun paimeneni.",
            ["am"] = "እግዚአብሔር እረኛዬ ነው።"
        };

        var actual = _sut.Format(metadata, verseText, translations);

        Assert.Equal(1, actual.VerseId);
        Assert.Equal("Psalm", actual.Book);
        Assert.Equal(23, actual.Chapter);
        Assert.Equal("1", actual.VerseNumber);
        Assert.Equal("The Lord is my shepherd.", actual.Text);
        Assert.Equal("en", actual.Language);
        Assert.Equal(3, actual.Translations.Count);
        Assert.Equal("The Lord is my shepherd.", actual.Translations["en"]);
    }

    [Fact]
    public void Format_ShouldPreserveAllFields_WhenGivenVerseRange()
    {
        var metadata = new VerseMetadata(6, "Numbers", 6, "24-25");
        var verseText = new VerseText("Herra siunatkoon sinua.", "fi");
        var translations = new Dictionary<string, string>
        {
            ["en"] = "The Lord bless you.",
            ["fi"] = "Herra siunatkoon sinua.",
            ["am"] = "እግዚአብሔር ይባርክህ።"
        };

        var actual = _sut.Format(metadata, verseText, translations);

        Assert.Equal(6, actual.VerseId);
        Assert.Equal("Numbers", actual.Book);
        Assert.Equal(6, actual.Chapter);
        Assert.Equal("24-25", actual.VerseNumber);
        Assert.Equal("fi", actual.Language);
        Assert.Equal(3, actual.Translations.Count);
    }
}

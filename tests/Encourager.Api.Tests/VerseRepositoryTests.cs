using Encourager.Api.Data;
using Xunit;

namespace Encourager.Api.Tests;

public class VerseRepositoryTests
{
    private readonly VerseRepository _sut = new();

    [Fact]
    public void Constructor_ShouldIndexAllVerses_WhenCreated()
    {
        Assert.True(_sut.Count > 0);
        Assert.Equal(_sut.Count, _sut.Metadata.Count);
        Assert.Equal(_sut.Count, _sut.Translations.Count);
    }

    [Fact]
    public void Metadata_ShouldHaveConsecutiveIds_WhenCreated()
    {
        for (var i = 1; i <= _sut.Count; i++)
            Assert.True(_sut.Metadata.ContainsKey(i), $"Missing metadata for verseId {i}");
    }

    [Fact]
    public void Metadata_ShouldParseReferenceCorrectly_WhenFirstVerse()
    {
        var actual = _sut.Metadata[1];

        Assert.Equal(1, actual.VerseId);
        Assert.Equal("Jeremiah", actual.Book);
        Assert.Equal(29, actual.Chapter);
        Assert.Equal("11", actual.VerseNumber);
    }

    [Fact]
    public void Translations_ShouldContainAllThreeLanguages_WhenAnyVerse()
    {
        foreach (var (verseId, texts) in _sut.Translations)
        {
            Assert.True(texts.ContainsKey("en"), $"Verse {verseId} missing English");
            Assert.True(texts.ContainsKey("am"), $"Verse {verseId} missing Amharic");
            Assert.True(texts.ContainsKey("fi"), $"Verse {verseId} missing Finnish");
            Assert.False(string.IsNullOrWhiteSpace(texts["en"]), $"Verse {verseId} has empty English text");
        }
    }
}

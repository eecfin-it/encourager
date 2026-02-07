using Encourager.Api.Services;
using Xunit;

namespace Encourager.Api.Tests;

public class ReferenceParserTests
{
    [Fact]
    public void Parse_ShouldReturnBookChapterVerse_WhenSimpleReference()
    {
        var (book, chapter, verseNumber) = ReferenceParser.Parse("Psalm 23:1");

        Assert.Equal("Psalm", book);
        Assert.Equal(23, chapter);
        Assert.Equal("1", verseNumber);
    }

    [Fact]
    public void Parse_ShouldHandleNumberedBook_WhenReferenceStartsWithNumber()
    {
        var (book, chapter, verseNumber) = ReferenceParser.Parse("1 Peter 5:7");

        Assert.Equal("1 Peter", book);
        Assert.Equal(5, chapter);
        Assert.Equal("7", verseNumber);
    }

    [Fact]
    public void Parse_ShouldPreserveVerseRange_WhenReferenceContainsDash()
    {
        var (book, chapter, verseNumber) = ReferenceParser.Parse("Numbers 6:24-25");

        Assert.Equal("Numbers", book);
        Assert.Equal(6, chapter);
        Assert.Equal("24-25", verseNumber);
    }

    [Fact]
    public void Parse_ShouldHandleTwoWordBook_WhenBookNameHasMultipleWords()
    {
        var (book, chapter, verseNumber) = ReferenceParser.Parse("2 Corinthians 12:9");

        Assert.Equal("2 Corinthians", book);
        Assert.Equal(12, chapter);
        Assert.Equal("9", verseNumber);
    }
}

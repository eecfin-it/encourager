using Encourager.Api.Data;
using Encourager.Api.Services;
using Xunit;

namespace Encourager.Api.Tests;

public class VerseLookupServiceTests
{
    private readonly VerseLookupService _sut = new();

    [Fact]
    public void GetRandom_ShouldReturnValidMetadata_WhenCalled()
    {
        var actual = _sut.GetRandom();

        Assert.InRange(actual.VerseId, 1, VerseRepository.Count);
        Assert.False(string.IsNullOrWhiteSpace(actual.Book));
        Assert.True(actual.Chapter > 0);
    }

    [Fact]
    public void GetRandom_ShouldReturnVariedResults_WhenCalledMultipleTimes()
    {
        var ids = new HashSet<int>();
        for (var i = 0; i < 30; i++)
            ids.Add(_sut.GetRandom().VerseId);

        Assert.True(ids.Count > 1);
    }

    [Fact]
    public void GetByVerseId_ShouldReturnCorrectMetadata_WhenValidId()
    {
        var actual = _sut.GetByVerseId(1);

        Assert.Equal(1, actual.VerseId);
        Assert.Equal("Jeremiah", actual.Book);
        Assert.Equal(29, actual.Chapter);
        Assert.Equal("11", actual.VerseNumber);
    }

    [Fact]
    public void GetByVerseId_ShouldClampToOne_WhenIdBelowRange()
    {
        var actual = _sut.GetByVerseId(-5);

        Assert.Equal(1, actual.VerseId);
    }

    [Fact]
    public void GetByVerseId_ShouldClampToMax_WhenIdAboveRange()
    {
        var actual = _sut.GetByVerseId(9999);

        Assert.Equal(VerseRepository.Count, actual.VerseId);
    }
}

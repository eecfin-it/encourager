using Encourager.Api.Data;
using Encourager.Api.Models;
using Encourager.Api.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Encourager.Api.Tests;

public class VerseLookupServiceTests
{
    private readonly IVerseRepository _repository = Substitute.For<IVerseRepository>();
    private readonly VerseLookupService _sut;

    public VerseLookupServiceTests()
    {
        var metadata = new Dictionary<int, VerseMetadata>
        {
            [1] = new(1, "Genesis", 1, "1"),
            [2] = new(2, "Psalm", 23, "1"),
            [3] = new(3, "John", 3, "16")
        };
        _repository.Count.Returns(3);
        _repository.Metadata.Returns(metadata);
        _sut = new VerseLookupService(_repository, Substitute.For<ILogger<VerseLookupService>>());
    }

    [Fact]
    public void GetRandom_ShouldReturnValidMetadata_WhenCalled()
    {
        var actual = _sut.GetRandom();

        Assert.InRange(actual.VerseId, 1, 3);
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
        var actual = _sut.GetByVerseId(2);

        Assert.Equal(2, actual.VerseId);
        Assert.Equal("Psalm", actual.Book);
        Assert.Equal(23, actual.Chapter);
        Assert.Equal("1", actual.VerseNumber);
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

        Assert.Equal(3, actual.VerseId);
    }
}

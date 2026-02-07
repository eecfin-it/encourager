using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Encourager.Api.Tests;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // --- Health endpoint ---

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HealthCheck_ReturnsJsonWithStatus()
    {
        var body = await _client.GetFromJsonAsync<HealthResponse>("/api/health");
        Assert.NotNull(body);
        Assert.Equal("healthy", body.Status);
        Assert.True(body.Timestamp > DateTime.MinValue);
    }

    // --- Random verse ---

    [Fact]
    public async Task GetRandomVerse_DefaultLang_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/verse/random");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetRandomVerse_DefaultLang_ReturnsVerseShape()
    {
        var body = await _client.GetFromJsonAsync<VerseResponse>("/api/verse/random");
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body.Text));
        Assert.False(string.IsNullOrWhiteSpace(body.Reference));
        Assert.True(body.Index >= 0);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("fi")]
    [InlineData("am")]
    public async Task GetRandomVerse_EachLanguage_ReturnsVerse(string lang)
    {
        var body = await _client.GetFromJsonAsync<VerseResponse>(
            $"/api/verse/random?lang={lang}");
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body.Text));
        Assert.False(string.IsNullOrWhiteSpace(body.Reference));
    }

    [Fact]
    public async Task GetRandomVerse_UnknownLang_FallsBackToEnglish()
    {
        var body = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?lang=xx");
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body.Text));
    }

    // --- Index-based verse ---

    [Fact]
    public async Task GetVerseByIndex_ReturnsCorrectIndex()
    {
        var body = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?index=0");
        Assert.NotNull(body);
        Assert.Equal(0, body.Index);
    }

    [Fact]
    public async Task GetVerseByIndex_SameIndexSameVerse()
    {
        var body1 = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?lang=en&index=5");
        var body2 = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?lang=en&index=5");
        Assert.Equal(body1!.Text, body2!.Text);
        Assert.Equal(body1.Reference, body2.Reference);
    }

    [Fact]
    public async Task GetVerseByIndex_NegativeIndex_ClampedToZero()
    {
        var body = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?index=-5");
        Assert.NotNull(body);
        Assert.Equal(0, body.Index);
    }

    [Fact]
    public async Task GetVerseByIndex_OverflowIndex_ClampedToMax()
    {
        var body = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?index=9999");
        Assert.NotNull(body);
        Assert.True(body.Index < 9999);
        Assert.True(body.Index >= 0);
    }

    [Fact]
    public async Task GetVerseByIndex_WithLang_ReturnsDifferentText()
    {
        var en = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?lang=en&index=0");
        var fi = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?lang=fi&index=0");
        Assert.NotNull(en);
        Assert.NotNull(fi);
        Assert.NotEqual(en.Text, fi.Text);
    }

    // --- Randomness ---

    [Fact]
    public async Task GetRandomVerse_MultipleCalls_ReturnsVariedResults()
    {
        var indices = new HashSet<int>();
        for (var i = 0; i < 30; i++)
        {
            var body = await _client.GetFromJsonAsync<VerseResponse>("/api/verse/random?lang=en");
            Assert.NotNull(body);
            indices.Add(body.Index);
        }

        Assert.True(indices.Count > 1, $"Expected varied verse indices over 30 calls, but got only {indices.Count} distinct index(es): [{string.Join(", ", indices)}]");
    }

    // --- Cache headers ---

    [Fact]
    public async Task GetRandomVerse_ResponseContainsNoCacheHeader()
    {
        var response = await _client.GetAsync("/api/verse/random?lang=en");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var actual = response.Headers.CacheControl;
        Assert.NotNull(actual);
        Assert.True(actual.NoStore, "Expected Cache-Control: no-store header on random verse endpoint");
    }

    // --- Response DTOs ---

    private record HealthResponse(string Status, DateTime Timestamp);
    private record VerseResponse(string Text, string Reference, int Index);
}

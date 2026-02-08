using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Encourager.Api.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    public TestWebApplicationFactory()
    {
        // Set environment variable before the application starts
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        base.ConfigureWebHost(builder);
    }
}

public class ApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiTests(TestWebApplicationFactory factory)
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
        Assert.False(string.IsNullOrWhiteSpace(body.Book));
        Assert.True(body.Chapter > 0);
        Assert.False(string.IsNullOrWhiteSpace(body.VerseNumber));
        Assert.True(body.VerseId >= 1);
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
        Assert.False(string.IsNullOrWhiteSpace(body.Book));
        Assert.Equal(lang, body.Language);
    }

    [Fact]
    public async Task GetRandomVerse_UnknownLang_FallsBackToEnglish()
    {
        var body = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?lang=xx");
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body.Text));
        Assert.Equal("en", body.Language);
    }

    // --- VerseId-based verse ---

    [Fact]
    public async Task GetVerseByVerseId_ReturnsCorrectVerseId()
    {
        var body = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?verseId=1");
        Assert.NotNull(body);
        Assert.Equal(1, body.VerseId);
    }

    [Fact]
    public async Task GetVerseByVerseId_SameIdSameVerse()
    {
        var body1 = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?lang=en&verseId=5");
        var body2 = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?lang=en&verseId=5");
        Assert.Equal(body1!.Text, body2!.Text);
        Assert.Equal(body1.Book, body2.Book);
    }

    [Fact]
    public async Task GetVerseByVerseId_NegativeId_ClampedToOne()
    {
        var body = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?verseId=-5");
        Assert.NotNull(body);
        Assert.Equal(1, body.VerseId);
    }

    [Fact]
    public async Task GetVerseByVerseId_OverflowId_ClampedToMax()
    {
        var body = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?verseId=9999");
        Assert.NotNull(body);
        Assert.True(body.VerseId < 9999);
        Assert.True(body.VerseId >= 1);
    }

    [Fact]
    public async Task GetVerseByVerseId_WithLang_ReturnsDifferentText()
    {
        var en = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?lang=en&verseId=1");
        var fi = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?lang=fi&verseId=1");
        Assert.NotNull(en);
        Assert.NotNull(fi);
        Assert.NotEqual(en.Text, fi.Text);
    }

    // --- Language field ---

    [Fact]
    public async Task GetRandomVerse_ResponseIncludesLanguageField()
    {
        var body = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?lang=fi");
        Assert.NotNull(body);
        Assert.Equal("fi", body.Language);
    }

    // --- Translations field ---

    [Fact]
    public async Task GetRandomVerse_ResponseIncludesAllTranslations()
    {
        var body = await _client.GetFromJsonAsync<VerseResponse>(
            "/api/verse/random?lang=en&verseId=1");
        Assert.NotNull(body);
        Assert.NotNull(body.Translations);
        Assert.Equal(3, body.Translations.Count);
        Assert.True(body.Translations.ContainsKey("en"));
        Assert.True(body.Translations.ContainsKey("fi"));
        Assert.True(body.Translations.ContainsKey("am"));
        Assert.Equal(body.Text, body.Translations["en"]);
    }

    // --- Randomness ---

    [Fact]
    public async Task GetRandomVerse_MultipleCalls_ReturnsVariedResults()
    {
        var ids = new HashSet<int>();
        for (var i = 0; i < 30; i++)
        {
            var body = await _client.GetFromJsonAsync<VerseResponse>("/api/verse/random?lang=en");
            Assert.NotNull(body);
            ids.Add(body.VerseId);
        }

        Assert.True(ids.Count > 1, $"Expected varied verse IDs over 30 calls, but got only {ids.Count} distinct ID(s): [{string.Join(", ", ids)}]");
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
    private record VerseResponse(int VerseId, string Book, int Chapter, string VerseNumber, string Text, string Language, Dictionary<string, string> Translations);
}

using System.Text.Json;
using Encourager.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Encourager.Api;

public static class AppConfiguration
{
    public static void ConfigureServices(IServiceCollection services)
    {
        var allowedOrigin = Environment.GetEnvironmentVariable("ALLOWED_ORIGIN") ?? "*";
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                if (allowedOrigin == "*")
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                }
                else
                {
                    policy.WithOrigins(allowedOrigin).AllowAnyHeader().AllowAnyMethod();
                }
            });
        });
        services.AddSingleton<VerseService>();
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    }

    public static void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/verse/random", (VerseService verseService, string? lang, int? index) =>
        {
            var result = index.HasValue
                ? verseService.GetByIndex(lang ?? "en", index.Value)
                : verseService.GetRandom(lang ?? "en");
            return Results.Ok(new { result.Verse.Text, result.Verse.Reference, result.Index });
        })
        .WithName("GetRandomVerse");

        endpoints.MapGet("/api/health", () =>
            Results.Ok(new { Status = "healthy", Timestamp = DateTime.UtcNow }))
        .WithName("HealthCheck");
    }
}

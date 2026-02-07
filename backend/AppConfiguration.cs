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
        services.AddSingleton<IVerseLookupService, VerseLookupService>();
        services.AddSingleton<IVerseLanguageService, VerseLanguageService>();
        services.AddSingleton<IVerseFormatterService, VerseFormatterService>();
        services.AddSingleton<IVerseCoordinatorService, VerseCoordinatorService>();
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    }

    public static void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/verse/random", (HttpContext httpContext, IVerseCoordinatorService coordinator, string? lang, int? verseId) =>
        {
            httpContext.Response.Headers.CacheControl = "no-store";
            var result = verseId.HasValue
                ? coordinator.GetVerseByVerseId(verseId.Value, lang ?? "en")
                : coordinator.GetRandomVerse(lang ?? "en");
            return Results.Ok(result);
        })
        .WithName("GetRandomVerse");

        endpoints.MapGet("/api/health", () =>
            Results.Ok(new { Status = "healthy", Timestamp = DateTime.UtcNow }))
        .WithName("HealthCheck");
    }
}

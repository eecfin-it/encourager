using System.Text.Json;
using Encourager.Api.Data;
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
        var allowedOrigin = Environment.GetEnvironmentVariable("ALLOWED_ORIGIN");
        var aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                // Only allow wildcard in Development environment
                if (string.IsNullOrEmpty(allowedOrigin))
                {
                    if (aspNetCoreEnvironment.Equals("Development", StringComparison.OrdinalIgnoreCase))
                    {
                        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    }
                    else
                    {
                        // In production, require explicit origin configuration
                        throw new InvalidOperationException(
                            "ALLOWED_ORIGIN environment variable must be set in non-Development environments. " +
                            "Set it to your CloudFront distribution URL (e.g., https://d1234567890.cloudfront.net) " +
                            "or custom domain (e.g., https://encourager.example.com)");
                    }
                }
                else if (allowedOrigin == "*")
                {
                    // Explicit wildcard - warn but allow (for backward compatibility)
                    Console.WriteLine("WARNING: CORS is configured with wildcard (*). This is not recommended for production.");
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                }
                else
                {
                    // Parse multiple origins separated by comma
                    var origins = allowedOrigin.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
                }
            });
        });
        services.AddSingleton<IVerseRepository, VerseRepository>();
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
        endpoints.MapGet("/api/verse/random", (HttpContext httpContext, IVerseCoordinatorService coordinator, ILogger<Program> logger, string? lang, int? verseId) =>
        {
            var language = lang ?? "en";
            logger.LogInformation("API request received: /api/verse/random with language={Language}, verseId={VerseId}", language, verseId);

            httpContext.Response.Headers.CacheControl = "no-store";
            var result = verseId.HasValue
                ? coordinator.GetVerseByVerseId(verseId.Value, language)
                : coordinator.GetRandomVerse(language);

            logger.LogInformation("API response sent: verse {VerseId} ({Book} {Chapter}:{VerseNumber}) in language {Language}",
                result.VerseId, result.Book, result.Chapter, result.VerseNumber, result.Language);
            return Results.Ok(result);
        })
        .WithName("GetRandomVerse");

        endpoints.MapGet("/api/health", (ILogger<Program> logger) =>
        {
            logger.LogDebug("Health check requested");
            return Results.Ok(new { Status = "healthy", Timestamp = DateTime.UtcNow });
        })
        .WithName("HealthCheck");
    }
}

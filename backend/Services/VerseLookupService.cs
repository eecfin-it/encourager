using Encourager.Api.Data;
using Encourager.Api.Models;
using Microsoft.Extensions.Logging;

namespace Encourager.Api.Services;

public class VerseLookupService(IVerseRepository repository, ILogger<VerseLookupService> logger) : IVerseLookupService
{
    public VerseMetadata GetRandom()
    {
        var verseId = Random.Shared.Next(1, repository.Count + 1);
        logger.LogDebug("Selected random verse ID: {VerseId} from {TotalVerses} verses", verseId, repository.Count);
        var metadata = repository.Metadata[verseId];
        return metadata;
    }

    public VerseMetadata GetByVerseId(int verseId)
    {
        var safeId = Math.Clamp(verseId, 1, repository.Count);
        if (safeId != verseId)
        {
            logger.LogWarning("Verse ID {RequestedVerseId} out of range, clamped to {SafeVerseId}", verseId, safeId);
        }
        logger.LogDebug("Looking up verse by ID: {VerseId}", safeId);
        var metadata = repository.Metadata[safeId];
        return metadata;
    }
}

using Encourager.Api.Data;
using Encourager.Api.Models;

namespace Encourager.Api.Services;

public class VerseLookupService : IVerseLookupService
{
    public VerseMetadata GetRandom()
    {
        var verseId = Random.Shared.Next(1, VerseRepository.Count + 1);
        return VerseRepository.Metadata[verseId];
    }

    public VerseMetadata GetByVerseId(int verseId)
    {
        var safeId = Math.Clamp(verseId, 1, VerseRepository.Count);
        return VerseRepository.Metadata[safeId];
    }
}

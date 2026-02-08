using Encourager.Api.Data;
using Encourager.Api.Models;

namespace Encourager.Api.Services;

public class VerseLookupService(IVerseRepository repository) : IVerseLookupService
{
    public VerseMetadata GetRandom()
    {
        var verseId = Random.Shared.Next(1, repository.Count + 1);
        return repository.Metadata[verseId];
    }

    public VerseMetadata GetByVerseId(int verseId)
    {
        var safeId = Math.Clamp(verseId, 1, repository.Count);
        return repository.Metadata[safeId];
    }
}

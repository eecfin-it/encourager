using Encourager.Api.Models;

namespace Encourager.Api.Services;

public interface IVerseLookupService
{
    VerseMetadata GetRandom();
    VerseMetadata GetByVerseId(int verseId);
}

using Encourager.Api.Models;

namespace Encourager.Api.Services;

public interface IVerseCoordinatorService
{
    VerseResponse GetRandomVerse(string language);
    VerseResponse GetVerseByVerseId(int verseId, string language);
}

using Encourager.Api.Models;

namespace Encourager.Api.Data;

public interface IVerseRepository
{
    int Count { get; }
    IReadOnlyDictionary<int, VerseMetadata> Metadata { get; }
    IReadOnlyDictionary<int, IReadOnlyDictionary<string, string>> Translations { get; }
}

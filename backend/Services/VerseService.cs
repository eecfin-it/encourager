using Encourager.Api.Data;
using Encourager.Api.Models;

namespace Encourager.Api.Services;

public class VerseService
{
    public Verse GetRandom(string language = "en")
    {
        var verses = language == "am" ? AmharicVerses.Verses : EnglishVerses.Verses;
        return verses[Random.Shared.Next(verses.Length)];
    }
}

using System.Text.RegularExpressions;

namespace Encourager.Api.Services;

public static partial class ReferenceParser
{
    [GeneratedRegex(@"^(.+?)\s+(\d+):(.+)$")]
    private static partial Regex ReferencePattern();

    public static (string Book, int Chapter, string VerseNumber) Parse(string reference)
    {
        var match = ReferencePattern().Match(reference);
        if (!match.Success)
            return (reference, 0, "0");

        return (match.Groups[1].Value, int.Parse(match.Groups[2].Value), match.Groups[3].Value);
    }
}

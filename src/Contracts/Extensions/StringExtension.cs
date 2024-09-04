using System.Text;
using System.Text.RegularExpressions;

namespace Contracts.Extensions;

public static partial class StringExtension
{
    public static string Underscored(this string s)
    {
        var builder = new StringBuilder();

        for (var i = 0; i < s.Length; ++i)
        {
            if (ShouldUnderscore(i, s))
            {
                builder.Append('_');
            }

            builder.Append(char.ToLowerInvariant(s[i]));
        }

        return builder.ToString();
    }

    public static string LowerCaseFirst(this string s)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToLower(s[0]) + s[1..];
    }

    public static string SpecialCharacterRemoving(this string s)
    {
        Regex regex = MyRegex();

        return regex.Replace(s, string.Empty);
    }

    private static bool ShouldUnderscore(int i, string s)
    {
        if (i == 0 || i >= s.Length || s[i] == '_') return false;

        var curr = s[i];
        var prev = s[i - 1];
        var next = i < s.Length - 2 ? s[i + 1] : '_';

        return prev != '_' && ((char.IsUpper(curr) && (char.IsLower(prev) || char.IsLower(next))) ||
            (char.IsNumber(curr) && (!char.IsNumber(prev))));
    }

    [GeneratedRegex("[^A-Za-z0-9_.]+")]
    private static partial Regex MyRegex();
}
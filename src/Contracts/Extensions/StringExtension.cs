using System.Security.Cryptography;
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
        Regex regex = RemoveSpecialCharacterRegex();

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

    public static string GenerateRandomString(int codeLength = 16)
    {
        const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._-";

        if (codeLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(codeLength), "length cannot be less than zero.");
        }

        const int byteSize = 0x100;
        var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
        if (allowedCharSet.Length > byteSize)
        {
            throw new ArgumentException(string.Format("allowedChars may contain no more than {0} characters.", byteSize));
        }

        using var rng = RandomNumberGenerator.Create();
        var result = new StringBuilder();
        var buf = new byte[128];
        while (result.Length < codeLength)
        {
            rng.GetBytes(buf);
            for (var i = 0; i < buf.Length && result.Length < codeLength; ++i)
            {
                var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                if (outOfRangeStart <= buf[i])
                {
                    continue;
                }

                result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
            }
        }

        return result.ToString();
    }

    public static string ToScreamingSnakeCase(this string input)
    {
        // Handle PascalCase and camelCase by inserting underscores before capital letters
        string result = PascalAndCamelRegex().Replace(input, "$1_$2");
        
        // Handle consecutive uppercase letters (e.g., "HTTPServer" -> "HTTP_SERVER")
        result = MyRegex().Replace(result, "$1_$2");
        
        // Replace dashes (for kebab-case and Train-Case) with underscores
        result = result.Replace("-", "_");

        // Convert the entire string to uppercase for SCREAMING_SNAKE_CASE
        return result.ToUpper();
    }

    [GeneratedRegex("[^A-Za-z0-9_.]+")]
    private static partial Regex RemoveSpecialCharacterRegex();

    [GeneratedRegex(@"([a-z])([A-Z])")]
    private static partial Regex PascalAndCamelRegex();
    
    [GeneratedRegex(@"([A-Z]+)([A-Z][a-z])")]
    private static partial Regex MyRegex();
}
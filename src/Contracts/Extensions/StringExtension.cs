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

    public static string SpecialCharacterRemoving(this string s)
    {
        Regex regex = RemoveSpecialCharacterRegex();

        return regex.Replace(s, string.Empty);
    }

    public static string GenerateRandomString(int codeLength = 16, string? allowedSources = null)
    {
        string allowedChars =
            allowedSources ?? "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._-";

        if (codeLength < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(codeLength),
                "length cannot be less than zero."
            );
        }

        const int byteSize = 0x100;
        var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
        if (allowedCharSet.Length > byteSize)
        {
            throw new ArgumentException(
                string.Format("allowedChars may contain no more than {0} characters.", byteSize)
            );
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

    public static string ToScreamingSnakeCase(this string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        // Handle PascalCase and camelCase by inserting underscores before capital letters
        string result = PascalAndCamelRegex().Replace(input, "$1_$2");

        // Handle consecutive uppercase letters (e.g., "HTTPServer" -> "HTTP_SERVER")
        result = UpperLetter().Replace(result, "$1_$2");

        // Replace dashes (for kebab-case and Train-Case) with underscores
        result = result.Replace("-", "_");

        // Convert the entire string to uppercase for SCREAMING_SNAKE_CASE
        return result.ToUpper();
    }

    /// <summary>
    /// For generate parameter name for nested any
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string NextUniformSequence(this string input)
    {
        // Convert the input string to a char array for manipulation
        char[] arr = input.ToCharArray();

        // Start by checking if all characters are 'z'
        bool allZ = true;
        foreach (char c in arr)
        {
            if (c != 'z')
            {
                allZ = false;
                break;
            }
        }

        // If all characters are 'z', return the next sequence with an additional 'a'
        if (allZ)
        {
            return new string('a', arr.Length + 1);
        }

        // Uniformly increment all characters in the string
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = (char)(arr[i] + 1);
        }

        return new string(arr); // Return the modified array as a new string
    }

    private static bool ShouldUnderscore(int i, string s)
    {
        if (i == 0 || i >= s.Length || s[i] == '_')
            return false;

        var curr = s[i];
        var prev = s[i - 1];
        var next = i < s.Length - 2 ? s[i + 1] : '_';

        return prev != '_'
            && (
                (char.IsUpper(curr) && (char.IsLower(prev) || char.IsLower(next)))
                || (char.IsNumber(curr) && (!char.IsNumber(prev)))
            );
    }

    [GeneratedRegex("[^A-Za-z0-9_.]+")]
    private static partial Regex RemoveSpecialCharacterRegex();

    [GeneratedRegex(@"([a-z])([A-Z])")]
    private static partial Regex PascalAndCamelRegex();

    [GeneratedRegex(@"([A-Z]+)([A-Z][a-z])")]
    private static partial Regex UpperLetter();
}

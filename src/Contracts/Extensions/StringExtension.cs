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
        // Split the input into the alphabetic part and the numeric suffix
        string alphabeticPart = input.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
        string numericPart = input[alphabeticPart.Length..];

        // Handle the numeric part: if no number exists, start with 1
        int suffix = numericPart == string.Empty ? 1 : int.Parse(numericPart);

        // Convert the alphabetic part to a character array
        char[] arr = alphabeticPart.ToCharArray();

        // Flag to track if all characters are 'z'
        bool allZ = true;

        // Check if the entire alphabetic part is 'z'
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] != 'z')
            {
                allZ = false;
                break;
            }
        }

        // If all characters are 'z', start the next cycle with 'a' and increment the suffix
        if (allZ)
        {
            // Return the next sequence with an incremented numeric suffix
            return "a" + (suffix + 1);
        }

        // Otherwise, increment the alphabetic characters
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = (char)(arr[i] + 1); // Increment the character
        }

        // Combine the alphabetic part with the current suffix
        return new string(arr) + (suffix > 1 ? suffix : string.Empty);
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

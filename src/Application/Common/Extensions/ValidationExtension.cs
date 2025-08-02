using System.Text.RegularExpressions;

namespace Application.Common.Extensions;

public static partial class ValidationExtension
{
    public static bool IsValidUsername(this string username) =>
        UsernameValidationRegex().IsMatch(username);

    public static bool IsValidPassword(this string password) =>
        PasswordValidationRegex().IsMatch(password);

    public static bool IsValidPhoneNumber(this string phoneNumber) =>
        PhoneValidationRegex().IsMatch(phoneNumber);

    public static bool IsValidEmail(this string email) => EmailValidationRegex().IsMatch(email);

    [GeneratedRegex(@"^[a-zA-Z0-9_.]+$")]
    private static partial Regex UsernameValidationRegex();

    [GeneratedRegex(@"^((?=\S*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9]).{8,})\S$")]
    private static partial Regex PasswordValidationRegex();

    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$")]
    private static partial Regex EmailValidationRegex();

    [GeneratedRegex(@"^\+?\d{7,15}$")]
    private static partial Regex PhoneValidationRegex();
}

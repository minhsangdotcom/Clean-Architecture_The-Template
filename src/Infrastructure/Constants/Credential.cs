namespace Infrastructure.Constants;

public static class Credential
{
    public const string USER_DEFAULT_PASSWORD = "123456";

    public const string ADMIN_ROLE = "ADMIN";

    public static readonly IReadOnlyCollection<KeyValuePair<string, string>> CLAIMS =
    [
        new("permission", "create.user"),
        new("permission", "update.user"),
        new("permission", "delete.user"),
        new("permission", "list.user"),
        new("permission", "detail.user"),
    ];

    public const string ADMIN_ROLE_ID = "01J79JQZRWAKCTCQV64VYKMZ56";
}

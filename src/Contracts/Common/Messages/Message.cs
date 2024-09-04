namespace Contracts.Common.Messages;

public static class Message
{
    public const string SUCCESS = nameof(SUCCESS);
    public const string LOGIN_SUCCESS = $"LOGIN {nameof(SUCCESS)}";

    public const string UNAUTHORIZED = nameof(UNAUTHORIZED);

    public const string FORBIDDEN = nameof(FORBIDDEN);
    
    public const string TOKEN_EXPIRED = "TOKEN EXPIRED";
}
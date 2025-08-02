namespace Infrastructure.Services.Token;

public class JwtSettings
{
    public string? SecretKey { get; set; }

    public string? ExpireTimeAccessTokenInMinute { get; set; }

    public string? ExpireTimeRefreshTokenInDay { get; set; }
}

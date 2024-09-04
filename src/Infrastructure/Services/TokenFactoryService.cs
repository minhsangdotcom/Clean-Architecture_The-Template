using System.Security.Claims;
using Application.Common.Interfaces.Services;
using Contracts.Common.Settings;
using Contracts.Dtos.Responses;
using Contracts.Extensions;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class TokenFactoryService(IOptions<JwtSettings> jwtSettings) : ITokenFactory
{
    private readonly JwtSettings settings = jwtSettings.Value;

    public DateTime AccesstokenExpiredTime => GetAccesstokenExpiredTime();

    public DateTime RefreshtokenExpiredTime => GetRefreshtokenExpiredTime();

    public string CreateToken(IEnumerable<Claim> claims, DateTime expirationTime)
    {
        return JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .AddClaims(claims.Select(x => new KeyValuePair<string, object>(x.Type, x.Value)))
                .WithSecret(settings.SecretKey)
                .ExpirationTime(expirationTime)
                .WithValidationParameters(new ValidationParameters()
                {
                    ValidateExpirationTime = true,
                    TimeMargin = 0
                })
                .Encode();
    }

    public DecodeTokenResponse DecodeToken(string token)
    {
        var json = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(settings.SecretKey)
                .MustVerifySignature()
                .Decode(token);

        return SerializerExtension.Deserialize<DecodeTokenResponse>(json)!;
    }

    private DateTime GetAccesstokenExpiredTime() =>
        DateTime.UtcNow.AddHours(double.Parse(settings.ExpireTimeAccessToken!));

    private DateTime GetRefreshtokenExpiredTime() =>
        DateTime.UtcNow.AddDays(double.Parse(settings.ExpireTimeRefreshToken!));
}
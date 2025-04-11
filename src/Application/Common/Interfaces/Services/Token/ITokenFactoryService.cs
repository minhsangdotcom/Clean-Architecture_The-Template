using System.Security.Claims;
using Contracts.Dtos.Responses;

namespace Application.Common.Interfaces.Services.Token;

public interface ITokenFactoryService
{
    public DateTime AccesstokenExpiredTime { get; }

    public DateTime RefreshtokenExpiredTime { get; }

    DecodeTokenResponse DecodeToken(string token);

    string CreateToken(IEnumerable<Claim> claims, DateTime expirationTime);
}

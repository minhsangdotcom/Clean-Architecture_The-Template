using System.Security.Claims;
using Application.Common.Interfaces.Registers;
using Contracts.Dtos.Responses;

namespace Application.Common.Interfaces.Services.Token;

public interface ITokenFactory : ISingleton
{
    public DateTime AccesstokenExpiredTime { get; }

    public DateTime RefreshtokenExpiredTime { get; }

    DecodeTokenResponse DecodeToken(string token);

    string CreateToken(IEnumerable<Claim> claims, DateTime expirationTime);
}

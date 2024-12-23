using Contracts.Dtos.Models;

namespace Application.Features.Common.Projections.Users;

public class UserClaimDetailProjection : DefaultBaseResponse
{
    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }
}

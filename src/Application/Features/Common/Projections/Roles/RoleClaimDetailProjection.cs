using Contracts.Dtos.Models;

namespace Application.Features.Common.Projections.Roles;

public class RoleClaimDetailProjection : DefaultBaseResponse
{
    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }
}

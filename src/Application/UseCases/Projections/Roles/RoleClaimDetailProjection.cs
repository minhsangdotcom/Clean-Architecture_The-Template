using Contracts.Dtos.Models;

namespace Application.UseCases.Projections.Roles;

public class RoleClaimDetailProjection : BaseResponse
{
    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }
}
using Contracts.Dtos.Models;

namespace Application.UseCases.Projections.Users;

public class UserClaimDetailProjection : DefaultBaseResponse
{
    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }
}

using Contracts.Dtos.Models;

namespace Application.Features.Common.Projections.Roles;

public class RoleProjection : DefaultBaseResponse
{
    public string? Guard { get; set; }

    public string? Description { get; set; }

    public string? Name { get; set; }
}

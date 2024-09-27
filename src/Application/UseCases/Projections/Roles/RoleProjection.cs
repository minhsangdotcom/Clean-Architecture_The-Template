using Contracts.Dtos.Models;

namespace Application.UseCases.Projections.Roles;

public class RoleProjection : DefaultBaseResponse
{
    public string? Guard { get; set; }

    public string? Description { get; set; }

    public string? Name { get; set; }
}
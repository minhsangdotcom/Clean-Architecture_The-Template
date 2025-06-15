using Application.Features.Common.Payloads.Roles;
using Application.Features.Common.Projections.Roles;
using Contracts.ApiWrapper;
using Mediator;

namespace Application.Features.Roles.Commands.Update;

public class UpdateRoleCommand : IRequest<Result<UpdateRoleResponse>>
{
    public string RoleId { get; set; } = string.Empty;

    public RoleUpdateRequest UpdateData { get; set; } = null!;
}

public class RoleUpdateRequest : RolePayload;

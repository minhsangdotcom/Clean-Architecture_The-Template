using Application.Features.Common.Payloads.Roles;
using Application.Features.Common.Projections.Roles;
using Contracts.ApiWrapper;
using Mediator;

namespace Application.Features.Roles.Commands.Create;

public class CreateRoleCommand : RolePayload, IRequest<Result<CreateRoleResponse>>;

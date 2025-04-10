using Application.Features.Common.Projections.Roles;
using Contracts.ApiWrapper;
using Mediator;

namespace Application.Features.Roles.Commands.Create;

public class CreateRoleCommand : RoleModel, IRequest<Result<CreateRoleResponse>>;

using Application.Features.Common.Projections.Roles;
using Mediator;

namespace Application.Features.Roles.Commands.Create;

public class CreateRoleCommand : RoleModel, IRequest<CreateRoleResponse>;

using Contracts.ApiWrapper;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Application.Features.Roles.Commands.Delete;

public record DeleteRoleCommand([FromRoute] Ulid RoleId) : IRequest<Result<string>>;

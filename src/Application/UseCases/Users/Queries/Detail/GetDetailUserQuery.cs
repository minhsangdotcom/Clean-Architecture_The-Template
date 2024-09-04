using Contracts.Routers;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Application.UseCases.Users.Queries.Detail;

public record GetDetailUserQuery([FromRoute(Name = Router.Id)]string UserId) : IRequest<GetDetailUserResponse>;
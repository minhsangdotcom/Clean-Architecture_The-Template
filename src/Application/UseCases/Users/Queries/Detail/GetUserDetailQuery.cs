using Mediator;

namespace Application.UseCases.Users.Queries.Detail;

public record GetUserDetailQuery(Ulid UserId) : IRequest<GetUserDetailResponse>;
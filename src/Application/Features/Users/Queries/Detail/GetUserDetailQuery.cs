using Mediator;

namespace Application.Features.Users.Queries.Detail;

public record GetUserDetailQuery(Ulid UserId) : IRequest<GetUserDetailResponse>;

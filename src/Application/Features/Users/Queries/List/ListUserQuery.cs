using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;

namespace Application.Features.Users.Queries.List;

public class ListUserQuery : QueryParamRequest, IRequest<PaginationResponse<ListUserResponse>>;

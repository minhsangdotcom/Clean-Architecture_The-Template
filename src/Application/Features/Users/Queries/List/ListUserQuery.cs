using Contracts.ApiWrapper;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;

namespace Application.Features.Users.Queries.List;

public class ListUserQuery
    : QueryParamRequest,
        IRequest<Result<PaginationResponse<ListUserResponse>>>;

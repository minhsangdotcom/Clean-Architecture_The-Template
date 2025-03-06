using Mediator;
using SharedKernel.Requests;
using SharedKernel.Responses;

namespace Application.Features.Users.Queries.List;

public class ListUserQuery : QueryParamRequest, IRequest<PaginationResponse<ListUserResponse>>;

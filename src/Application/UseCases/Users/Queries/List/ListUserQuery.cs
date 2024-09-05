using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;

namespace Application.UseCases.Users.Queries.List;

public class ListUserQuery : QueryRequest, IRequest<PaginationResponse<ListUserResponse>>;
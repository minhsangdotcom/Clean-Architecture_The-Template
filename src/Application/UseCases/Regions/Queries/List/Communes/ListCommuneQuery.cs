using Application.UseCases.Projections.Regions;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;

namespace Application.UseCases.Regions.Queries.List.Communes;

public class ListCommuneQuery : QueryParamRequest, IRequest<PaginationResponse<CommuneDetailProjection>>;

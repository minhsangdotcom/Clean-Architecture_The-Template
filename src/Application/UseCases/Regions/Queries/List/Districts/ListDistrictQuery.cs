using Application.UseCases.Projections.Regions;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;

namespace Application.UseCases.Regions.Queries.List.Districts;

public class ListDistrictQuery : QueryParamRequest, IRequest<PaginationResponse<DistrictDetailProjection>>
{
    
}

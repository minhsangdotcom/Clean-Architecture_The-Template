using Application.Features.Common.Projections.Regions;
using Mediator;
using SharedKernel.Requests;
using SharedKernel.Responses;

namespace Application.Features.Regions.Queries.List.Districts;

public class ListDistrictQuery
    : QueryParamRequest,
        IRequest<PaginationResponse<DistrictDetailProjection>>;

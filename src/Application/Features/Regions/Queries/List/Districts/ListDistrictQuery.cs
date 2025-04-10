using Application.Features.Common.Projections.Regions;
using Contracts.ApiWrapper;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;

namespace Application.Features.Regions.Queries.List.Districts;

public class ListDistrictQuery
    : QueryParamRequest,
        IRequest<Result<PaginationResponse<DistrictProjection>>>;

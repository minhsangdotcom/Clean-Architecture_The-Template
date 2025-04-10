using Application.Features.Common.Projections.Regions;
using Contracts.ApiWrapper;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;

namespace Application.Features.Regions.Queries.List.Provinces;

public class ListProvinceQuery
    : QueryParamRequest,
        IRequest<Result<PaginationResponse<ProvinceProjection>>>;

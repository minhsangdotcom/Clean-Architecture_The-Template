using Application.Features.Common.Projections.Regions;
using Mediator;
using SharedKernel.Requests;
using SharedKernel.Responses;

namespace Application.Features.Regions.Queries.List.Provinces;

public class ListProvinceQuery
    : QueryParamRequest,
        IRequest<PaginationResponse<ProvinceProjection>>;

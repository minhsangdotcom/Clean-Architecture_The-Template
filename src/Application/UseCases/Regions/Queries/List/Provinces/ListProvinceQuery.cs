using Application.UseCases.Projections.Regions;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;

namespace Application.UseCases.Regions.Queries.List.Provinces;

public class ListProvinceQuery
    : QueryParamRequest,
        IRequest<PaginationResponse<ProvinceProjection>>;

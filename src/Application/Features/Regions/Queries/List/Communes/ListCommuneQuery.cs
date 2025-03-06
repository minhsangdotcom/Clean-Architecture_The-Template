using Application.Features.Common.Projections.Regions;
using Mediator;
using SharedKernel.Requests;
using SharedKernel.Responses;

namespace Application.Features.Regions.Queries.List.Communes;

public class ListCommuneQuery
    : QueryParamRequest,
        IRequest<PaginationResponse<CommuneDetailProjection>>;

using Application.Common.Interfaces.Services;
using Application.UseCases.Projections.Regions;
using AutoMapper;
using Mediator;

namespace Application.UseCases.Regions.Queries.List.Provinces;

public class ListProvinceHandler(IRegionService regionService, IMapper mapper)
    : IRequestHandler<ListProvinceQuery, IEnumerable<ProvinceProjection>>
{
    public async ValueTask<IEnumerable<ProvinceProjection>> Handle(
        ListProvinceQuery request,
        CancellationToken cancellationToken
    )
    {
        return mapper.Map<IEnumerable<ProvinceProjection>>(await regionService.Provinces());
    }
}

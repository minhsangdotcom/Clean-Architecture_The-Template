using Application.Features.Common.Projections.Regions;
using AutoMapper;
using Domain.Aggregates.Regions;

namespace Application.Features.Regions.Queries.List.Provinces;

public class ListProvinceMapping : Profile
{
    public ListProvinceMapping()
    {
        CreateMap<Province, ProvinceProjection>();
        CreateMap<Province, ProvinceDetailProjection>();
    }
}

using Application.UseCases.Projections.Regions;
using AutoMapper;
using Domain.Aggregates.Regions;

namespace Application.UseCases.Regions.Queries.List.Provinces;

public class ListProvinceMapping : Profile
{
    public ListProvinceMapping()
    {
        CreateMap<Province, ProvinceProjection>();
    }
}

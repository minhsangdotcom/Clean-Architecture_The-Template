using Application.Features.Common.Projections.Regions;
using AutoMapper;
using Domain.Aggregates.Regions;

namespace Application.Features.Regions.Queries.List.Communes;

public class ListCommuneMapping : Profile
{
    public ListCommuneMapping()
    {
        CreateMap<Commune, CommuneProjection>();
        CreateMap<Commune, CommuneDetailProjection>();
    }
}

using Application.Features.Common.Projections.Regions;
using Domain.Aggregates.Regions;

namespace Application.Features.Common.Mapping.Regions;

public static class RegionsMapping
{
    public static CommuneProjection ToCommuneProjection(this Commune? commune)
    {
        if (commune == null)
        {
            return null!;
        }
        var response = new CommuneProjection();
        response.MappingFrom(commune);
        return response;
    }

    public static CommuneDetailProjection ToCommuneDetailProjection(this Commune commune)
    {
        if (commune == null)
        {
            return null!;
        }
        var response = new CommuneDetailProjection();
        response.MappingFrom(commune);
        return response;
    }
}

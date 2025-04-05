using Application.Features.Common.Projections.Regions;
using Application.Features.Regions.Queries.List.Districts;
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
        return new()
        {
            Code = commune.Code,
            Name = commune.Name,
            NameEn = commune.NameEn,
            FullName = commune.FullName,
            FullNameEn = commune.FullNameEn,
            CustomName = commune.CustomName,
            Id = commune.Id,
            CreatedAt = commune.CreatedAt,
            CreatedBy = commune.CreatedBy,
            UpdatedAt = commune.UpdatedAt,
            UpdatedBy = commune.UpdatedBy,
        };
    }

    public static CommuneDetailProjection ToCommuneDetailProjection(this Commune commune)
    {
        return new()
        {
            Code = commune.Code,
            Name = commune.Name,
            NameEn = commune.NameEn,
            FullName = commune.FullName,
            FullNameEn = commune.FullNameEn,
            CustomName = commune.CustomName,
            District = commune.District?.ToDistrictProjection(),
            Id = commune.Id,
            CreatedAt = commune.CreatedAt,
            CreatedBy = commune.CreatedBy,
            UpdatedAt = commune.UpdatedAt,
            UpdatedBy = commune.UpdatedBy,
        };
    }
}

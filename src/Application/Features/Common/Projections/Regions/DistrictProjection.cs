using Domain.Aggregates.Regions;

namespace Application.Features.Common.Projections.Regions;

public class DistrictProjection : Region
{
    public virtual void MappingFrom(District district)
    {
        Code = district.Code;
        Name = district.Name;
        EnglishName = district.EnglishName;
        FullName = district.FullName;
        EnglishFullName = district.EnglishFullName;
        CustomName = district.CustomName;

        Id = district.Id;
        CreatedAt = district.CreatedAt;
        CreatedBy = district.CreatedBy;
        UpdatedAt = district.UpdatedAt;
        UpdatedBy = district.UpdatedBy;
    }
}

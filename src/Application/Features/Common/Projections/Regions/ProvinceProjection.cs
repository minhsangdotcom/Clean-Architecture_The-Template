using Domain.Aggregates.Regions;

namespace Application.Features.Common.Projections.Regions;

public class ProvinceProjection : Region
{
    public virtual void MappingFrom(Province province)
    {
        Code = province.Code;
        Name = province.Name;
        NameEn = province.NameEn;
        FullName = province.FullName;
        FullNameEn = province.FullNameEn;
        CustomName = province.CustomName;

        Id = province.Id;
        CreatedAt = province.CreatedAt;
        CreatedBy = province.CreatedBy;
        UpdatedAt = province.UpdatedAt;
        UpdatedBy = province.UpdatedBy;
    }
}

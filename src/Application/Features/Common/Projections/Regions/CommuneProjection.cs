using Domain.Aggregates.Regions;

namespace Application.Features.Common.Projections.Regions;

public class CommuneProjection : Region
{
    public virtual void MappingFrom(Commune commune)
    {
        Id = commune.Id;
        Code = commune.Code;
        Name = commune.Name;
        EnglishName = commune.EnglishName;
        FullName = commune.FullName;
        EnglishFullName = commune.EnglishFullName;
        CustomName = commune.CustomName;
        CreatedAt = commune.CreatedAt;
        CreatedBy = commune.CreatedBy;
        UpdatedAt = commune.UpdatedAt;
        UpdatedBy = commune.UpdatedBy;
    }
}

namespace Application.Features.Common.Projections.Regions;

public class ProvinceDetailProjection : ProvinceProjection
{
    public IEnumerable<DistrictDetailProjection>? Districts { get; set; }
}

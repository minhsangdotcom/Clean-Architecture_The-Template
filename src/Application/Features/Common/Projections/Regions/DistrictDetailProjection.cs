namespace Application.Features.Common.Projections.Regions;

public class DistrictDetailProjection : DistrictProjection
{
    public IEnumerable<CommuneProjection>? Communes { get; set; }
}

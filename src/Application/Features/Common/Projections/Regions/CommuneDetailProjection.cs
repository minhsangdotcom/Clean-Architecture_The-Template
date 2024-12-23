namespace Application.Features.Common.Projections.Regions;

public class CommuneDetailProjection : CommuneProjection
{
    public DistrictProjection? District { get; set; }
}

namespace Application.UseCases.Projections.Regions;

public class CommuneDetailProjection : CommuneProjection
{
    public DistrictProjection? District { get; set; }
}

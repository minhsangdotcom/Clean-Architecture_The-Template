namespace Application.UseCases.Projections.Regions;

public class DistrictDetailProjection : DistrictProjection
{
    public IEnumerable<CommuneProjection>? Communes { get; set; }
}

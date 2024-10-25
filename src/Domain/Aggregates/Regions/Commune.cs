namespace Domain.Aggregates.Regions;

public class Commune : Region
{
    public Ulid DistrictId { get; set; }

    public District? District { get; set; }
}

namespace Domain.Aggregates.Regions;

public class Province : Region
{
    public ICollection<District> Districts { get; set; } = [];
}

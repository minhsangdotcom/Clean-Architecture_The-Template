namespace Domain.Aggregates.Regions;

public class District : Region
{
    public Ulid ProvinceId { get; set; }

    public ICollection<Commune> Communes { get; set; } = [];
}

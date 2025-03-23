using Ardalis.GuardClauses;
using Domain.Aggregates.Regions;
using Domain.Common;

namespace Domain.Aggregates.Users.ValueObjects;

public class Address : ValueObject
{
    public string? Street { get; set; }

    public Province? Province { get; set; }

    public District? District { get; set; }

    public Commune? Commune { get; set; }

    private Address() { }

    public Address(Province province, District district, Commune? commune, string street)
    {
        Street = Guard.Against.NullOrEmpty(street);
        Province = Guard.Against.Null(province);
        District = Guard.Against.Null(district);
        Commune = commune;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street!;
        yield return Province!.Id;
        yield return District!.Id;

        if (Commune != null)
        {
            yield return Commune!.Id;
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is Address other)
        {
            return Street == other.Street
                && Province!.Id == other.Province!.Id
                && District!.Id == other.District!.Id
                && Commune?.Id == other.Commune?.Id;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return string.Join(
            ",",
            [Street, Commune?.Name ?? string.Empty, District!.Name, Province!.Name]
        );
    }
}

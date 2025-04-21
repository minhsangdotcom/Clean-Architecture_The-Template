using Ardalis.GuardClauses;
using SharedKernel.Common;

namespace Domain.Aggregates.Users.ValueObjects;

public class Address : ValueObject
{
    public string? Street { get; set; }

    public Ulid ProvinceId { get; set; }
    public string Province { get; set; } = string.Empty;

    public Ulid DistrictId { get; set; }
    public string District { get; set; } = string.Empty;

    public Ulid? CommuneId { get; set; }
    public string? Commune { get; set; }

    private Address() { }

    public Address(
        string province,
        Ulid provinceId,
        string district,
        Ulid districtId,
        string? commune,
        Ulid? communeId,
        string street
    )
    {
        Province = Guard.Against.NullOrEmpty(province);
        District = Guard.Against.NullOrEmpty(district);
        Street = Guard.Against.NullOrEmpty(street);
        ProvinceId = Guard.Against.Null(provinceId);
        DistrictId = Guard.Against.Null(districtId);
        CommuneId = communeId;
        Commune = commune;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street!;
        yield return ProvinceId;
        yield return DistrictId;

        if (Commune != null)
        {
            yield return CommuneId!;
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is Address other)
        {
            return Street == other.Street
                && ProvinceId == other.ProvinceId
                && DistrictId == other.DistrictId
                && CommuneId == other.CommuneId;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        string? commune = $"{Commune}," ?? null;
        return $"{Street},{commune}{District},{Province}";
    }
}

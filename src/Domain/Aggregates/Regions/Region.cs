using Domain.Common;

namespace Domain.Aggregates.Regions;

public class Region : DefaultEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string NameEn { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string FullNameEn { get; set; } = string.Empty;

    public string? CustomName { get; set; }
}
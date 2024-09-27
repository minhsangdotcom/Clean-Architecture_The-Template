namespace Contracts.Dtos.Models;

public interface IBaseAuditable
{
    public string CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}

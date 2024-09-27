namespace Contracts.Dtos.Models;

public class BaseResponse : DefaultBaseResponse, IBaseAuditable
{
    public string CreatedBy { get; set; } = string.Empty;

    public string? UpdatedBy { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}

public class DefaultBaseResponse
{
    public Ulid Id { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }
}

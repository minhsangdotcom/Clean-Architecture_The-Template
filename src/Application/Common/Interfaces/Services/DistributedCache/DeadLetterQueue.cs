using Domain.Common;
namespace Application.Common.Interfaces.Services.DistributedCache;

public class DeadLetterQueue : BaseEntity
{
    public Guid RequestId { get; set; }
    public object? ErrorDetail { get; set; }
    public int RetryCount { get; set; }
}
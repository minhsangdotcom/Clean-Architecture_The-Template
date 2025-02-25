using Domain.Common;

namespace Domain.Aggregates.QueueLogs;

public class QueueLog : BaseEntity
{
    public Guid RequestId { get; set; }
    public object? Request { get; set; }
    public object? ErrorDetail { get; set; }
    public int RetryCount { get; set; }
}

using SharedKernel.Common;

namespace Domain.Aggregates.QueueLogs;

public class QueueLog : BaseEntity
{
    public Guid RequestId { get; set; }
    public object? Request { get; set; }
    public object? ErrorDetail { get; set; }
    public QueueType ProcessedBy { get; set; } = QueueType.OriginQueue;
    public int RetryCount { get; set; }
}

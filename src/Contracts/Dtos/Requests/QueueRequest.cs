namespace Contracts.Dtos.Requests;

public class QueueRequest<T>
{
    public Guid PayloadId { get; set; }

    public T? Payload { get; set; }
}

public enum PurchaseStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
}

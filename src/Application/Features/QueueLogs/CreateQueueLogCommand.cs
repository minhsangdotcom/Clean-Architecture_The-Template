using Mediator;

namespace Application.Features.QueueLogs;

public class CreateQueueLogCommand : IRequest
{
    public Guid RequestId { get; set; }
    public object? Request { get; set; }
    public object? ErrorDetail { get; set; }
    public int RetryCount { get; set; }
}

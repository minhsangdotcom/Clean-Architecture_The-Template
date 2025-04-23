using Domain.Aggregates.QueueLogs;

namespace Application.Features.QueueLogs;

public static class CreateQueueLogMapping
{
    public static QueueLog ToQueueLog(this CreateQueueLogCommand queueLogCommand)
    {
        return new()
        {
            RequestId = queueLogCommand.RequestId,
            Request = queueLogCommand.Request,
            ErrorDetail = queueLogCommand.ErrorDetail,
            RetryCount = queueLogCommand.RetryCount,
        };
    }
}

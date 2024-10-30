using Application.Common.Interfaces.Services.DistributedCache;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Contracts.Extensions;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services.DistributedCache;

public class QueueBackgroundService(IQueueService queueService) : BackgroundService
{
    private const int MAXIMUM_RETRY = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        long length = queueService.Size;
        while (length > 0)
        {
            QueueRequest<int>? payload = await queueService.DequeueAsync<int>();
            await Process(payload!);

            length = queueService.Size;
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private static async Task Process(QueueRequest<int> payload)
    {
        QueueResponse<int> result;
        int retry = 0;
        int retryTimeInSec = 5;
        do
        {
            result = await FakeTask(payload!.Payload, payload.PayloadId);

            if (result.IsSuccess)
            {
                break;
            }

            if (result.ErrorType == QueueErrorType.Persistent)
            {
                // put it into dead letter queue for reviewing or logging into db
                break;
            }

            if (result.ErrorType == QueueErrorType.Transient && retry == 0)
            {
                retry = MAXIMUM_RETRY;
                continue;
            }

            retry--;
            result.RetryCount = MAXIMUM_RETRY - retry;
            retryTimeInSec *= 2;

            await Task.Delay(TimeSpan.FromSeconds(retryTimeInSec));
        } while (retry > 0);
        if (!result.IsSuccess && result.ErrorType == QueueErrorType.Transient)
        {
            // put it into dead letter queue for reviewing or logging into db
        }
        Console.WriteLine(SerializerExtension.Serialize(result).StringJson);
    }

    private static async Task<QueueResponse<int>> FakeTask(int request, Guid payloadId) =>
        await Task.Run(() =>
        {
            if (request < 0)
            {
                return new QueueResponse<int>()
                {
                    IsSuccess = true,
                    PayloadId = payloadId,
                    LastAttemptTime = DateTimeOffset.UtcNow,
                    Error = new { Message = "error" },
                };
            }
            int data = 10 + request;

            return new QueueResponse<int>()
            {
                IsSuccess = true,
                PayloadId = payloadId,
                ResponseData = data,
            };
        });
}

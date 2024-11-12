using Application.Common.Interfaces.Services.DistributedCache;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services.DistributedCache;

public class QueueBackgroundService(IQueueService queueService) : BackgroundService
{
    private const int MAXIMUM_RETRY = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     QueueRequest<int>? payload = await queueService.DequeueAsync<int>();

        //     if (payload != null)
        //     {
        //     }

        //     await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        // }
        await Task.CompletedTask;
    }

    private static async Task Process<TRequest, TResponse>(
        QueueRequest<TRequest> request,
        Func<QueueRequest<TRequest>, Task<QueueResponse<TResponse>>> task
    )
    {
        QueueResponse<TResponse> queueResponse;
        int retry = 0;
        int retryTimeInSec = 5;
        do
        {
            queueResponse = await task(request);

            if (queueResponse.IsSuccess)
            {
                //log
                break;
            }

            if (queueResponse.ErrorType == QueueErrorType.Persistent)
            {
                //logging into db
                break;
            }

            if (queueResponse.ErrorType == QueueErrorType.Transient && retry == 0)
            {
                retry = MAXIMUM_RETRY;
                continue;
            }

            retry--;
            queueResponse.RetryCount = MAXIMUM_RETRY - retry;
            retryTimeInSec += 1;

            await Task.Delay(TimeSpan.FromSeconds(retryTimeInSec));
        } while (retry > 0);
        
        if (!queueResponse.IsSuccess && queueResponse.ErrorType == QueueErrorType.Transient)
        {
            //logging into db
        }
    }
}

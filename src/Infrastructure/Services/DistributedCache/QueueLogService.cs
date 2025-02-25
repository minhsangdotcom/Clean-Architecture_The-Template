using Application.Common.Interfaces.Services.DistributedCache;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Dtos.Responses;
using Domain.Aggregates.QueueLogs;
using Serilog;

namespace Infrastructure.Services.DistributedCache;

public class QueueLogService(ILogger logger, IUnitOfWork unitOfWork) : IQueueLogService
{
    public async Task CreateAsync<TRequest, TResponse>(
        QueueResponse<TResponse> response,
        TRequest request,
        QueueType? type = null
    )
        where TRequest : class
        where TResponse : class
    {
        logger.Information("Pushing request {payloadId} to logging queue.", response.PayloadId);
        var log = new QueueLog()
        {
            RequestId = response.PayloadId!.Value,
            ErrorDetail = response.Error,
            RetryCount = response.RetryCount,
            Request = request,
        };

        if (type != null)
        {
            log.ProcessedBy = type!.Value;
        }
        await unitOfWork.Repository<QueueLog>().AddAsync(log);
        await unitOfWork.SaveAsync();
    }
}

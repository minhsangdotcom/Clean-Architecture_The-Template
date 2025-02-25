using Application.Common.Interfaces.Registers;
using Contracts.Dtos.Responses;
using Domain.Aggregates.QueueLogs;

namespace Application.Common.Interfaces.Services.DistributedCache;

public interface IQueueLogService : IScope
{
    public Task CreateAsync<TRequest, TResponse>(
        QueueResponse<TResponse> response,
        TRequest request,
        QueueType? type = null
    )
        where TRequest : class
        where TResponse : class;
}

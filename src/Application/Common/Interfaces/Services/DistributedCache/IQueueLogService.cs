using Application.Common.Interfaces.Registers;
using Contracts.Dtos.Responses;

namespace Application.Common.Interfaces.Services.DistributedCache;

public interface IQueueLogService : IScope
{
    public Task CreateAsync<TRequest, TResponse>(
        QueueResponse<TResponse> response,
        TRequest request
    )
        where TRequest : class
        where TResponse : class;
}

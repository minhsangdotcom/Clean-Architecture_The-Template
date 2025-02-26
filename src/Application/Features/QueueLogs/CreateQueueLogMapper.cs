using AutoMapper;
using Domain.Aggregates.QueueLogs;

namespace Application.Features.QueueLogs;

public class CreateQueueLogMapper : Profile
{
    public CreateQueueLogMapper()
    {
        CreateMap<CreateQueueLogCommand, QueueLog>();
    }
}

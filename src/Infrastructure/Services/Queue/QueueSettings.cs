namespace Infrastructure.Services.Queue;

public class QueueSettings
{
    public string? OriginQueueName { get; set; }
    public string? DeadLetterQueueName { get; set; }
    public int MaxRetryAttempts { get; set; }
    public int DeadLetterMaxRetryAttempts { get; set; }
    public int MaximumDelayInSec { get; set; } = 90;
}

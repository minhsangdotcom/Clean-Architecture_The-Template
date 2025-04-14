namespace Infrastructure.Services.Hangfire;

public class HangfirePostgreStorageOptions
{
    public TimeSpan CommandBatchMaxTimeout { get; set; }

    public TimeSpan QueuePollIntervals { get; set; }

    public TimeSpan SlidingInvisibilityTimeout { get; set; }

    public bool UseRecommendedIsolationLevel { get; set; }

    public bool DisableGlobalLocks { get; set; }
}

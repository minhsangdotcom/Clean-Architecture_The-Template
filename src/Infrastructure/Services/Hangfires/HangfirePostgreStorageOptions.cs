namespace Infrastructure.Services.Hangfires;

public class HangfirePostgreStorageOptions
{
    public TimeSpan CommandBatchMaxTimeout { get; set; }

    public TimeSpan QueuePollIntervals { get; set; }

    public TimeSpan SlidingInvisibilityTimeout { get; set; }

    public bool UseRecommendedIsolationLevel { get; set; }

    public bool DisableGlobalLocks { get; set; }
}

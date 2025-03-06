namespace Infrastructure.Services.DistributedCache;

public static class QueueExtention
{
    public const int INIT_DELAY = 2;
    public const double MAXIMUM_JITTER = 0.2;

    public static double GenerateJitter(double minFactor, double maxFactor)
    {
        Random random = new();
        return minFactor + ((maxFactor - minFactor) * random.NextDouble());
    }
}

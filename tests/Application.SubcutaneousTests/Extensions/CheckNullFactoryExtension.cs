namespace Application.SubcutaneousTests.Extensions;

public static class CheckNullFactoryExtension
{
    public static void ThrowIfNull<T>(this CustomWebApplicationFactory<T>? factory)
        where T : class
    {
        if (factory == null)
        {
            throw new NullReferenceException("factory is null");
        }
    }
}

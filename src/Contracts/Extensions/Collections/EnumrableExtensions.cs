namespace Contracts.Extensions.Collections;

public static class EnumrableExtensions
{
    public static List<T> CastToList<T>(this IEnumerable<T> source)
    {
        if (source != null && source is not List<T>)
        {
            return [.. source];
        }

        return (List<T>)source!;
    }
}

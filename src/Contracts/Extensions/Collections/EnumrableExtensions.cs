namespace Contracts.Extensions.Collections;

public static class EnumrableExtensions
{
    public static List<T> Cast2List<T>(this IEnumerable<T> source)
    {
        try
        {
            List<T>? list = source as List<T>;
            return list!;
        }
        catch (Exception)
        {
            return source.ToList();
        }
    }
}

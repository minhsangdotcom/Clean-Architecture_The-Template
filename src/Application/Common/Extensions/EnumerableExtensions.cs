namespace Application.Common.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Returns the source as a List&lt;T&gt;.
    /// If it's already a List&lt;T&gt;, no new list is created.
    /// Otherwise, it materializes the sequence into a new List&lt;T&gt;.
    /// </summary>
    public static List<T> ToListIfNot<T>(this IEnumerable<T> source)
    {
        if (source is List<T> list)
        {
            return list;
        }
        return [..source];
    }
}
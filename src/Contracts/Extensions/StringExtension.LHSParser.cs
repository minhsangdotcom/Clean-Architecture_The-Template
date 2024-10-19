using System.Dynamic;
using Contracts.Dtos.Requests;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Contracts.Extensions;

public partial class StringExtension
{
    public static object? Parse(string[] queries)
    {
        if (queries == null || queries.Length == 0)
        {
            return null;
        }

        dynamic filters = new ExpandoObject();
        foreach (QueryResult query in TransformStringQuery(queries))
        {
            List<string> cleanKey = query.CleanKey;
            string value = query.Value;

            dynamic leaf = ParseKey(cleanKey, value);
            filters = Merge(filters, leaf);
        }

        return filters;
    }

    static dynamic ParseKey(List<string> keys, string value)
    {
        dynamic leaf = IsDigit(value) ? long.Parse(value) : value;
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            dynamic obj = new ExpandoObject();
            string key = keys[i];
            if (IsDigit(key))
            {
                int index = int.Parse(key);
                object[] element = new object[index + 1];
                element[index] = leaf;
                obj = element;
            }
            else
            {
                ((IDictionary<string, object>)obj)[key] = leaf;
            }
            leaf = obj;
        }
        return leaf;
    }

    static dynamic Merge(dynamic root, dynamic leaf)
    {
        var rootObject = (IDictionary<string, object>)root;
        var leafObject = (IDictionary<string, object>)leaf;
        var prop = leafObject.ElementAt(0);

        if (!rootObject.Any() || !rootObject.ContainsKey(prop.Key))
        {
            rootObject.Add(new(prop.Key, prop.Value));
            return rootObject;
        }

        for (int i = 0; i < rootObject.Count; i++)
        {
            var property = rootObject.ElementAt(i);
            if (leafObject.TryGetValue(property.Key, out object? value))
            {
                Type type = property.Value.GetType();
                Type leafValueType = value.GetType();

                if (type != leafValueType)
                {
                    continue;
                }

                if (typeof(IEnumerable<object>).IsAssignableFrom(type))
                {
                    object[] values = ((IEnumerable<object>)value).ToArray();
                    object[] rootValues = ((IEnumerable<object>)property.Value).ToArray();
                    object[] results = new object[values.Count(x => x != null) + rootValues.Length];
                    ConcatArray(rootValues, values, ref results);
                    rootObject[property.Key] = results.Where(x => x != null).ToArray();
                }
                else
                {
                    var valueRoot = (IDictionary<string, object>)property.Value;
                    var Valueleaf = (IDictionary<string, object>)value;
                    rootObject[property.Key] = Merge(valueRoot, Valueleaf);
                }
            }
        }

        return rootObject;
    }

    static void ConcatArray(object[] root, object[] leaf, ref object[] results)
    {
        for (int i = 0; i < leaf.Length; i++)
        {
            object leafValue = leaf[i];

            if (leafValue == null)
            {
                continue;
            }
            results[i] = leafValue;
        }

        for (int i = 0; i < root.Length; i++)
        {
            object rootValue = root[i];

            if (rootValue == null)
            {
                continue;
            }

            if (results[i] != null)
            {
                results[i] = Merge(results[i], rootValue);
            }
            else
            {
                results[i] = rootValue;
            }
        }
    }

    public static bool IsDigit(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return false;
        }

        foreach (var c in str)
        {
            if (c is < '0' or > '9')
                return false;
        }
        return true;
    }

    public static IEnumerable<QueryResult> TransformStringQuery(string[] queries)
    {
        return queries.Select(query =>
        {
            Dictionary<string, StringValues> queryStrings = QueryHelpers.ParseQuery(query);
            KeyValuePair<string, StringValues> queryString = queryStrings.ElementAt(0);

            string[] keyList = queryString
                .Key.Trim()
                .Replace(
                    nameof(QueryParamRequest.Filter),
                    string.Empty,
                    StringComparison.OrdinalIgnoreCase
                )
                .Split("]", StringSplitOptions.RemoveEmptyEntries);
            List<string> cleanKey = keyList.Select(x => x.Replace("[", string.Empty)).ToList();
            string value = queryString.Value.ToString();

            return new QueryResult(cleanKey, value);
        });
    }
}

public record QueryResult(List<string> CleanKey, string Value);

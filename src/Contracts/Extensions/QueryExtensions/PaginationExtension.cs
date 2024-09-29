using System.Linq.Expressions;
using System.Reflection;
using Ardalis.GuardClauses;
using Contracts.Dtos.Models;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Contracts.Extensions.Encryption;
using Contracts.Extensions.Expressions;
using Contracts.Extensions.Reflections;
using Microsoft.EntityFrameworkCore;

namespace Contracts.Extensions.QueryExtensions;

public static class PaginationExtension
{
    /// <summary>
    /// offset pagination for IQueryable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entities"></param>
    /// <param name="current"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static async Task<PaginationResponse<T>> ToPagedListAsync<T>(
        this IQueryable<T> query,
        int current,
        int size
    )
    {
        int totalPage = query.Count();

        return new PaginationResponse<T>(
            await query.Skip((current - 1) * size).Take(size).ToListAsync(),
            totalPage,
            current,
            size
        );
    }

    /// <summary>
    /// offset pagination for IEnumerable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entities"></param>
    /// <param name="current"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static PaginationResponse<T> ToPagedList<T>(
        this IEnumerable<T> query,
        int current,
        int size
    ) => new(query.Skip((current - 1) * size).Take(size), query.Count(), current, size);

    /// <summary>
    /// Cursor pagination
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public static async Task<PaginationResponse<T>> ToCursorPagedListAsync<T>(
        this IQueryable<T> query,
        CursorPaginationRequest request
    )
    {
        string sort = string.IsNullOrWhiteSpace(request.Sort)
            ? $"{request.UniqueSort}"
            : $"{request.Sort},{request.UniqueSort}";

        int totalPage = await query.CountAsync();
        if (totalPage == 0)
        {
            return new PaginationResponse<T>(query, totalPage, request.Size);
        }

        T? first = await query.FirstOrDefaultAsync();
        T? last = await query.LastOrDefaultAsync();

        var result = await CusorPaginateAsync(
            new PaginationPayload<T>(
                query,
                request.Before,
                request.After,
                first!,
                last!,
                sort,
                request.Size
            )
        );

        return new PaginationResponse<T>(
            result.Data,
            totalPage,
            request.Size,
            result.Pre,
            result.Next
        );
    }

    /// <summary>
    /// do cursor paging
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="payload"></param>
    /// <returns></returns>
    private static async Task<PaginationResult<T>> CusorPaginateAsync<T>(
        PaginationPayload<T> payload
    )
    {
        bool isFirstMove =
            string.IsNullOrWhiteSpace(payload.Prev) && string.IsNullOrWhiteSpace(payload.Next);

        if (isFirstMove)
        {
            IQueryable<T> list = payload.Query.Take(payload.Size).Sort(payload.Sort);
            T? theLast = await list.LastAsync();

            string? cursor = GenerateCursor(theLast, payload.Sort);

            return new PaginationResult<T>(list, cursor);
        }

        IQueryable<T> results = null!;

        if (!string.IsNullOrWhiteSpace(payload.Next))
        {
            Dictionary<string, object?>? cursorObject = DecodeCursor(payload.Next);
            results = GetForwardAndBackwardAsync(payload.Query, cursorObject!, payload.Sort)
                .Take(payload.Size)
                .Sort(payload.Sort);
        }

        if (!string.IsNullOrWhiteSpace(payload.Prev))
        {
            Dictionary<string, object?>? cursorObject = DecodeCursor(payload.Prev);
            string sort = ReverseSortOrder(payload.Sort);
            IQueryable<T> list = GetForwardAndBackwardAsync(payload.Query, cursorObject!, sort)
                .Take(payload.Size)
                .Sort(sort);
            results = list.Sort(payload.Sort);
        }

        T? last = await results.LastOrDefaultAsync();
        T? first = await results.FirstOrDefaultAsync();

        string? nextCursor = CompareToTheflag(last, payload.Last)
            ? null
            : GenerateCursor(last, payload.Sort);
        string? preCursor = CompareToTheflag(first, payload.Fist)
            ? null
            : GenerateCursor(first, payload.Sort);

        return new PaginationResult<T>(results, nextCursor, preCursor);
    }

    private static string ReverseSortOrder(string input)
    {
        // Split the input by comma to separate each field
        var fields = input.Split(',');

        // Process each field
        for (int i = 0; i < fields.Length; i++)
        {
            var parts = fields[i].Split(OrderTerm.DELIMITER);
            string fieldName = parts[0]; // The actual field name
            string sortOrder = parts.Length > 1 ? parts[1] : OrderTerm.ASC; // Default to asc if no order specified

            // Reverse the sort order
            if (sortOrder == OrderTerm.ASC)
            {
                sortOrder = OrderTerm.DESC;
            }
            else
            {
                sortOrder = OrderTerm.ASC;
            }

            // Rebuild the field with the new sort order
            fields[i] = $"{fieldName}:{sortOrder}";
        }

        // Join the fields back into a single string and return
        return string.Join(",", fields);
    }

    /// <summary>
    /// make sure that whether we have next or previous move
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cursor"></param>
    /// <param name="destination"></param>
    /// <returns>true we're not gonna move</returns>
    private static bool CompareToTheflag<T>(T cursor, T destination)
    {
        var cursorProperty = typeof(T).GetNestedPropertyValue(
            nameof(DefaultBaseResponse.Id),
            cursor!
        );
        var desProperty = typeof(T).GetNestedPropertyValue(
            nameof(DefaultBaseResponse.Id),
            destination!
        );

        return cursorProperty == desProperty;
    }

    private static Dictionary<string, object?>? DecodeCursor(string cursor)
    {
        string stringCursor = AesEncryptionUtility.Decrypt(cursor, EncryptKey());
        var serializeResult = SerializerExtension.Deserialize<Dictionary<string, object?>>(
            stringCursor
        );
        return serializeResult.Object;
    }

    private static string? GenerateCursor<T>(T? entity, string sort)
    {
        if (entity == null)
        {
            return null;
        }

        Dictionary<string, object?> properties = GetEncryptionProperties(entity, sort);
        SerializeResult serialize = SerializerExtension.Serialize(properties);
        return AesEncryptionUtility.Encrypt(serialize.StringJson, EncryptKey());
    }

    /// <summary>
    /// move forward and backward
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="cursors">consist of property name and it's value in cursor</param>
    /// <param name="sort"></param>
    /// <returns></returns>
    private static IQueryable<T> GetForwardAndBackwardAsync<T>(
        IQueryable<T> query,
        Dictionary<string, object?>? cursors,
        string sort
    )
    {
        List<KeyValuePair<string, string>> sorts = TransformSort(sort);

        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        Expression? body = null;
        for (int i = 0; i < sorts.Count; i++)
        {
            KeyValuePair<string, string> sortField = sorts[i];

            //* x."property"
            Expression expressionMember = ExpressionExtension.GetExpressionMember(
                sortField.Key,
                parameter,
                false,
                typeof(T)
            );

            PropertyInfo propertyInfo = typeof(T).GetNestedPropertyInfo(sortField.Key);
            object? value = cursors?.GetValueOrDefault(sortField.Key);

            BinaryExpression binaryExpression = BuildInnerExpression<T>(
                propertyInfo.PropertyType,
                (MemberExpression)expressionMember,
                value!,
                i,
                sortField.Value
            );

            //* outer query
            body = body == null ? body : Expression.OrElse(body, binaryExpression);
        }

        //* x => x.Age < AgeValue ||
        //*     (x.Age == AgeValue && x.Aname > NameValue) ||
        //*     (x.Id > IdValue)
        var lamda = Expression.Lambda<Func<T, bool>>(body!, parameter);
        return query.Where(lamda);
    }

    /// <summary>
    /// Build inner query like (x.Age == AgeValue && x.Aname > NameValue)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyType"></param>
    /// <param name="member"></param>
    /// <param name="value"></param>
    /// <param name="index"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    private static BinaryExpression BuildInnerExpression<T>(
        Type propertyType,
        MemberExpression member,
        object value,
        int index,
        string order
    )
    {
        BinaryExpression? body = null!;
        for (int i = 0; i <= index; i++)
        {
            BinaryExpression operation = true switch
            {
                bool when propertyType == typeof(string) => order == OrderTerm.DESC
                    ? Expression.LessThan(
                        StringCompareExpression(member, Expression.Constant(value)),
                        Expression.Constant(0)
                    )
                    : Expression.GreaterThan(
                        StringCompareExpression(member, Expression.Constant(value)),
                        Expression.Constant(0)
                    ),

                _ => order == OrderTerm.DESC
                    ? Expression.LessThan(member, Expression.Constant(value))
                    : Expression.GreaterThan(member, Expression.Constant(value)),
            };

            body = body == null ? body : Expression.AndAlso(body, operation);
        }

        return body!;
    }

    private static MethodCallExpression StringCompareExpression(Expression left, Expression right)
    {
        return Expression.Call(typeof(string), nameof(string.Compare), null, [left, right]);
    }

    /// <summary>
    /// Get all of properties that we need to put them into cursor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <param name="sort"></param>
    /// <returns></returns>
    private static Dictionary<string, object?> GetEncryptionProperties<T>(T entity, string sort)
    {
        Dictionary<string, object?> properties = [];
        List<string> sortFields = SortFields(sort);
        foreach (string field in sortFields)
        {
            if (typeof(T).IsNestedPropertyValid(field))
            {
                throw new NotFoundException(nameof(field), field);
            }

            object? value = typeof(T).GetNestedPropertyValue(field, entity!);
            properties.Add(field, value);
        }

        return properties;
    }

    /// <summary>
    /// turn string sort to easy form
    /// </summary>
    /// <param name="sort"></param>
    /// <returns></returns>
    private static List<KeyValuePair<string, string>> TransformSort(string sort)
    {
        string[] fields = sort.Trim().Split(",", StringSplitOptions.TrimEntries);

        return fields
            .Select(field =>
            {
                string[] orderFields = field.Split(OrderTerm.DELIMITER);

                if (orderFields.Length == 1)
                {
                    return new KeyValuePair<string, string>(orderFields[0], OrderTerm.ASC);
                }
                return new KeyValuePair<string, string>(orderFields[0], orderFields[1]);
            })
            .ToList();
    }

    /// <summary>
    /// get all of fiels of string sort
    /// </summary>
    /// <param name="sort">string sort</param>
    /// <returns></returns>
    private static List<string> SortFields(string sort)
    {
        return TransformSort(sort).Select(field => field.Key).ToList();
    }

    /// <summary>
    /// Encrypt aes key
    /// </summary>
    /// <returns></returns>
    private static string EncryptKey() => "+%9d$t}L76?Zh2TtNcNR,DNy&a6/W9";
}

internal record PaginationPayload<T>(
    IQueryable<T> Query,
    string? Prev,
    string? Next,
    T Fist,
    T Last,
    string Sort,
    int Size
);

internal record PaginationResult<T>(IEnumerable<T> Data, string? Next = null, string? Pre = null);

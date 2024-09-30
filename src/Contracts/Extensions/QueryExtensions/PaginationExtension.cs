using System.Linq.Expressions;
using System.Reflection;
using Ardalis.GuardClauses;
using Contracts.Dtos.Models;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Contracts.Extensions.Encryption;
using Contracts.Extensions.Expressions;
using Contracts.Extensions.Reflections;
using Cysharp.Serialization.Json;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

        IQueryable<T> sortedQuery = query.Sort(sort);

        T? first = await sortedQuery.FirstOrDefaultAsync();
        T? last = await sortedQuery.LastOrDefaultAsync();

        PaginationResult<T> result = await CusorPaginateAsync(
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
            IEnumerable<T> list = await payload.Query.Take(payload.Size).ToListAsync();
            T? theLast = list.Last();

            string? cursor = GenerateCursor(theLast, payload.Sort);
            return new PaginationResult<T>(list, cursor);
        }

        IEnumerable<T> results = [];

        if (!string.IsNullOrWhiteSpace(payload.Next))
        {
            Dictionary<string, object?>? cursorObject = DecodeCursor(payload.Next);
            results = await MoveForwardOrBackwardAsync(payload.Query, cursorObject!, payload.Sort)
                .Take(payload.Size)
                .ToListAsync();
        }

        if (!string.IsNullOrWhiteSpace(payload.Prev))
        {
            Dictionary<string, object?>? cursorObject = DecodeCursor(payload.Prev);
            string sort = ReverseSortOrder(payload.Sort);
            IQueryable<T> list = MoveForwardOrBackwardAsync(payload.Query, cursorObject!, sort)
                .Take(payload.Size)
                .Sort(sort);
            results = await list.Sort(payload.Sort).ToListAsync();
        }

        T? last = results.LastOrDefault();
        T? first = results.FirstOrDefault();

        string? nextCursor = CompareToTheflag(last, payload.Last)
            ? null
            : GenerateCursor(last, payload.Sort);
        string? preCursor = CompareToTheflag(first, payload.Fist)
            ? null
            : GenerateCursor(first, payload.Sort);

        return new PaginationResult<T>(results, nextCursor, preCursor);
    }

    /// <summary>
    /// reverse sort to move backward
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
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
        var serializeResult = JsonConvert.DeserializeObject<Dictionary<string, object?>>(
            stringCursor
        );
        return serializeResult;
    }

    private static string? GenerateCursor<T>(T? entity, string sort)
    {
        if (entity == null)
        {
            return null;
        }

        Dictionary<string, object?> properties = GetEncryptionProperties(entity, sort);
        string serialize = JsonConvert.SerializeObject(properties, Formatting.Indented);
        return AesEncryptionUtility.Encrypt(serialize, EncryptKey());
    }

    /// <summary>
    /// move forward or backward <- | ->
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="cursors">consist of property name and it's value in cursor</param>
    /// <param name="sort"></param>
    /// <returns></returns>
    private static IQueryable<T> MoveForwardOrBackwardAsync<T>(
        IQueryable<T> query,
        Dictionary<string, object?>? cursors,
        string sort
    )
    {
        List<KeyValuePair<string, string>> sorts = TransformSort(sort);

        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        Expression? body = null;

        List<KeyValuePair<MemberExpression, object?>> CompararisonValues = [];
        for (int i = 0; i < sorts.Count; i++)
        {
            KeyValuePair<string, string> sortField = sorts[i];
            string order = sortField.Value;
            string propertyName = sortField.Key;

            //* x."property"
            Expression expressionMember = ExpressionExtension.GetExpressionMember(
                propertyName,
                parameter,
                false,
                typeof(T)
            );
            object? value = cursors?.GetValueOrDefault(propertyName);
            CompararisonValues.Add(new((MemberExpression)expressionMember, value));

            BinaryExpression andClause = BuildAndClause<T>(
                i,
                CompararisonValues,
                propertyName,
                order
            );

            body = body == null ? andClause : Expression.OrElse(body, andClause);
        }

        //* x => x.Age < AgeValue ||
        //*     (x.Age == AgeValue && x.Aname > NameValue) ||
        //*     (x.Age == AgeValue && x.Aname == NameValue && x.Id > IdValue)
        var lamda = Expression.Lambda<Func<T, bool>>(body!, parameter);
        return query.Where(lamda);
    }

    /// <summary>
    /// build and clause (x.Age == AgeValue && x.Aname > NameValue)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <param name="CompararisonValues"></param>
    /// <param name="propertyName"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    private static BinaryExpression BuildAndClause<T>(
        int index,
        List<KeyValuePair<MemberExpression, object?>> CompararisonValues,
        string propertyName,
        string order
    )
    {
        BinaryExpression? innerExpression = BuildEqualOperationClause(index, CompararisonValues);

        PropertyInfo propertyInfo = typeof(T).GetNestedPropertyInfo(propertyName);
        Expression expressionMember = CompararisonValues[index].Key;
        object? value = CompararisonValues[index].Value;

        BinaryExpression binaryExpression = BuildCompareOperation(
            propertyInfo.PropertyType,
            (MemberExpression)expressionMember,
            value,
            order
        );

        return innerExpression == null
            ? binaryExpression
            : Expression.AndAlso(innerExpression, binaryExpression);
    }

    private static BinaryExpression BuildCompareOperation(
        Type type,
        MemberExpression member,
        object? value,
        string order
    )
    {
        if (type == typeof(Ulid))
        {
            MethodCallExpression compareExpression = UlidCompareExpression(member, value);

            return order == OrderTerm.DESC
                ? Expression.LessThan(compareExpression, Expression.Constant(0))
                : Expression.GreaterThan(compareExpression, Expression.Constant(0));
        }

        if (type == typeof(string))
        {
            ConstantExpression comparisonValue = Expression.Constant(0);
            ConstantExpression constantValue = Expression.Constant(value);
            MethodCallExpression comparisonExpression = StringCompareExpression(
                member,
                constantValue
            );

            return order == OrderTerm.DESC
                ? Expression.LessThan(comparisonExpression, comparisonValue)
                : Expression.GreaterThan(comparisonExpression, comparisonValue);
        }

        return order == OrderTerm.DESC
            ? Expression.LessThan(member, Expression.Constant(value))
            : Expression.GreaterThan(member, Expression.Constant(value));
    }

    /// <summary>
    /// Build equal query like (x.Age == AgeValue && .....)
    /// </summary>
    /// <param name="index"></param>
    /// <param name="CompararisonValues"></param>
    /// <returns></returns>
    private static BinaryExpression? BuildEqualOperationClause(
        int index,
        List<KeyValuePair<MemberExpression, object?>> CompararisonValues
    )
    {
        BinaryExpression? body = null;
        for (int i = 0; i < index; i++)
        {
            var compararisonValue = CompararisonValues[i];
            BinaryExpression operation;

            //! check tomorrow
            if (compararisonValue.Key.GetExpressionType() == typeof(Ulid))
            {
                MethodCallExpression compararison = UlidCompareExpression(
                    compararisonValue.Key,
                    compararisonValue.Value
                );
                operation = Expression.Equal(compararison, Expression.Constant(0));
            }
            else
            {
                operation = Expression.Equal(
                    compararisonValue.Key,
                    Expression.Constant(compararisonValue.Value)
                );
            }

            body = body == null ? operation : Expression.AndAlso(body, operation);
        }
        return body;
    }

    private static MethodCallExpression UlidCompareExpression(Expression left, object? value)
    {
        Ulid compararisonValue = value == null ? Ulid.Empty : Ulid.Parse(value!.ToString());
        MethodInfo? compareToMethod = typeof(Ulid).GetMethod(
            nameof(Ulid.CompareTo),
            [typeof(Ulid)]
        );

        return Expression.Call(left, compareToMethod!, Expression.Constant(compararisonValue));
    }

    private static MethodCallExpression StringCompareExpression(
        Expression left,
        Expression right
    ) => Expression.Call(typeof(string), nameof(string.Compare), null, [left, right]);

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
            if (!typeof(T).IsNestedPropertyValid(field))
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

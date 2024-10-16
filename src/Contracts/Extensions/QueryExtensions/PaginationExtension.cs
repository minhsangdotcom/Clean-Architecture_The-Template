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

        bool IsPreviousMove = !string.IsNullOrWhiteSpace(request.Before);
        string originalSort = sort;
        if (IsPreviousMove)
        {
            sort = ReverseSortOrder(sort);
        }

        IQueryable<T> sortedQuery = query.Sort(sort);

        T? first = await sortedQuery.FirstOrDefaultAsync();
        T? last = await sortedQuery.LastOrDefaultAsync();

        PaginationResult<T> result = await CusorPaginateAsync(
            new PaginationPayload<T>(
                sortedQuery,
                request.After ?? request.Before,
                IsPreviousMove,
                first!,
                last!,
                originalSort,
                sort,
                request.Size
            )
        );

        return new PaginationResponse<T>(
            result.Data,
            totalPage,
            result.PageSize,
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
        bool isFirstMove = string.IsNullOrWhiteSpace(payload.Cursor);
        if (isFirstMove)
        {
            IEnumerable<T> list = await payload.Query.Take(payload.Size).ToListAsync();
            T? theLast = list.Last();

            string? cursor = EncodeCursor(theLast, payload.Sort);
            return new PaginationResult<T>(list, list.Count(), cursor);
        }

        IQueryable<T> results = Enumerable.Empty<T>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(payload.Cursor))
        {
            Dictionary<string, object?>? cursorObject = DecodeCursor(payload.Cursor);
            IQueryable<T> list = MoveForwardOrBackwardAsync(
                    payload.Query,
                    cursorObject!,
                    payload.Sort
                )
                .Take(payload.Size);

            results = payload.IsPrevious ? list.Sort(payload.OriginalSort) : list;
        }

        T? last = await results.LastOrDefaultAsync();
        T? first = await results.FirstOrDefaultAsync();
        int count = await results.CountAsync();

        // whether or not we're currently at first or last page
        string? nextCursor =
            (
                count < payload.Size
                || CompareToTheflag(last, payload.IsPrevious ? payload.First : payload.Last)
            ) && !payload.IsPrevious
                ? null
                : EncodeCursor(last, payload.Sort);
        string? preCursor =
            (
                count < payload.Size
                || CompareToTheflag(first, payload.IsPrevious ? payload.Last : payload.First)
            ) && payload.IsPrevious
                ? null
                : EncodeCursor(first, payload.Sort);

        return new PaginationResult<T>(results, count, nextCursor, preCursor);
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
            KeyValuePair<string, string> orderField = sorts[i];
            string order = orderField.Value;
            string field = orderField.Key;

            //* x."property"
            Expression expressionMember = parameter.MemberExpression(typeof(T), field);
            object? value = cursors?[field];
            CompararisonValues.Add(new((MemberExpression)expressionMember, value));

            BinaryExpression andClause = BuildAndClause<T>(i, CompararisonValues, order);
            body = body == null ? andClause : Expression.OrElse(body, andClause);
        }

        //* x => x.Age < AgeValue ||
        //*     (x.Age == AgeValue && x.Name > NameValue) ||
        //*     (x.Age == AgeValue && x.Name == NameValue && x.Id > IdValue)
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
        string order
    )
    {
        BinaryExpression? innerExpression = BuildEqualOperationClause(index, CompararisonValues);

        MemberExpression expressionMember = CompararisonValues[index].Key;
        object? value = CompararisonValues[index].Value;

        BinaryExpression binaryExpression = BuildCompareOperation(
            expressionMember.GetMemberExpressionType(),
            expressionMember,
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
            MethodCallExpression compareExpression = CompareUlidByExpression(member, value);
            ConstantExpression comparisonValue = Expression.Constant(0);

            return order == OrderTerm.DESC
                ? Expression.LessThan(compareExpression, comparisonValue)
                : Expression.GreaterThan(compareExpression, comparisonValue);
        }

        if (type == typeof(string))
        {
            ConstantExpression comparisonValue = Expression.Constant(0);
            MethodCallExpression comparisonExpression = CompareStringByExpression(
                member,
                Expression.Constant(value)
            );

            return order == OrderTerm.DESC
                ? Expression.LessThan(comparisonExpression, comparisonValue)
                : Expression.GreaterThan(comparisonExpression, comparisonValue);
        }

        ConvertExpressionTypeResult typeResult = ConvertType(member, value);
        return order == OrderTerm.DESC
            ? Expression.LessThan(typeResult.Member, typeResult.Value)
            : Expression.GreaterThan(typeResult.Member, typeResult.Value);
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
            MemberExpression memberExpression = compararisonValue.Key;
            object? value = compararisonValue.Value;

            BinaryExpression operation;
            if (compararisonValue.Key.GetMemberExpressionType() == typeof(Ulid))
            {
                MethodCallExpression compararison = CompareUlidByExpression(
                    memberExpression,
                    value
                );
                operation = Expression.Equal(compararison, Expression.Constant(0));
            }
            else
            {
                ConvertExpressionTypeResult typeResult = ConvertType(memberExpression, value);
                operation = Expression.Equal(typeResult.Member, typeResult.Value!);
            }

            body = body == null ? operation : Expression.AndAlso(body, operation);
        }
        return body;
    }

    private static ConvertExpressionTypeResult ConvertType(
        MemberExpression memberExpression,
        object? value
    )
    {
        Expression member = memberExpression;
        Type memberType = memberExpression.GetMemberExpressionType();

        if (
            (
                memberType.IsNullable()
                && memberType.GenericTypeArguments.Length > 0
                && memberType.GenericTypeArguments[0].IsEnum
            ) || memberType.IsEnum
        )
        {
            Type type = value?.GetType() ?? typeof(long);
            return new(Expression.Convert(member, type), Expression.Constant(value, type));
        }

        if (memberType != value?.GetType())
        {
            return new(member, Expression.Constant(value, memberType));
        }

        return new(member, Expression.Constant(value));
    }

    private static MethodCallExpression CompareUlidByExpression(Expression left, object? value)
    {
        Ulid compararisonValue = value == null ? Ulid.Empty : Ulid.Parse(value!.ToString());
        MethodInfo? compareToMethod = typeof(Ulid).GetMethod(
            nameof(Ulid.CompareTo),
            [typeof(Ulid)]
        );

        return Expression.Call(left, compareToMethod!, Expression.Constant(compararisonValue));
    }

    private static MethodCallExpression CompareStringByExpression(
        Expression left,
        Expression right
    ) => Expression.Call(typeof(string), nameof(string.Compare), null, [left, right]);

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
        PropertyInfo cursorPropertyInfo = typeof(T).GetNestedPropertyInfo(
            nameof(DefaultBaseResponse.Id)
        );
        object? cursorProperty = typeof(T).GetNestedPropertyValue(
            nameof(DefaultBaseResponse.Id),
            cursor!
        );

        PropertyInfo destinationPropertyInfo = typeof(T).GetNestedPropertyInfo(
            nameof(DefaultBaseResponse.Id)
        );
        object? desProperty = typeof(T).GetNestedPropertyValue(
            nameof(DefaultBaseResponse.Id),
            destination!
        );

        if (cursorPropertyInfo.PropertyType != destinationPropertyInfo.PropertyType)
        {
            throw new Exception($"{cursor} and {destination} key is difference");
        }

        if (cursorProperty == null || desProperty == null)
        {
            throw new Exception($"{cursor} or {destination} key is null");
        }

        return cursorPropertyInfo.PropertyType == typeof(Ulid)
            ? ((Ulid)cursorProperty).CompareTo((Ulid)desProperty) == 0
            : cursorProperty == desProperty;
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
            if (!typeof(T).IsNestedPropertyValid(field))
            {
                throw new NotFoundException(nameof(field), field);
            }

            object? value = typeof(T).GetNestedPropertyValue(field, entity!);
            properties.Add(field, value);
        }

        return properties;
    }

    private static Dictionary<string, object?>? DecodeCursor(string cursor)
    {
        string key = AesGcmEncryption.GenKey(EncryptKey());
        string stringCursor = AesGcmEncryption.Decrypt(cursor, key);
        var serializeResult = JsonConvert.DeserializeObject<Dictionary<string, object?>>(
            stringCursor
        );
        return serializeResult;
    }

    private static string? EncodeCursor<T>(T? entity, string sort)
    {
        if (entity == null)
        {
            return null;
        }

        Dictionary<string, object?> properties = GetEncryptionProperties(entity, sort);
        string json = JsonConvert.SerializeObject(properties, Formatting.Indented);
        string key = AesGcmEncryption.GenKey(EncryptKey());
        return AesGcmEncryption.Encrypt(json, key);
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
    string? Cursor,
    bool IsPrevious,
    T First,
    T Last,
    string OriginalSort,
    string Sort,
    int Size
);

internal record PaginationResult<T>(
    IEnumerable<T> Data,
    int PageSize,
    string? Next = null,
    string? Pre = null
);

internal record ConvertExpressionTypeResult(Expression Member, ConstantExpression Value);

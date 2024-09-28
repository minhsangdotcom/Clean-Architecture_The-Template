using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Contracts.Dtos.Models;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Contracts.Extensions.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Contracts.Extensions.QueryExtensions;

public static class PaginationExtension
{
    public static async Task<PaginationResponse<T>> PaginateAsync<T>(
        this IQueryable<T> entities,
        int current,
        int size
    )
    {
        int totalPage = entities.Count();

        return new PaginationResponse<T>(
            await entities.Skip((current - 1) * size).Take(size).ToListAsync(),
            totalPage,
            current,
            size
        );
    }

    public static PaginationResponse<T> Paginate<T>(
        this IEnumerable<T> entities,
        int current,
        int size
    ) => new(entities.Skip((current - 1) * size).Take(size), entities.Count(), current, size);

    public static async Task<PaginationResponse<T>> PointerPaginateAsync<T>(
        this IQueryable<T> entities,
        CursorPaginationRequest request
    )
    {
        string sortRequests =
            request.Order != null
                ? $"{request.Order},{request.UniqueOrdering}"
                : $"{request.UniqueOrdering}";

        IQueryable<T> sortData = entities.Sort(sortRequests);

        int totalPage = sortData.Count();

        T? firstPage = sortData.FirstOrDefault();

        T? lastPage = sortData.LastOrDefault();

        bool isBefore = request.BeforeCursor != null && request.AfterCursor == null;

        string cursorQuery = request.AfterCursor!;
        string sort = sortRequests;

        if (isBefore)
        {
            sort = GetReverseSort(sortRequests);
            sortData = sortData.Sort(sort);
            cursorQuery = request.BeforeCursor!;
        }

        PaginationMetadata<T> metadata = await GetMetadata(
            sortData,
            request.Size,
            sort,
            cursorQuery,
            isBefore
        );

        return new PaginationResponse<T>(
            metadata.Entities,
            totalPage,
            request.Size,
            firstPage,
            lastPage,
            metadata.PreviousCursor,
            metadata.NextCursor
        );
    }

    private static async Task<PaginationMetadata<T>> GetMetadata<T>(
        IQueryable<T> entities,
        int size,
        string sortRequest,
        string? cursor = null,
        bool isBefore = false
    )
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

        Expression<Func<T, bool>> lamda = GetlamdaExpression<T>(
            parameter,
            cursor == null,
            cursor!,
            sortRequest
        );

        IQueryable<T> query = entities.Where(lamda).Take(size);

        if (isBefore)
        {
            query = query.Sort(GetReverseSort(sortRequest));
        }

        List<T> dataEntities = await query.ToListAsync();

        string nextCursor = null!;
        string previousCursor = null!;

        if (dataEntities.Count != 0)
        {
            previousCursor = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(
                    SerializerExtension.Serialize(dataEntities.FirstOrDefault()!).StringJson
                )
            );
            nextCursor = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(
                    SerializerExtension.Serialize(dataEntities.LastOrDefault()!).StringJson
                )
            );
        }

        return new PaginationMetadata<T>(dataEntities, nextCursor, previousCursor);
    }

    private static Expression<Func<T, bool>> GetlamdaExpression<T>(
        ParameterExpression parameter,
        bool isFirstPage,
        string cursor,
        string sortRequest
    )
    {
        if (isFirstPage)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Constant(true), parameter);
        }

        Expression body = null!;

        IEnumerable<OrderInfo> orders = GetOrderInfo(sortRequest);

        for (int i = 0; i < orders.Count(); i++)
        {
            OrderInfo orderInfo = orders.ElementAt(i);

            Expression currentOperation = GetBodyExpression<T>(orderInfo, cursor, parameter);

            Expression previousOperation = GetPreviousExpression<T>(
                orders,
                parameter,
                cursor,
                orderInfo.Propertyname
            );

            Expression operation =
                previousOperation != null
                    ? Expression.And(previousOperation, currentOperation)
                    : currentOperation;

            body = body == null ? operation : Expression.OrElse(body, operation);
        }

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression GetPreviousExpression<T>(
        IEnumerable<OrderInfo> orders,
        ParameterExpression parameter,
        string cursor,
        string breakProperty
    )
    {
        Expression body = null!;

        foreach (var order in orders)
        {
            if (breakProperty == order.Propertyname)
            {
                break;
            }

            OperationInfo operationInfo = GetOperationInfo<T>(parameter, cursor, order);

            Expression operation = Expression.Equal(
                operationInfo.MemberExpression,
                operationInfo.ConstantExpression
            );

            body = body == null ? operation : Expression.AndAlso(body, operation);
        }

        return body;
    }

    private static Expression GetBodyExpression<T>(
        OrderInfo orderInfo,
        string cursor,
        ParameterExpression parameter
    )
    {
        OperationInfo operationInfo = GetOperationInfo<T>(parameter, cursor, orderInfo);

        Expression operation = true switch
        {
            bool when operationInfo.PropertyType == typeof(string) => orderInfo.OrderType
            == OrderTerm.DESC
                ? Expression.LessThan(
                    StringCompareExpression(
                        operationInfo.MemberExpression,
                        operationInfo.ConstantExpression
                    ),
                    Expression.Constant(0)
                )
                : Expression.GreaterThan(
                    StringCompareExpression(
                        operationInfo.MemberExpression,
                        operationInfo.ConstantExpression
                    ),
                    Expression.Constant(0)
                ),

            _ => orderInfo.OrderType == OrderTerm.DESC
                ? Expression.LessThan(
                    operationInfo.MemberExpression,
                    operationInfo.ConstantExpression
                )
                : Expression.GreaterThan(
                    operationInfo.MemberExpression,
                    operationInfo.ConstantExpression
                ),
        };

        return operation;
    }

    private static OperationInfo GetOperationInfo<T>(
        ParameterExpression parameter,
        string cursor,
        OrderInfo orderInfo
    )
    {
        var memberExpression = ExpressionExtension.GetExpressionMember<T>(
            orderInfo.Propertyname,
            parameter,
            false
        );

        T? cursorObject = GetCursor<T>(cursor);

        PropertyInfo propertyInfo =
            cursorObject?.GetType()?.GetProperty(orderInfo.Propertyname)
            ?? throw new Exception($"{orderInfo.Propertyname} is not found.");

        ConstantExpression expressionValue = GetExpresionValue(propertyInfo, cursorObject!);

        return new OperationInfo
        {
            MemberExpression = memberExpression,
            ConstantExpression = expressionValue,
            PropertyType = propertyInfo.PropertyType,
        };
    }

    private static T? GetCursor<T>(string cursor)
    {
        byte[] byteArray = Convert.FromBase64String(cursor!);

        string jsonBack = Encoding.UTF8.GetString(byteArray);

        return SerializerExtension.Deserialize<T>(jsonBack).Object;
    }

    private static ConstantExpression GetExpresionValue(
        PropertyInfo propertyInfo,
        object cursorObject
    )
    {
        var value = propertyInfo?.GetValue(cursorObject, null);

        return Expression.Constant(value);
    }

    private static IEnumerable<OrderInfo> GetOrderInfo(string sortRequest)
    {
        foreach (var orederValue in sortRequest.Trim().Split(","))
        {
            var order = orederValue.Trim().Split(" ");

            OrderInfo orderInfo = new() { Propertyname = order.FirstOrDefault()! };

            if (order.Length > 1)
            {
                orderInfo.OrderType = order.LastOrDefault()!;
            }

            yield return orderInfo;
        }
    }

    private static MethodCallExpression StringCompareExpression(Expression left, Expression right)
    {
        return Expression.Call(typeof(string), nameof(string.Compare), null, [left, right]);
    }

    private static string GetReverseSort(string sortQuery)
    {
        StringBuilder stringBuilder = new();

        foreach (var query in sortQuery.Trim().Split(","))
        {
            var sortItem = query.Trim().Split(" ");

            stringBuilder.Append($"{sortItem.FirstOrDefault()}");

            if (sortItem.Length == 1)
            {
                stringBuilder.Append($" {OrderTerm.DESC}");
            }

            stringBuilder.Append(',');
        }

        return stringBuilder.Remove(stringBuilder.Length - 1, 1).ToString();
    }
}

public class OrderInfo
{
    public string Propertyname { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
}

public class OperationInfo
{
    public Expression MemberExpression { get; set; } = default!;

    public ConstantExpression ConstantExpression { get; set; } = default!;

    public Type? PropertyType { get; set; }
}

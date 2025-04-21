using System.Reflection;
using Application.Common.Errors;
using Application.Common.Extensions;
using Contracts.Dtos.Requests;
using Serilog;
using SharedKernel.Common.Messages;
using SharedKernel.Extensions;
using SharedKernel.Extensions.Reflections;
using StringExtension = Application.Common.Extensions.StringExtension;

namespace Application.Common.QueryStringProcessing;

public static partial class QueryParamValidate
{
    private const string Message = "Your request parameters didn't validate.";

    public static ValidationRequestResult<T, BadRequestError> ValidateQuery<T>(this T request)
        where T : QueryParamRequest
    {
        if (!string.IsNullOrWhiteSpace(request.Before) && !string.IsNullOrWhiteSpace(request.After))
        {
            return new(
                Error: new BadRequestError(
                    Message,
                    Messager
                        .Create<QueryParamRequest>("QueryParam")
                        .Property("Cursor")
                        .Message(MessageType.Redundant)
                        .Build()
                )
            );
        }

        return new(request);
    }

    public static ValidationRequestResult<TRequest, BadRequestError> ValidateFilter<
        TRequest,
        TResponse
    >(this TRequest request)
        where TResponse : class
        where TRequest : QueryParamRequest
    {
        if (request.OriginFilters?.Length <= 0)
        {
            return new(request);
        }

        List<QueryResult> queries =
        [
            .. StringExtension.TransformStringQuery(request.OriginFilters!),
        ];

        int length = queries.Count;

        for (int i = 0; i < length; i++)
        {
            QueryResult query = queries[i];

            //if it's $and,$or,$in and $between then they must have a index after like $or[0],$[in][1]
            if (!ValidateArrayOperator(query.CleanKey))
            {
                return new(
                    Error: new BadRequestError(
                        Message,
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property(x => x.Filter!)
                            .Message(MessageType.Missing)
                            .ObjectName("ArrayIndex")
                            .Build()
                    )
                );
            }

            /// check if the index of array operator has to start with 0 like $and[0][firstName]
            if (i == 0 && !ValidateArrayOperatorInvalidIndex(query.CleanKey))
            {
                return new(
                    Error: new BadRequestError(
                        Message,
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property(x => x.Filter!)
                            .Message(MessageType.Valid)
                            .Negative()
                            .ObjectName("ArrayIndex")
                            .Build()
                    )
                );
            }

            // lack of operator
            if (!ValidateLackOfOperator(query.CleanKey))
            {
                return new(
                    Error: new BadRequestError(
                        Message,
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property(x => x.Filter!)
                            .Message(MessageType.Missing)
                            .ObjectName("Operator")
                            .Build()
                    )
                );
            }

            // if the last element is logical operator ($and, $or) it's wrong like filter[$and][0] which is lack of body
            if (LackOfElementInArrayOperator(query.CleanKey))
            {
                return new(
                    Error: new BadRequestError(
                        Message,
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property(x => x.Filter!)
                            .Message(MessageType.Missing)
                            .ObjectName("Element")
                            .Build()
                    )
                );
            }

            IEnumerable<string> properties = query.CleanKey.Where(x =>
                string.Compare(x, "$or", StringComparison.OrdinalIgnoreCase) != 0
                && string.Compare(x, "$and", StringComparison.OrdinalIgnoreCase) != 0
                && !x.IsDigit()
                && !validOperators.Contains(x.ToLower())
            );

            // lack of property
            if (!properties.Any())
            {
                return new(
                    Error: new BadRequestError(
                        Message,
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property(x => x.Filter!)
                            .Message(MessageType.Missing)
                            .ObjectName("Property")
                            .Build()
                    )
                );
            }
            Type type = typeof(TResponse);
            PropertyInfo propertyInfo = type.GetNestedPropertyInfo(string.Join(".", properties));
            Type[] arguments = propertyInfo.PropertyType.GetGenericArguments();
            Type nullableType = arguments.Length > 0 ? arguments[0] : propertyInfo.PropertyType;

            // value must be enum
            if (
                (nullableType.IsEnum || IsNumericType(nullableType))
                && query.Value?.IsDigit() == false
            )
            {
                return new(
                    Error: new BadRequestError(
                        Message,
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property("FilterValue")
                            .Negative()
                            .ObjectName("Integer")
                            .Build()
                    )
                );
            }

            // value must be datetime
            if (
                (nullableType == typeof(DateTime) && !DateTime.TryParse(query.Value, out _))
                || (
                    nullableType == typeof(DateTimeOffset)
                    && !DateTimeOffset.TryParse(query.Value, out _)
                )
            )
            {
                return new(
                    Error: new BadRequestError(
                        Message,
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property("FilterValue")
                            .Negative()
                            .ObjectName("Datetime")
                            .Build()
                    )
                );
            }

            // value must be Ulid
            if ((nullableType == typeof(Ulid)) && !Ulid.TryParse(query.Value, out _))
            {
                return new(
                    Error: new BadRequestError(
                        Message,
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property("FilterValue")
                            .Negative()
                            .ObjectName("Ulid")
                            .Build()
                    )
                );
            }
        }

        // validate between operator is correct in format like [age][$between][0] = 1 & [age][$between][1] = 2
        if (!ValidateBetweenAndInOperator("$between", queries))
        {
            return new(
                Error: new BadRequestError(
                    Message,
                    Messager
                        .Create<QueryParamRequest>("QueryParam")
                        .Property(x => x.Filter!)
                        .Message(MessageType.Valid)
                        .ObjectName("BetweenOperator")
                        .Negative()
                        .Build()
                )
            );
        }

        // validate $in operator is correct in format like [age][$int][0] = 1 & [age][$in][1] = 2
        if (!ValidateBetweenAndInOperator("$in", queries))
        {
            return new(
                Error: new BadRequestError(
                    Message,
                    Messager
                        .Create<QueryParamRequest>("QueryParam")
                        .Property(x => x.Filter!)
                        .Message(MessageType.Valid)
                        .ObjectName("InOperator")
                        .Negative()
                        .Build()
                )
            );
        }

        // duplicated element of filter
        var trimQueries = queries.Select(x => string.Join(".", x.CleanKey));
        if (trimQueries.Distinct().Count() != queries.Count)
        {
            return new(
                Error: new BadRequestError(
                    Message,
                    Messager
                        .Create<QueryParamRequest>("QueryParam")
                        .Property("FilterElement")
                        .Message(MessageType.Unique)
                        .Negative()
                        .Build()
                )
            );
        }

        request.Filter = StringExtension.Parse(queries);

        Log.Information(
            "Filter has been bound {filter}",
            SerializerExtension.Serialize(request.Filter!).StringJson
        );

        return new(request);
    }

    private static bool ValidateBetweenAndInOperator(
        string operation,
        IEnumerable<QueryResult> queries
    )
    {
        IEnumerable<QueryResult> betweenOperators = queries.Where(x =>
            x.CleanKey.Contains(operation)
        );

        var betweenOperatorsGroup = betweenOperators
            .Select(betweenOperator =>
            {
                int betweenIndex = betweenOperator.CleanKey.IndexOf(operation);
                int index = betweenIndex - 1;

                if (index < 0)
                {
                    throw new InvalidOperationException("Invalid format of cleanKey.");
                }

                string key = string.Join(
                    ".",
                    betweenOperator
                        .CleanKey.Skip(index)
                        .Take(betweenOperator.CleanKey.Count - betweenIndex)
                );

                _ = int.TryParse(betweenOperator.CleanKey.Last(), out int indexValue);

                if (
                    int.TryParse(
                        betweenOperator.CleanKey[betweenOperator.CleanKey.IndexOf("$and") + 1],
                        out int andIndex
                    )
                )
                {
                    return new { key = $"$and.{andIndex}.{key}", indexValue };
                }
                _ = int.TryParse(
                    betweenOperator.CleanKey[betweenOperator.CleanKey.IndexOf("$or") + 1],
                    out int orInddex
                );

                return new { key = $"$or.{orInddex}.{key}", indexValue };
            })
            .GroupBy(x => x.key)
            .Select(x => new { x.Key, values = x.Select(x => x.indexValue).ToList() })
            .ToList();

        if (
            betweenOperatorsGroup.Count != 0
            && (
                betweenOperatorsGroup.Count != 1
                || !betweenOperatorsGroup[0].values.SequenceEqual([0, 1])
            )
        )
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// just for enum
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsNumericType(Type type)
    {
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Byte
            or TypeCode.SByte
            or TypeCode.UInt16
            or TypeCode.UInt32
            or TypeCode.UInt64
            or TypeCode.Int16
            or TypeCode.Int32
            or TypeCode.Int64
            or TypeCode.Decimal
            or TypeCode.Double
            or TypeCode.Single => true,
            _ => false,
        };
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="input"></param>
    /// <returns>true if any element is after $and,$or,$in,$between isn't degit otherwise false</returns>
    private static bool ValidateArrayOperator(List<string> input)
    {
        var validOperators = new HashSet<string> { "$and", "$or", "$in", "$between" };

        for (int i = 0; i < input.Count; i++)
        {
            if (!validOperators.Contains(input[i]))
            {
                continue;
            }

            if (i + 1 >= input.Count)
            {
                continue;
            }

            if (!input[i + 1].IsDigit())
            {
                return false;
            }
        }

        if (input[^1] == validOperators.Last() || input[^1] == "$in")
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// check if array operator has invalid index like $and[1][firstName], index must start with 0.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static bool ValidateArrayOperatorInvalidIndex(List<string> input)
    {
        var validOperators = new HashSet<string> { "$and", "$or", "$in", "$between" };

        for (int i = 0; i < input.Count; i++)
        {
            if (!validOperators.Contains(input[i]))
            {
                continue;
            }

            if (i + 1 >= input.Count)
            {
                continue;
            }

            string theNextItem = input[i + 1];
            if (!theNextItem.IsDigit() || int.Parse(theNextItem) != 0)
            {
                return false;
            }
        }

        if (input[^1] == validOperators.Last() || input[^1] == "$in")
        {
            return false;
        }

        return true;
    }

    private static bool ValidateLackOfOperator(List<string> input)
    {
        Stack<string> inputs = new(input);

        string last = inputs.Pop();
        string preLast = inputs.Pop();

        if (arrayOperators.Contains(preLast.ToLower()))
        {
            return true;
        }

        return validOperators.Contains(last.ToLower());
    }

    private static bool LackOfElementInArrayOperator(List<string> input)
    {
        Stack<string> inputs = new(input);
        string last = inputs.Pop();
        string preLast = inputs.Pop();

        return logicalOperators.Contains(preLast.ToLower()) && last.IsDigit();
    }

    // Array of valid operators
    private static readonly string[] validOperators =
    [
        "$eq",
        "$eqi",
        "$ne",
        "$nei",
        "$in",
        "$notin",
        "$lt",
        "$lte",
        "$gt",
        "$gte",
        "$between",
        "$notcontains",
        "$notcontainsi",
        "$contains",
        "$containsi",
        "$startswith",
        "$endswith",
    ];

    // Operators that don't require further validation after them
    private static readonly string[] arrayOperators = ["$in", "$between"];

    private static readonly string[] logicalOperators = ["$and", "$or"];
}

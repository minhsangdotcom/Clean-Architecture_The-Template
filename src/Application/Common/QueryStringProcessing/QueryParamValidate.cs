using System.Reflection;
using Application.Common.Exceptions;
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
    public static QueryParamRequest ValidateQuery(this QueryParamRequest request)
    {
        if (
            !string.IsNullOrWhiteSpace(request.Cursor?.Before)
            && !string.IsNullOrWhiteSpace(request.Cursor?.After)
        )
        {
            throw new BadRequestException(
                [
                    Messager
                        .Create<QueryParamRequest>("QueryParam")
                        .Property(x => x.Cursor!)
                        .Message(MessageType.Redundant)
                        .Build(),
                ]
            );
        }

        return request;
    }

    public static QueryParamRequest ValidateFilter<TResponse>(this QueryParamRequest request)
        where TResponse : class
    {
        if (request.OriginFilters?.Length <= 0)
        {
            return request;
        }

        List<QueryResult> queries =
        [
            .. StringExtension.TransformStringQuery(request.OriginFilters!),
        ];

        int length = queries.Count;

        if (length == 1 && !ValidateArrayOperatorInvalidIndex(queries[0].CleanKey))
        {
            throw new BadRequestException(
                [
                    Messager
                        .Create<QueryParamRequest>("QueryParam")
                        .Property(x => x.Filter!)
                        .Message(MessageType.Valid)
                        .Negative()
                        .ObjectName("ArrayIndex")
                        .Build(),
                ]
            );
        }

        for (int i = 0; i < length; i++)
        {
            QueryResult query = queries[i];

            //if it's $and,$or,$in and $between then they must have a index after
            if (ValidateArrayOperator(query.CleanKey))
            {
                throw new BadRequestException(
                    [
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property(x => x.Filter!)
                            .Message(MessageType.Missing)
                            .ObjectName("ArrayIndex")
                            .Build(),
                    ]
                );
            }

            // lack of operator
            if (!ValidateLackOfOperator(query.CleanKey))
            {
                throw new BadRequestException(
                    [
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property(x => x.Filter!)
                            .Message(MessageType.Missing)
                            .ObjectName("Operator")
                            .Build(),
                    ]
                );
            }

            // if the last element is logical operator it's wrong like filter[$and][0] which is lack of body
            if (LackOfElementInArrayOperator(query.CleanKey))
            {
                throw new BadRequestException(
                    [
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property(x => x.Filter!)
                            .Message(MessageType.Missing)
                            .ObjectName("Element")
                            .Build(),
                    ]
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
                throw new BadRequestException(
                    [
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property(x => x.Filter!)
                            .Message(MessageType.Missing)
                            .ObjectName("Property")
                            .Build(),
                    ]
                );
            }
            Type type = typeof(TResponse);
            PropertyInfo propertyInfo = type.GetNestedPropertyInfo(string.Join(".", properties));
            Type[] arguments = propertyInfo.PropertyType.GetGenericArguments();
            Type nullableType = arguments.Length > 0 ? arguments[0] : propertyInfo.PropertyType;

            //
            if (
                (nullableType.IsEnum || IsNumericType(nullableType))
                && query.Value?.IsDigit() == false
            )
            {
                throw new BadRequestException(
                    [
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property("FilterValue")
                            .Message(MessageType.Matching)
                            .Negative()
                            .ObjectName("Integer")
                            .Build(),
                    ]
                );
            }

            if (
                (nullableType == typeof(DateTime) || nullableType == typeof(DateTimeOffset))
                && !DateTime.TryParse(query.Value, out var value)
            )
            {
                throw new BadRequestException(
                    [
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property("FilterValue")
                            .Message(MessageType.Matching)
                            .Negative()
                            .ObjectName("Datetime")
                            .Build(),
                    ]
                );
            }

            if ((nullableType == typeof(Ulid)) && !Ulid.TryParse(query.Value, out Ulid result))
            {
                throw new BadRequestException(
                    [
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property("FilterValue")
                            .Message(MessageType.Matching)
                            .Negative()
                            .ObjectName("Ulid")
                            .Build(),
                    ]
                );
            }
        }

        // validate between operator is correct in format like [age][$between][0] = 1 & [age][$between][1] = 2
        IEnumerable<QueryResult> betweenOperators = queries.Where(x =>
            x.CleanKey.Contains("$between")
        );
        var betweenOpratorFormat = betweenOperators
            .Select(x =>
            {
                int betweenIndex = x.CleanKey.IndexOf("$between");
                int index = betweenIndex - 1;

                if (index < 0)
                {
                    throw new InvalidOperationException("Invalid format of cleanKey.");
                }
                string key = string.Join(
                    ".",
                    x.CleanKey.Skip(index).Take(x.CleanKey.Count - betweenIndex)
                );

                int indexValue = int.Parse(x.CleanKey.Last());
                return new { key, indexValue };
            })
            .GroupBy(x => x.key)
            .Select(x => new { x.Key, values = x.Select(x => x.indexValue).ToList() })
            .ToList();

        if (
            betweenOperators.Any()
            && (
                betweenOpratorFormat.Count != 1
                || !betweenOpratorFormat[0].values.SequenceEqual([0, 1])
            )
        )
        {
            throw new BadRequestException(
                [
                    Messager
                        .Create<QueryParamRequest>("QueryParam")
                        .Property(x => x.Filter!)
                        .Message(MessageType.Valid)
                        .ObjectName("BetweenOperator")
                        .Negative()
                        .Build(),
                ]
            );
        }

        // duplicated element of filter
        var trimQueries = queries.Select(x => string.Join(".", x.CleanKey));
        if (trimQueries.Distinct().Count() != queries.Count)
        {
            throw new BadRequestException(
                [
                    Messager
                        .Create<QueryParamRequest>("QueryParam")
                        .Property("FilterElement")
                        .Message(MessageType.Unique)
                        .Negative()
                        .Build(),
                ]
            );
        }

        request.DynamicFilter = StringExtension.Parse(queries);

        Log.Information(
            "Filter has been bound {filter}",
            SerializerExtension.Serialize(request.DynamicFilter!).StringJson
        );

        return request;
    }

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
        List<string> arrayOperators = ["$and", "$or", "$in", "$between"];

        return arrayOperators.Any(arrayOperator =>
        {
            int index = input.IndexOf(arrayOperator);

            if (index < 0)
            {
                return false;
            }

            if (index >= input.Count - 1)
            {
                return true;
            }

            string afterArrayOperator = input[index + 1];

            return !afterArrayOperator.IsDigit();
        });
    }

    /// <summary>
    /// check if array operator has invalid index like $and[1][firstName], index must start with 0.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static bool ValidateArrayOperatorInvalidIndex(List<string> input)
    {
        List<string> arrayOperators = ["$and", "$or", "$in", "$between"];

        return arrayOperators.Any(arrayOperator =>
        {
            int index = input.IndexOf(arrayOperator);

            if (index < 0)
            {
                return false;
            }

            if (index >= input.Count - 1)
            {
                return true;
            }

            string afterArrayOperator = input[index + 1];

            return afterArrayOperator.IsDigit() && int.Parse(afterArrayOperator) == 0;
        });
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

using System.Reflection;
using System.Text.RegularExpressions;
using Application.Common.Exceptions;
using Contracts.Common.Messages;
using Contracts.Dtos.Requests;
using Contracts.Extensions;
using Contracts.Extensions.Reflections;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

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

    public static QueryParamRequest ValidateFilter(this QueryParamRequest request, Type type)
    {
        if (request.OriginFilters?.Length <= 0)
        {
            return request;
        }

        string[] queries = request.OriginFilters!;

        foreach (string query in queries)
        {
            //if it's $and,$or,$in and $between then they must have a index after
            if (!ValidateArrayOperator(query))
            {
                throw new BadRequestException(
                    [
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property(x => x.Filter!)
                            .Message(MessageType.ValidFormat)
                            .Negative()
                            .Build(),
                    ]
                );
            }

            // lack of operator
            if (!ValidateLackOfOperator(query))
            {
                throw new BadRequestException(
                    [
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property(x => x.Filter!)
                            .Message(MessageType.ValidFormat)
                            .Negative()
                            .Build(),
                    ]
                );
            }

            Dictionary<string, StringValues> queryStrings = QueryHelpers.ParseQuery(query);
            KeyValuePair<string, StringValues> queryString = queryStrings.ElementAt(0);

            string key = TransformKeyFilter(queryString.Key);
            string? value = queryString.Value;

            PropertyInfo propertyInfo = type.GetNestedPropertyInfo(key);

            //
            if (
                (propertyInfo.PropertyType.IsEnum || IsNumericType(propertyInfo.PropertyType))
                && value?.IsDigit() == false
            )
            {
                throw new BadRequestException(
                    [
                        Messager
                            .Create<QueryParamRequest>("QueryParam")
                            .Property(x => x.Filter!)
                            .Message(MessageType.ValidFormat)
                            .Negative()
                            .Build(),
                    ]
                );
            }
        }

        var trimQueries = queries.Select(x => x[..x.IndexOf('=')]);

        // duplicated element of filter
        if (trimQueries.Distinct().Count() != queries.Length)
        {
            throw new BadRequestException(
                [
                    Messager
                        .Create<QueryParamRequest>("QueryParam")
                        .Property(x => x.Filter!)
                        .Message(MessageType.ValidFormat)
                        .Negative()
                        .Build(),
                ]
            );
        }

        return request;
    }

    public static bool IsNumericType(Type type)
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

    private static string TransformKeyFilter(string input)
    {
        // Step 1: Remove filter, $or, $and, digits, and valid operators
        string pattern =
            @"filter|\$or|\$and|\[\d+\]|\["
            + string.Join("|", validOperators.Select(Regex.Escape))
            + @"\]";
        string cleanedInput = Regex.Replace(input, pattern, "");

        // Step 2: Remove remaining square brackets and concatenate with "."
        cleanedInput = MyRegex3().Replace(cleanedInput, ".");

        // Step 3: Remove any leading or trailing dots
        cleanedInput = cleanedInput.Trim('.');

        return cleanedInput;
    }

    private static bool ValidateArrayOperator(string input)
    {
        // This regex checks for "$or","$and","$in" or "between" followed by something that is not a digit.
        string pattern = @"(\$or|\$and|\$in|\$between)\[(?!\d)";

        // Check if the pattern is found in the input
        return !Regex.IsMatch(input, pattern);
    }

    private static bool ValidateLackOfOperator(string input)
    {
        // Regex to capture everything before the "=" operator
        var match = MyRegex1().Match(input);

        if (match.Success)
        {
            // Extract the part before the "=" operator
            string beforeEqual = match.Groups[1].Value;

            // Find the last operator in the string before the '='
            var lastOperatorMatch = MyRegex2().Match(beforeEqual);

            if (lastOperatorMatch.Success)
            {
                string lastOperator = $"${lastOperatorMatch.Groups[1].Value.ToLower()}";

                // Check if the last operator is $in or $between
                if (specialOperators.Contains(lastOperator))
                {
                    // Skip further validation if it's a special operator
                    return true;
                }

                // Otherwise, check if the operator is one of the valid ones
                return validOperators.Contains(lastOperator);
            }
        }

        // If there is no valid match or no "=" operator, return false
        return false;
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
    private static readonly string[] specialOperators = ["$in", "$between"];

    [GeneratedRegex(@"\[(.*?)\]")]
    private static partial Regex MyRegex();

    [GeneratedRegex(@"(.*)=")]
    private static partial Regex MyRegex1();

    [GeneratedRegex(@"\$(\w+)\]?$")]
    private static partial Regex MyRegex2();

    [GeneratedRegex(@"[\[\]]")]
    private static partial Regex MyRegex3();
}

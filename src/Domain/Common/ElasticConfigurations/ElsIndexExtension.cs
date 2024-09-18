using System.Linq.Expressions;
using System.Reflection;
using Contracts.Extensions;
using Contracts.Extensions.Expressions;
using Contracts.Extensions.Reflections;

namespace Domain.Common.ElasticConfigurations;

public static class ElsIndexExtension
{
    public static string GetName<T>() =>
        $"{ElsPrefix.prefix}{typeof(T).Name}".Underscored();

    public static string GetKeywordName<T>(Expression<Func<T, object>> expression)
    {
        PropertyInfo propertyInfo = ExpressionExtension.ToPropertyInfo(expression);
        return $"{propertyInfo.Name.LowerCaseFirst()}{ElsPrefix.KeywordPrefixName}";
    }

    public static string GetKeywordName<T>(string propertyName)
    {
        PropertyInfo propertyInfo = propertyName.Contains('.') ?
            typeof(T).GetNestedPropertyInfo(propertyName) :
                typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) ??
            throw new ArgumentException($"{propertyName} is not found.");

        return $"{propertyInfo.Name.LowerCaseFirst()}{ElsPrefix.KeywordPrefixName}";
    }

    // public static SearchDescriptor<T> Sort<T>(this SearchDescriptor<T> search, QueryRequest request) where T : class
    // {
    //     SortDescriptor<T> sortDescriptor = new();

    //     var orderRequest = request.Order?.Trim().Split(',');

    //     for (int i = 0; i < (orderRequest?.Length ?? 0); i++)
    //     {
    //         string order = orderRequest![i];
    //         string[] orderQuery = order.Trim().Split(' ');

    //         string property = orderQuery[0];

    //         property += $".{GetKeywordName<T>(property)}";

    //         string sort = orderQuery.Length > 1 ? orderQuery[1] : string.Empty;

    //         IFieldSort sortSelector(FieldSortDescriptor<T> f)
    //         {
    //             FieldSortDescriptor<T> fieldSort = new();

    //             fieldSort = fieldSort.Field(property).Order(sort == string.Empty ? SortOrder.Ascending : SortOrder.Descending);

    //             if (property.Trim().Contains('.'))
    //             {
    //                 string name = string.Empty;
    //                 string[] propertyName = property.Trim().Split('.');
    //                 for (int i = 0; i < propertyName.Length - 2; i++)
    //                 {
    //                     name += i == 0 ? $"{propertyName[i]}" : $".{propertyName[i]}";

    //                     fieldSort = fieldSort.Nested(n => n.Path(name));
    //                 }
    //             }

    //             return fieldSort;
    //         };

    //         sortDescriptor = sortDescriptor.Field(sortSelector);
    //     }

    //     return search?.Sort(x => sortDescriptor)!;
    // }
}
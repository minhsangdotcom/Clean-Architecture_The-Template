using System.Text.Json.Serialization;
using Contracts.Binds;
using Microsoft.AspNetCore.Mvc;

namespace Contracts.Dtos.Requests;

public class QueryParamRequest
{
    /// <summary>
    /// The current page
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Maximum items that display per page
    /// </summary>
    public int PageSize { get; set; } = 100;

    /// <summary>
    /// Cursor pagination
    /// </summary>
    public Cursor? Cursor { get; set; }

    public Search? Search { get; set; }

    /// <summary>
    /// example : Sort=Age:desc,Name:asc
    /// default is asc
    /// </summary>
    public string? Sort { get; set; }

    public Dictionary<string, string>? Filter { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public object? DynamicFilter { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    [ModelBinder(BinderType = typeof(FilterModelBinder))]
    public string[]? OriginFilters { get; set; }
}

public class Cursor
{
    public string? Before { get; set; }

    public string? After { get; set; }
}

public class Search
{
    public string? Keyword { get; set; }

    /// <summary>
    /// Fields want to search for
    /// </summary>
    public List<string>? Targets { get; set; }
}

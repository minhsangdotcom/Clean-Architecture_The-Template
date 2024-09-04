namespace Domain.Specs.Models;

public class GroupByResponse<T, TProperty>
{
    public List<T> Keys { get; set; } = [];

    public List<TProperty> Elements { get; set; } = [];
}
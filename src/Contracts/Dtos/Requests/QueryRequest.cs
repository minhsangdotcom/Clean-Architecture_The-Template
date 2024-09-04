namespace Contracts.Dtos.Requests;

public class QueryRequest
{
    public int Size { get; set; } = 10;

    public int CurrentPage { get; set; } = 1;

    public string? Before { get; set; }
    
    public string? After { get; set; }

    public string? Keyword { get; set; }

    public List<string>? SearchTarget { get; set; }

    public string? Order { get; set; }
}
namespace Contracts.ApiWrapper;

public class ValidationError
{
    public string Property { get; set; } = string.Empty;

    public List<string> Reasons { get; set; } = [];
}
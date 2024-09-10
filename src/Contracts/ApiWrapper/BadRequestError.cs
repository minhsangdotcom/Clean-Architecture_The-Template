namespace Contracts.ApiWrapper;

public class BadRequestError
{
    public string? PropertyName { get; set; }

    public IEnumerable<ReasonTranslation> Reasons { get; set; } = [];
}

public class ReasonTranslation
{
    public string? Message { get; set; }
    
    public string? En { get; set; }

    public string? Vi { get; set; }
}

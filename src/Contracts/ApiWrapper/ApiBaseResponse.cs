namespace Contracts.ApiWrapper;

[Serializable]
public class ApiBaseResponse
{
    public int StatusCode { get; set; }

    public string? Message { get; set; }
}

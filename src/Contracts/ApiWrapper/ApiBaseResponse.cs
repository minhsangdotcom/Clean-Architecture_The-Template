namespace Contracts.ApiWrapper;

[Serializable]
public class ApiBaseResponse
{
    public int Status { get; set; }

    public string? Message { get; set; }
}

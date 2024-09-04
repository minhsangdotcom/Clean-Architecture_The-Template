namespace Contracts.Dtos.Responses;

public class AwsResponse
{
    public bool IsSuccess { get; private set; }

    public string? S3UploadedPath { get; set; }

    public string? Key { get; set; }

    public string? Error { get; set; }

    public AwsResponse()
    {
        IsSuccess = true;
    }

    public AwsResponse(string error)
    {
        Error = error;
    }
}
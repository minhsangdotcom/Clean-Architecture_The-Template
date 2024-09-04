using Microsoft.AspNetCore.Http;
namespace Contracts.ApiWrapper;

public class ApiResponse : ApiBaseResponse
{
    public object? Results { get; set; }

    public ApiResponse()
    {
    }

    public ApiResponse(object result, string message, int? statusCode = StatusCodes.Status200OK)
    {
        Results = result;

        StatusCode = statusCode!.Value;

        Message = message;
    }
}
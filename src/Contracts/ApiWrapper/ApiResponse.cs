using Microsoft.AspNetCore.Http;

namespace Contracts.ApiWrapper;

public class ApiResponse<T> : ApiBaseResponse
    where T : class
{
    public T? Results { get; set; }

    public ApiResponse() { }

    public ApiResponse(T? result, string message, int? statusCode = StatusCodes.Status200OK)
    {
        Results = result;

        StatusCode = statusCode!.Value;

        Message = message;
    }
}

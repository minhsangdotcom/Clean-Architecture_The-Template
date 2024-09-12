using Contracts.ApiWrapper;
using Contracts.Constants;

namespace Api.Middlewares.GlobalExceptionHandlers;

public class InternalServerExceptionHandler()
    : IHandlerException
{
    public async Task Handle(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        string? traceId = httpContext.Items[Global.TRACE_ID]?.ToString();

        var error = new ErrorResponse(ex.Message, traceId: traceId);

        await httpContext.Response.WriteAsJsonAsync(error, error.GetOptions());
    }
}

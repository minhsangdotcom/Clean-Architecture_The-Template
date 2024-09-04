
using Contracts.ApiWrapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Api.Middlewares.GlobalExceptionHandlers;

public class InternalServerExceptionHandler(ILogger<InternalServerExceptionHandler> logger) : IHandlerException
{
    public async Task Handle(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        Guid traceId = Guid.NewGuid();

        logger.LogError("Server {exception} error has id {Id} with message '{Message}'\n {StackTrace}\n at {DatetimeUTC}", ex.GetType().Name, traceId, ex.Message, ex.StackTrace?.TrimStart(), DateTimeOffset.UtcNow);

        await httpContext.Response.WriteAsJsonAsync(new ErrorResponse(ex.Message, traceId: traceId));
    }
}
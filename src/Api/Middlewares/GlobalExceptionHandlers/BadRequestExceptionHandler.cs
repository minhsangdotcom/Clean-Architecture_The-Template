using System.Diagnostics;
using Application.Common.Exceptions;
using Contracts.ApiWrapper;

namespace Api.Middlewares.GlobalExceptionHandlers;

public class BadRequestExceptionHandler : IHandlerException<BadRequestException>
{
    public async Task Handle(HttpContext httpContext, Exception ex)
    {
        var exception = (BadRequestException)ex;

        httpContext.Response.StatusCode = exception.HttpStatusCode;

        ErrorResponse error =
            new(
                exception.Errors,
                exception.GetType().Name,
                exception.Message,
                new()
                {
                    TraceId = Activity.Current?.Context.TraceId.ToString(),
                    SpanId = Activity.Current?.Context.SpanId.ToString(),
                }
            );

        await httpContext.Response.WriteAsJsonAsync(error, error.GetOptions());
    }
}

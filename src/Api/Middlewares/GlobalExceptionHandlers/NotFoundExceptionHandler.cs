using System.Diagnostics;
using Application.Common.Exceptions;
using Contracts.ApiWrapper;

namespace Api.Middlewares.GlobalExceptionHandlers;

public class NotFoundExceptionHandler : IHandlerException<NotFoundException>
{
    public async Task Handle(HttpContext httpContext, Exception ex)
    {
        var exception = (NotFoundException)ex;

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
                },
                exception.HttpStatusCode
            );

        await httpContext.Response.WriteAsJsonAsync(error, error.GetOptions());
    }
}

using System.Diagnostics;
using Application.Common.Exceptions;
using Contracts.ApiWrapper;

namespace Api.Middlewares.GlobalExceptionHandlers;

public class ValidationExceptionHandler : IHandlerException<ValidationException>
{
    public async Task Handle(HttpContext httpContext, Exception ex)
    {
        var exception = (ValidationException)ex;

        httpContext.Response.StatusCode = exception.HttpStatusCode;

        var error = new ErrorResponse(
            exception.ValidationErrors,
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

using Application.Common.Exceptions;
using Contracts.ApiWrapper;

namespace Api.Middlewares.GlobalExceptionHandlers;

public class ValidationExceptionHandler : IHandlerException<ValidationException>
{
    public async Task Handle(HttpContext httpContext, Exception ex)
    {
        var exception = (ValidationException)ex;

        httpContext.Response.StatusCode = exception.HttpStatusCode;

        var error = new ErrorResponse(exception.ValidationErrors);

        await httpContext.Response.WriteAsJsonAsync(error, error.Serialize().Options);
    }
}

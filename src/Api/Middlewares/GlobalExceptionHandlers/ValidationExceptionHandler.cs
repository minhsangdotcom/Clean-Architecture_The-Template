using Application.Common.Exceptions;
using Contracts.ApiWrapper;

namespace Api.Middlewares.GlobalExceptionHandlers;

public class ValidationExceptionHandler : IHandlerException<ValidationException>
{
    public async Task Handle(HttpContext httpContext, Exception ex)
    {
        var exception = (ValidationException)ex;

        int statusCode = exception.HttpStatusCode;

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(new ErrorResponse(exception.ValidationErrors));
    }
}

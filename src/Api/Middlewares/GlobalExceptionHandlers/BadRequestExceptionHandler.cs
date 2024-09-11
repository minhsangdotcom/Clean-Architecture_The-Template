using Application.Common.Exceptions;
using Contracts.ApiWrapper;

namespace Api.Middlewares.GlobalExceptionHandlers;

public class BadRequestExceptionHandler : IHandlerException<BadRequestException>
{
    public async Task Handle(HttpContext httpContext, Exception ex)
    {
        var exception = (BadRequestException)ex;

        httpContext.Response.StatusCode = exception.HttpStatusCode;

        ErrorResponse error = new(exception.Errors);

        await httpContext.Response.WriteAsJsonAsync(error, error.Serialize().Options);
    }
}

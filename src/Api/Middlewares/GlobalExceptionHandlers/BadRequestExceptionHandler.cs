using Application.Common.Exceptions;
using Contracts.ApiWrapper;

namespace Api.Middlewares.GlobalExceptionHandlers;

public class BadRequestExceptionHandler : IHandlerException<BadRequestException>
{
    public async Task Handle(HttpContext httpContext, Exception ex)
    {
        var exception = (BadRequestException)ex;

        int statusCode = exception.HttpStatusCode;

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(new ErrorResponse(exception.Message, nameof(BadRequestException), statusCode: statusCode));
    }
}
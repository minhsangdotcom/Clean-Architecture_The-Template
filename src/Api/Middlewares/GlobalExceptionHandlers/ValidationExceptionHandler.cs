using Contracts.ApiWrapper;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Api.Middlewares.GlobalExceptionHandlers;

public class ValidationExceptionHandler : IHandlerException<ValidationException>
{
    public async Task Handle(HttpContext httpContext, Exception ex)
    {
        var exception = (ValidationException)ex;

        int statusCode = exception.HttpStatusCode;

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(new ErrorResponse(exception.ValidationErrors.ToList(), exception.Message));
    }
}
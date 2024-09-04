using Application.Common.Exceptions;
using Contracts.ApiWrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Middlewares.GlobalExceptionHandlers;

public class ForbiddenExceptionHandler
{
     public static async Task Handle(ForbiddenContext httpContext, ForbiddenException exception)
    {
        int statusCode = exception.HttpStatusCode;
        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(new ErrorResponse(exception.Message, nameof(ForbiddenException), statusCode: statusCode));
    }
}
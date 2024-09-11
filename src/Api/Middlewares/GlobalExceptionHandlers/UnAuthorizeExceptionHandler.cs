using Application.Common.Exceptions;
using Contracts.ApiWrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Middlewares.GlobalExceptionHandlers;

public class UnAuthorizeExceptionHandler
{
    public static async Task Handle(
        JwtBearerChallengeContext httpContext,
        UnauthorizedException exception
    )
    {
        int statusCode = exception.HttpStatusCode;
        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new ErrorResponse(
                exception.Message,
                nameof(UnauthorizedException),
                statusCode: statusCode
            )
        );
    }
}

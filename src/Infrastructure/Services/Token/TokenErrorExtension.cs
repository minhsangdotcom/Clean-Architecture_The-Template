using Application.Common.Exceptions;
using Contracts.ApiWrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Token;

public class TokenErrorExtension
{
    public static async Task ForbiddenException(ForbiddenContext httpContext, ForbiddenException exception)
    {
        int statusCode = exception.HttpStatusCode;
        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new ErrorResponse(exception.Message, nameof(ForbiddenException), statusCode: statusCode)
        );
    }

    public static async Task UnauthorizedException(
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

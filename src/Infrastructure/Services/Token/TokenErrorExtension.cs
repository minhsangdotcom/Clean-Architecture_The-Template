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

        ErrorResponse error = new(exception.Message, nameof(ForbiddenException), statusCode: statusCode);

        await httpContext.Response.WriteAsJsonAsync(
            error,
            error.GetOptions()
        );
    }

    public static async Task UnauthorizedException(
        JwtBearerChallengeContext httpContext,
        UnauthorizedException exception
    )
    {
        int statusCode = exception.HttpStatusCode;
        httpContext.Response.StatusCode = statusCode;

        ErrorResponse error = new(
                exception.Message,
                nameof(UnauthorizedException),
                statusCode: statusCode
            );

        await httpContext.Response.WriteAsJsonAsync(
            error,
            error.GetOptions()
        );
    }
}

using Application.Common.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Token;

public class TokenErrorExtension
{
    public static async Task ForbiddenException(
        ForbiddenContext httpContext,
        ForbiddenError forbiddenError
    )
    {
        var problemDetailsService =
            httpContext.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

        int statusCode = forbiddenError.Status;
        httpContext.Response.StatusCode = statusCode;

        ProblemDetails problemDetails =
            new()
            {
                Title = forbiddenError.Title,
                Type = forbiddenError.Type,
                Status = forbiddenError.Status,
                Detail = forbiddenError.Detail,
            };

        // await httpContext.Response.WriteAsJsonAsync(
        //     problemDetails,
        //     SerializerExtension.Options(),
        //     contentType: "application/problem+json"
        // );

        await problemDetailsService.TryWriteAsync(
            new() { ProblemDetails = problemDetails, HttpContext = httpContext.HttpContext }
        );
    }

    public static async Task UnauthorizedException(
        JwtBearerChallengeContext httpContext,
        UnauthorizedError unauthorizedError
    )
    {
        var problemDetailsService =
            httpContext.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

        int statusCode = unauthorizedError.Status;
        httpContext.Response.StatusCode = statusCode;

        ProblemDetails problemDetails =
            new()
            {
                Title = unauthorizedError.Title,
                Type = unauthorizedError.Type,
                Status = unauthorizedError.Status,
                Detail = unauthorizedError.Detail,
            };

        await problemDetailsService.TryWriteAsync(
            new() { ProblemDetails = problemDetails, HttpContext = httpContext.HttpContext }
        );
    }
}

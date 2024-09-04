using System.Text;
using Api.Middlewares.GlobalExceptionHandlers;
using Application.Common.Exceptions;
using Contracts.Common.Messages;
using Contracts.Common.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Api.Extensions;

public static class JwtRegisterExtension
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtSettings>(config.GetSection($"SecuritySettings:{nameof(JwtSettings)}"));

        var jwtSettings = config.GetSection($"SecuritySettings:{nameof(JwtSettings)}").Get<JwtSettings>();

        return services
         .AddAuthentication(authentication =>
        {
            authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(bearer =>
        {
            bearer.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings!.SecretKey!)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };

            bearer.IncludeErrorDetails = true;
            bearer.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    return UnAuthorizeExceptionHandler.Handle(
                        context,
                        !context.Response.HasStarted ?
                            new UnauthorizedException(Message.UNAUTHORIZED) :
                                new UnauthorizedException(Message.TOKEN_EXPIRED)
                    );
                },
                OnForbidden = context => ForbiddenExceptionHandler.Handle(
                        context,
                        new ForbiddenException(Message.FORBIDDEN)
                    )
            };
        })
        .Services;
    }
}
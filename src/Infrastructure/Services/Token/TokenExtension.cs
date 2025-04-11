using System.Text;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Services.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Common.Messages;

namespace Infrastructure.Services.Token;

public static class TokenExtension
{
    public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtSettings>(
            config.GetSection($"SecuritySettings:{nameof(JwtSettings)}")
        );

        var jwtSettings = config
            .GetSection($"SecuritySettings:{nameof(JwtSettings)}")
            .Get<JwtSettings>();

        return services
            .AddSingleton<ITokenFactoryService, TokenFactoryService>()
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
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(jwtSettings!.SecretKey!)
                    ),
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
                        return TokenErrorExtension.UnauthorizedException(
                            context,
                            !context.Response.HasStarted
                                ? new UnauthorizedException(Message.UNAUTHORIZED)
                                : new UnauthorizedException(Message.TOKEN_EXPIRED)
                        );
                    },
                    OnForbidden = context =>
                        TokenErrorExtension.ForbiddenException(
                            context,
                            new ForbiddenException(Message.FORBIDDEN)
                        ),
                };
            })
            .Services;
    }
}

using System.Reflection;
using Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace Api.Extensions;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwagger(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        OpenApiSettings? openApiSettings = configuration
            .GetSection(nameof(OpenApiSettings))
            .Get<OpenApiSettings>();

        return services.AddSwaggerGen(option =>
        {
            option.AddSecurityDefinition(
                JwtBearerDefaults.AuthenticationScheme,
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                }
            );

            option.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme,
                            },
                        },
                        Array.Empty<string>()
                    },
                }
            );

            option.SwaggerDoc(
                "v1",
                new OpenApiInfo()
                {
                    Title = $"{openApiSettings?.ApplicationName} Documentation",
                    Version = "v1",
                    Description = $"Well come to the {openApiSettings?.ApplicationName} API",
                    Contact = new OpenApiContact()
                    {
                        Name = openApiSettings?.Name,
                        Email = openApiSettings?.Email,
                    },
                }
            );

            string? path = Assembly.GetExecutingAssembly().GetName().Name;
            var xmlFile = $"{path}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            option.IncludeXmlComments(xmlPath);
        });
    }
}

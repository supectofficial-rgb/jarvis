namespace Insurance.CoreService.Endpoints.Api.Extensions.Swaggers.Extentions;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, string title, string version)
    {
        string version2 = version;
        string title2 = title;
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(delegate (SwaggerGenOptions c)
        {
            c.SwaggerDoc(version2, new OpenApiInfo
            {
                Title = title2,
                Version = version2
            });

            c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description = "Enter your API Key",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Name = "X-Api-Key",          // the header name
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Scheme = "ApiKeyScheme"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {

                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },

                Array.Empty<string>()
            } });
            string path = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
            string text = Path.Combine(AppContext.BaseDirectory, path);
            if (File.Exists(text))
            {
                c.IncludeXmlComments(text, includeControllerXmlComments: true);
            }
        });
        return services;
    }
}

public static class SwaggerMiddlewareExtensions
{
    public static IApplicationBuilder UseSwaggerMiddleware(this IApplicationBuilder app, string title = "API V1")
    {
        string title2 = title;
        IWebHostEnvironment service = app.ApplicationServices.GetService<IWebHostEnvironment>();
        if (service != null && service.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(delegate (SwaggerUIOptions options)
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", title2);
            });
        }

        return app;
    }
}
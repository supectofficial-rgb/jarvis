namespace Insurance.UserService.Endpoints.Api.Extensions.Swaggers.Extentions;

using Swashbuckle.AspNetCore.SwaggerUI;

public static class SwaggerMiddlewareExtensions
{
    public static IApplicationBuilder UseSwaggerMiddleware(this IApplicationBuilder app, string title = "API V1")
    {
        string title2 = title;
        if (app is not null)
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

namespace Insurance.CacheService.Endpoints.Api;

using Elastic.Apm.NetCoreAll;
using FluentValidation;
using Insurance.CacheService.AppCore.AppServices;
using Insurance.CacheService.Endpoints.Api.Validators;
using Insurance.CacheService.Infra.Persistence.Redis;
using Serilog;
using System.Diagnostics;

public static class Hosting
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddAllElasticApm();

        builder.Services.AddCacheServiceAppCoreServices();
        builder.Services.AddCacheServiceRedisPersistenceServices(builder.Configuration);

        builder.Services.AddValidatorsFromAssemblyContaining<GetFromCacheRequestValidator>();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["*"];
                var allowedMethods = builder.Configuration.GetSection("Cors:AllowedMethods").Get<string[]>() ?? ["*"];
                var allowedHeaders = builder.Configuration.GetSection("Cors:AllowedHeaders").Get<string[]>() ?? ["*"];

                if (allowedOrigins.Contains("*"))
                    policy.AllowAnyOrigin();
                else
                    policy.WithOrigins(allowedOrigins);

                if (allowedMethods.Contains("*"))
                    policy.AllowAnyMethod();
                else
                    policy.WithMethods(allowedMethods);

                if (allowedHeaders.Contains("*"))
                    policy.AllowAnyHeader();
                else
                    policy.WithHeaders(allowedHeaders);
            });
        });

        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Cache API",
                Version = "v1",
                Description = "Redis cache and vector API"
            });
        });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;

        app.UseAllElasticApm(app.Configuration);

        app.UseResponseCompression();
        app.UseHttpsRedirection();
        app.UseCors();

        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress);
            };
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cache API v1");
                c.RoutePrefix = string.Empty;
            });
        }

        app.MapControllers();

        return app;
    }
}

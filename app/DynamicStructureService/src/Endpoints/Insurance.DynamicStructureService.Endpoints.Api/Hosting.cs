namespace Insurance.DynamicStructureService.Endpoints.Api;

using Elastic.Apm.NetCoreAll;
using Insurance.DynamicStructureService.Endpoints.Api.Extensions.Db;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using OysterFx.Endpoints.Api.Extensions.DI;
using System.Diagnostics;

public static class Hosting
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.Services.AddAllElasticApm();
        builder.Services.AddOysterFxApiCore("OysterFx", "Insurance", "Insurance.DynamicStructureService");
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddDatabase(builder.Configuration);
        builder.Services.AddHealthChecks();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Dynamic Structure Service API",
                Version = "v1",
                Description = "Dynamic structure/form definition service extracted from core."
            });
        });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseOysterFxApiExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;
        app.UseAllElasticApm(app.Configuration);
        app.UseHttpsRedirection();
        app.MapHealthChecks("/health", new HealthCheckOptions());
        app.MapControllers();
        return app;
    }
}

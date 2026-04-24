namespace Insurance.GraphService.Endpoints.Api;

using Elastic.Apm.NetCoreAll;
using Insurance.GraphService.Infra.InternalServices.Neo4j.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using OysterFx.Endpoints.Api.Extensions.DI;
using OysterFx.Infra.EventBus.RabbitMqBroker;
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
        builder.Services.AddOysterFxApiCore("OysterFx", "Insurance", "Insurance.GraphService");
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHealthChecks();
        builder.Services.AddGraphNeo4jServices(builder.Configuration);
        builder.Services.AddRabbitMqConsumer(builder.Configuration);
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Graph Service API",
                Version = "v1",
                Description = "Graph node/relation service extracted from core."
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
        app.Services.ReceiveEventFromRabbitMqMessageBus(new KeyValuePair<string, string>("InventoryService", "graph.projection.node.upsert.requested.v1"));
        app.Services.ReceiveEventFromRabbitMqMessageBus(new KeyValuePair<string, string>("InventoryService", "graph.projection.relation.upsert.requested.v1"));
        app.MapHealthChecks("/health", new HealthCheckOptions());
        app.MapControllers();
        return app;
    }
}

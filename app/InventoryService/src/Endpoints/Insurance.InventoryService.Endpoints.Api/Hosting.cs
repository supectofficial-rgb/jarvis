namespace Insurance.InventoryService.Endpoints.Api;

using Elastic.Apm.NetCoreAll;
using Insurance.CacheService.Infra.CallerService.Models;
using Insurance.InventoryService.Endpoints.Api.Authorization;
using Insurance.InventoryService.Endpoints.Api.Extensions.Db;
using Insurance.InventoryService.Infra.InternalServices.GraphApiCaller.Models;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using OysterFx.Endpoints.Api.Extensions.DI;
using OysterFx.Infra.Auth.JwtServices;
using OysterFx.Infra.EventBus.Outbox;
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
        builder.Services.AddOysterFxApiCore("OysterFx", "Insurance", "Insurance.InventoryService");
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddInventoryJwtAuthentication(builder.Configuration);
        builder.Services.AddJwtUserInfoService();
        builder.Services.AddCacheServices(builder.Configuration);
        builder.Services.AddDatabase(builder.Configuration);
        builder.Services.AddGraphProjectionServices(builder.Configuration);
        builder.Services.AddRabbitMqProducer(builder.Configuration);
        builder.Services.AddDomainEventOutboxDispatcher<InventoryServiceCommandDbContext>(builder.Configuration);
        builder.Services.AddHealthChecks();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Inventory Service API",
                Version = "v1",
                Description = "Inventory core service bootstrap for documents, transactions and stock truth."
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
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseTokenProcessing();
        app.MapHealthChecks("/health", new HealthCheckOptions());
        app.MapControllers().RequireAuthorization();
        return app;
    }
}

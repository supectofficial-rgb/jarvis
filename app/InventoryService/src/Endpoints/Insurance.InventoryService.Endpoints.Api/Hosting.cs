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
using System.Reflection;

public static class Hosting
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        // Telemetry registration deliberately stays close to the platform bootstrap sequence.
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

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
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
                }
            });

            var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }
        });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseOysterFxApiExceptionHandler();

        var swaggerEnabled = app.Configuration.GetValue("Swagger:Enabled", app.Environment.IsDevelopment());
        if (swaggerEnabled)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;
        app.UseAllElasticApm(app.Configuration);
        app.UseHttpsRedirection();
        // inventory api deployment trace marker
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseTokenProcessing();
        app.MapHealthChecks("/health", new HealthCheckOptions());
        app.MapControllers().RequireAuthorization();
        return app;
    }
}

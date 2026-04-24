namespace Insurance.CoreService.Endpoints.Api;

using Elastic.Apm.NetCoreAll;
using HealthChecks.UI.Client;
using Insurance.CacheService.Infra.CallerService.Models;
using Insurance.CoreService.Endpoints.Api.Extensions;
using Insurance.CoreService.Endpoints.Api.Extensions.Db;
using Insurance.CoreService.Endpoints.Api.Extensions.Permissions;
using Insurance.CoreService.Endpoints.Api.Extensions.Swaggers.Extentions;
using Insurance.CoreService.Endpoints.Api.Middlewares;
using Insurance.Infra.InternalServices.UserApiCaller.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using OysterFx.Endpoints.Api.Extensions.DI;
using OysterFx.Endpoints.Api.ModelBindings;
using OysterFx.Infra.Auth.JwtServices;
using System.Diagnostics;

public static class Hosting
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        IConfiguration configuration = builder.Configuration;

        builder.Services.AddAllElasticApm();

        builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        // ثبت Authorization Handler
        builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        builder.Services.AddOysterFxApiCore("OysterFx", "Insurance", "Insurance.CoreService");
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddNonValidatingValidator();
        builder.Services.AddDatabase(configuration);
        builder.Services.AddHealthCheckServices(configuration);

        builder.Services.AddSwaggerDocumentation("Insurance Core Service API", "v1");

        builder.Services.AddUserServices(builder.Configuration);
        builder.Services.AddJwtAuthentication(configuration);
        builder.Services.AddJwtUserInfoService();
        builder.Services.AddRateLimitServices(configuration);
        builder.Services.AddCacheServices(builder.Configuration);
        return builder.Build();

    }
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseOysterFxApiExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerMiddleware("CoreService API v1");
        }
        app.UseMiddleware<BusinessRuleExceptionMiddleware>();
        app.UseStatusCodePages();

        app.UseCors(delegate (CorsPolicyBuilder builder)
        {
            builder.AllowAnyOrigin();
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
        });

        app.UseHttpsRedirection();
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,  // Include all checks
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("database"),  // Only include external dependencies
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false  // No checks - just return 200 if service is reachable
        });

        if (bool.Parse(app.Configuration["Jwt:Enabled"]!))
        {
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseTokenProcessing();
        }

        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;
        
        app.UseAllElasticApm(app.Configuration);

        var controllerBuilder = app.MapControllers();
        return app;
    }
}

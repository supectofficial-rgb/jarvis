namespace Insurance.HubService.Endpoints.Api;

using Elastic.Apm.NetCoreAll;
using Insurance.HubService.AppCore.Shared.Conversations.Options;
using Insurance.HubService.Endpoints.Api.Extensions.Db;
using Insurance.HubService.Endpoints.Api.Hubs;
using Insurance.HubService.Infra.InternalServices.AiAssistantApiCaller.Models;
using Insurance.HubService.Infra.Persistence.RDB.Commands;
using OysterFx.Endpoints.Api.Extensions.DI;
using Serilog;

public static class Hosting
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        IConfiguration configuration = builder.Configuration;

        builder.Host.UseSerilog((context, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(context.Configuration));

        builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.Services.AddAllElasticApm();
        builder.Services.AddOysterFxApiCore("OysterFx", "Insurance", "Insurance.HubService");
        builder.Services.AddSignalR();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();

        builder.Services.Configure<HubConversationOptions>(configuration.GetSection(HubConversationOptions.SectionName));
        builder.Services.AddHubServiceCommandPersistenceServices(configuration);
        builder.Services.AddAiAssistantServices(configuration);
        builder.Services.AddDatabase(configuration);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["*"];

                if (allowedOrigins.Contains("*", StringComparer.Ordinal))
                {
                    policy.AllowAnyOrigin();
                }
                else
                {
                    policy.WithOrigins(allowedOrigins);
                }

                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
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

        app.UseAllElasticApm(app.Configuration);

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

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthorization();

        app.MapHealthChecks("/health");
        app.MapControllers();
        app.MapHub<LiveAssistHub>("/hubs/live-assist");

        return app;
    }
}

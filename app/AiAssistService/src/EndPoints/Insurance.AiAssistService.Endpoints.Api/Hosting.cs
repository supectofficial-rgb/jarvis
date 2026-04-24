namespace Insurance.AiAssistService.Endpoints.Api;

using Insurance.AiAssistService.AppCore.AppServices;
using Insurance.AiAssistService.Infra.InternalServices.LLM;
using Insurance.AiAssistService.Infra.InternalServices.STT;
using Insurance.CacheService.Infra.CallerService.Models;
using OysterFx.Endpoints.Api.Extensions.DI;
using OysterFx.Endpoints.Api.ModelBindings;
using Serilog;

public static class Hosting
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddOysterFxApiCore("OysterFx", "Insurance", "Insurance.AiAssistService");
        builder.Services.AddNonValidatingValidator();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCacheServices(builder.Configuration);
        builder.Services.AddAiAssistAppCoreServices(builder.Configuration);
        builder.Services.AddAiAssistSttServices();
        builder.Services.AddAiAssistLlmServices();

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
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}

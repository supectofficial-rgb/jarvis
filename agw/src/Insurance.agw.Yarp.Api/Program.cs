using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Winton.Extensions.Configuration.Consul;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

var consulEnabled = builder.Configuration.GetValue<bool>("Consul:Enabled", false);
if (consulEnabled)
{
    var consulAddress = builder.Configuration["Consul:Address"] ?? "http://localhost:8500";
    var consulKey = builder.Configuration["Consul:Key"] ?? "yarp-api-gateway/appsettings.json";
    var consulToken = builder.Configuration["Consul:Token"];

    builder.Configuration.AddConsul(
        consulKey,
        options =>
        {
            options.ConsulConfigurationOptions = cco =>
            {
                cco.Address = new Uri(consulAddress);
                if (!string.IsNullOrEmpty(consulToken))
                {
                    cco.Token = consulToken;
                }
            };
            options.Optional = true;
            options.ReloadOnChange = true;
            options.OnLoadException = exceptionContext =>
            {
                Console.WriteLine($"Error loading configuration from Consul: {exceptionContext.Exception.Message}");
                exceptionContext.Ignore = true;
            };
        })
        .AddEnvironmentVariables();
}

var config = builder.Configuration;

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1",
        Description = "YARP API Gateway with Swagger documentation for all downstream services"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new List<string>()
        }
    });
});

builder.Services.AddAllElasticApm();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "your-issuer",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "your-audience",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "your-secret-key-minimum-32-characters-long"))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authenticated", policy =>
        policy.RequireAuthenticatedUser());
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformBuilderContext =>
    {
        var routeId = transformBuilderContext.Route.RouteId;
        var apiKeys = builder.Configuration.GetSection("ApiKeys");
        var apiKey = apiKeys[routeId];

        if (!string.IsNullOrEmpty(apiKey))
        {
            transformBuilderContext.AddRequestTransform(async context =>
            {
                context.ProxyRequest.Headers.Add("X-API-Key", apiKey);

                // Forward Bearer token
                var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    context.ProxyRequest.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
            });
        }
    })
    .ConfigureHttpClient((context, handler) =>
    {
        handler.ActivityHeadersPropagator = DistributedContextPropagator.CreateDefaultPropagator();
    });

builder.Services.AddHealthChecks();

var app = builder.Build();


app.MapGet("/test-apm", async (context) =>
{
    await context.Response.WriteAsync("APM Test");
});

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseAllElasticApm(builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapReverseProxy();
app.MapGet("/", () => "Yarp API Gateway with Elastic APM + Tracing");

app.Run();
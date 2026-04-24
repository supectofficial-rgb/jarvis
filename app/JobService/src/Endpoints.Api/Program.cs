using Elastic.Apm.NetCoreAll;
using Hangfire;
using Hangfire.EntityFrameworkCore;
using Insurance.JobService.Endpoints.Api.Data;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.EventBus.Outbox;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add Elastic APM
builder.Services.AddAllElasticApm();

builder.Services.AddDbContext<JobServiceDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);

        sqlOptions.MigrationsAssembly(typeof(JobServiceDbContext).Assembly.FullName);
        sqlOptions.CommandTimeout(30);
    });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
}, ServiceLifetime.Scoped);

builder.Services.AddDbContextFactory<JobServiceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfire((serviceProvider, configuration) =>
    configuration.UseEFCoreStorage(
        () => serviceProvider.GetRequiredService<IDbContextFactory<JobServiceDbContext>>().CreateDbContext(),
        new EFCoreStorageOptions
        {
            CountersAggregationInterval = TimeSpan.FromMinutes(5),
            DistributedLockTimeout = TimeSpan.FromMinutes(10),
            JobExpirationCheckInterval = TimeSpan.FromMinutes(30),
            QueuePollInterval = TimeSpan.FromSeconds(15)
        }));

builder.Services.AddHangfireServer();

builder.Services.AddControllers();
builder.Services.AddEventOutbox<JobServiceDbContext>();

var app = builder.Build();

// Configure W3C Activity format for distributed tracing
Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

// Use Elastic APM
app.UseAllElasticApm(builder.Configuration);

app.UseHangfireDashboard("/hangfire");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

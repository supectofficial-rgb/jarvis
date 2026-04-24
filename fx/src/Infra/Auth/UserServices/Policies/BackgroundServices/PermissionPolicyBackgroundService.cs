namespace OysterFx.Infra.Auth.UserServices.Policies.BackgroundServices;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OysterFx.Infra.Auth.UserServices.Policies.Abstractions;

public class PermissionPolicyBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PermissionPolicyBackgroundService> _logger;
    private readonly AutoPermissionPolicyOptions _options;

    public PermissionPolicyBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<PermissionPolicyBackgroundService> logger,
        AutoPermissionPolicyOptions options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.EnablePeriodicUpdate)
            return;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdatePermissionPoliciesAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(_options.UpdateIntervalMinutes), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permission policies");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task UpdatePermissionPoliciesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var policyService = scope.ServiceProvider.GetRequiredService<IPermissionPolicyService>();
        var authorizationOptions = scope.ServiceProvider.GetRequiredService<IOptions<AuthorizationOptions>>();

        await policyService.RegisterAllPermissionsAsync(authorizationOptions.Value);

        _logger.LogInformation("Permission policies updated successfully");
    }
}
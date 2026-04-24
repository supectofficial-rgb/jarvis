namespace OysterFx.Infra.Auth.UserServices.Policies;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OysterFx.Infra.Auth.UserServices.Policies.Abstractions;
using OysterFx.Infra.Auth.UserServices.Policies.BackgroundServices;
using System;
using System.Threading.Tasks;

public static class PermissionPolicyServiceExtensions
{
    public static IServiceCollection AddAutoPermissionPolicies(
        this IServiceCollection services, 
        Action<AutoPermissionPolicyOptions>? configureOptions = null)
    {
        var options = new AutoPermissionPolicyOptions();
        configureOptions?.Invoke(options);

        services.AddSingleton(options);

        if (options.EnablePeriodicUpdate)
            services.AddHostedService<PermissionPolicyBackgroundService>();

        return services;
    }

    public static async Task UsePermissionPoliciesAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var policyService = scope.ServiceProvider.GetRequiredService<IPermissionPolicyService>();
        var authorizationOptions = scope.ServiceProvider.GetRequiredService<IOptions<AuthorizationOptions>>();

        await policyService.RegisterAllPermissionsAsync(authorizationOptions.Value);
    }

    public static IServiceCollection AddPermissionPoliciesFromDatabase(this IServiceCollection services)
    {
        // این متد باید در یک جایی صدا زده شود که ServiceProvider ساخته شده
        // بنابراین بهتر است از IStartupFilter استفاده کنیم
        return services;
    }
}
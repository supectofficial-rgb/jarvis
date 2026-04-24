namespace Insurance.UserService.AppCore.AppServices.Permissions.Services;

using Insurance.UserService.AppCore.Shared.Permissions.Queries;
using Insurance.UserService.AppCore.Shared.Permissions.Queries.GetAll;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using OysterFx.Infra.Auth.UserServices.Policies.Abstractions;
using OysterFx.Infra.Auth.UserServices.Policies.Services;

public class PermissionPolicyService : IPermissionPolicyService
{
    private readonly ILogger<PermissionPolicyService> _logger;
    private readonly IPermissionQueryRepository _permissionQueryRepository;

    public PermissionPolicyService(
        ILogger<PermissionPolicyService> logger,
        IPermissionQueryRepository permissionQueryRepository)
    {
        _logger = logger;
        _permissionQueryRepository = permissionQueryRepository;
    }

    public async Task RegisterAllPermissionsAsync(AuthorizationOptions options)
    {
        try
        {
            var permissions = await _permissionQueryRepository.QueryAsync(new GetAllPermissionsQuery());

            _logger.LogInformation("Found {Count} permissions in database", permissions.Count());

            foreach (var permission in permissions)
            {
                if (!string.IsNullOrEmpty(permission.Code))
                {
                    options.AddPolicy(permission.Code, policy =>
                    {
                        policy.Requirements.Add(new PermissionRequirement(permission.Code));
                    });

                    _logger.LogDebug("Registered policy for permission: {Permission}", permission);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering permissions from database");
            throw;
        }
    }
}
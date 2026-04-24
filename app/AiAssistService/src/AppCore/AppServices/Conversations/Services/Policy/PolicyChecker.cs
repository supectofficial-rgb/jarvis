using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Caching;
using Insurance.CacheService.Infra.CallerService.Abstractions;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.GetFromCache;
using Microsoft.Extensions.Logging;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Policy;

public sealed class PolicyChecker : IPolicyChecker
{
    private readonly ICacheServiceCaller _cacheServiceCaller;
    private readonly ILogger<PolicyChecker> _logger;

    public PolicyChecker(ICacheServiceCaller cacheServiceCaller, ILogger<PolicyChecker> logger)
    {
        _cacheServiceCaller = cacheServiceCaller;
        _logger = logger;
    }

    public Task<PolicyCheckResult> PreCheckAsync(string? userId, string permissionKey, string? accessToken, CancellationToken cancellationToken)
        => CheckAsync(userId, permissionKey, accessToken, cancellationToken);

    public Task<PolicyCheckResult> FinalCheckAsync(string? userId, string permissionKey, string? accessToken, CancellationToken cancellationToken)
        => CheckAsync(userId, permissionKey, accessToken, cancellationToken);

    private async Task<PolicyCheckResult> CheckAsync(string? userId, string permissionKey, string? accessToken, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return new PolicyCheckResult
            {
                Allowed = false,
                AuthRequired = true,
                Reason = "Authentication token is missing."
            };
        }

        if (string.IsNullOrWhiteSpace(permissionKey))
        {
            return new PolicyCheckResult
            {
                Allowed = true,
                AuthRequired = false,
                Reason = string.Empty
            };
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            return new PolicyCheckResult
            {
                Allowed = false,
                AuthRequired = false,
                Reason = "User id is missing for permission check."
            };
        }

        var cacheKey = AiAssistCacheKeys.UserPermissions(userId.Trim());
        var permissionsCsv = await _cacheServiceCaller.GetAsync(new GetFromCacheRequest(cacheKey));

        if (permissionsCsv.Error is not null && permissionsCsv.Error.Any())
        {
            _logger.LogWarning("Permission cache read failed for {UserId}: {Errors}", userId, string.Join(" | ", permissionsCsv.Error));
            return new PolicyCheckResult
            {
                Allowed = false,
                AuthRequired = false,
                Reason = "Permission cache is unavailable."
            };
        }

        if (string.IsNullOrWhiteSpace(permissionsCsv.Success?.Value))
        {
            return new PolicyCheckResult
            {
                Allowed = false,
                AuthRequired = false,
                Reason = "No cached permissions found for user."
            };
        }

        var allowed = permissionsCsv.Success.Value
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Contains(permissionKey, StringComparer.OrdinalIgnoreCase);

        return new PolicyCheckResult
        {
            Allowed = allowed,
            AuthRequired = false,
            Reason = allowed ? string.Empty : $"Permission '{permissionKey}' was not granted for the user."
        };
    }
}

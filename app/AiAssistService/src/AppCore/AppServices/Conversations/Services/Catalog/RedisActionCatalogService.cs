using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Caching;
using Insurance.CacheService.Infra.CallerService.Abstractions;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.GetFromCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.SetToCache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;

public sealed class RedisActionCatalogService : IActionCatalogService
{
    private static readonly IReadOnlyList<ActionMetadata> DefaultCatalog =
    [
        new ActionMetadata
        {
            ActionName = "login_by_credential",
            Description = "Authenticate user with username and password.",
            Aliases = ["login", "sign in", "signin"],
            RequiredParams = ["userName", "password"],
            OptionalParams = [],
            PermissionKey = "Auth.Login",
            IsAsync = false,
            ConfirmationRequired = false,
            DestinationService = "UserService",
            EndpointMetadata = "POST /api/UserService/Auth/login/by-credential"
        },
        new ActionMetadata
        {
            ActionName = "get_policy_status",
            Description = "Get policy status by policy number.",
            Aliases = ["policy status", "insurance status"],
            RequiredParams = ["policyNumber"],
            OptionalParams = [],
            PermissionKey = "Policy.Status.Read",
            IsAsync = false,
            ConfirmationRequired = false,
            DestinationService = "CoreService",
            EndpointMetadata = "GET /api/CoreService/Policies/status"
        }
    ];

    private readonly ICacheServiceCaller _cacheServiceCaller;
    private readonly IOptions<AiAssistCacheOptions> _cacheOptions;
    private readonly ILogger<RedisActionCatalogService> _logger;

    public RedisActionCatalogService(
        ICacheServiceCaller cacheServiceCaller,
        IOptions<AiAssistCacheOptions> cacheOptions,
        ILogger<RedisActionCatalogService> logger)
    {
        _cacheServiceCaller = cacheServiceCaller;
        _cacheOptions = cacheOptions;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ActionMetadata>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = AiAssistCacheKeys.ActionCatalog();
        var getResponse = await _cacheServiceCaller.GetAsync(new GetFromCacheRequest(key));

        if (getResponse.Error is null || !getResponse.Error.Any())
        {
            var payload = getResponse.Success?.Value;
            if (!string.IsNullOrWhiteSpace(payload))
            {
                try
                {
                    var catalog = JsonSerializer.Deserialize<List<ActionMetadata>>(payload);
                    if (catalog is { Count: > 0 })
                    {
                        return catalog;
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize cached action catalog from key {CacheKey}", key);
                }
            }
        }
        else
        {
            _logger.LogWarning("Cache get failed for action catalog key {CacheKey}: {Errors}", key, string.Join(" | ", getResponse.Error));
        }

        await SeedCatalogAsync(cancellationToken);
        return DefaultCatalog;
    }

    public async Task<ActionMetadata?> FindByActionNameAsync(string actionName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(actionName))
        {
            return null;
        }

        var catalog = await GetAllAsync(cancellationToken);
        return catalog.FirstOrDefault(x => x.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase));
    }

    private async Task SeedCatalogAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request = new SetToCacheRequest(
            Key: AiAssistCacheKeys.ActionCatalog(),
            Value: JsonSerializer.Serialize(DefaultCatalog),
            AbsoluteExpirationMinutes: _cacheOptions.Value.CatalogAbsoluteExpirationMinutes);

        var setResponse = await _cacheServiceCaller.SetAsync(request);
        if (setResponse.Error is not null && setResponse.Error.Any())
        {
            _logger.LogWarning("Cache set failed for action catalog key {CacheKey}: {Errors}", request.Key, string.Join(" | ", setResponse.Error));
        }
    }
}


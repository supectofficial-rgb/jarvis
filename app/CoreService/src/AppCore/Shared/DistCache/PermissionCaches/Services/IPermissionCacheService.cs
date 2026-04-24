namespace Insurance.AppCore.Shared.PermissionCaches.Services;

public interface IPermissionCacheService
{
    Task<IReadOnlyList<string>> GetAsync(long userId);
    Task SetAsync(long userId, IReadOnlyList<string> permissions);
}
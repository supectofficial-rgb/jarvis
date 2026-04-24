namespace Insurance.CacheService.Infra.CallerService.Abstractions;

using Insurance.CacheService.Infra.CallerService.Models.Dtos.ExistsInCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.GetFromCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.IncrementInCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.RemoveFromCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.SetIfNotExistsInCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.SetToCache;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.Vector.CreateIndex;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.Vector.Delete;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.Vector.Search;
using Insurance.CacheService.Infra.CallerService.Models.Dtos.Vector.Upsert;

public interface ICacheServiceCaller
{
    Task<(GetFromCacheResponse Success, List<string> Error)> GetAsync(GetFromCacheRequest request);
    Task<(SetToCacheResponse Success, List<string> Error)> SetAsync(SetToCacheRequest request);
    Task<(RemoveFromCacheResponse Success, List<string> Error)> RemoveAsync(RemoveFromCacheRequest request);
    Task<(ExistsInCacheResponse Success, List<string> Error)> ExistsAsync(ExistsInCacheRequest request);
    Task<(SetIfNotExistsInCacheResponse Success, List<string> Error)> SetIfNotExistsAsync(SetIfNotExistsInCacheRequest request);
    Task<(IncrementInCacheResponse Success, List<string> Error)> IncrementAsync(IncrementInCacheRequest request);

    Task<(CreateVectorIndexResponse Success, List<string> Error)> CreateVectorIndexAsync(CreateVectorIndexRequest request);
    Task<(UpsertVectorResponse Success, List<string> Error)> UpsertVectorAsync(UpsertVectorRequest request);
    Task<(SearchVectorResponse Success, List<string> Error)> SearchVectorAsync(SearchVectorRequest request);
    Task<(DeleteVectorResponse Success, List<string> Error)> DeleteVectorAsync(DeleteVectorRequest request);
}

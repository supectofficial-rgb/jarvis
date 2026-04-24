namespace Insurance.CacheService.Infra.CallerService.Services;

using Insurance.CacheService.Infra.CallerService.Abstractions;
using Insurance.CacheService.Infra.CallerService.Models;
using Insurance.CacheService.Infra.CallerService.Models.Common;
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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class CacheServiceCaller : ICacheServiceCaller
{
    private readonly IOptions<CacheApiOptions> _cacheOptions;
    private readonly HttpService _httpService;
    private readonly ILogger<CacheServiceCaller> _logger;
    private string _baseURL = string.Empty;

    public CacheServiceCaller(
        IOptions<CacheApiOptions> cacheOptions,
        HttpService httpService,
        ILogger<CacheServiceCaller> logger)
    {
        _cacheOptions = cacheOptions;
        _httpService = httpService;
        _logger = logger;
        _baseURL = _cacheOptions.Value.BaseUrl;
    }

    public async Task<(GetFromCacheResponse Success, List<string> Error)> GetAsync(GetFromCacheRequest request)
    {
        _logger.LogInformation("Begin getAsync {@request}", request);
        var response = await _httpService.PostAsync<GetFromCacheRequest, GetFromCacheResponse, List<string>>(
            $"{_baseURL.TrimEnd('/')}/cache/get", request);

        _logger.LogInformation("End getAsync {@response}", response);

        return await Task.FromResult(response!);
    }

    public async Task<(RemoveFromCacheResponse Success, List<string> Error)> RemoveAsync(RemoveFromCacheRequest request)
    {
        var response = await _httpService.PostAsync<RemoveFromCacheRequest, RemoveFromCacheResponse, List<string>>(
            $"{_baseURL.TrimEnd('/')}/cache/remove", request);

        return await Task.FromResult(response!);
    }

    public async Task<(SetToCacheResponse Success, List<string> Error)> SetAsync(SetToCacheRequest request)
    {
        var response = await _httpService.PostAsync<SetToCacheRequest, SetToCacheResponse, List<string>>(
            $"{_baseURL.TrimEnd('/')}/cache/set", request);

        return await Task.FromResult(response!);
    }

    public async Task<(ExistsInCacheResponse Success, List<string> Error)> ExistsAsync(ExistsInCacheRequest request)
    {
        var response = await _httpService.PostAsync<ExistsInCacheRequest, ExistsInCacheResponse, List<string>>(
            $"{_baseURL.TrimEnd('/')}/cache/exists", request);

        return await Task.FromResult(response!);
    }

    public async Task<(SetIfNotExistsInCacheResponse Success, List<string> Error)> SetIfNotExistsAsync(SetIfNotExistsInCacheRequest request)
    {
        var response = await _httpService.PostAsync<SetIfNotExistsInCacheRequest, SetIfNotExistsInCacheResponse, List<string>>(
            $"{_baseURL.TrimEnd('/')}/cache/set-if-not-exists", request);

        return await Task.FromResult(response!);
    }

    public async Task<(IncrementInCacheResponse Success, List<string> Error)> IncrementAsync(IncrementInCacheRequest request)
    {
        var response = await _httpService.PostAsync<IncrementInCacheRequest, IncrementInCacheResponse, List<string>>(
            $"{_baseURL.TrimEnd('/')}/cache/increment", request);

        return await Task.FromResult(response!);
    }

    public async Task<(CreateVectorIndexResponse Success, List<string> Error)> CreateVectorIndexAsync(CreateVectorIndexRequest request)
    {
        var response = await _httpService.PostAsync<CreateVectorIndexRequest, CreateVectorIndexResponse, List<string>>(
            $"{_baseURL.TrimEnd('/')}/vector/index/create", request);

        return await Task.FromResult(response!);
    }

    public async Task<(UpsertVectorResponse Success, List<string> Error)> UpsertVectorAsync(UpsertVectorRequest request)
    {
        var response = await _httpService.PostAsync<UpsertVectorRequest, UpsertVectorResponse, List<string>>(
            $"{_baseURL.TrimEnd('/')}/vector/upsert", request);

        return await Task.FromResult(response!);
    }

    public async Task<(SearchVectorResponse Success, List<string> Error)> SearchVectorAsync(SearchVectorRequest request)
    {
        var response = await _httpService.PostAsync<SearchVectorRequest, SearchVectorResponse, List<string>>(
            $"{_baseURL.TrimEnd('/')}/vector/search", request);

        return await Task.FromResult(response!);
    }

    public async Task<(DeleteVectorResponse Success, List<string> Error)> DeleteVectorAsync(DeleteVectorRequest request)
    {
        var response = await _httpService.PostAsync<DeleteVectorRequest, DeleteVectorResponse, List<string>>(
            $"{_baseURL.TrimEnd('/')}/vector/delete", request);

        return await Task.FromResult(response!);
    }
}

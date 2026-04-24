namespace Insurance.CacheService.Endpoints.Api.Controllers;

using FluentValidation;
using Insurance.CacheService.AppCore.AppServices.Cache.Services;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.ExistsInCache;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.GetFromCache;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.IncrementInCache;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.RemoveFromCache;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.SetIfNotExistsInCache;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.SetToCache;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("cache")]
public sealed class CacheController : ControllerBase
{
    private readonly ICacheApplicationService _cacheApplicationService;

    public CacheController(ICacheApplicationService cacheApplicationService)
    {
        _cacheApplicationService = cacheApplicationService;
    }

    [HttpPost("get")]
    public async Task<IActionResult> Get(
        [FromBody] GetFromCacheRequest request,
        [FromServices] IValidator<GetFromCacheRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var value = await _cacheApplicationService.GetAsync(request.Key, cancellationToken);
        return Ok(new GetFromCacheResponse(value));
    }

    [HttpPost("set")]
    public async Task<IActionResult> Set(
        [FromBody] SetToCacheRequest request,
        [FromServices] IValidator<SetToCacheRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _cacheApplicationService.SetAsync(
            request.Key,
            request.Value,
            request.AbsoluteExpirationMinutes,
            request.SlidingExpirationMinutes,
            cancellationToken);

        return Ok(new SetToCacheResponse(true));
    }

    [HttpPost("set-if-not-exists")]
    public async Task<IActionResult> SetIfNotExists(
        [FromBody] SetIfNotExistsInCacheRequest request,
        [FromServices] IValidator<SetIfNotExistsInCacheRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var success = await _cacheApplicationService.SetIfNotExistsAsync(
            request.Key,
            request.Value,
            request.AbsoluteExpirationMinutes,
            cancellationToken);

        return Ok(new SetIfNotExistsInCacheResponse(success));
    }

    [HttpPost("increment")]
    public async Task<IActionResult> Increment(
        [FromBody] IncrementInCacheRequest request,
        [FromServices] IValidator<IncrementInCacheRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var value = await _cacheApplicationService.IncrementAsync(
            request.Key,
            request.Value,
            request.AbsoluteExpirationMinutes,
            cancellationToken);

        return Ok(new IncrementInCacheResponse(value));
    }

    [HttpPost("exists")]
    public async Task<IActionResult> Exists(
        [FromBody] ExistsInCacheRequest request,
        [FromServices] IValidator<ExistsInCacheRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var exists = await _cacheApplicationService.ExistsAsync(request.Key, cancellationToken);
        return Ok(new ExistsInCacheResponse(exists));
    }

    [HttpPost("remove")]
    public async Task<IActionResult> Remove(
        [FromBody] RemoveFromCacheRequest request,
        [FromServices] IValidator<RemoveFromCacheRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _cacheApplicationService.RemoveAsync(request.Key, cancellationToken);
        return Ok(new RemoveFromCacheResponse(true));
    }

    [HttpGet("stats")]
    public IActionResult Stats()
    {
        return Ok(new
        {
            ServerTime = DateTime.UtcNow,
            Version = "2.0.0",
            Status = "Healthy"
        });
    }
}

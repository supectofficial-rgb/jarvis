namespace Insurance.CacheService.Endpoints.Api.Controllers;

using FluentValidation;
using Insurance.CacheService.AppCore.AppServices.Vector.Services;
using Insurance.CacheService.AppCore.Shared.Vector.Services;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.Vector.CreateIndex;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.Vector.Delete;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.Vector.Search;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.Vector.Upsert;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("vector")]
public sealed class VectorController : ControllerBase
{
    private readonly IVectorApplicationService _vectorApplicationService;

    public VectorController(IVectorApplicationService vectorApplicationService)
    {
        _vectorApplicationService = vectorApplicationService;
    }

    [HttpPost("index/create")]
    public async Task<IActionResult> CreateIndex(
        [FromBody] CreateVectorIndexRequest request,
        [FromServices] IValidator<CreateVectorIndexRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var result = await _vectorApplicationService.EnsureIndexAsync(
            new VectorIndexDefinition(
                request.IndexName,
                request.Prefix,
                request.Dimension,
                request.DistanceMetric,
                request.Algorithm),
            cancellationToken);

        return Ok(new CreateVectorIndexResponse(result.Created, result.AlreadyExists, result.Message));
    }

    [HttpPost("upsert")]
    public async Task<IActionResult> Upsert(
        [FromBody] UpsertVectorRequest request,
        [FromServices] IValidator<UpsertVectorRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _vectorApplicationService.UpsertAsync(
            request.IndexName,
            new VectorItem(request.Key, request.Embedding, request.Text, request.Namespace, request.ActionName),
            cancellationToken);

        return Ok(new UpsertVectorResponse(true));
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchVectorRequest request,
        [FromServices] IValidator<SearchVectorRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var items = await _vectorApplicationService.SearchAsync(
            request.IndexName,
            request.QueryEmbedding,
            request.TopK,
            request.NamespaceFilter,
            cancellationToken);

        var response = new SearchVectorResponse(items.Select(i => new SearchVectorMatchResponse(
            i.Key,
            i.Score,
            i.Text,
            i.Namespace,
            i.ActionName)).ToList());

        return Ok(response);
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete(
        [FromBody] DeleteVectorRequest request,
        [FromServices] IValidator<DeleteVectorRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _vectorApplicationService.DeleteAsync(request.Key, cancellationToken);
        return Ok(new DeleteVectorResponse(true));
    }
}

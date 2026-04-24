namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.ActivateQualityStatus;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.CreateQualityStatus;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.DeactivateQualityStatus;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.DeleteQualityStatus;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Commands.UpdateQualityStatus;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetActiveQualityStatuses;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetByCode;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetQualityStatusLookup;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.SearchQualityStatuses;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class QualityStatusController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateQualityStatusCommand command)
        => SendCommand<CreateQualityStatusCommand, CreateQualityStatusCommandResult>(command);

    [HttpPut("{qualityStatusBusinessKey:guid}")]
    public Task<IActionResult> Update([FromRoute] Guid qualityStatusBusinessKey, [FromBody] UpdateQualityStatusCommand command)
    {
        command.QualityStatusBusinessKey = qualityStatusBusinessKey;
        return SendCommand<UpdateQualityStatusCommand, UpdateQualityStatusCommandResult>(command);
    }

    [HttpPost("{qualityStatusBusinessKey:guid}/activate")]
    public Task<IActionResult> Activate([FromRoute] Guid qualityStatusBusinessKey)
        => SendCommand<ActivateQualityStatusCommand, ActivateQualityStatusCommandResult>(
            new ActivateQualityStatusCommand { QualityStatusBusinessKey = qualityStatusBusinessKey });

    [HttpPost("{qualityStatusBusinessKey:guid}/deactivate")]
    public Task<IActionResult> Deactivate([FromRoute] Guid qualityStatusBusinessKey)
        => SendCommand<DeactivateQualityStatusCommand, DeactivateQualityStatusCommandResult>(
            new DeactivateQualityStatusCommand { QualityStatusBusinessKey = qualityStatusBusinessKey });

    [HttpDelete("{qualityStatusBusinessKey:guid}")]
    public Task<IActionResult> Delete([FromRoute] Guid qualityStatusBusinessKey)
        => SendCommand<DeleteQualityStatusCommand, DeleteQualityStatusCommandResult>(
            new DeleteQualityStatusCommand { QualityStatusBusinessKey = qualityStatusBusinessKey });

    [HttpGet("{qualityStatusBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid qualityStatusBusinessKey)
        => ExecuteQueryAsync<GetQualityStatusByBusinessKeyQuery, GetQualityStatusByBusinessKeyQueryResult>(
            new GetQualityStatusByBusinessKeyQuery(qualityStatusBusinessKey));

    [HttpGet("by-id/{qualityStatusId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid qualityStatusId)
        => ExecuteQueryAsync<GetQualityStatusByIdQuery, GetQualityStatusByBusinessKeyQueryResult>(
            new GetQualityStatusByIdQuery(qualityStatusId));

    [HttpGet("by-code/{code}")]
    public Task<IActionResult> GetByCode([FromRoute] string code)
        => ExecuteQueryAsync<GetQualityStatusByCodeQuery, GetQualityStatusByBusinessKeyQueryResult>(
            new GetQualityStatusByCodeQuery(code));

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchQualityStatusesQuery query)
        => ExecuteQueryAsync<SearchQualityStatusesQuery, SearchQualityStatusesQueryResult>(query);

    [HttpGet("active")]
    public Task<IActionResult> GetActive()
        => ExecuteQueryAsync<GetActiveQualityStatusesQuery, GetActiveQualityStatusesQueryResult>(
            new GetActiveQualityStatusesQuery());

    [HttpGet("lookup")]
    public Task<IActionResult> GetLookup([FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetQualityStatusLookupQuery, GetQualityStatusLookupQueryResult>(
            new GetQualityStatusLookupQuery { IncludeInactive = includeInactive });
}

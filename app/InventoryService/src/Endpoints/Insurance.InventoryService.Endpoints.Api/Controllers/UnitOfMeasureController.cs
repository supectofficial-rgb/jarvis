namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.ActivateUnitOfMeasure;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.CreateUnitOfMeasure;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.DeactivateUnitOfMeasure;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.DeleteUnitOfMeasure;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Commands.UpdateUnitOfMeasure;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetActiveUnitOfMeasures;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetByCode;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetUnitOfMeasureLookup;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.SearchUnitOfMeasures;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class UnitOfMeasureController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateUnitOfMeasureCommand command)
        => SendCommand<CreateUnitOfMeasureCommand, CreateUnitOfMeasureCommandResult>(command);

    [HttpPut("{unitOfMeasureBusinessKey:guid}")]
    public Task<IActionResult> Update([FromRoute] Guid unitOfMeasureBusinessKey, [FromBody] UpdateUnitOfMeasureCommand command)
    {
        command.UnitOfMeasureBusinessKey = unitOfMeasureBusinessKey;
        return SendCommand<UpdateUnitOfMeasureCommand, UpdateUnitOfMeasureCommandResult>(command);
    }

    [HttpPost("{unitOfMeasureBusinessKey:guid}/activate")]
    public Task<IActionResult> Activate([FromRoute] Guid unitOfMeasureBusinessKey)
        => SendCommand<ActivateUnitOfMeasureCommand, ActivateUnitOfMeasureCommandResult>(
            new ActivateUnitOfMeasureCommand { UnitOfMeasureBusinessKey = unitOfMeasureBusinessKey });

    [HttpPost("{unitOfMeasureBusinessKey:guid}/deactivate")]
    public Task<IActionResult> Deactivate([FromRoute] Guid unitOfMeasureBusinessKey)
        => SendCommand<DeactivateUnitOfMeasureCommand, DeactivateUnitOfMeasureCommandResult>(
            new DeactivateUnitOfMeasureCommand { UnitOfMeasureBusinessKey = unitOfMeasureBusinessKey });

    [HttpDelete("{unitOfMeasureBusinessKey:guid}")]
    public Task<IActionResult> Delete([FromRoute] Guid unitOfMeasureBusinessKey)
        => SendCommand<DeleteUnitOfMeasureCommand, DeleteUnitOfMeasureCommandResult>(
            new DeleteUnitOfMeasureCommand { UnitOfMeasureBusinessKey = unitOfMeasureBusinessKey });

    [HttpGet("{unitOfMeasureBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid unitOfMeasureBusinessKey)
        => ExecuteQueryAsync<GetUnitOfMeasureByBusinessKeyQuery, GetUnitOfMeasureByBusinessKeyQueryResult>(
            new GetUnitOfMeasureByBusinessKeyQuery(unitOfMeasureBusinessKey));

    [HttpGet("by-id/{unitOfMeasureId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid unitOfMeasureId)
        => ExecuteQueryAsync<GetUnitOfMeasureByIdQuery, GetUnitOfMeasureByBusinessKeyQueryResult>(
            new GetUnitOfMeasureByIdQuery(unitOfMeasureId));

    [HttpGet("by-code/{code}")]
    public Task<IActionResult> GetByCode([FromRoute] string code)
        => ExecuteQueryAsync<GetUnitOfMeasureByCodeQuery, GetUnitOfMeasureByBusinessKeyQueryResult>(
            new GetUnitOfMeasureByCodeQuery(code));

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchUnitOfMeasuresQuery query)
        => ExecuteQueryAsync<SearchUnitOfMeasuresQuery, SearchUnitOfMeasuresQueryResult>(query);

    [HttpGet("active")]
    public Task<IActionResult> GetActive()
        => ExecuteQueryAsync<GetActiveUnitOfMeasuresQuery, GetActiveUnitOfMeasuresQueryResult>(
            new GetActiveUnitOfMeasuresQuery());

    [HttpGet("lookup")]
    public Task<IActionResult> GetLookup([FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetUnitOfMeasureLookupQuery, GetUnitOfMeasureLookupQueryResult>(
            new GetUnitOfMeasureLookupQuery { IncludeInactive = includeInactive });
}

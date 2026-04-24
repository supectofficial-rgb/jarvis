namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.ActivateWarehouse;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.CreateWarehouse;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.DeactivateWarehouse;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.DeleteWarehouse;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Commands.UpdateWarehouse;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetActiveWarehouses;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetByCode;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetWarehouseLookup;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetWarehouseSummary;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetWarehouseWithLocations;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.SearchWarehouses;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class WarehouseController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateWarehouseCommand command)
        => SendCommand<CreateWarehouseCommand, CreateWarehouseCommandResult>(command);

    [HttpPut("{warehouseBusinessKey:guid}")]
    public Task<IActionResult> Update([FromRoute] Guid warehouseBusinessKey, [FromBody] UpdateWarehouseCommand command)
    {
        command.WarehouseBusinessKey = warehouseBusinessKey;
        return SendCommand<UpdateWarehouseCommand, UpdateWarehouseCommandResult>(command);
    }

    [HttpPost("{warehouseBusinessKey:guid}/activate")]
    public Task<IActionResult> Activate([FromRoute] Guid warehouseBusinessKey)
        => SendCommand<ActivateWarehouseCommand, ActivateWarehouseCommandResult>(
            new ActivateWarehouseCommand { WarehouseBusinessKey = warehouseBusinessKey });

    [HttpPost("{warehouseBusinessKey:guid}/deactivate")]
    public Task<IActionResult> Deactivate([FromRoute] Guid warehouseBusinessKey)
        => SendCommand<DeactivateWarehouseCommand, DeactivateWarehouseCommandResult>(
            new DeactivateWarehouseCommand { WarehouseBusinessKey = warehouseBusinessKey });

    [HttpDelete("{warehouseBusinessKey:guid}")]
    public Task<IActionResult> Delete([FromRoute] Guid warehouseBusinessKey)
        => SendCommand<DeleteWarehouseCommand, DeleteWarehouseCommandResult>(
            new DeleteWarehouseCommand { WarehouseBusinessKey = warehouseBusinessKey });

    [HttpGet("{warehouseBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid warehouseBusinessKey)
        => ExecuteQueryAsync<GetWarehouseByBusinessKeyQuery, GetWarehouseByBusinessKeyQueryResult>(
            new GetWarehouseByBusinessKeyQuery(warehouseBusinessKey));

    [HttpGet("by-id/{warehouseId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid warehouseId)
        => ExecuteQueryAsync<GetWarehouseByIdQuery, GetWarehouseByBusinessKeyQueryResult>(
            new GetWarehouseByIdQuery(warehouseId));

    [HttpGet("by-code/{code}")]
    public Task<IActionResult> GetByCode([FromRoute] string code)
        => ExecuteQueryAsync<GetWarehouseByCodeQuery, GetWarehouseByBusinessKeyQueryResult>(
            new GetWarehouseByCodeQuery(code));

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchWarehousesQuery query)
        => ExecuteQueryAsync<SearchWarehousesQuery, SearchWarehousesQueryResult>(query);

    [HttpGet("active")]
    public Task<IActionResult> GetActive()
        => ExecuteQueryAsync<GetActiveWarehousesQuery, GetActiveWarehousesQueryResult>(
            new GetActiveWarehousesQuery());

    [HttpGet("lookup")]
    public Task<IActionResult> GetLookup([FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetWarehouseLookupQuery, GetWarehouseLookupQueryResult>(
            new GetWarehouseLookupQuery { IncludeInactive = includeInactive });

    [HttpGet("{warehouseBusinessKey:guid}/summary")]
    public Task<IActionResult> GetSummary([FromRoute] Guid warehouseBusinessKey)
        => ExecuteQueryAsync<GetWarehouseSummaryQuery, GetWarehouseSummaryQueryResult>(
            new GetWarehouseSummaryQuery(warehouseBusinessKey));

    [HttpGet("{warehouseBusinessKey:guid}/with-locations")]
    public Task<IActionResult> GetWithLocations([FromRoute] Guid warehouseBusinessKey, [FromQuery] bool includeInactiveLocations = false)
        => ExecuteQueryAsync<GetWarehouseWithLocationsQuery, GetWarehouseWithLocationsQueryResult>(
            new GetWarehouseWithLocationsQuery(warehouseBusinessKey, includeInactiveLocations));
}

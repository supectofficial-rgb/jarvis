namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.ActivateLocation;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.ChangeLocationType;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.CreateLocation;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.DeactivateLocation;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.DeleteLocation;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.MoveLocationToWarehouse;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Commands.UpdateLocation;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetActiveByWarehouseId;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByCode;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetLocationsByType;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByWarehouseId;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetLocationLookup;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.SearchLocations;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class LocationController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateLocationCommand command)
        => SendCommand<CreateLocationCommand, CreateLocationCommandResult>(command);

    [HttpPut("{locationBusinessKey:guid}")]
    public Task<IActionResult> Update([FromRoute] Guid locationBusinessKey, [FromBody] UpdateLocationCommand command)
    {
        command.LocationBusinessKey = locationBusinessKey;
        return SendCommand<UpdateLocationCommand, UpdateLocationCommandResult>(command);
    }

    [HttpPost("{locationBusinessKey:guid}/change-type")]
    public Task<IActionResult> ChangeType([FromRoute] Guid locationBusinessKey, [FromBody] ChangeLocationTypeCommand command)
    {
        command.LocationBusinessKey = locationBusinessKey;
        return SendCommand<ChangeLocationTypeCommand, ChangeLocationTypeCommandResult>(command);
    }

    [HttpPost("{locationBusinessKey:guid}/activate")]
    public Task<IActionResult> Activate([FromRoute] Guid locationBusinessKey)
        => SendCommand<ActivateLocationCommand, ActivateLocationCommandResult>(
            new ActivateLocationCommand { LocationBusinessKey = locationBusinessKey });

    [HttpPost("{locationBusinessKey:guid}/deactivate")]
    public Task<IActionResult> Deactivate([FromRoute] Guid locationBusinessKey)
        => SendCommand<DeactivateLocationCommand, DeactivateLocationCommandResult>(
            new DeactivateLocationCommand { LocationBusinessKey = locationBusinessKey });

    [HttpDelete("{locationBusinessKey:guid}")]
    public Task<IActionResult> Delete([FromRoute] Guid locationBusinessKey)
        => SendCommand<DeleteLocationCommand, DeleteLocationCommandResult>(
            new DeleteLocationCommand { LocationBusinessKey = locationBusinessKey });

    [HttpPost("{locationBusinessKey:guid}/move-warehouse/{targetWarehouseRef:guid}")]
    public Task<IActionResult> MoveToWarehouse([FromRoute] Guid locationBusinessKey, [FromRoute] Guid targetWarehouseRef)
        => SendCommand<MoveLocationToWarehouseCommand, MoveLocationToWarehouseCommandResult>(
            new MoveLocationToWarehouseCommand
            {
                LocationBusinessKey = locationBusinessKey,
                TargetWarehouseRef = targetWarehouseRef
            });

    [HttpGet("{locationBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid locationBusinessKey)
        => ExecuteQueryAsync<GetLocationByBusinessKeyQuery, GetLocationByBusinessKeyQueryResult>(
            new GetLocationByBusinessKeyQuery(locationBusinessKey));

    [HttpGet("by-id/{locationId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid locationId)
        => ExecuteQueryAsync<GetLocationByIdQuery, GetLocationByBusinessKeyQueryResult>(
            new GetLocationByIdQuery(locationId));

    [HttpGet("by-code/{locationCode}")]
    public Task<IActionResult> GetByCode([FromRoute] string locationCode)
        => ExecuteQueryAsync<GetLocationByCodeQuery, GetLocationByBusinessKeyQueryResult>(
            new GetLocationByCodeQuery(locationCode));

    [HttpGet("warehouse/{warehouseId:guid}")]
    public Task<IActionResult> GetByWarehouse([FromRoute] Guid warehouseId)
        => ExecuteQueryAsync<GetLocationsByWarehouseIdQuery, GetLocationsByWarehouseIdQueryResult>(
            new GetLocationsByWarehouseIdQuery(warehouseId));

    [HttpGet("warehouse/{warehouseId:guid}/active")]
    public Task<IActionResult> GetActiveByWarehouse([FromRoute] Guid warehouseId)
        => ExecuteQueryAsync<GetActiveLocationsByWarehouseIdQuery, GetActiveLocationsByWarehouseIdQueryResult>(
            new GetActiveLocationsByWarehouseIdQuery(warehouseId));

    [HttpGet("by-type/{locationType}")]
    public Task<IActionResult> GetByType([FromRoute] string locationType, [FromQuery] Guid? warehouseRef = null, [FromQuery] bool onlyActive = false)
        => ExecuteQueryAsync<GetLocationsByTypeQuery, GetLocationsByTypeQueryResult>(
            new GetLocationsByTypeQuery(locationType, warehouseRef, onlyActive));

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchLocationsQuery query)
        => ExecuteQueryAsync<SearchLocationsQuery, SearchLocationsQueryResult>(query);

    [HttpGet("lookup")]
    public Task<IActionResult> GetLookup([FromQuery] Guid? warehouseRef = null, [FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetLocationLookupQuery, GetLocationLookupQueryResult>(
            new GetLocationLookupQuery
            {
                WarehouseRef = warehouseRef,
                IncludeInactive = includeInactive
            });
}

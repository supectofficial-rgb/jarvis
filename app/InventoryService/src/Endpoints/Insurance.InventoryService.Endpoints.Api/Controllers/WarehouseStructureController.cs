namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands.CreateLocationStructureNode;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands.CreateLocationStructureValue;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands.UpdateLocationStructureNode;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Commands.UpdateLocationStructureValue;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.GetLocationStructureValues;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.GetWarehouseLocationStructureTree;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public sealed class WarehouseStructureController : OysterFxController
{
    [HttpPost("node")]
    public Task<IActionResult> CreateNode([FromBody] CreateLocationStructureNodeCommand command)
        => SendCommand<CreateLocationStructureNodeCommand, CreateLocationStructureNodeCommandResult>(command);

    [HttpPut("node/{locationStructureBusinessKey:guid}")]
    public Task<IActionResult> UpdateNode([FromRoute] Guid locationStructureBusinessKey, [FromBody] UpdateLocationStructureNodeCommand command)
    {
        command.LocationStructureBusinessKey = locationStructureBusinessKey;
        return SendCommand<UpdateLocationStructureNodeCommand, UpdateLocationStructureNodeCommandResult>(command);
    }

    [HttpPost("value")]
    public Task<IActionResult> CreateValue([FromBody] CreateLocationStructureValueCommand command)
        => SendCommand<CreateLocationStructureValueCommand, CreateLocationStructureValueCommandResult>(command);

    [HttpPut("value/{locationStructureValueBusinessKey:guid}")]
    public Task<IActionResult> UpdateValue([FromRoute] Guid locationStructureValueBusinessKey, [FromBody] UpdateLocationStructureValueCommand command)
    {
        command.LocationStructureValueBusinessKey = locationStructureValueBusinessKey;
        return SendCommand<UpdateLocationStructureValueCommand, UpdateLocationStructureValueCommandResult>(command);
    }

    [HttpGet("warehouse/{warehouseRef:guid}/tree")]
    public Task<IActionResult> GetTree([FromRoute] Guid warehouseRef, [FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetWarehouseLocationStructureTreeQuery, GetWarehouseLocationStructureTreeQueryResult>(
            new GetWarehouseLocationStructureTreeQuery(warehouseRef, includeInactive));

    [HttpGet("node/{locationStructureBusinessKey:guid}/values")]
    public Task<IActionResult> GetValues([FromRoute] Guid locationStructureBusinessKey, [FromQuery] bool includeInactive = false)
        => ExecuteQueryAsync<GetLocationStructureValuesQuery, GetLocationStructureValuesQueryResult>(
            new GetLocationStructureValuesQuery(locationStructureBusinessKey, includeInactive));
}

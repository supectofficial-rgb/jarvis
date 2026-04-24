namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.AssignSerialToStockDetail;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.CreateSerialItem;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.MoveSerialItem;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands.UpdateSerialItemStatus;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetAvailableSerialItems;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetBySerialNo;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetByVariant;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.SearchSerialItems;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class SerialItemController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateSerialItemCommand command)
        => SendCommand<CreateSerialItemCommand, CreateSerialItemCommandResult>(command);

    [HttpPost("{serialItemBusinessKey:guid}/status")]
    public Task<IActionResult> UpdateStatus([FromRoute] Guid serialItemBusinessKey, [FromBody] UpdateSerialItemStatusCommand command)
    {
        command.SerialItemBusinessKey = serialItemBusinessKey;
        return SendCommand<UpdateSerialItemStatusCommand, UpdateSerialItemStatusCommandResult>(command);
    }

    [HttpPost("{serialItemBusinessKey:guid}/move")]
    public Task<IActionResult> Move([FromRoute] Guid serialItemBusinessKey, [FromBody] MoveSerialItemCommand command)
    {
        command.SerialItemBusinessKey = serialItemBusinessKey;
        return SendCommand<MoveSerialItemCommand, MoveSerialItemCommandResult>(command);
    }

    [HttpPost("{serialItemBusinessKey:guid}/assign-stock-detail")]
    public Task<IActionResult> AssignStockDetail([FromRoute] Guid serialItemBusinessKey, [FromBody] AssignSerialToStockDetailCommand command)
    {
        command.SerialItemBusinessKey = serialItemBusinessKey;
        return SendCommand<AssignSerialToStockDetailCommand, AssignSerialToStockDetailCommandResult>(command);
    }

    [HttpGet("{serialItemBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid serialItemBusinessKey)
        => ExecuteQueryAsync<GetSerialItemByBusinessKeyQuery, GetSerialItemByBusinessKeyQueryResult>(
            new GetSerialItemByBusinessKeyQuery(serialItemBusinessKey));

    [HttpGet("by-id/{serialItemId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid serialItemId)
        => ExecuteQueryAsync<GetSerialItemByIdQuery, GetSerialItemByIdQueryResult>(
            new GetSerialItemByIdQuery(serialItemId));

    [HttpGet("by-serial/{serialNo}")]
    public Task<IActionResult> GetBySerialNo([FromRoute] string serialNo, [FromQuery] Guid? variantRef = null)
        => ExecuteQueryAsync<GetSerialItemBySerialNoQuery, GetSerialItemBySerialNoQueryResult>(
            new GetSerialItemBySerialNoQuery(serialNo, variantRef));

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchSerialItemsQuery query)
        => ExecuteQueryAsync<SearchSerialItemsQuery, SearchSerialItemsQueryResult>(query);

    [HttpGet("by-variant/{variantRef:guid}")]
    public Task<IActionResult> GetByVariant([FromRoute] Guid variantRef)
        => ExecuteQueryAsync<GetSerialItemsByVariantQuery, GetSerialItemsByVariantQueryResult>(
            new GetSerialItemsByVariantQuery(variantRef));

    [HttpGet("available")]
    public Task<IActionResult> GetAvailable([FromQuery] GetAvailableSerialItemsQuery query)
        => ExecuteQueryAsync<GetAvailableSerialItemsQuery, GetAvailableSerialItemsQueryResult>(query);
}

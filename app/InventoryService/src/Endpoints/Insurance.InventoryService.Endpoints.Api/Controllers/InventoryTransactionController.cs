namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands.CancelInventoryTransaction;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands.PostInventoryTransaction;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Commands.ReverseInventoryTransaction;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByNo;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByVariant;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.GetByWarehouse;
using Insurance.InventoryService.AppCore.Shared.InventoryTransactions.Queries.SearchInventoryTransactions;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class InventoryTransactionController : OysterFxController
{
    [HttpPost("{transactionBusinessKey:guid}/post")]
    public Task<IActionResult> Post([FromRoute] Guid transactionBusinessKey, [FromBody] PostInventoryTransactionCommand? command)
    {
        var request = command ?? new PostInventoryTransactionCommand();
        request.TransactionBusinessKey = transactionBusinessKey;
        return SendCommand<PostInventoryTransactionCommand, PostInventoryTransactionCommandResult>(request);
    }

    [HttpPost("{transactionBusinessKey:guid}/reverse")]
    public Task<IActionResult> Reverse([FromRoute] Guid transactionBusinessKey, [FromBody] ReverseInventoryTransactionCommand command)
    {
        command.TransactionBusinessKey = transactionBusinessKey;
        return SendCommand<ReverseInventoryTransactionCommand, ReverseInventoryTransactionCommandResult>(command);
    }

    [HttpPost("{transactionBusinessKey:guid}/cancel")]
    public Task<IActionResult> Cancel([FromRoute] Guid transactionBusinessKey, [FromBody] CancelInventoryTransactionCommand command)
    {
        command.TransactionBusinessKey = transactionBusinessKey;
        return SendCommand<CancelInventoryTransactionCommand, CancelInventoryTransactionCommandResult>(command);
    }

    [HttpGet("{transactionBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid transactionBusinessKey)
        => ExecuteQueryAsync<GetInventoryTransactionByBusinessKeyQuery, GetInventoryTransactionByBusinessKeyQueryResult>(
            new GetInventoryTransactionByBusinessKeyQuery(transactionBusinessKey));

    [HttpGet("by-id/{transactionId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid transactionId)
        => ExecuteQueryAsync<GetInventoryTransactionByIdQuery, GetInventoryTransactionByIdQueryResult>(
            new GetInventoryTransactionByIdQuery(transactionId));

    [HttpGet("by-no/{transactionNo}")]
    public Task<IActionResult> GetByNo([FromRoute] string transactionNo)
        => ExecuteQueryAsync<GetInventoryTransactionByNoQuery, GetInventoryTransactionByNoQueryResult>(
            new GetInventoryTransactionByNoQuery(transactionNo));

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchInventoryTransactionsQuery query)
        => ExecuteQueryAsync<SearchInventoryTransactionsQuery, SearchInventoryTransactionsQueryResult>(query);

    [HttpGet("by-variant/{variantRef:guid}")]
    public Task<IActionResult> GetByVariant([FromRoute] Guid variantRef)
        => ExecuteQueryAsync<GetInventoryTransactionsByVariantQuery, GetInventoryTransactionsByVariantQueryResult>(
            new GetInventoryTransactionsByVariantQuery(variantRef));

    [HttpGet("by-warehouse/{warehouseRef:guid}")]
    public Task<IActionResult> GetByWarehouse([FromRoute] Guid warehouseRef)
        => ExecuteQueryAsync<GetInventoryTransactionsByWarehouseQuery, GetInventoryTransactionsByWarehouseQueryResult>(
            new GetInventoryTransactionsByWarehouseQuery(warehouseRef));
}

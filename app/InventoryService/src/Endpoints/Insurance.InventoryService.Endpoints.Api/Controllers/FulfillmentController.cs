namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands.CreateFulfillment;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands.MarkFulfillmentPacked;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands.MarkFulfillmentPicked;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands.MarkFulfillmentReturned;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Commands.MarkFulfillmentShipped;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetByOrder;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetByStatus;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetFulfillmentSummary;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class FulfillmentController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateFulfillmentCommand command)
        => SendCommand<CreateFulfillmentCommand, Guid>(command);

    [HttpPost("{fulfillmentBusinessKey:guid}/pick")]
    public Task<IActionResult> MarkPicked([FromRoute] Guid fulfillmentBusinessKey, [FromBody] MarkFulfillmentPickedCommand? command)
    {
        var request = command ?? new MarkFulfillmentPickedCommand();
        request.FulfillmentBusinessKey = fulfillmentBusinessKey;
        return SendCommand<MarkFulfillmentPickedCommand, Guid>(request);
    }

    [HttpPost("{fulfillmentBusinessKey:guid}/pack")]
    public Task<IActionResult> MarkPacked([FromRoute] Guid fulfillmentBusinessKey, [FromBody] MarkFulfillmentPackedCommand? command)
    {
        var request = command ?? new MarkFulfillmentPackedCommand();
        request.FulfillmentBusinessKey = fulfillmentBusinessKey;
        return SendCommand<MarkFulfillmentPackedCommand, Guid>(request);
    }

    [HttpPost("{fulfillmentBusinessKey:guid}/ship")]
    public Task<IActionResult> MarkShipped([FromRoute] Guid fulfillmentBusinessKey, [FromBody] MarkFulfillmentShippedCommand? command)
    {
        var request = command ?? new MarkFulfillmentShippedCommand();
        request.FulfillmentBusinessKey = fulfillmentBusinessKey;
        return SendCommand<MarkFulfillmentShippedCommand, Guid>(request);
    }

    [HttpPost("{fulfillmentBusinessKey:guid}/return")]
    public Task<IActionResult> MarkReturned([FromRoute] Guid fulfillmentBusinessKey, [FromBody] MarkFulfillmentReturnedCommand? command)
    {
        var request = command ?? new MarkFulfillmentReturnedCommand();
        request.FulfillmentBusinessKey = fulfillmentBusinessKey;
        return SendCommand<MarkFulfillmentReturnedCommand, Guid>(request);
    }

    [HttpGet("{fulfillmentBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid fulfillmentBusinessKey)
        => ExecuteQueryAsync<GetFulfillmentByBusinessKeyQuery, GetFulfillmentByBusinessKeyQueryResult>(
            new GetFulfillmentByBusinessKeyQuery(fulfillmentBusinessKey));

    [HttpGet("by-id/{fulfillmentId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid fulfillmentId)
        => ExecuteQueryAsync<GetFulfillmentByIdQuery, GetFulfillmentByIdQueryResult>(
            new GetFulfillmentByIdQuery(fulfillmentId));

    [HttpGet("by-order/{orderRef:guid}")]
    public Task<IActionResult> GetByOrder([FromRoute] Guid orderRef)
        => ExecuteQueryAsync<GetFulfillmentsByOrderQuery, GetFulfillmentsByOrderQueryResult>(
            new GetFulfillmentsByOrderQuery(orderRef));

    [HttpGet("by-status/{status}")]
    public Task<IActionResult> GetByStatus([FromRoute] string status)
        => ExecuteQueryAsync<GetFulfillmentsByStatusQuery, GetFulfillmentsByStatusQueryResult>(
            new GetFulfillmentsByStatusQuery(status));

    [HttpGet("{fulfillmentBusinessKey:guid}/summary")]
    public Task<IActionResult> GetSummary([FromRoute] Guid fulfillmentBusinessKey)
        => ExecuteQueryAsync<GetFulfillmentSummaryQuery, GetFulfillmentSummaryQueryResult>(
            new GetFulfillmentSummaryQuery(fulfillmentBusinessKey));
}

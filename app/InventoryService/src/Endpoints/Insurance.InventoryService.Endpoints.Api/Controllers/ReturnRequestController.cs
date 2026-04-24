namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.Returns.Commands.ApproveReturnRequest;
using Insurance.InventoryService.AppCore.Shared.Returns.Commands.CloseReturnRequest;
using Insurance.InventoryService.AppCore.Shared.Returns.Commands.CreateReturnRequest;
using Insurance.InventoryService.AppCore.Shared.Returns.Commands.ReceiveReturnRequest;
using Insurance.InventoryService.AppCore.Shared.Returns.Commands.RejectReturnRequest;
using Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetByOrder;
using Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetByStatus;
using Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetReturnSummary;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class ReturnRequestController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateReturnRequestCommand command)
        => SendCommand<CreateReturnRequestCommand, Guid>(command);

    [HttpPost("{returnRequestBusinessKey:guid}/approve")]
    public Task<IActionResult> Approve([FromRoute] Guid returnRequestBusinessKey, [FromBody] ApproveReturnRequestCommand command)
    {
        command.ReturnRequestBusinessKey = returnRequestBusinessKey;
        return SendCommand<ApproveReturnRequestCommand, Guid>(command);
    }

    [HttpPost("{returnRequestBusinessKey:guid}/reject")]
    public Task<IActionResult> Reject([FromRoute] Guid returnRequestBusinessKey, [FromBody] RejectReturnRequestCommand command)
    {
        command.ReturnRequestBusinessKey = returnRequestBusinessKey;
        return SendCommand<RejectReturnRequestCommand, Guid>(command);
    }

    [HttpPost("{returnRequestBusinessKey:guid}/receive")]
    public Task<IActionResult> Receive([FromRoute] Guid returnRequestBusinessKey, [FromBody] ReceiveReturnRequestCommand command)
    {
        command.ReturnRequestBusinessKey = returnRequestBusinessKey;
        return SendCommand<ReceiveReturnRequestCommand, Guid>(command);
    }

    [HttpPost("{returnRequestBusinessKey:guid}/close")]
    public Task<IActionResult> Close([FromRoute] Guid returnRequestBusinessKey, [FromBody] CloseReturnRequestCommand? command)
    {
        var request = command ?? new CloseReturnRequestCommand();
        request.ReturnRequestBusinessKey = returnRequestBusinessKey;
        return SendCommand<CloseReturnRequestCommand, Guid>(request);
    }

    [HttpGet("{returnRequestBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid returnRequestBusinessKey)
        => ExecuteQueryAsync<GetReturnRequestByBusinessKeyQuery, GetReturnRequestByBusinessKeyQueryResult>(
            new GetReturnRequestByBusinessKeyQuery(returnRequestBusinessKey));

    [HttpGet("by-id/{returnRequestId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid returnRequestId)
        => ExecuteQueryAsync<GetReturnRequestByIdQuery, GetReturnRequestByIdQueryResult>(
            new GetReturnRequestByIdQuery(returnRequestId));

    [HttpGet("by-order/{orderRef:guid}")]
    public Task<IActionResult> GetByOrder([FromRoute] Guid orderRef)
        => ExecuteQueryAsync<GetReturnRequestsByOrderQuery, GetReturnRequestsByOrderQueryResult>(
            new GetReturnRequestsByOrderQuery(orderRef));

    [HttpGet("by-status/{status}")]
    public Task<IActionResult> GetByStatus([FromRoute] string status)
        => ExecuteQueryAsync<GetReturnRequestsByStatusQuery, GetReturnRequestsByStatusQueryResult>(
            new GetReturnRequestsByStatusQuery(status));

    [HttpGet("{returnRequestBusinessKey:guid}/summary")]
    public Task<IActionResult> GetSummary([FromRoute] Guid returnRequestBusinessKey)
        => ExecuteQueryAsync<GetReturnSummaryQuery, GetReturnSummaryQueryResult>(
            new GetReturnSummaryQuery(returnRequestBusinessKey));
}

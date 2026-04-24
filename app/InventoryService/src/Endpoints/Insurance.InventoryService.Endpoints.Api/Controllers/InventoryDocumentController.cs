namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateInventoryDocument;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateAdjustmentDocument;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateIssueDocument;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateQualityChangeDocument;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateReceiptDocument;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateReturnDocument;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateTransferDocument;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.ApproveInventoryDocument;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CancelInventoryDocument;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.PostInventoryDocument;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.RejectInventoryDocument;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetById;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByNo;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByStatus;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByType;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.SearchInventoryDocuments;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class InventoryDocumentController : OysterFxController
{
    [HttpPost]
    public Task<IActionResult> Create([FromBody] CreateInventoryDocumentCommand command)
        => SendCommand<CreateInventoryDocumentCommand, CreateInventoryDocumentCommandResult>(command);

    [HttpPost("receipt")]
    public Task<IActionResult> CreateReceipt([FromBody] CreateReceiptDocumentCommand command)
        => SendCommand<CreateReceiptDocumentCommand, Guid>(command);

    [HttpPost("issue")]
    public Task<IActionResult> CreateIssue([FromBody] CreateIssueDocumentCommand command)
        => SendCommand<CreateIssueDocumentCommand, Guid>(command);

    [HttpPost("transfer")]
    public Task<IActionResult> CreateTransfer([FromBody] CreateTransferDocumentCommand command)
        => SendCommand<CreateTransferDocumentCommand, Guid>(command);

    [HttpPost("adjustment")]
    public Task<IActionResult> CreateAdjustment([FromBody] CreateAdjustmentDocumentCommand command)
        => SendCommand<CreateAdjustmentDocumentCommand, Guid>(command);

    [HttpPost("return")]
    public Task<IActionResult> CreateReturn([FromBody] CreateReturnDocumentCommand command)
        => SendCommand<CreateReturnDocumentCommand, Guid>(command);

    [HttpPost("quality-change")]
    public Task<IActionResult> CreateQualityChange([FromBody] CreateQualityChangeDocumentCommand command)
        => SendCommand<CreateQualityChangeDocumentCommand, Guid>(command);

    [HttpPost("{documentBusinessKey:guid}/post")]
    public Task<IActionResult> Post([FromRoute] Guid documentBusinessKey, [FromBody] PostInventoryDocumentCommand? command)
    {
        var request = command ?? new PostInventoryDocumentCommand();
        request.DocumentBusinessKey = documentBusinessKey;
        return SendCommand<PostInventoryDocumentCommand, PostInventoryDocumentCommandResult>(request);
    }

    [HttpPost("{documentBusinessKey:guid}/approve")]
    public Task<IActionResult> Approve([FromRoute] Guid documentBusinessKey, [FromBody] ApproveInventoryDocumentCommand command)
    {
        command.DocumentBusinessKey = documentBusinessKey;
        return SendCommand<ApproveInventoryDocumentCommand, Guid>(command);
    }

    [HttpPost("{documentBusinessKey:guid}/reject")]
    public Task<IActionResult> Reject([FromRoute] Guid documentBusinessKey, [FromBody] RejectInventoryDocumentCommand command)
    {
        command.DocumentBusinessKey = documentBusinessKey;
        return SendCommand<RejectInventoryDocumentCommand, Guid>(command);
    }

    [HttpPost("{documentBusinessKey:guid}/cancel")]
    public Task<IActionResult> Cancel([FromRoute] Guid documentBusinessKey, [FromBody] CancelInventoryDocumentCommand command)
    {
        command.DocumentBusinessKey = documentBusinessKey;
        return SendCommand<CancelInventoryDocumentCommand, Guid>(command);
    }

    [HttpGet("{documentBusinessKey:guid}")]
    public Task<IActionResult> GetByBusinessKey([FromRoute] Guid documentBusinessKey)
        => ExecuteQueryAsync<GetInventoryDocumentByBusinessKeyQuery, GetInventoryDocumentByBusinessKeyQueryResult>(
            new GetInventoryDocumentByBusinessKeyQuery(documentBusinessKey));

    [HttpGet("by-id/{documentId:guid}")]
    public Task<IActionResult> GetById([FromRoute] Guid documentId)
        => ExecuteQueryAsync<GetInventoryDocumentByIdQuery, GetInventoryDocumentByIdQueryResult>(
            new GetInventoryDocumentByIdQuery(documentId));

    [HttpGet("by-no/{documentNo}")]
    public Task<IActionResult> GetByNo([FromRoute] string documentNo)
        => ExecuteQueryAsync<GetInventoryDocumentByNoQuery, GetInventoryDocumentByNoQueryResult>(
            new GetInventoryDocumentByNoQuery(documentNo));

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] SearchInventoryDocumentsQuery query)
        => ExecuteQueryAsync<SearchInventoryDocumentsQuery, SearchInventoryDocumentsQueryResult>(query);

    [HttpGet("by-type/{documentType}")]
    public Task<IActionResult> GetByType([FromRoute] string documentType)
        => ExecuteQueryAsync<GetInventoryDocumentsByTypeQuery, GetInventoryDocumentsByTypeQueryResult>(
            new GetInventoryDocumentsByTypeQuery(documentType));

    [HttpGet("by-status/{status}")]
    public Task<IActionResult> GetByStatus([FromRoute] string status)
        => ExecuteQueryAsync<GetInventoryDocumentsByStatusQuery, GetInventoryDocumentsByStatusQueryResult>(
            new GetInventoryDocumentsByStatusQuery(status));
}

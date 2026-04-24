namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.ApproveInventoryDocument;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.ApproveInventoryDocument;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class ApproveInventoryDocumentCommandHandler : CommandHandler<ApproveInventoryDocumentCommand, Guid>
{
    private readonly IInventoryDocumentCommandRepository _repository;

    public ApproveInventoryDocumentCommandHandler(IInventoryDocumentCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(ApproveInventoryDocumentCommand command)
    {
        if (command.DocumentBusinessKey == Guid.Empty)
            return Fail("DocumentBusinessKey is required.");

        var document = await _repository.GetByBusinessKeyAsync(command.DocumentBusinessKey);
        if (document is null)
            return Fail("Inventory document was not found.");

        try
        {
            document.Approve(command.ApprovedBy);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(document.BusinessKey.Value);
    }
}

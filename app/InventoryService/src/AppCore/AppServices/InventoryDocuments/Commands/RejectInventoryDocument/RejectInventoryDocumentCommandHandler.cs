namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.RejectInventoryDocument;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.RejectInventoryDocument;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class RejectInventoryDocumentCommandHandler : CommandHandler<RejectInventoryDocumentCommand, Guid>
{
    private readonly IInventoryDocumentCommandRepository _repository;

    public RejectInventoryDocumentCommandHandler(IInventoryDocumentCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(RejectInventoryDocumentCommand command)
    {
        if (command.DocumentBusinessKey == Guid.Empty)
            return Fail("DocumentBusinessKey is required.");

        var document = await _repository.GetByBusinessKeyAsync(command.DocumentBusinessKey);
        if (document is null)
            return Fail("Inventory document was not found.");

        try
        {
            document.Reject("system", command.ReasonCode);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(document.BusinessKey.Value);
    }
}

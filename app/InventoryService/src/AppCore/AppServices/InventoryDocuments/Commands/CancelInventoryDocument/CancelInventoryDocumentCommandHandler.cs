namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.CancelInventoryDocument;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CancelInventoryDocument;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CancelInventoryDocumentCommandHandler : CommandHandler<CancelInventoryDocumentCommand, Guid>
{
    private readonly IInventoryDocumentCommandRepository _repository;

    public CancelInventoryDocumentCommandHandler(IInventoryDocumentCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(CancelInventoryDocumentCommand command)
    {
        if (command.DocumentBusinessKey == Guid.Empty)
            return Fail("DocumentBusinessKey is required.");

        var document = await _repository.GetByBusinessKeyAsync(command.DocumentBusinessKey);
        if (document is null)
            return Fail("Inventory document was not found.");

        try
        {
            document.Cancel("system", command.ReasonCode);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(document.BusinessKey.Value);
    }
}

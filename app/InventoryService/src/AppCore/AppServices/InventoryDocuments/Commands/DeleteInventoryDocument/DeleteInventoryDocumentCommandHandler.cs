namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.DeleteInventoryDocument;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.DeleteInventoryDocument;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class DeleteInventoryDocumentCommandHandler : CommandHandler<DeleteInventoryDocumentCommand, Guid>
{
    private readonly IInventoryDocumentCommandRepository _repository;

    public DeleteInventoryDocumentCommandHandler(IInventoryDocumentCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(DeleteInventoryDocumentCommand command)
    {
        var document = await _repository.GetByBusinessKeyAsync(command.DocumentBusinessKey);
        if (document is null)
        {
            return Fail("Document not found.");
        }

        if (document.Status != InventoryDocumentStatus.Draft)
        {
            return Fail("Only draft documents can be deleted.");
        }

        var deleted = await _repository.DeleteByBusinessKeyAsync(command.DocumentBusinessKey);
        if (!deleted)
        {
            return Fail("Document delete failed.");
        }

        await _repository.CommitAsync();
        return Ok(command.DocumentBusinessKey);
    }
}

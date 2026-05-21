namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.DeleteInventoryDocumentLine;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.DeleteInventoryDocumentLine;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class DeleteInventoryDocumentLineCommandHandler : CommandHandler<DeleteInventoryDocumentLineCommand, Guid>
{
    private readonly IInventoryDocumentCommandRepository _repository;

    public DeleteInventoryDocumentLineCommandHandler(IInventoryDocumentCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<Guid>> Handle(DeleteInventoryDocumentLineCommand command)
    {
        var document = await _repository.GetByBusinessKeyAsync(command.DocumentBusinessKey);
        if (document is null)
            return Fail("Document not found.");

        if (document.Status != InventoryDocumentStatus.Draft)
            return Fail("Only draft documents can be modified.");

        try
        {
            document.RemoveLine(command.LineBusinessKey);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(command.LineBusinessKey);
    }
}

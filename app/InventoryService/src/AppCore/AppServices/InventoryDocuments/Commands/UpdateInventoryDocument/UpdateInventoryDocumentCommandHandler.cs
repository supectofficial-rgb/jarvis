namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.UpdateInventoryDocument;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.UpdateInventoryDocument;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class UpdateInventoryDocumentCommandHandler
    : CommandHandler<UpdateInventoryDocumentCommand, Guid>
{
    private readonly IInventoryDocumentCommandRepository _documentRepository;

    public UpdateInventoryDocumentCommandHandler(IInventoryDocumentCommandRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public override async Task<CommandResult<Guid>> Handle(UpdateInventoryDocumentCommand command)
    {
        var document = await _documentRepository.GetByBusinessKeyAsync(command.DocumentBusinessKey);
        if (document is null)
        {
            return Fail("Inventory document was not found.");
        }

        if (!Enum.TryParse<InventoryDocumentType>(command.DocumentType, true, out var documentType))
        {
            return Fail($"Unsupported inventory document type '{command.DocumentType}'.");
        }

        if (document.DocumentType != documentType)
        {
            return Fail("Inventory document type cannot be changed.");
        }

        if (document.Status != InventoryDocumentStatus.Draft)
        {
            return Fail("Only draft documents can be edited.");
        }

        if (await _documentRepository.ExistsByDocumentNoAsync(command.DocumentNo ?? document.DocumentNo, command.DocumentBusinessKey))
        {
            return Fail($"Document number '{command.DocumentNo}' already exists.");
        }

        document.UpdateHeader(
            string.IsNullOrWhiteSpace(command.DocumentNo) ? document.DocumentNo : command.DocumentNo!,
            command.WarehouseRef,
            command.SellerRef,
            command.OccurredAt == default ? DateTime.UtcNow : command.OccurredAt,
            command.ReferenceType,
            command.ReferenceBusinessId,
            command.ReasonCode);

        await _documentRepository.CommitAsync();

        return Ok(document.BusinessKey.Value);
    }
}

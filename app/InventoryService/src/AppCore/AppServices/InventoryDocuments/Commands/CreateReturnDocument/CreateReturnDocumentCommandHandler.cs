namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.CreateReturnDocument;

using Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.Common;
using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateReturnDocument;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateReturnDocumentCommandHandler : CommandHandler<CreateReturnDocumentCommand, Guid>
{
    private readonly InventoryDocumentCreationService _creationService;

    public CreateReturnDocumentCommandHandler(IInventoryDocumentCommandRepository repository)
    {
        _creationService = new InventoryDocumentCreationService(repository);
    }

    public override async Task<CommandResult<Guid>> Handle(CreateReturnDocumentCommand command)
    {
        var documentType = NormalizeDocumentType(command.DocumentType);
        var result = await _creationService.CreateAsync(
            documentType,
            command.DocumentNo,
            command.ExternalReferenceNo,
            command.ReferenceType,
            command.ReferenceBusinessId,
            command.WarehouseRef,
            command.SellerRef,
            command.OccurredAt,
            command.CorrelationId,
            command.IdempotencyKey,
            command.ReasonCode,
            command.Lines);

        return result.Success ? Ok(result.DocumentBusinessKey) : Fail(result.Error!);
    }

    private static InventoryDocumentType NormalizeDocumentType(string? documentType)
    {
        return documentType switch
        {
            "ReturnFromBuy" => InventoryDocumentType.ReturnFromBuy,
            "ReturnFromTransfer" => InventoryDocumentType.ReturnFromTransfer,
            "Return" => InventoryDocumentType.ReturnFromSell,
            _ => InventoryDocumentType.ReturnFromSell
        };
    }
}

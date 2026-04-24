namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.CreateQualityChangeDocument;

using Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.Common;
using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateQualityChangeDocument;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateQualityChangeDocumentCommandHandler : CommandHandler<CreateQualityChangeDocumentCommand, Guid>
{
    private readonly InventoryDocumentCreationService _creationService;

    public CreateQualityChangeDocumentCommandHandler(IInventoryDocumentCommandRepository repository)
    {
        _creationService = new InventoryDocumentCreationService(repository);
    }

    public override async Task<CommandResult<Guid>> Handle(CreateQualityChangeDocumentCommand command)
    {
        var result = await _creationService.CreateAsync(
            InventoryDocumentType.QualityChange,
            command.DocumentNo,
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
}

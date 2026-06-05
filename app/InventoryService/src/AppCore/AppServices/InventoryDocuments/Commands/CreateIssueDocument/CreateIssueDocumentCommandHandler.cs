namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.CreateIssueDocument;

using Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.Common;
using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateIssueDocument;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateIssueDocumentCommandHandler : CommandHandler<CreateIssueDocumentCommand, Guid>
{
    private readonly InventoryDocumentCreationService _creationService;

    public CreateIssueDocumentCommandHandler(
        IInventoryDocumentCommandRepository repository,
        IInventorySourceBalanceCommandRepository sourceBalanceRepository,
        ISerialItemCommandRepository serialItemCommandRepository,
        ISerialItemQueryRepository serialItemQueryRepository)
    {
        _creationService = new InventoryDocumentCreationService(repository, sourceBalanceRepository, serialItemCommandRepository, serialItemQueryRepository);
    }

    public override async Task<CommandResult<Guid>> Handle(CreateIssueDocumentCommand command)
    {
        var result = await _creationService.CreateAsync(
            InventoryDocumentType.Issue,
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
}

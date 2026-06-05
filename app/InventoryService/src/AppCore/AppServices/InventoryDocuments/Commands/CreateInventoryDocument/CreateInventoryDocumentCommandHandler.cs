namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.CreateInventoryDocument;

using Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.Common;
using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateInventoryDocument;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateInventoryDocumentCommandHandler
    : CommandHandler<CreateInventoryDocumentCommand, CreateInventoryDocumentCommandResult>
{
    private readonly IInventoryDocumentCommandRepository _documentRepository;
    private readonly IInventorySourceBalanceCommandRepository _sourceBalanceRepository;

    public CreateInventoryDocumentCommandHandler(
        IInventoryDocumentCommandRepository documentRepository,
        IInventorySourceBalanceCommandRepository sourceBalanceRepository)
    {
        _documentRepository = documentRepository;
        _sourceBalanceRepository = sourceBalanceRepository;
    }

    public override async Task<CommandResult<CreateInventoryDocumentCommandResult>> Handle(CreateInventoryDocumentCommand command)
    {
        if (!Enum.TryParse<InventoryDocumentType>(command.DocumentType, true, out var documentType))
            return Fail($"Unsupported inventory document type '{command.DocumentType}'.");

        var documentNo = string.IsNullOrWhiteSpace(command.DocumentNo)
            ? await _documentRepository.GetNextDocumentNoAsync()
            : command.DocumentNo.Trim();

        if (await _documentRepository.ExistsByDocumentNoAsync(documentNo))
            return Fail($"Document number '{documentNo}' already exists.");

        // Small no-op touch to keep the service image rebuild path explicit.
        var document = InventoryDocument.Create(
            documentNo,
            documentType,
            command.WarehouseRef,
            command.SellerRef,
            command.ReceivedBy,
            command.DeliveredBy,
            command.OccurredAt == default ? DateTime.UtcNow : command.OccurredAt,
            command.ExternalReferenceNo,
            command.ReferenceType,
            command.ReferenceBusinessId,
            command.CorrelationId,
            command.IdempotencyKey,
            command.ReasonCode);

        foreach (var line in command.Lines)
        {
            Enum.TryParse<InventoryAdjustmentDirection>(line.AdjustmentDirection, true, out var adjustmentDirection);

            var documentLine = InventoryDocumentLine.Create(
                line.VariantRef,
                line.Qty,
                line.UomRef,
                line.BaseQty,
                line.BaseUomRef,
                line.SourceLocationRef,
                line.DestinationLocationRef,
                line.QualityStatusRef,
                line.FromQualityStatusRef,
                line.ToQualityStatusRef,
                line.LotBatchNo,
                line.ReasonCode,
                line.AdjustmentDirection is null ? null : adjustmentDirection);

            document.AddLine(documentLine);

            if (document.DocumentType == InventoryDocumentType.Receipt)
            {
                InventoryDocumentReceiptLotHelper.ApplyReceiptLotBatchNo(document);
            }

            if (InventoryDocumentLineSourceAllocationHelper.ShouldReserveSourceBalances(document.DocumentType))
            {
                await InventoryDocumentLineSourceAllocationHelper.AllocateSourceBalancesAsync(document, documentLine, _sourceBalanceRepository);
            }
        }

        await _documentRepository.InsertAsync(document);
        await _documentRepository.CommitAsync();

        return Ok(new CreateInventoryDocumentCommandResult
        {
            DocumentBusinessKey = document.BusinessKey.Value,
            DocumentNo = document.DocumentNo,
            Status = document.Status.ToString()
        });
    }
}

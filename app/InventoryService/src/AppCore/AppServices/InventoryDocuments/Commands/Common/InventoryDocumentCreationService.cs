namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.Common;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateInventoryDocument;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Commands;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;

internal sealed class InventoryDocumentCreationService
{
    private readonly IInventoryDocumentCommandRepository _repository;
    private readonly IInventorySourceBalanceCommandRepository _sourceBalanceRepository;
    private readonly ISerialItemCommandRepository _serialItemCommandRepository;
    private readonly ISerialItemQueryRepository _serialItemQueryRepository;

    public InventoryDocumentCreationService(
        IInventoryDocumentCommandRepository repository,
        IInventorySourceBalanceCommandRepository sourceBalanceRepository,
        ISerialItemCommandRepository serialItemCommandRepository,
        ISerialItemQueryRepository serialItemQueryRepository)
    {
        _repository = repository;
        _sourceBalanceRepository = sourceBalanceRepository;
        _serialItemCommandRepository = serialItemCommandRepository;
        _serialItemQueryRepository = serialItemQueryRepository;
    }

    public async Task<(bool Success, string? Error, Guid DocumentBusinessKey)> CreateAsync(
        InventoryDocumentType documentType,
        string? documentNo,
        string? externalReferenceNo,
        string? referenceType,
        Guid? referenceBusinessId,
        Guid warehouseRef,
        Guid sellerRef,
        DateTime occurredAt,
        string? correlationId,
        string? idempotencyKey,
        string? reasonCode,
        List<InventoryDocumentCommandLineItem> lines)
    {
        var finalDocumentNo = string.IsNullOrWhiteSpace(documentNo)
            ? await _repository.GetNextDocumentNoAsync()
            : documentNo.Trim();

        if (await _repository.ExistsByDocumentNoAsync(finalDocumentNo))
            return (false, $"Document number '{finalDocumentNo}' already exists.", Guid.Empty);

        InventoryDocument document;
        try
        {
            document = InventoryDocument.Create(
                finalDocumentNo,
                documentType,
                warehouseRef,
                sellerRef,
                receivedBy: null,
                deliveredBy: null,
                occurredAt == default ? DateTime.UtcNow : occurredAt,
                externalReferenceNo,
                referenceType,
                referenceBusinessId,
                correlationId,
                idempotencyKey,
                reasonCode);

            foreach (var line in lines)
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

                foreach (var serial in line.Serials)
                    documentLine.AddSerial(serial.SerialRef, serial.SerialNo);

                document.AddLine(documentLine);

                if (document.DocumentType == InventoryDocumentType.Receipt)
                {
                    InventoryDocumentReceiptLotHelper.ApplyReceiptLotBatchNo(document);
                }

                if (InventoryDocumentLineSerialStatusHelper.ShouldReserveSerials(document.DocumentType))
                {
                    var serialItems = await InventoryDocumentLineSerialStatusHelper.ResolveSerialItemsAsync(
                        line.Serials.Select(serial => (serial.SerialRef, serial.SerialNo)),
                        line.VariantRef,
                        _serialItemCommandRepository,
                        _serialItemQueryRepository);

                    InventoryDocumentLineSerialStatusHelper.ReserveSerialItems(serialItems);
                }

                if (InventoryDocumentLineSourceAllocationHelper.ShouldReserveSourceBalances(document.DocumentType))
                {
                    await InventoryDocumentLineSourceAllocationHelper.AllocateSourceBalancesAsync(document, documentLine, _sourceBalanceRepository);
                }
            }
        }
        catch (Exception ex)
        {
            return (false, ex.Message, Guid.Empty);
        }

        await _repository.InsertAsync(document);
        await _repository.CommitAsync();

        return (true, null, document.BusinessKey.Value);
    }
}

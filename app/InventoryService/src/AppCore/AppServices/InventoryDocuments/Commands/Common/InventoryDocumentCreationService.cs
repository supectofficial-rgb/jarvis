namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.Common;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateInventoryDocument;

internal sealed class InventoryDocumentCreationService
{
    private readonly IInventoryDocumentCommandRepository _repository;

    public InventoryDocumentCreationService(IInventoryDocumentCommandRepository repository)
    {
        _repository = repository;
    }

    public async Task<(bool Success, string? Error, Guid DocumentBusinessKey)> CreateAsync(
        InventoryDocumentType documentType,
        string? documentNo,
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
        if (lines.Count == 0)
            return (false, "Inventory document must contain at least one line.", Guid.Empty);

        var finalDocumentNo = string.IsNullOrWhiteSpace(documentNo)
            ? $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}"
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
                occurredAt == default ? DateTime.UtcNow : occurredAt,
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

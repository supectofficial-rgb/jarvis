namespace Insurance.InventoryService.AppCore.AppServices.InventoryDocuments.Commands.Common;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using OysterFx.AppCore.Domain.Exceptions;

internal static class InventoryDocumentReceiptLotHelper
{
    public static void ApplyReceiptLotBatchNo(InventoryDocument document)
    {
        if (document.DocumentType != InventoryDocumentType.Receipt)
            return;

        var lotBatchNo = ResolveReceiptLotBatchNo(document);
        document.ApplyReceiptLotBatchNo(lotBatchNo);
    }

    public static string ResolveReceiptLotBatchNo(InventoryDocument document)
    {
        if (document.DocumentType != InventoryDocumentType.Receipt)
            throw new AggregateStateExceptions("Receipt lot batch number can only be resolved for receipt documents.", nameof(document.DocumentType));

        var lots = document.Lines
            .Select(line => NormalizeLotBatchNo(line.LotBatchNo))
            .Where(lot => !string.IsNullOrWhiteSpace(lot))
            .Select(lot => lot!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (lots.Count > 1)
        {
            throw new AggregateStateExceptions(
                $"Receipt '{document.DocumentNo}' contains multiple lot batch numbers. Use a single lot batch number for the whole receipt.",
                nameof(document.Lines));
        }

        if (lots.Count == 1)
            return lots[0];

        return GenerateReceiptLotBatchNo(document);
    }

    private static string GenerateReceiptLotBatchNo(InventoryDocument document)
    {
        var documentPart = NormalizeSerialSegment(string.IsNullOrWhiteSpace(document.DocumentNo)
            ? document.BusinessKey.Value.ToString("N")
            : document.DocumentNo);

        return $"LOT-{documentPart}";
    }

    private static string NormalizeSerialSegment(string value)
    {
        var normalized = value.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            return "DOC";

        normalized = normalized.Replace(' ', '-');
        normalized = normalized.Replace('/', '-');
        normalized = normalized.Replace('\\', '-');
        normalized = normalized.Replace(':', '-');
        normalized = normalized.Replace('.', '-');
        return normalized;
    }

    private static string? NormalizeLotBatchNo(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

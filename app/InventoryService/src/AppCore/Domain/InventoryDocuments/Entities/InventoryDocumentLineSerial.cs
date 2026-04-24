namespace Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class InventoryDocumentLineSerial : Aggregate
{
    public Guid DocumentLineRef { get; private set; }
    public Guid? SerialRef { get; private set; }
    public string SerialNo { get; private set; } = string.Empty;

    private InventoryDocumentLineSerial()
    {
    }

    internal static InventoryDocumentLineSerial Create(Guid documentLineRef, Guid? serialRef, string serialNo)
    {
        return new InventoryDocumentLineSerial
        {
            DocumentLineRef = documentLineRef,
            SerialRef = serialRef,
            SerialNo = serialNo
        };
    }
}

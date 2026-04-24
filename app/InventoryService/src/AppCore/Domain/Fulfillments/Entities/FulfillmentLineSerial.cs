namespace Insurance.InventoryService.AppCore.Domain.Fulfillments.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class FulfillmentLineSerial : Aggregate
{
    public Guid FulfillmentLineRef { get; private set; }
    public Guid? SerialRef { get; private set; }
    public string SerialNo { get; private set; } = string.Empty;

    private FulfillmentLineSerial()
    {
    }

    internal static FulfillmentLineSerial Create(Guid fulfillmentLineRef, Guid? serialRef, string serialNo)
    {
        return new FulfillmentLineSerial
        {
            FulfillmentLineRef = fulfillmentLineRef,
            SerialRef = serialRef,
            SerialNo = serialNo
        };
    }
}

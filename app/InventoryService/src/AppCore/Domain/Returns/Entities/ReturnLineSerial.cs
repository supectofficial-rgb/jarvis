namespace Insurance.InventoryService.AppCore.Domain.Returns.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class ReturnLineSerial : Aggregate
{
    public Guid ReturnLineRef { get; private set; }
    public Guid? SerialRef { get; private set; }
    public string SerialNo { get; private set; } = string.Empty;

    private ReturnLineSerial()
    {
    }

    internal static ReturnLineSerial Create(Guid returnLineRef, Guid? serialRef, string serialNo)
    {
        return new ReturnLineSerial
        {
            ReturnLineRef = returnLineRef,
            SerialRef = serialRef,
            SerialNo = serialNo
        };
    }
}

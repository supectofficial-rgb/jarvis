namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetBySerialNo;

using OysterFx.AppCore.Shared.Queries;

public class GetSerialItemBySerialNoQuery : IQuery<GetSerialItemBySerialNoQueryResult>
{
    public GetSerialItemBySerialNoQuery(string serialNo, Guid? variantRef = null)
    {
        SerialNo = serialNo;
        VariantRef = variantRef;
    }

    public string SerialNo { get; }
    public Guid? VariantRef { get; }
}

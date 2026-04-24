namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByQualityStatus;

using OysterFx.AppCore.Shared.Queries;

public class GetStockDetailsByQualityStatusQuery : IQuery<GetStockDetailsByQualityStatusQueryResult>
{
    public GetStockDetailsByQualityStatusQuery(Guid qualityStatusRef)
    {
        QualityStatusRef = qualityStatusRef;
    }

    public Guid QualityStatusRef { get; }
}

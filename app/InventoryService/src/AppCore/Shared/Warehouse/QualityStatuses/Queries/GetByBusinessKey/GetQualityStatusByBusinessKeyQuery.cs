namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetQualityStatusByBusinessKeyQuery : IQuery<GetQualityStatusByBusinessKeyQueryResult>
{
    public GetQualityStatusByBusinessKeyQuery(Guid qualityStatusBusinessKey)
    {
        QualityStatusBusinessKey = qualityStatusBusinessKey;
    }

    public Guid QualityStatusBusinessKey { get; }
}

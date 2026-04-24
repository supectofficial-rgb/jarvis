namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public class GetQualityStatusByIdQuery : IQuery<GetQualityStatusByBusinessKeyQueryResult>
{
    public GetQualityStatusByIdQuery(Guid qualityStatusId)
    {
        QualityStatusId = qualityStatusId;
    }

    public Guid QualityStatusId { get; }
}

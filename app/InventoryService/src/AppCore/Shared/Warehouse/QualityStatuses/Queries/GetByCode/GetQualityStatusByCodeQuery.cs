namespace Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetByCode;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public class GetQualityStatusByCodeQuery : IQuery<GetQualityStatusByBusinessKeyQueryResult>
{
    public GetQualityStatusByCodeQuery(string code)
    {
        Code = code;
    }

    public string Code { get; }
}

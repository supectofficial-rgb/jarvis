namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetByCode;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public class GetUnitOfMeasureByCodeQuery : IQuery<GetUnitOfMeasureByBusinessKeyQueryResult>
{
    public GetUnitOfMeasureByCodeQuery(string code)
    {
        Code = code;
    }

    public string Code { get; }
}

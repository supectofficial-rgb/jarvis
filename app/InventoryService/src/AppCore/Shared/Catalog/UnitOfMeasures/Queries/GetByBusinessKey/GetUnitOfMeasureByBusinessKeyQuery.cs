namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetUnitOfMeasureByBusinessKeyQuery : IQuery<GetUnitOfMeasureByBusinessKeyQueryResult>
{
    public GetUnitOfMeasureByBusinessKeyQuery(Guid unitOfMeasureBusinessKey)
    {
        UnitOfMeasureBusinessKey = unitOfMeasureBusinessKey;
    }

    public Guid UnitOfMeasureBusinessKey { get; }
}

namespace Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public class GetUnitOfMeasureByIdQuery : IQuery<GetUnitOfMeasureByBusinessKeyQueryResult>
{
    public GetUnitOfMeasureByIdQuery(Guid unitOfMeasureId)
    {
        UnitOfMeasureId = unitOfMeasureId;
    }

    public Guid UnitOfMeasureId { get; }
}

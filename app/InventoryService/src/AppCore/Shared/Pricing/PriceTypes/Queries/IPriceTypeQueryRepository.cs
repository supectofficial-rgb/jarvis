namespace Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.GetByBusinessKey;
using OysterFx.AppCore.Shared.Queries;

public interface IPriceTypeQueryRepository : IQueryRepository
{
    Task<GetPriceTypeByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid priceTypeBusinessKey);
}

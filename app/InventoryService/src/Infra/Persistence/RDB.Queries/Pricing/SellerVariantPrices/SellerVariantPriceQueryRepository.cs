namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.SellerVariantPrices;

using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries;
using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Queries.SearchSellerVariantPrices;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.SellerVariantPrices.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class SellerVariantPriceQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, ISellerVariantPriceQueryRepository
{
    public SellerVariantPriceQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetSellerVariantPriceByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid sellerVariantPriceBusinessKey)
    {
        var item = await _dbContext.Set<SellerVariantPriceReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == sellerVariantPriceBusinessKey);

        if (item is null)
            return null;

        var offers = await _dbContext.Set<OfferReadModel>()
            .Where(x => x.PriceRef == sellerVariantPriceBusinessKey)
            .OrderBy(x => x.Priority)
            .Select(x => new SellerVariantPriceOfferResultItem
            {
                Name = x.Name,
                DiscountAmount = x.DiscountAmount,
                DiscountPercent = x.DiscountPercent,
                MaxQuantity = x.MaxQuantity,
                Priority = x.Priority,
                StartAt = x.StartAt,
                EndAt = x.EndAt,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return new GetSellerVariantPriceByBusinessKeyQueryResult
        {
            SellerVariantPriceBusinessKey = item.BusinessKey,
            SellerRef = item.SellerRef,
            VariantRef = item.VariantRef,
            PriceTypeRef = item.PriceTypeRef,
            PriceChannelRef = item.PriceChannelRef,
            Amount = item.Amount,
            Currency = item.Currency,
            MinQty = item.MinQty,
            Priority = item.Priority,
            EffectiveFrom = item.EffectiveFrom,
            EffectiveTo = item.EffectiveTo,
            IsActive = item.IsActive,
            Offers = offers
        };
    }

    public async Task<SearchSellerVariantPricesQueryResult> SearchAsync(SearchSellerVariantPricesQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 50 : query.PageSize;
        var itemsQuery = _dbContext.Set<SellerVariantPriceReadModel>().AsQueryable();

        if (query.SellerRef.HasValue)
            itemsQuery = itemsQuery.Where(x => x.SellerRef == query.SellerRef.Value);

        if (query.VariantRef.HasValue)
            itemsQuery = itemsQuery.Where(x => x.VariantRef == query.VariantRef.Value);

        if (query.PriceTypeRef.HasValue)
            itemsQuery = itemsQuery.Where(x => x.PriceTypeRef == query.PriceTypeRef.Value);

        if (query.PriceChannelRef.HasValue)
            itemsQuery = itemsQuery.Where(x => x.PriceChannelRef == query.PriceChannelRef.Value);

        if (query.IsActive.HasValue)
            itemsQuery = itemsQuery.Where(x => x.IsActive == query.IsActive.Value);

        var totalCount = await itemsQuery.CountAsync();
        var items = await itemsQuery
            .OrderBy(x => x.VariantRef)
            .ThenBy(x => x.PriceTypeRef)
            .ThenBy(x => x.PriceChannelRef)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SellerVariantPriceListItem
            {
                SellerVariantPriceBusinessKey = x.BusinessKey,
                SellerRef = x.SellerRef,
                VariantRef = x.VariantRef,
                PriceTypeRef = x.PriceTypeRef,
                PriceChannelRef = x.PriceChannelRef,
                Amount = x.Amount,
                Currency = x.Currency,
                MinQty = x.MinQty,
                Priority = x.Priority,
                EffectiveFrom = x.EffectiveFrom,
                EffectiveTo = x.EffectiveTo,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return new SearchSellerVariantPricesQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }
}

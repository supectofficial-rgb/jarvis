namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.ProductVariants;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands;

public class ProductVariantCommandRepository : CommandRepository<ProductVariant, InventoryServiceCommandDbContext>, IProductVariantCommandRepository
{
    public ProductVariantCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<ProductVariant?> GetByBusinessKeyAsync(Guid productVariantBusinessKey)
    {
        return _dbContext.Set<ProductVariant>()
            .Include(x => x.AttributeValues)
            .Include(x => x.UomConversions)
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(productVariantBusinessKey));
    }

    public Task<bool> ExistsByVariantSkuAsync(string variantSku, Guid? exceptBusinessKey = null)
    {
        var normalized = variantSku.Trim();
        var query = _dbContext.Set<ProductVariant>().Where(x => x.VariantSku == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey != BusinessKey.FromGuid(exceptBusinessKey.Value));

        return query.AnyAsync();
    }

    public Task<bool> ExistsByBarcodeAsync(string barcode, Guid? exceptBusinessKey = null)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return Task.FromResult(false);

        var normalized = barcode.Trim();
        var query = _dbContext.Set<ProductVariant>().Where(x => x.Barcode == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey != BusinessKey.FromGuid(exceptBusinessKey.Value));

        return query.AnyAsync();
    }

    public Task<bool> ExistsByBaseUomRefAsync(Guid baseUomRef, bool onlyActive = true)
    {
        var query = _dbContext.Set<ProductVariant>().Where(x => x.BaseUomRef == baseUomRef);
        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return query.AnyAsync();
    }

    public Task<bool> ExistsByProductRefAsync(Guid productRef, bool onlyActive = true)
    {
        var query = _dbContext.Set<ProductVariant>().Where(x => x.ProductRef == productRef);
        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return query.AnyAsync();
    }

    public Task<bool> ExistsAttributeValueByAttributeRefAsync(Guid attributeRef, bool onlyActive = true)
    {
        if (attributeRef == Guid.Empty)
            return Task.FromResult(false);

        var query = _dbContext.Set<ProductVariant>().AsQueryable();
        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return query.AnyAsync(x => x.AttributeValues.Any(v => v.AttributeRef == attributeRef));
    }

    public Task<bool> ExistsAttributeValueByOptionRefAsync(Guid optionRef, bool onlyActive = true)
    {
        if (optionRef == Guid.Empty)
            return Task.FromResult(false);

        var query = _dbContext.Set<ProductVariant>().AsQueryable();
        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return query.AnyAsync(x => x.AttributeValues.Any(v => v.OptionRef == optionRef));
    }
}

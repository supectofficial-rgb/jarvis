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
            .Include(x => x.Components)
            .Include(x => x.AddOns)
            .Include(x => x.Images)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(productVariantBusinessKey));
    }

    public Task<ProductVariant?> GetByVariantAddOnBusinessKeyAsync(Guid variantAddOnBusinessKey)
    {
        if (variantAddOnBusinessKey == Guid.Empty)
        {
            return Task.FromResult<ProductVariant?>(null);
        }

        var businessKey = BusinessKey.FromGuid(variantAddOnBusinessKey);
        return _dbContext.Set<ProductVariant>()
            .Include(x => x.AttributeValues)
            .Include(x => x.UomConversions)
            .Include(x => x.Components)
            .Include(x => x.AddOns)
            .Include(x => x.Images)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.AddOns.Any(addOn => addOn.BusinessKey == businessKey));
    }

    public async Task<bool> DeleteVariantAddOnByBusinessKeyAsync(Guid variantAddOnBusinessKey)
    {
        if (variantAddOnBusinessKey == Guid.Empty)
        {
            return false;
        }

        var businessKey = BusinessKey.FromGuid(variantAddOnBusinessKey);
        var deleted = await _dbContext.Set<VariantAddOn>()
            .Where(x => x.BusinessKey == businessKey)
            .ExecuteDeleteAsync();

        return deleted > 0;
    }

    public Task<ProductVariant?> GetByComponentBusinessKeyAsync(Guid variantComponentBusinessKey)
    {
        if (variantComponentBusinessKey == Guid.Empty)
        {
            return Task.FromResult<ProductVariant?>(null);
        }

        var businessKey = BusinessKey.FromGuid(variantComponentBusinessKey);
        return _dbContext.Set<ProductVariant>()
            .Include(x => x.AttributeValues)
            .Include(x => x.UomConversions)
            .Include(x => x.Components)
            .Include(x => x.AddOns)
            .Include(x => x.Images)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Components.Any(component => component.BusinessKey == businessKey));
    }

    public async Task<bool> DeleteVariantTagByBusinessKeyAsync(Guid variantTagBusinessKey)
    {
        if (variantTagBusinessKey == Guid.Empty)
        {
            return false;
        }

        var businessKey = BusinessKey.FromGuid(variantTagBusinessKey);
        var deleted = await _dbContext.Set<VariantTag>()
            .Where(x => x.BusinessKey == businessKey)
            .ExecuteDeleteAsync();

        return deleted > 0;
    }

    public async Task<bool> DeleteVariantImageByBusinessKeyAsync(Guid variantImageBusinessKey)
    {
        if (variantImageBusinessKey == Guid.Empty)
        {
            return false;
        }

        var businessKey = BusinessKey.FromGuid(variantImageBusinessKey);
        var deleted = await _dbContext.Set<VariantImage>()
            .Where(x => x.BusinessKey == businessKey)
            .ExecuteDeleteAsync();

        return deleted > 0;
    }

    public Task<List<ProductVariant>> GetByProductRefAsync(Guid productRef)
    {
        return _dbContext.Set<ProductVariant>()
            .Where(x => x.ProductRef == productRef)
            .Include(x => x.AttributeValues)
            .Include(x => x.UomConversions)
            .Include(x => x.Components)
            .Include(x => x.AddOns)
            .Include(x => x.Images)
            .Include(x => x.Tags)
            .ToListAsync();
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

    public Task<bool> ExistsByBusinessKeyAsync(Guid productVariantBusinessKey, bool onlyActive = false)
    {
        if (productVariantBusinessKey == Guid.Empty)
            return Task.FromResult(false);

        var query = _dbContext.Set<ProductVariant>()
            .Where(x => x.BusinessKey == BusinessKey.FromGuid(productVariantBusinessKey));

        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return query.AnyAsync();
    }

    public Task<bool> ExistsByTagRefAsync(Guid productVariantBusinessKey, Guid tagRef, Guid? exceptBusinessKey = null)
    {
        if (productVariantBusinessKey == Guid.Empty || tagRef == Guid.Empty)
            return Task.FromResult(false);

        var query = _dbContext.Set<ProductVariant>()
            .Where(x => x.BusinessKey == BusinessKey.FromGuid(productVariantBusinessKey))
            .SelectMany(x => x.Tags)
            .Where(x => x.TagRef == tagRef);

        if (exceptBusinessKey.HasValue && exceptBusinessKey.Value != Guid.Empty)
            query = query.Where(x => x.BusinessKey != BusinessKey.FromGuid(exceptBusinessKey.Value));

        return query.AnyAsync();
    }
}

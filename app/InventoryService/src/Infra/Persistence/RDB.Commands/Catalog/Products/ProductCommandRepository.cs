namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.Products;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands;

public class ProductCommandRepository : CommandRepository<Product, InventoryServiceCommandDbContext>, IProductCommandRepository
{
    public ProductCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Product?> GetByBusinessKeyAsync(Guid productBusinessKey)
    {
        return _dbContext.Set<Product>()
            .Include(x => x.AttributeValues)
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(productBusinessKey));
    }

    public Task<bool> ExistsByBaseSkuAsync(string baseSku, Guid? exceptBusinessKey = null)
    {
        var normalized = baseSku.Trim();
        var query = _dbContext.Set<Product>().Where(x => x.BaseSku == normalized);

        if (exceptBusinessKey.HasValue)
            query = query.Where(x => x.BusinessKey != BusinessKey.FromGuid(exceptBusinessKey.Value));

        return query.AnyAsync();
    }

    public Task<bool> ExistsByDefaultUomRefAsync(Guid defaultUomRef, bool onlyActive = true)
    {
        var query = _dbContext.Set<Product>().Where(x => x.DefaultUomRef == defaultUomRef);
        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return query.AnyAsync();
    }

    public Task<bool> ExistsByCategoryRefAsync(Guid categoryRef, bool onlyActive = true)
    {
        var query = _dbContext.Set<Product>().Where(x => x.CategoryRef == categoryRef);
        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return query.AnyAsync();
    }

    public Task<bool> ExistsByCategorySchemaVersionRefAsync(Guid categorySchemaVersionRef, bool onlyActive = false)
    {
        if (categorySchemaVersionRef == Guid.Empty)
            return Task.FromResult(false);

        var query = _dbContext.Set<Product>().Where(x => x.CategorySchemaVersionRef == categorySchemaVersionRef);
        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return query.AnyAsync();
    }

    public Task<bool> ExistsAttributeValueByAttributeRefAsync(Guid attributeRef, bool onlyActive = true)
    {
        if (attributeRef == Guid.Empty)
            return Task.FromResult(false);

        var query = _dbContext.Set<Product>().AsQueryable();
        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return query.AnyAsync(x => x.AttributeValues.Any(v => v.AttributeRef == attributeRef));
    }

    public Task<bool> ExistsAttributeValueByOptionRefAsync(Guid optionRef, bool onlyActive = true)
    {
        if (optionRef == Guid.Empty)
            return Task.FromResult(false);

        var query = _dbContext.Set<Product>().AsQueryable();
        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return query.AnyAsync(x => x.AttributeValues.Any(v => v.OptionRef == optionRef));
    }
}

namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.UnitOfMeasures.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.UnitOfMeasures.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryTransactions.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryTransactions.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Reservations.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Reservations.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Fulfillments.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Fulfillments.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Returns.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Returns.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Seller.Sellers.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Seller.Sellers.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.SourceTracing.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.SourceTracing.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.StockDetails.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.StockDetails.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.StockDetails.SerialItems.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.StockDetails.SerialItems.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Locations.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Locations.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.QualityStatuses.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.QualityStatuses.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Warehouses.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Warehouses.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class InventoryServiceQueryDbContext : QueryDbContext
{
    public InventoryServiceQueryDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<AttributeDefinitionReadModel> AttributeDefinitions => Set<AttributeDefinitionReadModel>();
    public DbSet<AttributeOptionReadModel> AttributeOptions => Set<AttributeOptionReadModel>();
    public DbSet<CategoryReadModel> Categories => Set<CategoryReadModel>();
    public DbSet<CategorySchemaVersionReadModel> CategorySchemaVersions => Set<CategorySchemaVersionReadModel>();
    public DbSet<CategoryAttributeRuleReadModel> CategoryAttributeRules => Set<CategoryAttributeRuleReadModel>();
    public DbSet<ProductReadModel> Products => Set<ProductReadModel>();
    public DbSet<ProductAttributeValueReadModel> ProductAttributeValues => Set<ProductAttributeValueReadModel>();
    public DbSet<ProductVariantReadModel> ProductVariants => Set<ProductVariantReadModel>();
    public DbSet<VariantAttributeValueReadModel> VariantAttributeValues => Set<VariantAttributeValueReadModel>();
    public DbSet<VariantUomConversionReadModel> VariantUomConversions => Set<VariantUomConversionReadModel>();
    public DbSet<UnitOfMeasureReadModel> UnitOfMeasures => Set<UnitOfMeasureReadModel>();
    public DbSet<InventoryDocumentReadModel> InventoryDocuments => Set<InventoryDocumentReadModel>();
    public DbSet<InventoryTransactionReadModel> InventoryTransactions => Set<InventoryTransactionReadModel>();
    public DbSet<InventoryTransactionLineReadModel> InventoryTransactionLines => Set<InventoryTransactionLineReadModel>();
    public DbSet<InventoryReservationReadModel> InventoryReservations => Set<InventoryReservationReadModel>();
    public DbSet<ReservationAllocationReadModel> ReservationAllocations => Set<ReservationAllocationReadModel>();
    public DbSet<FulfillmentReadModel> Fulfillments => Set<FulfillmentReadModel>();
    public DbSet<ReturnRequestReadModel> ReturnRequests => Set<ReturnRequestReadModel>();
    public DbSet<InventorySourceBalanceReadModel> InventorySourceBalances => Set<InventorySourceBalanceReadModel>();
    public DbSet<InventorySourceAllocationReadModel> InventorySourceAllocations => Set<InventorySourceAllocationReadModel>();
    public DbSet<InventorySourceConsumptionReadModel> InventorySourceConsumptions => Set<InventorySourceConsumptionReadModel>();
    public DbSet<StockDetailReadModel> StockDetails => Set<StockDetailReadModel>();
    public DbSet<SerialItemReadModel> SerialItems => Set<SerialItemReadModel>();
    public DbSet<SellerReadModel> Sellers => Set<SellerReadModel>();
    public DbSet<WarehouseReadModel> Warehouses => Set<WarehouseReadModel>();
    public DbSet<LocationReadModel> Locations => Set<LocationReadModel>();
    public DbSet<QualityStatusReadModel> QualityStatuses => Set<QualityStatusReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AttributeDefinitionReadModelConfig());
        modelBuilder.ApplyConfiguration(new AttributeOptionReadModelConfig());
        modelBuilder.ApplyConfiguration(new CategoryReadModelConfig());
        modelBuilder.ApplyConfiguration(new CategorySchemaVersionReadModelConfig());
        modelBuilder.ApplyConfiguration(new CategoryAttributeRuleReadModelConfig());
        modelBuilder.ApplyConfiguration(new ProductReadModelConfig());
        modelBuilder.ApplyConfiguration(new ProductAttributeValueReadModelConfig());
        modelBuilder.ApplyConfiguration(new ProductVariantReadModelConfig());
        modelBuilder.ApplyConfiguration(new VariantAttributeValueReadModelConfig());
        modelBuilder.ApplyConfiguration(new VariantUomConversionReadModelConfig());
        modelBuilder.ApplyConfiguration(new UnitOfMeasureReadModelConfig());
        modelBuilder.ApplyConfiguration(new InventoryDocumentReadModelConfig());
        modelBuilder.ApplyConfiguration(new InventoryTransactionReadModelConfig());
        modelBuilder.ApplyConfiguration(new InventoryTransactionLineReadModelConfig());
        modelBuilder.ApplyConfiguration(new InventoryReservationReadModelConfig());
        modelBuilder.ApplyConfiguration(new ReservationAllocationReadModelConfig());
        modelBuilder.ApplyConfiguration(new FulfillmentReadModelConfig());
        modelBuilder.ApplyConfiguration(new ReturnRequestReadModelConfig());
        modelBuilder.ApplyConfiguration(new InventorySourceBalanceReadModelConfig());
        modelBuilder.ApplyConfiguration(new InventorySourceAllocationReadModelConfig());
        modelBuilder.ApplyConfiguration(new InventorySourceConsumptionReadModelConfig());
        modelBuilder.ApplyConfiguration(new StockDetailReadModelConfig());
        modelBuilder.ApplyConfiguration(new SerialItemReadModelConfig());
        modelBuilder.ApplyConfiguration(new SellerReadModelConfig());
        modelBuilder.ApplyConfiguration(new WarehouseReadModelConfig());
        modelBuilder.ApplyConfiguration(new LocationReadModelConfig());
        modelBuilder.ApplyConfiguration(new QualityStatusReadModelConfig());
    }
}

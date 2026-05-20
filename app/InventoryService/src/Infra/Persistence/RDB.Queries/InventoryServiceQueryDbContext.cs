namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.AttributeDefinitions.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Categories.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Tags.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Tags.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.UnitOfMeasures.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.UnitOfMeasures.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.VariantNameFormulas.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.VariantNameFormulas.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryTransactions.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryTransactions.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.PriceChannels.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.PriceChannels.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.PriceTypes.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.PriceTypes.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.SellerVariantPrices.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.SellerVariantPrices.Entities;
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
using OysterFx.Infra.Auth.UserServices;
using OysterFx.Infra.Persistence.RDB.Queries;

public class InventoryServiceQueryDbContext : QueryDbContext
{
    public InventoryServiceQueryDbContext(
        DbContextOptions options,
        IUserInfoService? userInfoService = null)
        : base(options, userInfoService)
    {
    }

    public DbSet<AttributeDefinitionReadModel> AttributeDefinitions => Set<AttributeDefinitionReadModel>();
    public DbSet<AttributeOptionReadModel> AttributeOptions => Set<AttributeOptionReadModel>();
    public DbSet<CategoryReadModel> Categories => Set<CategoryReadModel>();
    public DbSet<CategoryVariantNameFormulaReadModel> CategoryVariantNameFormulas => Set<CategoryVariantNameFormulaReadModel>();
    public DbSet<CategoryVariantNameFormulaPartReadModel> CategoryVariantNameFormulaParts => Set<CategoryVariantNameFormulaPartReadModel>();
    public DbSet<CategorySchemaVersionReadModel> CategorySchemaVersions => Set<CategorySchemaVersionReadModel>();
    public DbSet<CategoryAttributeRuleReadModel> CategoryAttributeRules => Set<CategoryAttributeRuleReadModel>();
    public DbSet<ProductReadModel> Products => Set<ProductReadModel>();
    public DbSet<ProductAttributeValueReadModel> ProductAttributeValues => Set<ProductAttributeValueReadModel>();
    public DbSet<ProductVariantReadModel> ProductVariants => Set<ProductVariantReadModel>();
    public DbSet<TagReadModel> Tags => Set<TagReadModel>();
    public DbSet<VariantAttributeValueReadModel> VariantAttributeValues => Set<VariantAttributeValueReadModel>();
    public DbSet<VariantUomConversionReadModel> VariantUomConversions => Set<VariantUomConversionReadModel>();
    public DbSet<VariantComponentReadModel> VariantComponents => Set<VariantComponentReadModel>();
    public DbSet<VariantAddOnReadModel> VariantAddOns => Set<VariantAddOnReadModel>();
    public DbSet<VariantImageReadModel> VariantImages => Set<VariantImageReadModel>();
    public DbSet<VariantTagReadModel> VariantTags => Set<VariantTagReadModel>();
    public DbSet<UnitOfMeasureReadModel> UnitOfMeasures => Set<UnitOfMeasureReadModel>();
    public DbSet<InventoryDocumentReadModel> InventoryDocuments => Set<InventoryDocumentReadModel>();
    public DbSet<InventoryDocumentLineReadModel> InventoryDocumentLines => Set<InventoryDocumentLineReadModel>();
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
    public DbSet<PriceTypeReadModel> PriceTypes => Set<PriceTypeReadModel>();
    public DbSet<PriceChannelReadModel> PriceChannels => Set<PriceChannelReadModel>();
    public DbSet<SellerVariantPriceReadModel> SellerVariantPrices => Set<SellerVariantPriceReadModel>();
    public DbSet<OfferReadModel> Offers => Set<OfferReadModel>();
    public DbSet<WarehouseReadModel> Warehouses => Set<WarehouseReadModel>();
    public DbSet<LocationReadModel> Locations => Set<LocationReadModel>();
    public DbSet<QualityStatusReadModel> QualityStatuses => Set<QualityStatusReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        AddOrganizationShadowProperties(modelBuilder);
    }
}

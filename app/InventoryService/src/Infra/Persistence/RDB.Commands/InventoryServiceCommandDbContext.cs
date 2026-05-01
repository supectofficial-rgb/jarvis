namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Domain.Fulfillments.Entities;
using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;
using Insurance.InventoryService.AppCore.Domain.InventoryTransactions.Entities;
using Insurance.InventoryService.AppCore.Domain.Pricing.Entities;
using Insurance.InventoryService.AppCore.Domain.Reservations.Entities;
using Insurance.InventoryService.AppCore.Domain.Returns.Entities;
using Insurance.InventoryService.AppCore.Domain.Seller.Entities;
using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;
using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.AttributeDefinitions.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.Categories.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.Products.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.ProductVariants.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.UnitOfMeasures.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.InventoryDocuments.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.InventoryTransactions.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.Pricing.PriceChannels.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.Pricing.PriceTypes.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.Pricing.SellerVariantPrices.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.Seller.Sellers.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.StockDetails.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.StockDetails.SerialItems.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.Warehouse.Locations.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.Warehouse.QualityStatuses.Configs;
using Insurance.InventoryService.Infra.Persistence.RDB.Commands.Warehouse.Warehouses.Configs;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;
using LocationAggregate = Insurance.InventoryService.AppCore.Domain.Warehouse.Entities.Location;
using QualityStatusAggregate = Insurance.InventoryService.AppCore.Domain.Warehouse.Entities.QualityStatus;
using SellerAggregate = Insurance.InventoryService.AppCore.Domain.Seller.Entities.Seller;
using WarehouseAggregate = Insurance.InventoryService.AppCore.Domain.Warehouse.Entities.Warehouse;

public class InventoryServiceCommandDbContext : CommandDbContext
{
    public InventoryServiceCommandDbContext(DbContextOptions<InventoryServiceCommandDbContext> options)
        : base(options)
    {
    }

    public DbSet<AttributeDefinition> AttributeDefinitions => Set<AttributeDefinition>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategorySchemaVersion> CategorySchemaVersions => Set<CategorySchemaVersion>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<UnitOfMeasure> UnitOfMeasures => Set<UnitOfMeasure>();
    public DbSet<InventoryDocument> InventoryDocuments => Set<InventoryDocument>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<StockDetail> StockDetails => Set<StockDetail>();
    public DbSet<SerialItem> SerialItems => Set<SerialItem>();
    public DbSet<SellerAggregate> Sellers => Set<SellerAggregate>();
    public DbSet<PriceType> PriceTypes => Set<PriceType>();
    public DbSet<PriceChannel> PriceChannels => Set<PriceChannel>();
    public DbSet<SellerVariantPrice> SellerVariantPrices => Set<SellerVariantPrice>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<WarehouseAggregate> Warehouses => Set<WarehouseAggregate>();
    public DbSet<LocationAggregate> Locations => Set<LocationAggregate>();
    public DbSet<QualityStatusAggregate> QualityStatuses => Set<QualityStatusAggregate>();
    public DbSet<InventoryReservation> InventoryReservations => Set<InventoryReservation>();
    public DbSet<Fulfillment> Fulfillments => Set<Fulfillment>();
    public DbSet<ReturnRequest> ReturnRequests => Set<ReturnRequest>();
    public DbSet<InventorySourceBalance> InventorySourceBalances => Set<InventorySourceBalance>();
    public DbSet<InventorySourceAllocation> InventorySourceAllocations => Set<InventorySourceAllocation>();
    public DbSet<InventorySourceConsumption> InventorySourceConsumptions => Set<InventorySourceConsumption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AttributeDefinitionConfig());
        modelBuilder.ApplyConfiguration(new AttributeOptionConfig());
        modelBuilder.ApplyConfiguration(new CategoryConfig());
        modelBuilder.ApplyConfiguration(new CategorySchemaVersionConfig());
        modelBuilder.ApplyConfiguration(new CategoryAttributeRuleConfig());
        modelBuilder.ApplyConfiguration(new ProductConfig());
        modelBuilder.ApplyConfiguration(new ProductAttributeValueConfig());
        modelBuilder.ApplyConfiguration(new ProductVariantConfig());
        modelBuilder.ApplyConfiguration(new VariantAttributeValueConfig());
        modelBuilder.ApplyConfiguration(new VariantUomConversionConfig());
        modelBuilder.ApplyConfiguration(new UnitOfMeasureConfig());
        modelBuilder.ApplyConfiguration(new InventoryDocumentConfig());
        modelBuilder.ApplyConfiguration(new InventoryDocumentLineConfig());
        modelBuilder.ApplyConfiguration(new InventoryDocumentLineSerialConfig());
        modelBuilder.ApplyConfiguration(new InventoryTransactionConfig());
        modelBuilder.ApplyConfiguration(new InventoryTransactionLineConfig());
        modelBuilder.ApplyConfiguration(new StockDetailConfig());
        modelBuilder.ApplyConfiguration(new SerialItemConfig());
        modelBuilder.ApplyConfiguration(new SellerConfig());
        modelBuilder.ApplyConfiguration(new PriceTypeConfig());
        modelBuilder.ApplyConfiguration(new PriceChannelConfig());
        modelBuilder.ApplyConfiguration(new SellerVariantPriceConfig());
        modelBuilder.ApplyConfiguration(new OfferConfig());
        modelBuilder.ApplyConfiguration(new WarehouseConfig());
        modelBuilder.ApplyConfiguration(new LocationConfig());
        modelBuilder.ApplyConfiguration(new QualityStatusConfig());

        modelBuilder.Entity<InventoryReservation>().ToTable("InventoryReservations");
        modelBuilder.Entity<ReservationAllocation>().ToTable("ReservationAllocations");
        modelBuilder.Entity<ReservationTransition>().ToTable("ReservationTransitions");

        modelBuilder.Entity<Fulfillment>().ToTable("Fulfillments");
        modelBuilder.Entity<FulfillmentLine>().ToTable("FulfillmentLines");
        modelBuilder.Entity<FulfillmentLineSerial>().ToTable("FulfillmentLineSerials");
        modelBuilder.Entity<FulfillmentTransition>().ToTable("FulfillmentTransitions");

        modelBuilder.Entity<ReturnRequest>().ToTable("ReturnRequests");
        modelBuilder.Entity<ReturnLine>().ToTable("ReturnLines");
        modelBuilder.Entity<ReturnLineSerial>().ToTable("ReturnLineSerials");
        modelBuilder.Entity<ReturnRequestTransition>().ToTable("ReturnRequestTransitions");

        modelBuilder.Entity<InventorySourceBalance>().ToTable("InventorySourceBalances");
        modelBuilder.Entity<InventorySourceAllocation>().ToTable("InventorySourceAllocations");
        modelBuilder.Entity<InventorySourceConsumption>().ToTable("InventorySourceConsumptions");

        modelBuilder.Entity<InventoryReservation>()
            .HasMany(x => x.Allocations)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventoryReservation>()
            .HasMany(x => x.Transitions)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Fulfillment>()
            .HasMany(x => x.Lines)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FulfillmentLine>()
            .HasMany(x => x.Serials)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Fulfillment>()
            .HasMany(x => x.Transitions)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReturnRequest>()
            .HasMany(x => x.Lines)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReturnLine>()
            .HasMany(x => x.Serials)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReturnRequest>()
            .HasMany(x => x.Transitions)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventorySourceBalance>()
            .HasMany(x => x.Allocations)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventorySourceBalance>()
            .HasMany(x => x.Consumptions)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

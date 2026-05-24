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
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Auth.UserServices;
using OysterFx.Infra.Persistence.RDB.Commands;
using LocationAggregate = Insurance.InventoryService.AppCore.Domain.Warehouse.Entities.Location;
using LocationStructureNodeAggregate = Insurance.InventoryService.AppCore.Domain.Warehouse.Entities.LocationStructureNode;
using LocationStructureValueAggregate = Insurance.InventoryService.AppCore.Domain.Warehouse.Entities.LocationStructureValue;
using LocationStructureSelectionAggregate = Insurance.InventoryService.AppCore.Domain.Warehouse.Entities.LocationStructureSelection;
using QualityStatusAggregate = Insurance.InventoryService.AppCore.Domain.Warehouse.Entities.QualityStatus;
using SellerAggregate = Insurance.InventoryService.AppCore.Domain.Seller.Entities.Seller;
using WarehouseAggregate = Insurance.InventoryService.AppCore.Domain.Warehouse.Entities.Warehouse;

public class InventoryServiceCommandDbContext : CommandDbContext
{
    public InventoryServiceCommandDbContext(
        DbContextOptions<InventoryServiceCommandDbContext> options,
        IUserInfoService? userInfoService = null)
        : base(options, userInfoService)
    {
    }

    public DbSet<AttributeDefinition> AttributeDefinitions => Set<AttributeDefinition>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategoryVariantNameFormula> CategoryVariantNameFormulas => Set<CategoryVariantNameFormula>();
    public DbSet<CategorySchemaVersion> CategorySchemaVersions => Set<CategorySchemaVersion>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<VariantComponent> VariantComponents => Set<VariantComponent>();
    public DbSet<VariantAddOn> VariantAddOns => Set<VariantAddOn>();
    public DbSet<VariantImage> VariantImages => Set<VariantImage>();
    public DbSet<VariantTag> VariantTags => Set<VariantTag>();
    public DbSet<UnitOfMeasure> UnitOfMeasures => Set<UnitOfMeasure>();
    public DbSet<InventoryDocument> InventoryDocuments => Set<InventoryDocument>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<InventoryTransactionLineSerial> InventoryTransactionLineSerials => Set<InventoryTransactionLineSerial>();
    public DbSet<StockDetail> StockDetails => Set<StockDetail>();
    public DbSet<SerialItem> SerialItems => Set<SerialItem>();
    public DbSet<SellerAggregate> Sellers => Set<SellerAggregate>();
    public DbSet<PriceType> PriceTypes => Set<PriceType>();
    public DbSet<PriceChannel> PriceChannels => Set<PriceChannel>();
    public DbSet<SellerVariantPrice> SellerVariantPrices => Set<SellerVariantPrice>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<WarehouseAggregate> Warehouses => Set<WarehouseAggregate>();
    public DbSet<LocationAggregate> Locations => Set<LocationAggregate>();
    public DbSet<LocationStructureNodeAggregate> LocationStructureNodes => Set<LocationStructureNodeAggregate>();
    public DbSet<LocationStructureValueAggregate> LocationStructureValues => Set<LocationStructureValueAggregate>();
    public DbSet<LocationStructureSelectionAggregate> LocationStructureSelections => Set<LocationStructureSelectionAggregate>();
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryServiceCommandDbContext).Assembly);

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

        AddOrganizationShadowProperties(modelBuilder);
    }
}

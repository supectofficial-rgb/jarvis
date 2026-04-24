namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class InventoryServiceCommandDbContextFactory : IDesignTimeDbContextFactory<InventoryServiceCommandDbContext>
{
    public InventoryServiceCommandDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("INVENTORYSERVICE_COMMANDDB")
            ?? "Host=localhost;Port=5432;Database=inventory_service;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<InventoryServiceCommandDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new InventoryServiceCommandDbContext(optionsBuilder.Options);
    }
}

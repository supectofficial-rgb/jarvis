namespace Insurance.DynamicStructureService.Infra.Persistence.RDB.Commands;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class DynamicStructureServiceCommandDbContextFactory : IDesignTimeDbContextFactory<DynamicStructureServiceCommandDbContext>
{
    public DynamicStructureServiceCommandDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<DynamicStructureServiceCommandDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("CommandDb"));

        return new DynamicStructureServiceCommandDbContext(optionsBuilder.Options);
    }
}

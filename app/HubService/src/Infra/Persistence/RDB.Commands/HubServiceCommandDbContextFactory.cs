namespace Insurance.HubService.Infra.Persistence.RDB.Commands;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class HubServiceCommandDbContextFactory : IDesignTimeDbContextFactory<HubServiceCommandDbContext>
{
    public HubServiceCommandDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<HubServiceCommandDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("CommandDb"));

        return new HubServiceCommandDbContext(optionsBuilder.Options);
    }
}

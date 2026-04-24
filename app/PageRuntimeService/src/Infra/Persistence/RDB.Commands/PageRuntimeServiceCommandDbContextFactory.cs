namespace Insurance.PageRuntimeService.Infra.Persistence.RDB.Commands;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class PageRuntimeServiceCommandDbContextFactory : IDesignTimeDbContextFactory<PageRuntimeServiceCommandDbContext>
{
    public PageRuntimeServiceCommandDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<PageRuntimeServiceCommandDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("CommandDb"));

        return new PageRuntimeServiceCommandDbContext(optionsBuilder.Options);
    }
}

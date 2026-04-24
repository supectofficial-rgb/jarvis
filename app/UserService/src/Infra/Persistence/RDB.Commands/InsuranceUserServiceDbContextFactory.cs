namespace Insurance.UserService.Infra.Persistence.RDB.Commands;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class InsuranceUserServiceDbContextFactory : IDesignTimeDbContextFactory<InsuranceUserServiceDbContext>
{
    public InsuranceUserServiceDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<InsuranceUserServiceDbContext>();
        optionsBuilder.UseNpgsql(
            configuration.GetConnectionString("DefaultConnection"));

        return new InsuranceUserServiceDbContext(optionsBuilder.Options);
    }
}

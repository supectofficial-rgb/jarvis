namespace Insurance.Infra.Persistence.Sql.Commands;

using Insurance.AppCore.Domain.Parvandes.Entities;
using Insurance.Infra.Persistence.Sql.Commands.Losts.ValueConversions;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class InsuranceCommandDbContext : CommandDbContext
{
    public InsuranceCommandDbContext(DbContextOptions<InsuranceCommandDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<LostClaimantInfo>().Property(e => e.VehiclePlate).HasConversion<VehiclePlateConversion>();
        builder.Entity<Lost>().Property(e => e.AccidentLocation).HasConversion<LocationConversion>();
    }

    public DbSet<Lost> Losts { get; set; }
    public DbSet<LostClaimantInfo> LostClaimantInfos { get; set; }
}
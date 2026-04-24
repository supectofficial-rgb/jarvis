namespace Insurance.UserService.Infra.Persistence.RDB.Commands.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OysterFx.AppCore.Domain.Aggregates;
using OysterFx.AppCore.Domain.ValueObjects;

public static class ModelBuilderExtensions
{
    public static void AddBusinessId(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model
                                               .GetEntityTypes()
                                               .Where(e => typeof(AggregateRoot).IsAssignableFrom(e.ClrType) ||
                                                    typeof(Aggregate).IsAssignableFrom(e.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .Property<BusinessKey>(nameof(BusinessKey)).HasConversion(c => c.Value, d => d)
                .IsUnicode()
                .IsRequired();
            modelBuilder.Entity(entityType.ClrType).HasAlternateKey(nameof(BusinessKey));
        }
    }
    public static ModelBuilder UseValueConverterForType<T>(this ModelBuilder modelBuilder, ValueConverter converter, int maxLenght = 0)
    {
        return modelBuilder.UseValueConverterForType(typeof(T), converter, maxLenght);
    }
    public static ModelBuilder UseValueConverterForType(this ModelBuilder modelBuilder, Type type, ValueConverter converter, int maxLength = 0)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == type);

            foreach (var property in properties)
            {
                modelBuilder.Entity(entityType.Name).Property(property.Name)
                    .HasConversion(converter);
                if (maxLength > 0)
                    modelBuilder.Entity(entityType.Name).Property(property.Name).HasMaxLength(maxLength);
            }
        }

        return modelBuilder;
    }
}
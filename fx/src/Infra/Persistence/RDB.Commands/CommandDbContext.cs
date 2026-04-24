namespace OysterFx.Infra.Persistence.RDB.Commands;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.EventSourcing.Abstractions;
using OysterFx.Infra.Persistence.RDB.Commands.Configs;
using OysterFx.Infra.Persistence.RDB.Commands.Extensions;
using OysterFx.Infra.Persistence.RDB.Commands.Interceptors;
using OysterFx.Infra.Persistence.RDB.Commands.ValueConversions;
using System.Globalization;

public abstract class CommandDbContext : DbContext
{
    protected IDbContextTransaction _transaction;
    public DbSet<OutboxEvent> OutboxEvents { get; set; }

    public CommandDbContext(DbContextOptions options) : base(options) { }

    protected CommandDbContext() { }

    public T? GetShadowPropertyValue<T>(object entity, string propertyName) where T : IConvertible
    {
        var value = Entry(entity).Property(propertyName).CurrentValue;
        return value != null
            ? (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)
            : default;
    }

    public object GetShadowPropertyValue(object entity, string propertyName)
    {
        return Entry(entity).Property(propertyName).CurrentValue!;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.AddAuditableShadowProperties();
        builder.ApplyConfiguration(new OutboxEventConfig());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.AddInterceptors(new AddAuditDataInterceptor(), new AddOutBoxEventSourcingInterceptor());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<Description>().HaveConversion<DescriptionConversion>();
        configurationBuilder.Properties<Title>().HaveConversion<TitleConversion>();
        configurationBuilder.Properties<BusinessKey>().HaveConversion<BusinessKeyConversion>();
        configurationBuilder.Properties<LegalNationalId>().HaveConversion<LegalNationalId>();
        configurationBuilder.Properties<NationalCode>().HaveConversion<NationalCodeConversion>();
    }

    public IEnumerable<string> GetIncludePaths(Type clrEntityType)
    {
        var entityType = Model.FindEntityType(clrEntityType);
        var includedNavigations = new HashSet<INavigation>();
        var stack = new Stack<IEnumerator<INavigation>>();
        while (true)
        {
            var entityNavigations = new List<INavigation>();
            foreach (var navigation in entityType.GetNavigations())
            {
                if (includedNavigations.Add(navigation))
                    entityNavigations.Add(navigation);
            }
            if (entityNavigations.Count == 0)
            {
                if (stack.Count > 0)
                    yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
            }
            else
            {
                foreach (var navigation in entityNavigations)
                {
                    var inverseNavigation = navigation.Inverse;
                    if (inverseNavigation != null)
                        includedNavigations.Add(inverseNavigation);
                }
                stack.Push(entityNavigations.GetEnumerator());
            }
            while (stack.Count > 0 && !stack.Peek().MoveNext())
                stack.Pop();
            if (stack.Count == 0) break;
            entityType = stack.Peek().Current.TargetEntityType;
        }
    }
}

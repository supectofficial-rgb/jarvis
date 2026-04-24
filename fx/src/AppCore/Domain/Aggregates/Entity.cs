namespace OysterFx.AppCore.Domain.Aggregates;

using OysterFx.AppCore.Domain.ValueObjects;

public abstract class Entity<TId> : IAuditableEntity
          where TId : struct,
          IComparable,
          IComparable<TId>,
          IConvertible,
          IEquatable<TId>,
          IFormattable
{
    public TId Id { get; protected set; }
    public BusinessKey BusinessKey { get; protected set; } = BusinessKey.FromGuid(Guid.NewGuid());
    protected Entity() { }
    protected Entity(TId id)
    {
        Id = id;
    }

    public override bool Equals(object obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity<TId> a, Entity<TId> b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity<TId> a, Entity<TId> b) => !(a == b);
}

public abstract class Aggregate : Entity<long> { }
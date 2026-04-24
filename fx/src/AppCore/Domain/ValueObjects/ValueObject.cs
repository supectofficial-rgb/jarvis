using OysterFx.AppCore.Domain.BusinessRules;

namespace OysterFx.AppCore.Domain.ValueObjects;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object> GetEqualityComponents();
    public bool Equals(ValueObject? other) => Equals((object)other!);

    #region Overrides
    public override int GetHashCode()
        => GetEqualityComponents()
        .Select(x => x?.GetHashCode() ?? 0)
        .Aggregate((x, y) => x ^ y);

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType()) return false;

        var other = obj as ValueObject;
        return GetEqualityComponents().SequenceEqual(other?.GetEqualityComponents()!);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null ^ right is null)
            return false;

        return left?.Equals(right) != false;
    }
    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);

    public static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken()) throw new BusinessRuleValidationException(rule);
    }

    #endregion
}
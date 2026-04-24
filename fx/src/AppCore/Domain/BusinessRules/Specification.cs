namespace OysterFx.AppCore.Domain.BusinessRules;

public abstract class Specification<T>
{
    public abstract bool IsSatisfiedBy(T entity);
    public abstract string Message { get; }

    public virtual SpecificationResult Validate(T entity)
    {
        bool satisfied = IsSatisfiedBy(entity);
        if (!satisfied)
            return SpecificationResult.Fail(Message);

        return SpecificationResult.Success();
    }

    public static Specification<T> operator &(Specification<T> left, Specification<T> right)
        => new AndSpecification<T>(left, right);

    public static Specification<T> operator |(Specification<T> left, Specification<T> right)
        => new OrSpecification<T>(left, right);

    public static Specification<T> operator !(Specification<T> spec)
        => new NotSpecification<T>(spec);

    public static bool operator true(Specification<T> _) => false;
    public static bool operator false(Specification<T> _) => false;
}
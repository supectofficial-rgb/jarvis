namespace OysterFx.AppCore.Domain.BusinessRules;

public sealed class NotSpecification<T> : Specification<T>
{
    private readonly Specification<T> _inner;

    public NotSpecification(Specification<T> inner)
    {
        _inner = inner;
    }

    public override string Message => $"NOT ({_inner.Message})";

    public override bool IsSatisfiedBy(T entity)
        => !_inner.IsSatisfiedBy(entity);

    public override SpecificationResult Validate(T entity)
    {
        var innerResult = _inner.Validate(entity);
        if (innerResult.IsSatisfied)
            return SpecificationResult.Fail(Message);

        return SpecificationResult.Success();
    }
}
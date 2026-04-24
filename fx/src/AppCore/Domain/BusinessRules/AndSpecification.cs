namespace OysterFx.AppCore.Domain.BusinessRules;

public sealed class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override string Message => $"{_left.Message} AND {_right.Message}";

    public override bool IsSatisfiedBy(T entity) 
        => _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);

    public override SpecificationResult Validate(T entity)
    {
        var leftResult = _left.Validate(entity);
        if (!leftResult.IsSatisfied)
            return leftResult;

        var rightResult = _right.Validate(entity);
        if (!rightResult.IsSatisfied)
            return rightResult;

        return SpecificationResult.Success();
    }
}
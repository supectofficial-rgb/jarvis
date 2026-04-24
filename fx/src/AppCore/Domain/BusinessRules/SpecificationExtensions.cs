namespace OysterFx.AppCore.Domain.BusinessRules;

public static class SpecificationExtensions
{
    public static void ThrowIfNotSatisfied<T>(this Specification<T> spec, T entity, string? customMessage = null)
    {
        var result = spec.Validate(entity);
        if (!result.IsSatisfied)
            throw new BusinessRuleValidationException(customMessage ?? result.Message!);
    }
}
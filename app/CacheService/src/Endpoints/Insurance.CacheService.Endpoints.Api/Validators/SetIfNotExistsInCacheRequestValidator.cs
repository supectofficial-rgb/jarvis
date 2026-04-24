namespace Insurance.CacheService.Endpoints.Api.Validators;

using FluentValidation;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.SetIfNotExistsInCache;

public class SetIfNotExistsInCacheRequestValidator : AbstractValidator<SetIfNotExistsInCacheRequest>
{
    public SetIfNotExistsInCacheRequestValidator()
    {
        RuleFor(c => c.Key)
            .NotEmpty()
            .WithMessage("Key cannot be empty");

        RuleFor(c => c.Value)
            .NotNull()
            .WithMessage("Value cannot be null");

        RuleFor(c => c.AbsoluteExpirationMinutes)
            .GreaterThan(0)
            .When(c => c.AbsoluteExpirationMinutes.HasValue)
            .WithMessage("AbsoluteExpirationMinutes must be greater than 0 when specified");
    }
}

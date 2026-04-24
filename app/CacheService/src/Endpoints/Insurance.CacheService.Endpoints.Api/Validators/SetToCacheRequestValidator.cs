namespace Insurance.CacheService.Endpoints.Api.Validators;

using FluentValidation;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.SetToCache;

public class SetToCacheRequestValidator : AbstractValidator<SetToCacheRequest>
{
    public SetToCacheRequestValidator()
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

        RuleFor(c => c.SlidingExpirationMinutes)
            .GreaterThan(0)
            .When(c => c.SlidingExpirationMinutes.HasValue)
            .WithMessage("SlidingExpirationMinutes must be greater than 0 when specified");
    }
}

namespace Insurance.CacheService.Endpoints.Api.Validators;

using FluentValidation;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.IncrementInCache;

public class IncrementInCacheRequestValidator : AbstractValidator<IncrementInCacheRequest>
{
    public IncrementInCacheRequestValidator()
    {
        RuleFor(c => c.Key)
            .NotEmpty()
            .WithMessage("Key cannot be empty");

        RuleFor(c => c.Value)
            .NotEqual(0)
            .WithMessage("Value must not be 0");

        RuleFor(c => c.AbsoluteExpirationMinutes)
            .GreaterThan(0)
            .When(c => c.AbsoluteExpirationMinutes.HasValue)
            .WithMessage("AbsoluteExpirationMinutes must be greater than 0 when specified");
    }
}

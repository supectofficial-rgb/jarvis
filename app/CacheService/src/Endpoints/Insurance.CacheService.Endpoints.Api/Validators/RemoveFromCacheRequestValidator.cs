namespace Insurance.CacheService.Endpoints.Api.Validators;

using FluentValidation;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.RemoveFromCache;

public class RemoveFromCacheRequestValidator : AbstractValidator<RemoveFromCacheRequest>
{
    public RemoveFromCacheRequestValidator()
    {
        RuleFor(c => c.Key)
            .NotEmpty()
            .WithMessage("Key cannot be empty");
    }
}

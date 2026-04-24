namespace Insurance.CacheService.Endpoints.Api.Validators;

using FluentValidation;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.ExistsInCache;

public class ExistsInCacheRequestValidator : AbstractValidator<ExistsInCacheRequest>
{
    public ExistsInCacheRequestValidator()
    {
        RuleFor(c => c.Key)
            .NotEmpty()
            .WithMessage("Key cannot be empty");
    }
}

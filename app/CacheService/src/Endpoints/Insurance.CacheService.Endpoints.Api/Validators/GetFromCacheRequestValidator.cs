namespace Insurance.CacheService.Endpoints.Api.Validators;

using FluentValidation;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.GetFromCache;

public class GetFromCacheRequestValidator : AbstractValidator<GetFromCacheRequest>
{
    public GetFromCacheRequestValidator()
    {
        RuleFor(c => c.Key)
            .NotEmpty()
            .WithMessage("Key cannot be empty");
    }
}

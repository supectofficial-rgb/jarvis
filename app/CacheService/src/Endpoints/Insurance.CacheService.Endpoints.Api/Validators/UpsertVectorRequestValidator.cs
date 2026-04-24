namespace Insurance.CacheService.Endpoints.Api.Validators;

using FluentValidation;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.Vector.Upsert;

public class UpsertVectorRequestValidator : AbstractValidator<UpsertVectorRequest>
{
    public UpsertVectorRequestValidator()
    {
        RuleFor(c => c.IndexName).NotEmpty();
        RuleFor(c => c.Key).NotEmpty();
        RuleFor(c => c.Embedding).NotNull();
        RuleFor(c => c.Embedding.Length).GreaterThan(0);
    }
}

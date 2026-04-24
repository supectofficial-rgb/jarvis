namespace Insurance.CacheService.Endpoints.Api.Validators;

using FluentValidation;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.Vector.Search;

public class SearchVectorRequestValidator : AbstractValidator<SearchVectorRequest>
{
    public SearchVectorRequestValidator()
    {
        RuleFor(c => c.IndexName).NotEmpty();
        RuleFor(c => c.QueryEmbedding).NotNull();
        RuleFor(c => c.QueryEmbedding.Length).GreaterThan(0);
        RuleFor(c => c.TopK).GreaterThan(0);
    }
}

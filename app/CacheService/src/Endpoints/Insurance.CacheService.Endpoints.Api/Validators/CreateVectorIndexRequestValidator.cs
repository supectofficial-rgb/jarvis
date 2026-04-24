namespace Insurance.CacheService.Endpoints.Api.Validators;

using FluentValidation;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.Vector.CreateIndex;

public class CreateVectorIndexRequestValidator : AbstractValidator<CreateVectorIndexRequest>
{
    public CreateVectorIndexRequestValidator()
    {
        RuleFor(c => c.IndexName).NotEmpty();
        RuleFor(c => c.Prefix).NotEmpty();
        RuleFor(c => c.Dimension).GreaterThan(0);
        RuleFor(c => c.DistanceMetric).NotEmpty();
        RuleFor(c => c.Algorithm).NotEmpty();
    }
}

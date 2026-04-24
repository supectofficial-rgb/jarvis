namespace Insurance.CacheService.Endpoints.Api.Validators;

using FluentValidation;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.Vector.Delete;

public class DeleteVectorRequestValidator : AbstractValidator<DeleteVectorRequest>
{
    public DeleteVectorRequestValidator()
    {
        RuleFor(c => c.Key).NotEmpty();
    }
}

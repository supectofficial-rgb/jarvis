namespace Insurance.UserService.AppCore.AppServices.Permissions.Commands.CreateSystemPermission;

using FluentValidation;
using Insurance.UserService.AppCore.Shared.Permissions.Commands.Create;

public class CreateSystemPermissionCommandValidator : AbstractValidator<CreateSystemPermissionCommand>
{
    public CreateSystemPermissionCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
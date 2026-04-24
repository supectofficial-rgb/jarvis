namespace Insurance.UserService.AppCore.AppServices.Permissions.Commands.CreateApplicationPermission;

using FluentValidation;
using Insurance.UserService.AppCore.Shared.Permissions.Commands.CreateApplicationPermission;

public class CreateApplicationPermissionCommandValidator : AbstractValidator<CreateApplicationPermissionCommand>
{
    public CreateApplicationPermissionCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
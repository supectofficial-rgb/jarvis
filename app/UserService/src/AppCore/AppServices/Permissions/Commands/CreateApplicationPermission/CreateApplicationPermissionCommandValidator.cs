namespace Insurance.UserService.AppCore.AppServices.Permissions.Commands.CreateApplicationPermission;

using FluentValidation;
using Insurance.UserService.AppCore.Shared.Permissions.Commands.CreateApplicationPermission;

public class CreateApplicationPermissionCommandValidator : AbstractValidator<CreateApplicationPermissionCommand>
{
    public CreateApplicationPermissionCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Title).MaximumLength(200);
        RuleFor(x => x.Module).MaximumLength(100);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

namespace Insurance.UserService.AppCore.AppServices.Permissions.Commands.CreateSystemPermission;

using FluentValidation;
using Insurance.UserService.AppCore.Shared.Permissions.Commands.Create;

public class CreateSystemPermissionCommandValidator : AbstractValidator<CreateSystemPermissionCommand>
{
    public CreateSystemPermissionCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Title).MaximumLength(200);
        RuleFor(x => x.Module).MaximumLength(100);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

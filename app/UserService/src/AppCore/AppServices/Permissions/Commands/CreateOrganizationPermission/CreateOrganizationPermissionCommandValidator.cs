namespace Insurance.UserService.AppCore.AppServices.Permissions.Commands.CreateOrganizationPermission;

using FluentValidation;
using Insurance.UserService.AppCore.Shared.Permissions.Commands.CreateOrganizationPermission;

public class CreateOrganizationPermissionCommandValidator : AbstractValidator<CreateOrganizationPermissionCommand>
{
    public CreateOrganizationPermissionCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
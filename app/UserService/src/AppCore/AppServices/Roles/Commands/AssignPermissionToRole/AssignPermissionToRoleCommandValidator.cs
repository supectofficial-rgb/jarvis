namespace Insurance.UserService.AppCore.AppServices.Roles.Commands.AssignPermissionToRole;

using FluentValidation;
using Insurance.UserService.AppCore.Shared.Roles.Commands.AssignPermissionToRole;

public class AssignPermissionToRoleCommandValidator : AbstractValidator<AssignPermissionToRoleCommand>
{
    public AssignPermissionToRoleCommandValidator()
    {
        RuleFor(x => x.RoleBusinessKey).NotEmpty();
        RuleFor(x => x.PermissionBusinessKey).NotEmpty();
    }
}
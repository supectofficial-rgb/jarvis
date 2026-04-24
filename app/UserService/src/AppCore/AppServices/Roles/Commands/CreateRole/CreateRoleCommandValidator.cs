namespace Insurance.UserService.AppCore.AppServices.Roles.Commands.CreateRole;

using FluentValidation;
using Insurance.UserService.AppCore.Domain.Roles.Enums;
using Insurance.UserService.AppCore.Shared.Roles.Commands.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Scope)
            .IsInEnum()
            .NotEqual(RoleScope.Unknown);
    }
}
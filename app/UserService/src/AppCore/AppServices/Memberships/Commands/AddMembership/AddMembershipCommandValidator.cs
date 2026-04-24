namespace Insurance.UserService.AppCore.AppServices.Memberships.Commands.AddMembership;

using FluentValidation;
using Insurance.UserService.AppCore.Shared.Users.Commands.AddMembership;

public class AddMembershipCommandValidator : AbstractValidator<AddMembershipCommand>
{
    public AddMembershipCommandValidator()
    {
        RuleFor(x => x.UserBusinessKey).NotEmpty();
        RuleFor(x => x.OrganizationBusinessKey).NotEmpty();
    }
}
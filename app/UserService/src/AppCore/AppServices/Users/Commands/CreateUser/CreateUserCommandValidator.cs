namespace Insurance.UserService.AppCore.AppServices.Users.Commands.CreateUser;

using FluentValidation;
using Insurance.UserService.AppCore.Shared.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.MobileNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(150);
    }
}
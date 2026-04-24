namespace Insurance.UserService.AppCore.AppServices.Organizations.Commands;

using FluentValidation;
using Insurance.UserService.AppCore.Shared.Organizations.Commands.Create;

public class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator() { }
}
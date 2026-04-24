namespace Insurance.UserService.AppCore.Shared.Users.Commands.CreateUser;

using OysterFx.AppCore.Shared.Commands;
using System;

public class CreateUserCommand : ICommand<Guid>
{
    public Guid OrganizationBusinessKey { get; set; }
    public string? MobileNumber { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public Guid RoleBusinessKey { get; set; }
}
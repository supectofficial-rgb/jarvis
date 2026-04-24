namespace Insurance.UserService.AppCore.Shared.Users.Commands.AddMembership;

using OysterFx.AppCore.Shared.Commands;
using System;

public class AddMembershipCommand : ICommand<Guid>
{
    public Guid UserBusinessKey { get; set; }
    public Guid OrganizationBusinessKey { get; set; }
}
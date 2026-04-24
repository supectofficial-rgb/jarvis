namespace Insurance.UserService.AppCore.Shared.Users.Commands.AssignRoleToMembership;

using OysterFx.AppCore.Shared.Commands;
using System;

public class AssignRoleToMembershipCommand : ICommand<bool>
{
    public Guid MembershipBusinessKey { get; set; }
    public Guid RoleBusinessKey { get; set; }
}
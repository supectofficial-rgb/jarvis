namespace Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByCredential;

using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using OysterFx.AppCore.Domain.ValueObjects;

public class LoginByCredentialCommandResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime TokenExpiration { get; set; }
    public Account User { get; set; } = null!;
    public List<MembershipDto> Memberships { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
    public List<string> Roles { get; set; } = new();

    public Guid? ActiveMembershipBusinessKey { get; set; }
    public Guid? ActiveOrganizationBusinessKey { get; set; }
    public List<Guid> ActiveRoleBusinessKeys { get; set; } = new();
}

public class MembershipDto
{
    public required BusinessKey BusinessKey { get; set; }
    public required BusinessKey RoleBusinessKey { get; set; }
    public required BusinessKey OrganizationBusinessKey { get; set; }
}

namespace Insurance.UserService.AppCore.AppServices.Users.Commands.DataSeed;

using Insurance.UserService.AppCore.Shared.Organizations.Commands.Create;
using Insurance.UserService.AppCore.Shared.Permissions.Commands.Create;
using Insurance.UserService.AppCore.Shared.Roles.Commands.AssignPermissionToRole;
using Insurance.UserService.AppCore.Shared.Roles.Commands.CreateRole;
using Insurance.UserService.AppCore.Shared.Users.Commands.AddMembership;
using Insurance.UserService.AppCore.Shared.Users.Commands.AssignRoleToMembership;
using Insurance.UserService.AppCore.Shared.Users.Commands.CreateUser;
using Insurance.UserService.AppCore.Shared.Users.Commands.DataSeed;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class DataSeedCommandHandler(ICommandBus commandBus) : CommandHandler<DataSeedCommand, bool>
{
    private readonly ICommandBus _commandBus = commandBus;

    public override async Task<CommandResult<bool>> Handle(DataSeedCommand command)
    {
        var organizationBusinessKey = await SeedOrganization();
        var roleBusinessKey = await SeedRole();
        await SeedPermission(roleBusinessKey);
        var userBusinessKey = await SeedUser(organizationBusinessKey, roleBusinessKey);
        var membershipBusinessKey = await SeedMembership(organizationBusinessKey, userBusinessKey);
        await SeedMembershipRoleAssignment(membershipBusinessKey, roleBusinessKey);

        return await OkAsync(true);
    }

    private async Task<Guid> SeedOrganization()
    {
        CreateOrganizationCommand createOrganizationCommand = new CreateOrganizationCommand()
        {
            Title = "Org01"
        };

        var commandResult = await _commandBus.SendAsync<CreateOrganizationCommand, Guid>(createOrganizationCommand);
        return commandResult.Data;

    }

    private async Task<Guid> SeedRole()
    {
        CreateRoleCommand createRoleCommand = new()
        {
            Name = "SysAdmin",
            Scope = Domain.Roles.Enums.RoleScope.System
        };

        var commandResult = await _commandBus.SendAsync<CreateRoleCommand, Guid>(createRoleCommand);
        return commandResult.Data;
    }

    private async Task SeedPermission(Guid Role1BusinessKey)
    {
        var permissionBusinessKeys = new Dictionary<string, Guid>();

        var permissions = new[]
        {
        new { Code = "Role.Create", Description = "ایجاد نقش" },
        new { Code = "Role.AssignPermission", Description = "تخصیص دسترسی به نقش" },
        new { Code = "Role.RemovePermission", Description = "حذف دسترسی از نقش" },
        new { Code = "Permission.Create", Description = "ایجاد دسترسی" },
        new { Code = "Organization.Create", Description = "تعریف سازمان" },
        new { Code = "Organization.GetAll", Description = "مشاهده لیست سازمانها" },
        new { Code = "Membership.Create", Description = "ایجاد Membership" },
        new { Code = "Membership.AssignRole", Description = "تخصیص نقش به Membership" },
        new { Code = "Auth.Register", Description = "ثبت نام" },
        new { Code = "User.Create", Description = "تعریف کاربر" }
    };

        foreach (var permission in permissions)
        {
            CreateSystemPermissionCommand createSystemPermissionCommand = new()
            {
                Code = permission.Code,
                Description = permission.Description
            };

            var businessKey = await _commandBus.SendAsync<CreateSystemPermissionCommand, Guid>(createSystemPermissionCommand);
            permissionBusinessKeys[permission.Code] = businessKey.Data;
        }

        // حالا می‌توانید از permissionBusinessKeys برای تخصیص به نقش استفاده کنید
        await SeedAllRolePermissions(Role1BusinessKey, permissionBusinessKeys);
    }

    private async Task SeedAllRolePermissions(Guid Role1BusinessKey, Dictionary<string, Guid> permissionBusinessKeys)
    {
        // فرض کنید Role1BusinessKey از قبل تعریف شده است
        Guid roleBusinessKey = Role1BusinessKey; // این باید از قبل مقداردهی شده باشد

        foreach (var permissionBusinessKey in permissionBusinessKeys.Values)
        {
            AssignPermissionToRoleCommand assignPermissionToRoleCommand = new()
            {
                RoleBusinessKey = roleBusinessKey,
                PermissionBusinessKey = permissionBusinessKey
            };

            await _commandBus.SendAsync<AssignPermissionToRoleCommand, bool>(assignPermissionToRoleCommand);
        }
    }

    private async Task<Guid> SeedUser(Guid organizationBusinessKey, Guid roleBusinessKey)
    {
        CreateUserCommand createUserCommand = new()
        {
            MobileNumber = "+989390000000",
            UserName = "admin",
            Password = "1qaz!QAZ",
            OrganizationBusinessKey = organizationBusinessKey,
            RoleBusinessKey = roleBusinessKey
        };

        var commandResult = await _commandBus.SendAsync<CreateUserCommand, Guid>(createUserCommand);
        return commandResult.Data;
    }

    private async Task<Guid> SeedMembership(Guid organizationBusinessKey, Guid userBusinessKey)
    {
        AddMembershipCommand addMembershipCommand = new()
        {
            OrganizationBusinessKey = organizationBusinessKey,
            UserBusinessKey = userBusinessKey
        };

        var commandResult = await _commandBus.SendAsync<AddMembershipCommand, Guid>(addMembershipCommand);
        return commandResult.Data;
    }

    private async Task SeedMembershipRoleAssignment(Guid membershipBusinessKey, Guid roleBusinessKey)
    {
        AssignRoleToMembershipCommand assignRoleToMembershipCommand = new()
        {
            MembershipBusinessKey = membershipBusinessKey,
            RoleBusinessKey = roleBusinessKey
        };

        await _commandBus.SendAsync<AssignRoleToMembershipCommand, bool>(assignRoleToMembershipCommand);
    }
}
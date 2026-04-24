namespace Insurance.UserService.AppCore.AppServices.Users.Commands.CreateUser;

using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using Insurance.UserService.AppCore.Domain.Memberships.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Shared.Memberships.Commands;
using Insurance.UserService.AppCore.Shared.Organizations.Commands;
using Insurance.UserService.AppCore.Shared.Roles.Commands;
using Insurance.UserService.AppCore.Shared.Users.Commands;
using Insurance.UserService.AppCore.Shared.Users.Commands.CreateUser;
using Microsoft.AspNetCore.Identity;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using System;
using System.Threading.Tasks;

public class CreateUserCommandHandler : CommandHandler<CreateUserCommand, Guid>
{
    private readonly IUserCommandRepository _userCommandRepository;
    private readonly UserManager<Account> _userManager;
    private readonly IRoleCommandRepository _roleCommandRepository;
    private readonly IOrganizationCommandRepository _organizationCommandRepository;
    private readonly IMembershipCommandRepository _membershipCommandRepository;
    private readonly RoleManager<AppRole> _roleManager;

    public CreateUserCommandHandler(
        IUserCommandRepository userCommandRepository,
        UserManager<Account> userManager,
        IRoleCommandRepository roleCommandRepository,
        IOrganizationCommandRepository organizationCommandRepository,
        IMembershipCommandRepository membershipCommandRepository,
        RoleManager<AppRole> roleManager)
    {
        _userCommandRepository = userCommandRepository;
        _userManager = userManager;
        _roleCommandRepository = roleCommandRepository;
        _organizationCommandRepository = organizationCommandRepository;
        _membershipCommandRepository = membershipCommandRepository;
        _roleManager = roleManager;
    }

    public override async Task<CommandResult<Guid>> Handle(CreateUserCommand command)
    {
        var user = User.NewUser(command.MobileNumber!);
        await _userCommandRepository.InsertAsync(user);

        var account = Account.Create(user.BusinessKey, command.UserName!);

        var createdUserResult = await _userManager.CreateAsync(account, command.Password!);
        if (!createdUserResult.Succeeded)
        {
            var errors = string.Join(" | ", createdUserResult.Errors.Select(c => c.Description));
            return await FailAsync(errors);
        }

        var selectedRole = await _roleCommandRepository.GetAsync(command.RoleBusinessKey);

        var identityRole = await _roleManager.FindByIdAsync(selectedRole.Id.ToString());
        if (identityRole != null)
        {
            // Check if normalized name is set
            if (string.IsNullOrEmpty(identityRole.NormalizedName))
            {
                identityRole.NormalizedName = _roleManager.KeyNormalizer.NormalizeName(identityRole.Name!);
                await _roleManager.UpdateAsync(identityRole);
            }

            // Verify the role can be found by name in the role manager
            var roleByName = await _roleManager.FindByNameAsync(identityRole.Name!);
            if (roleByName == null)
            {
                // Force update the normalized name
                identityRole.NormalizedName = identityRole.Name!.ToUpperInvariant();
                await _roleManager.UpdateAsync(identityRole);
            }

            await _userManager.AddToRoleAsync(account, identityRole.Name!);
        }

        //if (selectedRole is not null)
        //{
        //    //await _userManager.AddToRoleAsync(account, selectedRole.Name!);

        //    var identityRole = await _roleManager.FindByIdAsync(selectedRole.Id.ToString());
        //    if (identityRole != null)
        //    {
        //        await _userManager.AddToRoleAsync(account, identityRole.Name!);
        //    }
        //    else
        //    {
        //        // If role exists in your repo but not in Identity, sync it
        //        var createResult = await _roleManager.CreateAsync(selectedRole);
        //        if (createResult.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(account, selectedRole.Name!);
        //        }
        //    }
        //}

        var organization = await _organizationCommandRepository.GetAsync(command.OrganizationBusinessKey);

        if (organization is not null)
        {
            var membership = Membership.Create(organization.TenantId, user.BusinessKey, organization.BusinessKey);
            await _membershipCommandRepository.InsertAsync(membership);
            //await _membershipCommandRepository.CommitAsync();
        }

        await _userCommandRepository.CommitAsync();

        return await OkAsync(user.BusinessKey.Value);
    }
}
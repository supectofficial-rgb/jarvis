namespace Insurance.UserService.AppCore.AppServices.Users.Commands.CreateUser;

using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using Insurance.UserService.AppCore.Domain.MembershipRoleAssignments.Entities;
using Insurance.UserService.AppCore.Domain.Memberships.Entities;
using Insurance.UserService.AppCore.Domain.Roles.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Shared.MembershipRoleAssignments.Commands;
using Insurance.UserService.AppCore.Shared.Memberships.Commands;
using Insurance.UserService.AppCore.Shared.Organizations.Commands;
using Insurance.UserService.AppCore.Shared.Roles.Commands;
using Insurance.UserService.AppCore.Shared.Users.Commands;
using Insurance.UserService.AppCore.Shared.Users.Commands.CreateUser;
using Microsoft.AspNetCore.Identity;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using OysterFx.AppCore.Domain.ValueObjects;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class CreateUserCommandHandler : CommandHandler<CreateUserCommand, Guid>
{
    private readonly IUserCommandRepository _userCommandRepository;
    private readonly UserManager<Account> _userManager;
    private readonly IRoleCommandRepository _roleCommandRepository;
    private readonly IOrganizationCommandRepository _organizationCommandRepository;
    private readonly IMembershipCommandRepository _membershipCommandRepository;
    private readonly IMembershipRoleAssignmentCommandRepository _membershipRoleAssignmentCommandRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserCommandRepository userCommandRepository,
        UserManager<Account> userManager,
        IRoleCommandRepository roleCommandRepository,
        IOrganizationCommandRepository organizationCommandRepository,
        IMembershipCommandRepository membershipCommandRepository,
        IMembershipRoleAssignmentCommandRepository membershipRoleAssignmentCommandRepository,
        ICurrentUserService currentUserService,
        RoleManager<AppRole> roleManager,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userCommandRepository = userCommandRepository;
        _userManager = userManager;
        _roleCommandRepository = roleCommandRepository;
        _organizationCommandRepository = organizationCommandRepository;
        _membershipCommandRepository = membershipCommandRepository;
        _membershipRoleAssignmentCommandRepository = membershipRoleAssignmentCommandRepository;
        _currentUserService = currentUserService;
        _roleManager = roleManager;
        _logger = logger;
    }

    public override async Task<CommandResult<Guid>> Handle(CreateUserCommand command)
    {
        try
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
            if (selectedRole is null)
            {
                throw new InvalidOperationException($"Role not found for BusinessKey: {command.RoleBusinessKey:D}");
            }

            var identityRole = await _roleManager.FindByIdAsync(selectedRole.Id.ToString());
            if (identityRole is null)
            {
                throw new InvalidOperationException($"Identity role not found for Role Id: {selectedRole.Id}");
            }

            if (string.IsNullOrWhiteSpace(identityRole.Name))
            {
                throw new InvalidOperationException($"Identity role name is empty for Role Id: {selectedRole.Id}");
            }

            if (string.IsNullOrEmpty(identityRole.NormalizedName))
            {
                identityRole.NormalizedName = _roleManager.KeyNormalizer.NormalizeName(identityRole.Name);
                await _roleManager.UpdateAsync(identityRole);
            }

            var roleByName = await _roleManager.FindByNameAsync(identityRole.Name);
            if (roleByName is null)
            {
                throw new InvalidOperationException($"Identity role could not be resolved by name: {identityRole.Name}");
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(account, identityRole.Name);
            if (!addToRoleResult.Succeeded)
            {
                var errors = string.Join(" | ", addToRoleResult.Errors.Select(c => c.Description));
                throw new InvalidOperationException($"Failed to assign identity role '{identityRole.Name}' to user '{account.UserName}'. Errors: {errors}");
            }

            var organizationBusinessKey = _currentUserService.CurrentOrganizationKey;

            if (organizationBusinessKey is null)
            {
                throw new InvalidOperationException("CurrentOrganizationKey claim was not found. Membership and role assignment cannot be created.");
            }

            var organization = await _organizationCommandRepository.GetAsync(organizationBusinessKey);
            if (organization is null)
            {
                throw new InvalidOperationException($"Organization not found for BusinessKey: {organizationBusinessKey.Value:D}");
            }

            var membership = Membership.Create(organization.TenantId, user.BusinessKey, organization.BusinessKey);
            await _membershipCommandRepository.InsertAsync(membership);

            var membershipRoleAssignment = MembershipRoleAssignment.Create(membership.BusinessKey, BusinessKey.FromGuid(command.RoleBusinessKey));
            await _membershipRoleAssignmentCommandRepository.InsertAsync(membershipRoleAssignment);

            await _userCommandRepository.CommitAsync();

            return await OkAsync(user.BusinessKey.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create user failed. Command: {@Command}", command);

            var errorDetails = ex.ToString();
            if (ex.InnerException is not null)
            {
                errorDetails += Environment.NewLine + "INNER:" + Environment.NewLine + ex.InnerException;
            }

            return await FailAsync(errorDetails);
        }
    }
}

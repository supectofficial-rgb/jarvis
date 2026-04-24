namespace Insurance.UserService.Infra.Persistence.RDB.Commands;

using Insurance.UserService.AppCore.Shared.MembershipRoleAssignments.Commands;
using Insurance.UserService.AppCore.Shared.Memberships.Commands;
using Insurance.UserService.AppCore.Shared.Organizations.Commands;
using Insurance.UserService.AppCore.Shared.OtpCodeContracts.OtpCodes.Commands;
using Insurance.UserService.AppCore.Shared.Permissions.Commands;
using Insurance.UserService.AppCore.Shared.Roles.Commands;
using Insurance.UserService.AppCore.Shared.Users.Commands;
using Insurance.UserService.Infra.Persistence.RDB.Commands.MembershipRoleAssignments;
using Insurance.UserService.Infra.Persistence.RDB.Commands.Memberships;
using Insurance.UserService.Infra.Persistence.RDB.Commands.Organizations;
using Insurance.UserService.Infra.Persistence.RDB.Commands.OtpCodes;
using Insurance.UserService.Infra.Persistence.RDB.Commands.Permissions;
using Insurance.UserService.Infra.Persistence.RDB.Commands.Roles;
using Insurance.UserService.Infra.Persistence.RDB.Commands.Users;
using Microsoft.Extensions.DependencyInjection;

public static class DIExtensions
{
    public static IServiceCollection AddUserServiceCommandPersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<IOtpCodeCommandRepository, OtpCodeCommandRepository>();
        services.AddScoped<IUserCommandRepository, UserCommandRepository>();
        services.AddScoped<IRoleCommandRepository, RoleCommandRepository>();
        services.AddScoped<IPermissionCommandRepository, PermissionCommandRepository>();
        services.AddScoped<IOrganizationCommandRepository, OrganizationCommandRepository>();
        services.AddScoped<IMembershipCommandRepository, MembershipCommandRepository>();
        services.AddScoped<IMembershipRoleAssignmentCommandRepository, MembershipRoleAssignmentCommandRepository>();
        return services;
    }
}
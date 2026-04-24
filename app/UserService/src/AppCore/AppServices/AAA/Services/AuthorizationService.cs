namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

using Insurance.UserService.AppCore.Shared.AAA.Services;
using Insurance.UserService.Infra.Persistence.RDB.Commands;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AuthorizationService : IAuthorizationService
{
    private readonly InsuranceUserServiceDbContext _context;

    public AuthorizationService(InsuranceUserServiceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasPermissionAsync(long userId, string permissionCode, BusinessKey organizationKey)
    {
        //var query = from user in _context.Users
        //            join userPersona in _context.UserPersonas on user.BusinessKey equals userPersona.UserBusinessKey
        //            join persona in _context.Personas on userPersona.PersonaBusinessKey equals persona.BusinessKey
        //            join userRole in _context.UserRoles on user.BusinessKey equals userRole.UserId
        //            join role in _context.Roles on userRole.RoleId equals role.BusinessKey
        //            join rolePermission in _context.RolePermissions on role.BusinessKey equals rolePermission.RoleBusinessKey
        //            join permission in _context.Permissions on rolePermission.PermissionBusinessKey equals permission.BusinessKey
        //            where user.Id == userId &&
        //                  persona.OrganizationBusinessKey == organizationKey &&
        //                  permission.Code == permissionCode &&
        //                  user.IsActive &&
        //                  role.IsActive &&
        //                  permission.IsActive
        //            select permission;

        //return await query.AnyAsync();
        return default;
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(long userId, BusinessKey personaKey)
    {
        //var permissions = await (from user in _context.Users
        //                         join userRole in _context.UserRoles on user.BusinessKey equals userRole.UserId
        //                         join rolePermission in _context.RolePermissions on userRole.RoleId equals rolePermission.RoleBusinessKey
        //                         join permission in _context.Permissions on rolePermission.PermissionBusinessKey equals permission.BusinessKey
        //                         where user.Id == userId &&
        //                               userRole.PersonaId == personaKey &&
        //                               user.IsActive &&
        //                               permission.IsActive
        //                         select permission.Code)
        //                         .Distinct()
        //                         .ToListAsync();

        //return permissions;
        return default;
    }

    public Task<bool> IsUserInRoleAsync(long userId, string roleName, BusinessKey personaKey)
    {
        throw new NotImplementedException();
    }
}
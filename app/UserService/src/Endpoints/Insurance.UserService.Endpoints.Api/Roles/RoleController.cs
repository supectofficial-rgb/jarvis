namespace Insurance.UserService.Endpoints.Api.Roles;

using Insurance.UserService.AppCore.Domain.Roles.Entities;
using Insurance.UserService.AppCore.Shared.Roles.Commands.AssignPermissionToRole;
using Insurance.UserService.AppCore.Shared.Roles.Commands.CreateRole;
using Insurance.UserService.AppCore.Shared.Roles.Commands.RemovePermissionFromRole;
using Insurance.UserService.Infra.Persistence.RDB.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OysterFx.Endpoints.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/UserService/[controller]")]
public class RoleController : OysterFxController
{
    private readonly InsuranceUserServiceDbContext _dbContext;

    public RoleController(InsuranceUserServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Authorize(Policy = "Role.Create")]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
        => await SendCommand<CreateRoleCommand, Guid>(command);

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var items = await _dbContext.Set<AppRole>()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new
            {
                roleBusinessKey = x.BusinessKey.Value.ToString("D"),
                name = x.Name,
                scope = x.Scope.ToString()
            })
            .ToListAsync();

        return Json(new
        {
            isSuccess = true,
            data = items
        });
    }

    [Authorize(Policy = "Role.AssignPermission")]
    [HttpPost("assign-permission")]
    public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionToRoleCommand command)
        => await SendCommand<AssignPermissionToRoleCommand, bool>(command);

    [Authorize(Policy = "Role.RemovePermission")]
    [HttpPost("remove-permission")]
    public async Task<IActionResult> RemovePermission([FromBody] RemovePermissionFromRoleCommand command)
        => await SendCommand<RemovePermissionFromRoleCommand, bool>(command);
}

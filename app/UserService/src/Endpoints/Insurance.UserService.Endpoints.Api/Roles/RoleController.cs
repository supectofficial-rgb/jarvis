namespace Insurance.UserService.Endpoints.Api.Roles;

using Insurance.UserService.AppCore.Shared.Roles.Commands.AssignPermissionToRole;
using Insurance.UserService.AppCore.Shared.Roles.Commands.CreateRole;
using Insurance.UserService.AppCore.Shared.Roles.Commands.RemovePermissionFromRole;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/UserService/[controller]")]
public class RoleController : OysterFxController
{
    [Authorize(Policy = "Role.Create")]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
        => await SendCommand<CreateRoleCommand, Guid>(command);

    [Authorize(Policy = "Role.AssignPermission")]
    [HttpPost("assign-permission")]
    public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionToRoleCommand command)
        => await SendCommand<AssignPermissionToRoleCommand, bool>(command);

    [Authorize(Policy = "Role.RemovePermission")]
    [HttpPost("remove-permission")]
    public async Task<IActionResult> RemovePermission([FromBody] RemovePermissionFromRoleCommand command)
        => await SendCommand<RemovePermissionFromRoleCommand, bool>(command);
}
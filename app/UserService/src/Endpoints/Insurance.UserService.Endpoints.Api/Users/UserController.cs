namespace Insurance.UserService.Endpoints.Api.Users;

using Insurance.UserService.AppCore.Shared.Users.Commands.CreateUser;
using Insurance.UserService.AppCore.Shared.Users.Commands.DataSeed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/UserService/[controller]")]
public class UserController : OysterFxController
{
    [Authorize(Policy = "User.Create")]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        => await SendCommand<CreateUserCommand, Guid>(command);

    [AllowAnonymous]
    [HttpPost("data-seed")]
    public async Task<IActionResult> Create([FromBody] DataSeedCommand command)
        => await SendCommand<DataSeedCommand, bool>(command);
}
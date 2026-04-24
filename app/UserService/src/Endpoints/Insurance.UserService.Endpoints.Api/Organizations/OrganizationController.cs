namespace Insurance.UserService.Endpoints.Api.Organizations;

using Insurance.UserService.AppCore.Shared.Organizations.Commands.Create;
using Insurance.UserService.AppCore.Shared.Organizations.Queries.GetAll;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/UserService/[controller]")]

public class OrganizationController : OysterFxController
{
    [Authorize(Policy = "Organization.Create")]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationCommand command)
        => await SendCommand<CreateOrganizationCommand, Guid>(command);

    [Authorize(Policy = "Organization.GetAll")]
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllOrganizationQuery request)
        => await ExecuteQueryAsync<GetAllOrganizationQuery, IEnumerable<GetAllOrganizationQueryResult?>>(request);
}
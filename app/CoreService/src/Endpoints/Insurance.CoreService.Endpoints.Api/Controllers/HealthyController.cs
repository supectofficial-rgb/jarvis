namespace Insurance.CoreService.Endpoints.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[AllowAnonymous]
[Route("api/CoreService")]
public class HealthyController: OysterFxController
{
    [HttpGet("health")]
    public async Task<IActionResult> Health()
    {
        return Ok("healthy");
    }
}
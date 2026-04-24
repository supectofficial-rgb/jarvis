namespace Insurance.InventoryService.Endpoints.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/InventoryService/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Service = "InventoryService",
            Status = "Healthy",
            UtcNow = DateTime.UtcNow
        });
    }
}

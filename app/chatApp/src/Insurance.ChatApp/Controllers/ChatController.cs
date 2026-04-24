namespace Insurance.ChatApp.Controllers;

using Microsoft.AspNetCore.Mvc;

public class ChatController : Controller
{
    [HttpGet("/")]
    [HttpGet("/chat")]
    public IActionResult Index()
    {
        return View();
    }
}

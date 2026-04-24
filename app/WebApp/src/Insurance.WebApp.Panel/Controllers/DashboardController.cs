using Insurance.WebApp.Panel.Models;
using Insurance.WebApp.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.WebApp.Panel.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IApiService apiService, ILogger<DashboardController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetOrganizations()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            var response = await _apiService.GetOrganizationsAsync(token);

            if (response.IsSuccess && response.Data != null)
            {
                return Json(new { success = true, data = response.Data });
            }
            else
            {
                return Json(new { success = false, message = response.ErrorMessage ?? "Failed to get organizations" });
            }
        }
    }
}


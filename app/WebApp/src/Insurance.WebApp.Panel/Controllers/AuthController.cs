using Insurance.WebApp.Panel.Models;
using Insurance.WebApp.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.WebApp.Panel.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiService _apiService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IApiService apiService, ILogger<AuthController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _apiService.LoginAsync(model.UserName, model.Password);

            if (response.IsSuccess && response.Data != null)
            {
                // Store token in session
                HttpContext.Session.SetString("Token", response.Data.Token);
                HttpContext.Session.SetString("UserName", response.Data.User?.UserName ?? model.UserName);
                HttpContext.Session.SetString("TokenExpiration", response.Data.TokenExpiration.ToString());

                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ModelState.AddModelError("", response.ErrorMessage ?? "Invalid username or password");
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}


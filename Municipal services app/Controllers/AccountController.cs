using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;

namespace Municipal_services_app.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        public AccountController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm vm, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) return View(vm);

            var adminCfg = _config.GetSection("AdminAuth");
            var user = adminCfg.GetValue<string>("Username");
            var pass = adminCfg.GetValue<string>("Password");

            // Simple credential check (replace with secure lookup in prod)
            if (string.Equals(vm.Username, user, System.StringComparison.OrdinalIgnoreCase)
                && vm.Password == pass)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, vm.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var identity = new ClaimsIdentity(claims, "MunicipalCookie");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MunicipalCookie", principal);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "AdminRequests");
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MunicipalCookie");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // small VM for login
        public class LoginVm
        {
            [System.ComponentModel.DataAnnotations.Required]
            public string Username { get; set; } = string.Empty;

            [System.ComponentModel.DataAnnotations.Required]
            public string Password { get; set; } = string.Empty;
        }
    }
}
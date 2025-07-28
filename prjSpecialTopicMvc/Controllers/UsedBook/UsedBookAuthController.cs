using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicMvc.Controllers.UsedBook.Common;
using prjSpecialTopicMvc.Features.Usedbook.Application.Authentication;

namespace prjSpecialTopicMvc.Controllers.UsedBook
{
    [Route("usedbooks/auth")]
    public class UsedBookAuthController : BaseController
    {
        public UsedBookAuthController() { }

        [HttpGet]
        public IActionResult Index()
            => View();

        [HttpPost("login-admin")]
        public async Task<IActionResult> LoginAdmin()
        {
            string userId = "EC70B00F-2F1C-4682-A007-EA53F5540683";
            string userName = "Admin Nick";
            string roleName = RoleNames.Admin;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, roleName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(48)
                });
            return RedirectToAction(nameof(UsedBookUserController.Me), "UsedBookUser");
        }

        [HttpPost("login-seller")]
        public async Task<IActionResult> LoginSeller()
        {
            string userId = "8D56A9E9-B7B9-4B71-BDC0-CF22E47F5A63";
            string userName = "Emily Lin";
            string roleName = RoleNames.Seller;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, roleName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(48)
                });
            return RedirectToAction(nameof(UsedBookUserController.Me), "UsedBookUser");
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("denied")]
        public IActionResult Denied()
            => View();
    }
}

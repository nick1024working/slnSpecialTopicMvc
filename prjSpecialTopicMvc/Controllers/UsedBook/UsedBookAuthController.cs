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
        {
            return View();
        }

        [HttpPost("login-admin")]
        public async Task<IActionResult> LoginAdmin()
        {
            string userId = "22B888CB-32AB-4B07-96BF-228B60D3717A";
            string userName = "Admin user554";
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
            string userId = "8B6D8408-02E4-451A-A226-01550CC818E8";
            string userName = "user773";
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

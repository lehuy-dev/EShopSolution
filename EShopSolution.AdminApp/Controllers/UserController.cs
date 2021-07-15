using EShopSolution.AdminApp.Services;
using EShopSolution.Utilities.Tokens;
using EShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EShopSolution.AdminApp.Controllers
{
    public class UserController : Controller
    {
        public readonly IUserServiceAdmin _userServiceAdmin;
        private readonly JwtAuthentication _jwtAuthentication;
        public UserController(IUserServiceAdmin userServiceAdmin, JwtAuthentication jwtAuthentication)
        {
            _userServiceAdmin = userServiceAdmin;
            _jwtAuthentication = jwtAuthentication;
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(ModelState);
            }
            var token = await _userServiceAdmin.Authenticate(request);
            ClaimsPrincipal principal = _jwtAuthentication.GetPrincipalFromToken(token);
            AuthenticationProperties authenticationProperties = new AuthenticationProperties()
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10)
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authenticationProperties);

            return RedirectToAction("Index","Home");
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult ForgotPass()
        {
            return View();
        }
        public IActionResult Denied()
        {
            return View();
        }
    }
}

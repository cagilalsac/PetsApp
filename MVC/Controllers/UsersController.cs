using BLL.Controllers.Bases;
using BLL.DAL;
using BLL.Models;
using BLL.Services.Bases;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MVC.Controllers
{
    public class UsersController : MvcController
    {
        private readonly IService<User, UserModel> _userService;

        public UsersController(IService<User, UserModel> userService)
        {
            _userService = userService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var userModel = _userService.Query().SingleOrDefault(u => u.Record.UserName == user.Record.UserName &&
                    u.Record.Password == user.Record.Password && u.Record.IsActive);
                if (userModel is not null)
                {
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, userModel.UserName),
                        new Claim(ClaimTypes.Role, userModel.Role),
                        new Claim("Id", userModel.Record.Id.ToString())
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(principal, new AuthenticationProperties()
                    {
                        IsPersistent = true
                    });
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}

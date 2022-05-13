using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductWEB.Models;
using ProductWEB.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductWEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly Util<User> util;
        public AccountController(IHttpClientFactory httpClientFactory)
        {
            util = new Util<User>(httpClientFactory);
        }
        public IActionResult Login()
        {
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            if (ModelState.IsValid)
            {
                var modelstateError = await util.LoginAsync(Resource.LoginAPIURL, user);
                if (modelstateError.Response.Errors.Count >0)
                {
                    foreach (var error in modelstateError.Response.Errors)
                    {
                        user.Errors.Add(error);
                    }
                    return View(user);
                }

                if (modelstateError.Token == null) return View(user);

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, modelstateError.UserName));

                foreach (var rolename in modelstateError.Roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, rolename));
                }

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString("Token", modelstateError.Token);
                HttpContext.Session.SetString("UserName", modelstateError.UserName);

                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }

        public IActionResult Register()
        {
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                var modelstateError = await util.RegisterAsync(Resource.RegisterAPIURL, user);
                if (modelstateError.Response.Errors.Count > 0)
                {
                    foreach (var error in modelstateError.Response.Errors)
                    {
                        user.Errors.Add(error);
                    }
                    return View(user);
                }
                return RedirectToAction(nameof(Login));
            }
            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.SetString("Token", string.Empty);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

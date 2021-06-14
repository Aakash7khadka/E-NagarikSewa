using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using smartpalika.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        public AccountController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser>signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "Home");

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM obj,string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(obj.Email, obj.Password, obj.Rememberme,false);
                if (result.Succeeded)
                {
                    if(! string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    return RedirectToAction("index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempt");
            }
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserVM registerUserVM)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser()
                {
                    UserName = registerUserVM.Email,
                    Email=registerUserVM.Email
                };
                var result=await userManager.CreateAsync(user, registerUserVM.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(registerUserVM);
        }
    }
}

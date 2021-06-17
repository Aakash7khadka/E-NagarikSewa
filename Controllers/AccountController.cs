using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using smartpalika.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
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

        public IActionResult Login(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM obj)
        {
            string returnUrl="";
            if (TempData["returnUrl"] !=null)
            {
                 returnUrl = TempData["returnUrl"].ToString();
            }
            
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(obj.Email, obj.Password, obj.Rememberme,false);
                if (result.Succeeded)
                {
                    if(! string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
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
                var files = HttpContext.Request.Form.Files;
                var user = new ApplicationUser()
                {
                    UserName = registerUserVM.Name,
                    Email=registerUserVM.Email,
                    PhoneNumber=registerUserVM.PhoneNumber,
                    Address=registerUserVM.Address
                    

                };
                if (files.Count() != 0)
                {
                    using (var stream = new MemoryStream())
                    {


                        files[0].CopyTo(stream);
                        user.ProfileImage = stream.ToArray();
                    }

                }
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
        [Authorize]
        public async Task<IActionResult> EditUser()
        {
            
            var user =await userManager.FindByNameAsync(User.Identity.Name);
            EditUserVM usr = new EditUserVM()
            {
                Name = user.UserName,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                Email=user.Email,
                Address=user.Address

            };
            return View(usr);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditUser(EditUserVM usr)
        {
            var files = HttpContext.Request.Form.Files;
            var user = await userManager.FindByEmailAsync(usr.Email);
            //user.UserName = usr.Name;
            user.PhoneNumber = usr.PhoneNumber;
            user.Address = usr.Address;
           
            if (files.Count() != 0)
            {
                using (var stream = new MemoryStream())
                {
                
                    
                        files[0].CopyTo(stream);
                        user.ProfileImage = stream.ToArray();
                }
                    
            }
            
            await userManager.UpdateAsync(user);
            await signInManager.SignOutAsync();
           
            await signInManager.SignInAsync(user,isPersistent:false);
            return RedirectToAction("EditUser");
        }
        [Authorize]
        public async Task<IActionResult> DeleteUser()
        {

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            EditUserVM usr = new EditUserVM()
            {
                Name = user.UserName,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                Email = user.Email

            };
            return View(usr);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            await signInManager.SignOutAsync();
            await userManager.DeleteAsync(user);

            return RedirectToAction("Index", "home");
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            EditUserVM usr = new EditUserVM()
            {
                Name = user.UserName,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                Email = user.Email,
                Address=user.Address
                

            };
            return View(usr);
        }



    }
}

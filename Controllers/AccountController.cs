using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
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
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext db;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,RoleManager<IdentityRole> roleManager, IConfiguration configuration,ApplicationDbContext db)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.db = db;
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
                var user = await userManager.FindByEmailAsync(obj.Email);
                if(user!=null && !user.EmailConfirmed &&(await userManager.CheckPasswordAsync(user, obj.Password)))
                {
                    ModelState.AddModelError("", "Email is not verified");
                    return View(obj);
                }

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
                    UserName = registerUserVM.Email,
                    Email=registerUserVM.Email,
                    PhoneNumber=registerUserVM.PhoneNumber,
                    Address=registerUserVM.Address,
                    FullName=registerUserVM.FullName
                    

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
                    //var real_user = userManager.FindByEmailAsync(registerUserVM.Email);
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmation_link = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token },Request.Scheme);
                    Task<bool> email_result = PostMessage(user.Email, confirmation_link,user.FullName);
                    await Task.WhenAll(email_result);
                    var saveResult = email_result.Result;
                    if (saveResult == false)
                    {
                        ViewBag.ErrorTitle = "Error";
                        ViewBag.Message = "Cannot send email";
                        return View("Error");

                    }
                    
                    if (!await roleManager.RoleExistsAsync("citizen"))
                    {
                        try
                        {
                            await roleManager.CreateAsync(new IdentityRole("citizen"));
                        }
                        catch(Exception e)
                        {
                            ViewBag.ErrorTitle = "Error";
                            ViewBag.Message = "Error please contact admin";
                            return View("Error");
                        }
                    }
                    if (!await roleManager.RoleExistsAsync("Admin"))
                    {
                        try
                        {
                            await roleManager.CreateAsync(new IdentityRole("Admin"));
                        }
                        catch (Exception e)
                        {
                            ViewBag.ErrorTitle = "Error";
                            ViewBag.Message = "Error please contact admin";
                            return View("Error");
                        }
                    }
                    if(user.Email.ToUpper()=="AAKASH777KHADKA@GMAIL.COM"|| user.Email.ToUpper() == "BISHALKAN6A@GMAIL.COM")
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        await userManager.AddToRoleAsync(user, "citizen");
                    }
                    //await signInManager.SignInAsync(user, isPersistent: false);
                    ViewBag.ErrorTitle = "Registration Sucessful";
                    ViewBag.Message = "Please Confirm Email before you can get logged in ";
                    return View("RegistrationSucessful");
                    //return RedirectToAction("index", "home");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(registerUserVM);
        }

        public async Task<bool> PostMessage(string email, string message,string name)
        {
            var apiKey = configuration.GetSection("SENDGRID_API_KEY").Value;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("enagariksewa@gmail.com", "E-Nagarik team");
            var subject = "Email Verification";
            var to = new EmailAddress(email, name);
            var plainTextContent = "Please verify your email ";
            var htmlContent = "<strong>Dear "+name+",<br>Please verify your email with this link: </strong><a href=\" "+ message+"\"> Here</a><br>If you can't verify please email us with your registered email address<br>Otherwise, visit the ward office with your National ID card";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId,string token)
        {
            if(userId==null && token == null)
            {
                return View("index", "home");
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorTitle = "Error";
                ViewBag.Message = $"User with id{userId} cannot be found";
                return View("Error");
            }
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {

                ViewBag.Message = "Email verified successfully!";
                return View("Login");
            }
            ViewBag.ErrorTitle = "Error";
            ViewBag.Message = "Email not confirmed";
            return View("Error");
        }

        
        [Authorize]
        public async Task<IActionResult> EditUser()
        {
            
            var user =await userManager.FindByNameAsync(User.Identity.Name);
            EditUserVM usr = new EditUserVM()
            {
                ID=user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                Email=user.Email,
                Address=user.Address

            };
            return View(usr);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditUser(EditUserVM usr,string id)
        {
            var files = HttpContext.Request.Form.Files;
            var user = await userManager.FindByIdAsync(id);
            //user.UserName = usr.Name;
            user.PhoneNumber = usr.PhoneNumber;
            user.Address = usr.Address;
            user.FullName = usr.FullName;
           
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
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<IActionResult> DeleteUser()
        {

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            EditUserVM usr = new EditUserVM()
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                Email = user.Email,
                ID=user.Id,
                Address=user.Address,
                
                

            };
            return View(usr);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(EditUserVM model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            await signInManager.SignOutAsync();
            await userManager.DeleteAsync(user);
            //var appointments = db.Appointment.Where(s => s.ApplicationUser == user);
            
            //db.Appointment.Remove(appointment);
            
            return RedirectToAction("Index", "home");
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            EditUserVM usr = new EditUserVM()
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                Email = user.Email,
                Address=user.Address
                
                
            };
            
            
            return View(usr);
        }
        [AllowAnonymous]
        public  IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordVM());
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                
                var user = await userManager.FindByEmailAsync(model.Email);
                
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                   
                        
                        Task<bool> email_result = PostMessageForgotPassword(user.Email, passwordResetLink, user.FullName);
                        await Task.WhenAll(email_result);
                        var saveResult = email_result.Result;
                        if (saveResult == false)
                        {
                            ViewBag.ErrorTitle = "Error";
                            ViewBag.Message = "Cannot send email";
                            return View("Error");

                        }
                        // Log the password reset link
                        //logger.Log(LogLevel.Warning, passwordResetLink);

                        // Send the user to Forgot Password Confirmation view
                        return View("ForgotPasswordConfirmation");
                }

                
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        public async Task<bool> PostMessageForgotPassword(string email, string message, string name)
        {
            var apiKey = configuration.GetSection("SENDGRID_API_KEY").Value;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("enagariksewa@gmail.com", "E-Nagarik team");
            var subject = "Email Verification";
            var to = new EmailAddress(email, name);
            var plainTextContent = "Please verify your email ";
            var htmlContent = "<strong>Dear " + name +",<br>Please change your password with this link: </strong><a href=\" " + message + "\"> Here</a><br>If you can't change please email us with your registered email address<br>Otherwise, visit the ward office with your National ID card";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(PasswordResetVM model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    // reset the user password
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    // Display validation errors. For example, password reset token already
                    // used to change the password or password complexity rules not met
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return View("ResetPasswordConfirmation");
            }
            // Display validation errors if model state is not valid
            return View(model);
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using smartpalika.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Controllers
{
    [Authorize]
    public class AdministrationController : Controller
    {
        private RoleManager<IdentityRole> roleManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleVM createRoleVM)
        {
            if (ModelState.IsValid)
            {
                IdentityRole irole = new IdentityRole()
                {
                    Name = createRoleVM.RoleName
                };
                var result=await roleManager.CreateAsync(irole);
                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(createRoleVM);
        }
        public IActionResult ListRole()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }
    }
}

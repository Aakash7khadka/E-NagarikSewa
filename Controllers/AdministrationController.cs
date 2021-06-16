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
        private UserManager<IdentityUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
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

        public async Task<IActionResult> EditRole(Guid id)
        {
            var role = await roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                string error= $"Cannot find the role with id:{id}";
                return RedirectToAction(nameof(NotFound),new { error=error });
            }
            EditRoleVM model = new EditRoleVM(){
                Id = id,
                RoleName=role.Name
            };

            foreach(var user in userManager.Users)
            {
                if(await userManager.IsInRoleAsync(user, role.Name)){
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
           
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleVM obj)
        {
            var role = await roleManager.FindByIdAsync(obj.Id.ToString());
            if (role == null)
            {
                string msg= $"Cannot find the role with id:{obj.Id}";
                return RedirectToAction(nameof(NotFound), new { error = msg });
            }
            else
            {
                role.Name = obj.RoleName;
                var result=await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRole");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(obj);
           
        }
        public IActionResult NotFound(string error)
        {
            TempData["ErrorMessage"] = error;
            return View();
        }
    }
}

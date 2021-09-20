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
    [Authorize(Roles ="Admin")]
    public class AdministrationController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
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
                    return RedirectToAction("ListRole");
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

        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var role = await roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                string error = $"Cannot find the role with id:{id}";
                return RedirectToAction(nameof(NotFound), new { error = error });
            }
            CreateRoleVM model = new CreateRoleVM()
            {
                Id = id,
                RoleName = role.Name
            };

            
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(CreateRoleVM obj)
        {
            var role = await roleManager.FindByIdAsync(obj.Id.ToString());
            if (role == null)
            {
                string error = $"Cannot find the role with id:{obj.Id}";
                return RedirectToAction(nameof(NotFound), new { error = error });
            }
            else
            {
                var result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRole");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(Guid Id)
        {
            ViewBag.roleid = Id;
            var role = await roleManager.FindByIdAsync(Id.ToString());
            if (role == null)
            {
                string error = $"Cannot find the role with id:{Id}";
                return RedirectToAction(nameof(NotFound), new { error = error });
            }
            List<RoleUsersVM> model = new List<RoleUsersVM>();
            foreach(var user in userManager.Users)
            {
                RoleUsersVM obj = new RoleUsersVM()
                {
                    UserId = Id,
                    UserName = user.UserName,
                    IsSelected=false
                };
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    obj.IsSelected = true;
                    
                }
                model.Add(obj);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<RoleUsersVM> model , Guid Id)
        {
            var role = await roleManager.FindByIdAsync(Id.ToString());
            if (role == null)
            {
                string error = $"Cannot find the role with id:{Id}";
                return RedirectToAction(nameof(NotFound), new { error = error });
            }
            for(int i = 0; i < model.Count; i++)
            {
                IdentityResult result = null;
                var userex = await userManager.FindByNameAsync(model[i].UserName);
                if (userex == null)
                {
                    string error = $"Cannot find the user with name:{model[i].UserName}";
                    return RedirectToAction(nameof(NotFound), new { error = error });
                }
                
                if(model[i].IsSelected && !(await userManager.IsInRoleAsync(userex, role.Name))){
                    try
                    {
                        if (role.Name == "Admin" || role.Name == "Employee")
                        {
                            await userManager.RemoveFromRoleAsync(userex, "citizen");
                        }
                    }
                    catch(Exception e)
                    {
                        ViewBag.ErrorTitle = "Error";
                        ViewBag.Message = "Error please contact admin";
                        return View("Error");
                    }
                    result = await userManager.AddToRoleAsync(userex, role.Name);
                }
                else if(!model[i].IsSelected && (await userManager.IsInRoleAsync(userex, role.Name))){
                    result = await userManager.RemoveFromRoleAsync(userex, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    //else
                    //    return RedirectToAction(nameof(EditRole), new { id = Id });

                }
            }
            return RedirectToAction(nameof(EditRole), new { id = Id });
        }
    }
}

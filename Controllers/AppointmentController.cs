using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using smartpalika.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AppointmentController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            TimeZoneInfo Nepal_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
            DateTime dateTime_Nepal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Nepal_Standard_Time);
            //DateTime date = DateTime.Today;
            IEnumerable<Attendance> data = db.Attendances.Where(s => s.Entry_time.Contains(dateTime_Nepal.ToString("yyyy/MM/dd")));
            var users_from_attendance = data.Select(s => s.UserEmail).Distinct().ToList();
            
            var roles = roleManager.Roles.ToList();
            List < String > role_list= new List<String>();
            foreach(var role in roles)
            {
                if (role.ToString() == "Admin" || role.ToString() == "Employee")
                {
                    continue;
                }
                
                    var users_list = await userManager.GetUsersInRoleAsync(role.ToString());
                    int present_count = 0;
                    foreach (var user in users_list)
                    {
                        if (users_from_attendance.Any(s => s == user.NormalizedEmail))
                        {
                            var count = data.Where(s => s.UserEmail == user.NormalizedEmail).Count();
                        if (count % 2 == 1)
                        {
                            present_count = 0;
                        }
                            present_count += count;
                        }
                    }
                    if (present_count % 2 == 1)
                    {
                        role_list.Add(role.ToString());
                    }
                
            }
            
            ViewBag.role_list = role_list.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x
                                  });
            return View();
        }
    }
}

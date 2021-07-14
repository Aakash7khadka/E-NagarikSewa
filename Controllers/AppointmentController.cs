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
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            TimeZoneInfo Nepal_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
            DateTime dateTime_Nepal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Nepal_Standard_Time);
            //DateTime date = DateTime.Today;
            IEnumerable<Attendance> data = db.Attendances.Where(s => s.Entry_time.Contains(dateTime_Nepal.ToString("yyyy/MM/dd")));
            var users_from_attendance = data.Select(s => s.UserEmail).Distinct().ToList();
            
            var roles = roleManager.Roles.ToList();
            List < String > role_list= new List<String>();
            List<AppointmentVM> app_list =new List<AppointmentVM>();
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
                            AppointmentVM app = new AppointmentVM()
                            {
                                OfficerName = user.FullName,
                                service = role.ToString()
                            };
                            app_list.Add(app);
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

            var current_user = await userManager.GetUserAsync(User);
            AppointmentUserDetails user_details = new AppointmentUserDetails()
            {
                Email = current_user.Email,
                Name= current_user.FullName,
                Address= current_user.Address,
                Phone= current_user.PhoneNumber
            };
            ViewBag.user_details = user_details;
            ViewBag.app_list = app_list;
            return View(user_details);
        }

        [HttpPost]
        public IActionResult Index(AppointmentUserDetails obj)
        {
            if (ModelState.IsValid)
            {
                TimeZoneInfo Nepal_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time"); 
                DateTime dateTime_Nepal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Nepal_Standard_Time);
                obj.Date = dateTime_Nepal.ToString("yyyy/MM/dd HH:mm:ss");
                obj.Provider_role = "Vital Registration";
                db.Appointment.Add(obj);
                db.SaveChanges();
            }
            return RedirectToAction("Index","Home");
        }

        public IActionResult Delete_Confirm(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public IActionResult Delete(int id)
        {
            var appointment = db.Appointment.FirstOrDefault(s => s.ID== id);
            if (appointment == null)
            {
                string error = $"Cannot find the appointment with id:{TempData["id"]}";
                return RedirectToAction(nameof(NotFound), new { error = error });
            }
            db.Remove(appointment);
            db.SaveChanges();
            TempData["message"] = "Sucessfully Cancelled the appointment";
            return RedirectToAction("Index","Home");
        }
    }

    



}

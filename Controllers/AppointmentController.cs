using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using smartpalika.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace smartpalika.Controllers
{
    [Authorize]
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
        //[Authorize(Roles ="citizen")]
        [HttpGet]
        
        public async Task<IActionResult> Index()
        {
            TimeZoneInfo Nepal_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
            DateTime dateTime_Nepal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Nepal_Standard_Time);
            //DateTime date = DateTime.Today;
            string date_ymd = dateTime_Nepal.ToString("yyyy/MM/dd");

            var current_user = await userManager.GetUserAsync(User);
            var appointment_state = db.Appointment.Include(u => u.ApplicationUser).Where(s => s.ApplicationUser.Email == current_user.Email && s.Date==date_ymd).Count();
            //var appointment_state = db.Appointment.Where(s => s.Date.Contains(date_ymd) && s.Email == current_user.Email).Count();
            
            if (appointment_state > 0)
            {
                string error = $"You already have your appointments for today.You cannot make more than 1 appointment per day";
                TempData["ErrorMessage"] = error;
                return View("Denied");
            }


            IEnumerable<Attendance> data = db.Attendances.Where(s => s.Entry_time.Contains(dateTime_Nepal.ToString("yyyy/MM/dd")));
            var employees_identified_camera = data.Select(s => s.UserEmail).Distinct().ToList();
            
            var roles = roleManager.Roles.ToList();
            List < String > role_list= new List<String>();
            List<AppointmentVM> app_list =new List<AppointmentVM>();

            
            //foreach(var role in roles)
            
            //{
            //    //if (role.ToString() == "Admin" || role.ToString() == "Employee")
            //    //{
            //    //    continue;
            //    //}

            //        var users_list = await userManager.GetUsersInRoleAsync(role.ToString());
            //        int present_count = 0;
            //        foreach (var user in users_list)
            //        {
            //            if (employees_identified_camera.Any(s => s == user.NormalizedEmail))
            //            {
            //                var count = data.Where(s => s.UserEmail == user.NormalizedEmail).Count();
            //            if (count % 2 == 1)
            //            {
            //                AppointmentVM app = new AppointmentVM()
            //                {
            //                    OfficerName = user.FullName,
            //                    service = role.ToString()
            //                };
            //                app_list.Add(app);
            //                present_count = 0;
            //            }
            //                present_count += count;
            //            }
            //        }
            //        if (present_count % 2 == 1)
            //        {
            //            role_list.Add(role.ToString());
            //        }

            //}

            //get list of all the present employees
            var employee_list = await userManager.GetUsersInRoleAsync("Employee");
            int present_count = 0;
            
            foreach (var employee in employee_list)
            {
               
                if (employees_identified_camera.Any(s => s == employee.NormalizedEmail))
                {
                    var count = data.Where(s => s.UserEmail == employee.NormalizedEmail).Count();
                    if (count % 2 == 1)
                    {//if the service provider is present
                        AppointmentVM app = new AppointmentVM();
                        app.OfficerName = employee.FullName;
                        app.service = "Employee".ToString();
                        var appointment_list=db.Appointment.Where(s => s.Provider == employee.FullName && s.Date==date_ymd).ToList();
                        List<string> times =new List<string>(){ "11:00 AM", "12:00 PM", "1:00 PM", "2:00 PM", "3:00 PM" ,"4:00 PM"};
                        if (appointment_list.Count() == 0)
                        {
                            app.Available_Time = times;
                        }
                        else
                        {
                            var temp_list=appointment_list.Select(s => s.Time).ToList();
                            app.Available_Time = times.Except(temp_list).ToList();
                        }
                       
                       
                        app_list.Add(app);
                        present_count = 0;
                    }
                    present_count += count;
                }
            }
            if (present_count % 2 == 1)
            {
                role_list.Add("Employee".ToString());
            }




            ViewBag.role_list = role_list.Select(x =>
                                  new SelectListItem()
                                  {
                                      Text = x
                                  });

           //for user details in appointment list
           
            
            ViewBag.app_list = app_list;
            
            return View();
        }

        [HttpPost]
        public  IActionResult Index(AppointmentUserDetails obj)
        {
            TimeZoneInfo Nepal_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
            DateTime dateTime_Nepal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Nepal_Standard_Time);
            var date_ymd= dateTime_Nepal.ToString("yyyy/MM/dd HH:mm:ss");
            obj.Date = date_ymd;

            
            if (ModelState.IsValid)
            {
                
                obj.ServiceType = "Vital Registration";
                db.Appointment.Add(obj);
                db.SaveChanges();
            }
            return RedirectToAction("Index","Home");
        }

        public IActionResult Delete_Confirm(Guid id)
        {
            ViewBag.id = id;
            return View();
        }
        public IActionResult Delete(Guid id)
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
            return RedirectToAction("Appointment","Home");
        }

        public async Task<IActionResult> SetAppointment(string time,string officer)
        {
            TimeZoneInfo Nepal_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
            DateTime dateTime_Nepal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Nepal_Standard_Time);

            var current_user = await userManager.GetUserAsync(User);
            List<string> times = new List<string>() { "11:00 AM", "12:00 PM", "1:00 PM", "2:00 PM", "3:00 PM", "4:00 PM" };
            AppointmentUserDetails obj = new AppointmentUserDetails()
            {
                //Email = current_user.Email,
                //Name = current_user.FullName,
                //Address = current_user.Address,
                //Phone = current_user.PhoneNumber
            };
            
            obj.Date = dateTime_Nepal.ToString("yyyy/MM/dd");
            obj.priority = (times.IndexOf(time))+"";
            obj.Time = time;
            obj.Provider = officer;
           
            return View(obj);
        }
        [HttpPost]
        public async Task<IActionResult> SetAppointmentAsync(AppointmentUserDetails obj)
        {
            var current_user = await userManager.GetUserAsync(User);
            obj.ApplicationUser = current_user;
            if (ModelState.IsValid)
            {
                TimeZoneInfo Nepal_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
                DateTime dateTime_Nepal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Nepal_Standard_Time);
                string dt = dateTime_Nepal.ToString("yyyy/MM/dd");
                var result=db.Appointment.Where(s => s.Date == dt && s.Provider == obj.Provider && s.Time==obj.Time);
                if(result.Count()!=0)
                {
                    TempData["message"] = "Oops! Somebody stole your time please try another time slot. So your appointment was cancelled ";
                    return RedirectToAction("Index", "Home");
                }
                db.Appointment.Add(obj);
                db.SaveChanges();
                TempData["message"] = "Sucessfully created appointment";
            }
            return RedirectToAction("Appointment", "Home");
        }
    }

    



}

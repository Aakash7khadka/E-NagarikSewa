using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using smartpalika.Models;

namespace smartpalika.Controllers
{
    //[Authorize(Roles ="Admin")]
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> roleManager;

        public AttendanceController(ApplicationDbContext db,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            _db = db;
            this.roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            TimeZoneInfo Nepal_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
            DateTime dateTime_Nepal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Nepal_Standard_Time);
            //DateTime date = DateTime.Today;
            string ymd = dateTime_Nepal.ToString("yyyy/MM/dd");
            IEnumerable<Attendance> data = _db.Attendances.Where(s => s.Entry_time.Contains(ymd));
            var users = data.Select(s => s.UserEmail).Distinct().ToList();
            var users_list =await userManager.GetUsersInRoleAsync("Employee");
            List<UserAttendanceVM> objs = new List<UserAttendanceVM>();
            foreach (var user in users_list)
            {
                
                    UserAttendanceVM usr = new UserAttendanceVM()
                    {
                        Username = user.FullName,
                        
                    };
                    if (users.Any(s => s == user.NormalizedEmail))
                    {
                       
                        var count = data.Where(s => s.UserEmail == user.NormalizedEmail).Count();
                        string entrydate= data.Last(s => s.UserEmail == user.NormalizedEmail).Entry_time;

                        if (entrydate != null)
                        {
                            var _date = entrydate.Substring(0, entrydate.IndexOf(" "));
                            usr.date = _date;
                            var _time = entrydate.Substring(entrydate.IndexOf(" "));
                            
                            usr.time = _time;
                        
                        }
                    if (count % 2 == 1)
                        {
                            usr.isPresent = true;
                            
                        }
                        else if(count!=0)
                        {
                            usr.isPresent = false;
                            
                        }
                        else
                        {
                            usr.isPresent = false;
                        
                        }

                        

                    }

                    objs.Add(usr);
              
                
                

            }
            return View(objs);
        }
    }
}

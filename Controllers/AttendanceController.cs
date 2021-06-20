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
    [Authorize(Roles ="Admin")]
    public class AttendanceController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _db;
        public AttendanceController(ApplicationDbContext db,UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            DateTime date = DateTime.Today;
            IEnumerable<Attendance> data = _db.Attendances.Where(s => s.Entry_time.Contains(date.ToString("yyyy/MM/dd")));
            var users = data.Select(s => s.UserEmail).Distinct().ToList();
            
            List<UserAttendanceVM> objs = new List<UserAttendanceVM>();
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, "Admin"))
                {
                    UserAttendanceVM usr = new UserAttendanceVM()
                    {
                        Username = user.FullName,
                        
                    };
                    if (users.Any(s => s == user.NormalizedEmail))
                    {
                       
                        var count = data.Where(s => s.UserEmail == user.NormalizedEmail).Count();
                        if (count % 2 == 1)
                        {
                            usr.isPresent = true;
                            usr.entryDate = data.Last(s => s.UserEmail == user.NormalizedEmail).Entry_time;
                        }
                        else if(count!=0)
                        {
                            usr.isPresent = false;
                            usr.entryDate = data.Last(s => s.UserEmail == user.NormalizedEmail).Entry_time;
                        }
                        else
                        {
                            usr.isPresent = false;
                            usr.entryDate = "Not Available";
                        }

                        

                    }

                    objs.Add(usr);
                }
                
                

            }
            return View(objs);
        }
    }
}

using Microsoft.AspNetCore.Identity;
using smartpalika.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace smartpalika.Services
{

   
    public class NotificationService

    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
   

        public NotificationService(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
         
        }
        public  async Task<int> GetAppointmentCount()
        {
            var httpContext = new HttpContextAccessor().HttpContext;
            TimeZoneInfo Nepal_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
            DateTime dateTime_Nepal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Nepal_Standard_Time);
            //DateTime date = DateTime.Today;
            var dateTime_ = dateTime_Nepal.ToString("yyyy/MM/dd");
            var current_user = await _userManager.GetUserAsync(httpContext.User);
            var User = httpContext.User;
            IEnumerable<AppointmentUserDetails> appointments = _db.Appointment.Include(u => u.ApplicationUser).Where(s => s.Date.Contains(dateTime_));
            IEnumerable<AppointmentUserDetails> data = null;

            var count = 0;
            if (User.IsInRole("Admin"))
            {
                data = appointments;
                count = data.Count();
            }
            else if (User.IsInRole("Employee"))
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var name = user.FullName;

                data = appointments.Where(s => s.Provider == name);

                count = data.Count();
            }
            return count;
        }
    }
}

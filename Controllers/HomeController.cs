using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using smartpalika.Models;

namespace smartpalika.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
          
        }

        public async Task<IActionResult> Index()
        {
            TimeZoneInfo Nepal_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
            DateTime dateTime_Nepal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Nepal_Standard_Time);
            //DateTime date = DateTime.Today;
            var dateTime_ = dateTime_Nepal.ToString("yyyy/MM/dd");
            var current_user = await _userManager.GetUserAsync(User);
            IEnumerable<AppointmentUserDetails> data=null;
            if (User.IsInRole("Admin"))
            {
                data = _db.Appointment.Where(s => s.Date.Contains(dateTime_));
            }
            else if (User.IsInRole("Employee"))
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var name = user.FullName;
                data = _db.Appointment.Where(s => s.Provider == name).Where(s =>s.Date.Contains(dateTime_)) ;
            }
            else if(User.Identity.IsAuthenticated)
            {
               data = _db.Appointment.Where(s => s.Date.Contains(dateTime_) && s.Email==current_user.Email);
            }
            return View(data);
        }
        public IActionResult Detail(Guid id)
        {
            var obj = _db.Appointment.Find(id);
            if(obj==null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost]
        public IActionResult Detail(AppointmentUserDetails obj)
        {
            if(ModelState.IsValid)
            {
                if (obj == null)
                    return NotFound();
                _db.Appointment.Update(obj);
                _db.SaveChanges();
                TempData["message"] = "Sucessfully updated your availability";
                return RedirectToAction("Index", "Home");
            }
            return View(obj);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

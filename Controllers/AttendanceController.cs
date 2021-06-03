using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using smartpalika.Models;

namespace smartpalika.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AttendanceController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            DateTime date = DateTime.Today;
            IEnumerable<Attendance> data = _db.Attendances.Where(s => s.Entry_time.Contains(date.ToString("yyyy/MM/dd")));
            return View(data);
        }
    }
}

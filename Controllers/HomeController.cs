﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using smartpalika.Models;
using System.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Nancy.Json;

namespace smartpalika.Controllers
{
    [Authorize]
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
            try
            {
                var current_user = await _userManager.GetUserAsync(User);
                var appointment_day = _db.Appointment.Where(s=>s.isCompleted==true).GroupBy(s => s.Date).Select(x=> new{Date= x.Key, AppointmentCount=x.Count() }).OrderBy(s=>s.Date).ToList();
                int today_total_appointments=0;
                int today_total_pending = 0;
                int today_total_available_citizens = 0;
                TimeZoneInfo Nepal_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
                DateTime dateTime_Nepal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Nepal_Standard_Time);
                //DateTime date = DateTime.Today;
                var dateTime_ = dateTime_Nepal.ToString("yyyy/MM/dd");

                if (User.IsInRole("Admin"))
                {
                    today_total_appointments = _db.Appointment.Where(s=> s.Date == dateTime_).Count();
                    today_total_pending = _db.Appointment.Where(s=> s.Date == dateTime_ && s.isCompleted == false).Count();
                    today_total_available_citizens = _db.Appointment.Where(s => s.Date == dateTime_ && s.isCompleted == false && s.isAvailable == true).Count();
                }
                else if(User.IsInRole("Employee"))
                {
                    today_total_appointments = _db.Appointment.Where(s => s.Provider == current_user.FullName && s.Date == dateTime_).Count();
                    today_total_pending = _db.Appointment.Where(s => s.Provider == current_user.FullName && s.Date == dateTime_ && s.isCompleted == false).Count();
                    today_total_available_citizens = _db.Appointment.Where(s => s.Provider == current_user.FullName && s.Date == dateTime_ && s.isCompleted == false && s.isAvailable == true).Count();
                }
                ViewBag.today_total_appointments = today_total_appointments;
                ViewBag.today_total_pending = today_total_pending;
                ViewBag.today_total_available_citizens = today_total_available_citizens;
                //var appointment_day_json=  JsonConvert.SerializeObject(appointment_day);
                var appointment_day_json = new JavaScriptSerializer().Serialize(appointment_day);
                ViewBag.appointment_day_json = appointment_day_json;
            }
            catch (Exception e)
            {

                throw;
            }
           
            return View();
        }
            public async Task<IActionResult> Appointment()
        {
            TimeZoneInfo Nepal_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time");
            DateTime dateTime_Nepal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Nepal_Standard_Time);
            //DateTime date = DateTime.Today;
            var dateTime_ = dateTime_Nepal.ToString("yyyy/MM/dd");
            var current_user = await _userManager.GetUserAsync(User);
            IEnumerable<AppointmentUserDetails> appointments=_db.Appointment.Include(u=>u.ApplicationUser).Where(s => s.Date.Contains(dateTime_));
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

                data = appointments.Where(s => s.Provider == name) ;

                count = data.Count();
            }
            else if (User.IsInRole("citizen"))
            {
                //data = _db.Appointment.Where(s => s.Date.Contains(dateTime_) && s.Email==current_user.Email);
                data = appointments.Where(s => s.ApplicationUser.Email == current_user.Email);
            }

            TempData["count"] = count.ToString();
           return View(data);

        }
        public IActionResult Detail(Guid id)
        {
            var obj = _db.Appointment.Include(u=>u.ApplicationUser).FirstOrDefault(u=>u.ID==id);
            //var obj1=obj.Include(u => u.ApplicationUser);
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

        //public async Task<IActionResult> NotifyCitizen(string email,string name,string time)
        //{
        //    Task<bool> email_result = PostMessage(email,name,time);
        //    await Task.WhenAll(email_result);
        //    var saveResult = email_result.Result;
        //    if (saveResult == false)
        //    {
        //        ViewBag.ErrorTitle = "Error";
        //        ViewBag.Message = "Cannot send email";
        //        return View("Error");

        //    }
        //    TempData["message"] = "Sucessfully invitation sent";
        //    return RedirectToAction("Index");
        //}
        public async Task<IActionResult> SendMessageToCitizen(string email, string name, string time)
        {
            DetailAppointmentVM obj = new DetailAppointmentVM()
            {
                name = name,
                email=email,
                time=time,
                

        };
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var input = user.FullName;
            obj.sender = input;
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageToCitizen(DetailAppointmentVM obj)
        {
            Task<bool> email_result = PostMessage(obj.email, obj.name, obj.time,obj.sender,obj.message);
            await Task.WhenAll(email_result);
            var saveResult = email_result.Result;
            if (saveResult == false)
            {
                ViewBag.ErrorTitle = "Error";
                ViewBag.Message = "Cannot send email";
                return View("Error");

            }
            TempData["message"] = "Sucessfully invitation sent";
            return RedirectToAction("Index");
            
        }
        public async Task<bool> PostMessage(string email, string name, string time,string sender,string message)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("enagariksewa@gmail.com", "E-Nagarik team");
            var subject = "Ward office visit confirmation";
            var to = new EmailAddress(email, name);
            var plainTextContent = "Please visit ward office on time ";
            var htmlContent = "Dear "+name+",<br><strong>"+message+"<br>At:"+time+ " </strong><br>Sender:"+sender ;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace SchoolJournal.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly SchoolJournalContext _db;

        public ScheduleController(SchoolJournalContext db) 
        {
            _db = db;
        }

        public IActionResult StatusRedirect() 
        {
            string status = HttpContext.Session.GetString("Status");
            if (status == "Student")
            {
                return RedirectToAction("StudentSchedule");
            }
            else if (status == "Teacher")
            {
                return View(); //Change later
            }
            else if (status == "Admin")
            {
                return View(); //Change later
            }
            else 
            {
                return RedirectToAction("Authorization", "Authorization");
            }
        }
        public IActionResult StudentSchedule()
        {
            return View();
        }
    }
}

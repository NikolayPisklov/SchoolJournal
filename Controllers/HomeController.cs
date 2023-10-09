using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.Models;

namespace SchoolJournal.Controllers
{
    public class HomeController : Controller
    {
        private readonly SchoolJournalContext _db;
        private enum Status
        {
            Teacher,
            Student
        };

        public HomeController(SchoolJournalContext db)
        {
            _db = db;
        }

        public IActionResult StudentHome() 
        {
            return View("Home");
        }
        public IActionResult TeacherHome()
        {
            return View("Home");
        }
        public IActionResult AdminHome()
        {
            return View("Home");
        }
    }
}

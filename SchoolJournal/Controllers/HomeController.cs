using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.Models;
using SchoolJournal.ViewModels;
using System.Diagnostics;

namespace SchoolJournal.Controllers
{
    public class HomeController : Controller
    {
        private readonly SchoolJournalContext _db;

        public HomeController(SchoolJournalContext db)
        {
            _db = db;
        }

        public IActionResult Home(User user) 
        {
            return View();
        }
    }
}
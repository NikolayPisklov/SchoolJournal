using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.Models;
using SchoolJournal.ViewModels;
using System.Linq;
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
        [HttpGet]
        public IActionResult Home(User user) 
        {
            if (HttpContext.Session.GetString("Status") == "Student")
            {
                List<JournalListContent> journals = GetJournalListContentForStudent(user.FkClass);
                return View(journals);
            }
            else if (HttpContext.Session.GetString("Status") == "Teacher")
            {
                List<JournalListContent> journals = GetJournalListContentForTeacher(user.Id);
                return View(journals);
            }
            else if (HttpContext.Session.GetString("Status") == "Admin")
            {
                List<JournalListContent> journals = GetJournalListContentForAdmin();
                return View(journals);
            }
            else 
            {
                ViewBag.Message = "Час вашої сесії вийшов! Просимо авторизуватись знову!";
                return RedirectToAction("Authorization", "Authorization");
            }
        }

        private List<JournalListContent> GetJournalListContentForStudent(int classId) 
        {
            return (from j in _db.Journals
                   join c in _db.Classes on j.FkClass equals c.Id
                   join s in _db.Subjects on j.FkSubject equals s.Id
                   join t in _db.Teachers on j.FkTeacher equals t.Id
                   where j.FkClass == classId && j.FkSchoolYear == SchoolDateTime.GetCurrentYearId(_db)
                   select new JournalListContent
                   { Subject = s, Class = c, Teacher = t }).ToList();
        }
        private List<JournalListContent> GetJournalListContentForTeacher(int teacherId) 
        {
            return (from j in _db.Journals
                    join c in _db.Classes on j.FkClass equals c.Id
                    join s in _db.Subjects on j.FkSubject equals s.Id
                    join t in _db.Teachers on j.FkTeacher equals t.Id
                    where j.FkTeacher == teacherId && j.FkSchoolYear == SchoolDateTime.GetCurrentYearId(_db)
                    select new JournalListContent
                    { Subject = s, Class = c, Teacher = t }).ToList();
        }
        private List<JournalListContent> GetJournalListContentForAdmin() 
        {
            return (from j in _db.Journals
                    join c in _db.Classes on j.FkClass equals c.Id
                    join s in _db.Subjects on j.FkSubject equals s.Id
                    join t in _db.Teachers on j.FkTeacher equals t.Id
                    where j.FkSchoolYear == SchoolDateTime.GetCurrentYearId(_db)
                    orderby c.Title.Length, c.Title
                    select new JournalListContent
                    { Subject = s, Class = c, Teacher = t }).ToList();
        }
    }
}
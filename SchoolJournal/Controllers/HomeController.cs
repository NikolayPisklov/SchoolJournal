using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.Models;
using SchoolJournal.ViewModels;
using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

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
                SetFiltersViewBags();
                return View(journals);
            }
            else
            {
                ViewBag.Message = "Час вашої сесії вийшов! Просимо авторизуватись знову!";
                return RedirectToAction("Authorization", "Authorization");
            }
        }
        [HttpPost]
        public IActionResult Home(int? subjectId, int? classRangId)
        {
            List<JournalListContent> journals = GetJournalListContentForAdmin();
            if (subjectId != null && classRangId == null)
            {
                List<JournalListContent> filteredJournals = GetJournalListFilteredBySubject((int)subjectId, journals);
                SetFiltersViewBags();
                return View(filteredJournals);
            }
            else if (subjectId == null && classRangId != null)
            {
                List<JournalListContent> filteredJournals = GetJournalListFilteredByClassRank((int)classRangId, journals);
                SetFiltersViewBags();
                return View(filteredJournals);
            }
            else if (subjectId != null && classRangId != null)
            {
                List<JournalListContent> filteredJournals = GetJournalListFilteredBySubject((int)subjectId, journals);
                filteredJournals = GetJournalListFilteredByClassRank((int)classRangId, filteredJournals);
                SetFiltersViewBags();
                return View(filteredJournals);
            }
            else
            {
                SetFiltersViewBags();
                return View(journals);
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
                   { Journal = j, Subject = s, Class = c, Teacher = t }).ToList();
        }
        private List<JournalListContent> GetJournalListContentForTeacher(int teacherId) 
        {
            return (from j in _db.Journals
                    join c in _db.Classes on j.FkClass equals c.Id
                    join s in _db.Subjects on j.FkSubject equals s.Id
                    join t in _db.Teachers on j.FkTeacher equals t.Id
                    where j.FkTeacher == teacherId && j.FkSchoolYear == SchoolDateTime.GetCurrentYearId(_db)
                    select new JournalListContent
                    { Journal = j, Subject = s, Class = c, Teacher = t }).ToList();
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
                    { Journal = j, Subject = s, Class = c, Teacher = t }).ToList();
        }
        private List<JournalListContent> GetJournalListFilteredBySubject(int subjectId, List<JournalListContent> journals) 
        {
            return journals.Where(j => j.Subject.Id == subjectId).ToList();
        }
        private List<JournalListContent> GetJournalListFilteredByClassRank(int classRankId, List<JournalListContent> journals)
        {
            if (classRankId == 1)
            {
                return journals.Where(j => j.Class.Title.Length == 3 
                    && (DateTime.Now.Year - j.Class.RecruitmentDate.Year) <= 3).ToList();
            }
            else if (classRankId == 2)
            {
                return journals.Where(j => j.Class.Title.Length == 3
                    && (DateTime.Now.Year - j.Class.RecruitmentDate.Year) >= 4).ToList();
            }
            else
            {
                return journals.Where(j => j.Class.Title.Length == 4).ToList();
            }

        }
        private void SetFiltersViewBags() 
        {
            HomePageFilters filters = new HomePageFilters(_db.Subjects.ToList());
            ViewBag.SubjectsSelectList = filters.GetSubjectsSelectList();
            ViewBag.ClassRanksSelectList = filters.GetClassRangsSelecteList();
        }
    }
}
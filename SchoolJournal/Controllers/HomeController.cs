using Microsoft.AspNetCore.Mvc;
using SchoolJournal.Classes;
using SchoolJournal.ViewModels;

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
        public IActionResult StudentHome(int fkClass)
        {
            List<Journal> journals = _db.Journals.Where(j => j.FkClass == fkClass &&
                j.FkSchoolYear == SchoolDateTime.GetCurrentYearId(_db)).ToList();
            return View("Home", journals);
        }
        [HttpGet]
        public IActionResult TeacherHome(int teacherId)
        {
            List<Journal> journals = _db.Journals.Where(j => j.FkTeacherSubjectNavigation.FkTeacher == teacherId &&
                j.FkSchoolYear == SchoolDateTime.GetCurrentYearId(_db)).ToList();
            return View("Home", journals);
        }
        [HttpGet]
        public IActionResult Home()
        {
            List<Journal> journals = _db.Journals.Where(j => j.FkSchoolYear ==
                SchoolDateTime.GetCurrentYearId(_db)).ToList();
            SetFiltersViewBags();
            return View(journals);
        }
        [HttpPost]
        public IActionResult Home(int? subjectId, int? classRangId)
        {
            List<Journal> journals = _db.Journals.Where(j => j.FkSchoolYear ==
                SchoolDateTime.GetCurrentYearId(_db)).ToList();
            JournalsFilter filter = new JournalsFilter(journals);
            SetFiltersViewBags();
            return View(filter.FilterJournals(subjectId, classRangId));
        }

        private void SetFiltersViewBags()
        {
            SubjectSelectList subjectSelectList =
                new SubjectSelectList(_db.Subjects.OrderBy(s => s.Title));
            ClassRankSelectList classRankSelectList =
                new ClassRankSelectList(_db.ClassRanks.OrderBy(c => c.Title));
            ViewBag.SubjectsSelectList = subjectSelectList.GetSelectList();
            ViewBag.ClassRanksSelectList = classRankSelectList.GetSelectList();
        }
    }
}
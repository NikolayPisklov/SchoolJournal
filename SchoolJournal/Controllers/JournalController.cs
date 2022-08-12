using Microsoft.AspNetCore.Mvc;
using SchoolJournal.ViewModels;
using SchoolJournal.Models;
using Microsoft.EntityFrameworkCore;

namespace SchoolJournal.Controllers
{
    public class JournalController : Controller
    {
        private readonly SchoolJournalContext _db;

        private int _currentPage = 1;

        public JournalController(SchoolJournalContext db)
        {
            _db = db;
        }

        public IActionResult Journal(int? pageNumber)
        {
            Journal journal = SessionJson.GetObjectFromJson<Journal>(HttpContext.Session, "Journal");
            List<Lesson> lessons = _db.Lessons.Where(l => l.FkJournal == journal.Id).ToList();
            SetViewBagForJournal(lessons, journal);
            return View(Paging<Lesson>.Create(lessons, pageNumber ?? _currentPage, 15));
        }
        [HttpGet]
        public IActionResult EditLesson(int lessonId)
        {
            Lesson lesson = _db.Lessons.Where(l => l.Id == lessonId).First();
            return View(lesson);
        }
        [HttpPost]
        public IActionResult EditLesson(Lesson lesson) 
        {
            _db.Update(lesson);
            _db.SaveChanges();
            return RedirectToAction("Journal");
        }

        private List<ProgressContent> GetProgressContent(int journalId) 
        {
            return (from l in _db.Lessons
                    join p in _db.Progresses on l.Id equals p.FkLesson
                    join m in _db.Marks on p.FkMark equals m.Id
                    where l.FkJournal == journalId
                    select new ProgressContent
                    { Progress = p, Mark = m }).ToList();
        }
        private void SetViewBagForJournal(List<Lesson> lessons, Journal journal) 
        {
            ViewBag.ClassTitle = _db.Classes.Where(c => c.Id == journal.FkClass).Select(c => c.Title).First();
            ViewBag.SubjectTitle = _db.Subjects.Where(s => s.Id == journal.FkSubject).Select(s => s.Title).First();
            ViewBag.Lessons = lessons;
            ViewBag.Progresses = GetProgressContent(journal.Id);
            ViewBag.Students = _db.Students.Where(s => s.FkClass == journal.FkClass).ToList();
            ViewBag.Journal = journal;
        }
    }
}

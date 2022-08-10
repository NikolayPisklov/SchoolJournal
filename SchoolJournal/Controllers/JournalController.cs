using Microsoft.AspNetCore.Mvc;
using SchoolJournal.ViewModels;

namespace SchoolJournal.Controllers
{
    public class JournalController : Controller
    {
        private readonly SchoolJournalContext _db;
        private Journal _journal;
        private int _currentPage = 1;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public JournalController(SchoolJournalContext db)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _db = db;            
        }

        public IActionResult Journal(int journalId, int? pageNumber)
        {
            _journal = _db.Journals.Where(j => j.Id == journalId).First();            
            List<Lesson> lessons = _db.Lessons.Where(l => l.FkJournal == _journal.Id).ToList();
            ViewBag.ClassTitle = _db.Classes.Where(c => c.Id == _journal.FkClass).Select(c => c.Title).First();
            ViewBag.SubjectTitle = _db.Subjects.Where(s => s.Id == _journal.FkSubject).Select(s => s.Title).First();
            ViewBag.Lessons = lessons;
            ViewBag.Progresses = GetProgressContent();
            ViewBag.Students = _db.Students.Where(s => s.FkClass == _journal.FkClass).ToList();
            ViewBag.Journal = _journal;
            return View(Paging<Lesson>.Create(lessons, pageNumber ?? _currentPage, 15));
        }

        private List<ProgressContent> GetProgressContent() 
        {
            return (from l in _db.Lessons
                    join p in _db.Progresses on l.Id equals p.FkLesson
                    join m in _db.Marks on p.FkMark equals m.Id
                    where l.FkJournal == _journal.Id
                    select new ProgressContent
                    { Progress = p, Mark = m }).ToList();
        }
    }
}

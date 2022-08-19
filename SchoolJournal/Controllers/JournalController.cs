using Microsoft.AspNetCore.Mvc;
using SchoolJournal.ViewModels;
using SchoolJournal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace SchoolJournal.Controllers
{
    public class JournalController : Controller
    {
        private readonly SchoolJournalContext _db;

        public JournalController(SchoolJournalContext db)
        {
            _db = db;
        }

        public IActionResult Journal(int? pageNumber, int journalId)
        {
            Journal journal = _db.Journals.Where(j => j.Id == journalId).First();
            List<Lesson> lessons = _db.Lessons.Where(l => l.FkJournal == journalId).ToList();
            SetViewBagForJournal(lessons, journal);
            HttpContext.Session.SetInt32("journalPage", pageNumber ?? GetCurrentPage(lessons));
            return View(Paging<Lesson>.Create(lessons, pageNumber ?? GetCurrentPage(lessons), 15));
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
            return RedirectToRoute(new
            {
                action = "Journal",
                pageNumber = HttpContext.Session.GetInt32("journalPage"),
                journalId = lesson.FkJournal
            });
        }
        [HttpGet]
        public IActionResult AddProgress(int fkStudent, int fkLesson) 
        {
            Progress progress = new Progress()
            {
                FkStudent = fkStudent,
                FkLesson = fkLesson
            };
            ViewBag.Student = _db.Students.Where(s => s.Id == fkStudent).First();
            ViewBag.LessonDate = _db.Lessons.Where(l => l.Id == fkLesson).Select(l => l.Date).First();
            ViewBag.MarksSelectedList = GetMarksSelectList();
            return View(progress);
        }
        [HttpPost]
        public IActionResult AddProgress(Progress progress) 
        {
            if (ModelState.IsValid)
            {
                _db.Add(progress);
                _db.SaveChanges();
                var fkJournal = _db.Lessons.Where(l => l.Id == progress.FkLesson).Select(l => l.FkJournal).First();
                return RedirectToRoute(new
                {
                    action = "Journal",
                    pageNumber = HttpContext.Session.GetInt32("journalPage"),
                    journalId = fkJournal
                });
            }
            else
            {
                ViewBag.Student = _db.Students.Where(s => s.Id == progress.FkStudent).First();
                ViewBag.LessonDate = _db.Lessons.Where(l => l.Id == progress.FkLesson).Select(l => l.Date).First();
                ViewBag.MarksSelectedList = GetMarksSelectList();
                return View();
            }
        }
        [HttpGet]
        public IActionResult EditProgress(int progressId)
        {
            Progress progress = _db.Progresses.Where(p => p.Id == progressId).First();
            ViewBag.Student = _db.Students.Where(s => s.Id == progress.FkStudent).First();
            ViewBag.LessonDate = _db.Lessons.Where(l => l.Id == progress.FkLesson).Select(l => l.Date).First();
            ViewBag.MarksSelectedList = GetMarksSelectList();
            return View(progress);
        }
        [HttpPost]
        public IActionResult EditProgress(Progress progress)
        {
            if (ModelState.IsValid)
            {
                _db.Update(progress);
                _db.SaveChanges();
                var fkJournal = _db.Lessons.Where(l => l.Id == progress.FkLesson).Select(l => l.FkJournal).First();
                return RedirectToRoute(new
                {
                    action = "Journal",
                    pageNumber = HttpContext.Session.GetInt32("journalPage"),
                    journalId = fkJournal
                });
            }
            else
            {
                ViewBag.Student = _db.Students.Where(s => s.Id == progress.FkStudent).First();
                ViewBag.LessonDate = _db.Lessons.Where(l => l.Id == progress.FkLesson).Select(l => l.Date).First();
                ViewBag.MarksSelectedList = GetMarksSelectList();
                return View();
            }
        }
        [HttpPost]
        public IActionResult DeleteProgress(Progress progress) 
        {
            var fkJournal = _db.Lessons.Where(l => l.Id == progress.FkLesson).Select(l => l.FkJournal).First();
            _db.Remove(progress);
            _db.SaveChanges();
            return RedirectToRoute(new { action = "Journal", pageNumber = HttpContext.Session.GetInt32("journalPage"), journalId = fkJournal });
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
        private int GetCurrentPage(List<Lesson> lessons)
        {
            Lesson currentDay = lessons.Where(l => l.Date.Date == DateTime.Now.Date).FirstOrDefault();
            double dayIndex = lessons.IndexOf(currentDay) + 1;
            double result = dayIndex / 15;
            if (result <= 1)
            {
                return 1;
            }
            else
            {
                return (int)Math.Truncate(result) + 1;
            }
        }
        private SelectList GetMarksSelectList() 
        {
            Dictionary<int, string> marksDict = new Dictionary<int, string>();
            List<Mark> marks = _db.Marks.ToList();
            foreach (Mark m in marks) 
            {
                marksDict.Add(m.Id, m.Title);
            }
            return new SelectList(marksDict, "Key", "Value");
        }
    }
}

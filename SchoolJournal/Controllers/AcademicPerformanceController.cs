using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.ViewModels;

namespace SchoolJournal.Controllers
{
    public class AcademicPerformanceController : Controller
    {
        private readonly SchoolJournalContext _db;

        public AcademicPerformanceController(SchoolJournalContext db) 
        {
            _db = db;
        }

        public IActionResult SubjectsList(int fkClass)
        {
            List<Journal> journals = _db.Journals.Where(j => j.FkClass == fkClass).ToList();
            return View(journals);
        }
        public IActionResult AcademicPerformance(int fkJournal, int fkStudent) 
        {
            List<Progress> progresses = _db.Students.Find(fkStudent).Progresses.
                Where(p => p.FkLessonNavigation.FkJournal == fkJournal).ToList();
            Journal journal = _db.Journals.Find(fkJournal);
            ViewBag.High = progresses.Where(m => m.FkMark <= 12 && m.FkMark >= 10).Count();
            ViewBag.Middle = progresses.Where(m => m.FkMark <= 9 && m.FkMark >= 7).Count();
            ViewBag.SemiMiddle = progresses.Where(m => m.FkMark <= 6 && m.FkMark >= 4).Count();
            ViewBag.Low = progresses.Where(m => m.FkMark <= 3 && m.FkMark >= 1).Count();
            ViewBag.Eps = progresses.Where(m => m.FkMark == 13).Count();
            ViewBag.Pp = progresses.Where(m => m.FkMark == 14).Count();
            ViewBag.SubjectTitle = _db.Subjects.Where(s => s.Id == journal.FkTeacherSubjectNavigation.FkSubject)
                .Select(s => s.Title).First();
            ViewBag.Avg = GetAveregeMark(progresses);
            return View();
        }


        private double GetAveregeMark(List<Progress> progresses)
        {
            if (progresses.Count() == 0)
            {
                return 0;
            }
            else 
            {
                double? result = progresses.Where(m => m.FkMark != 13 && m.FkMark != 14).Average(m=>m.FkMark);
                return (double)result;
            }            
        }
    }
}

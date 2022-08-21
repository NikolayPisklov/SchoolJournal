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
            List<AcademicPerformanceContent> content = GetSubjectsForStudent(fkClass);
            return View(content);
        }
        public IActionResult AcademicPerformance(int fkJournal, int fkStudent) 
        {
            List<Mark> studentMarks = GetMarks(fkJournal, fkStudent);
            Journal journal = _db.Journals.Find(fkJournal);
            ViewBag.High = studentMarks.Where(m => m.Id <= 12 && m.Id >= 10).Count();
            ViewBag.Middle = studentMarks.Where(m => m.Id <= 9 && m.Id >= 7).Count();
            ViewBag.SemiMiddle = studentMarks.Where(m => m.Id <= 6 && m.Id >= 4).Count();
            ViewBag.Low = studentMarks.Where(m => m.Id <= 3 && m.Id >= 1).Count();
            ViewBag.Eps = studentMarks.Where(m => m.Id == 13).Count();
            ViewBag.Pp = studentMarks.Where(m => m.Id == 14).Count();
            ViewBag.SubjectTitle = _db.Subjects.Where(s => s.Id == journal.FkSubject).Select(s => s.Title).First();
            ViewBag.Avg = GetAveregeMark(studentMarks);
            return View();
        }
        private List<AcademicPerformanceContent> GetSubjectsForStudent(int fkClass) 
        {
            return (from j in _db.Journals
                   join s in _db.Subjects on j.FkSubject equals s.Id
                   where j.FkClass == fkClass && j.FkSchoolYear == SchoolDateTime.GetCurrentYearId(_db)
                   select new AcademicPerformanceContent {Subject = s, Journal = j }).ToList();
        }
        private List<Mark> GetMarks(int journalId, int studentId)
        {
            return (from l in _db.Lessons
                    join p in _db.Progresses on l.Id equals p.FkLesson
                    join m in _db.Marks on p.FkMark equals m.Id
                    join j in _db.Journals on l.FkJournal equals j.Id
                    where l.FkJournal == journalId && p.FkStudent == studentId
                    select new Mark
                    {
                        Id = m.Id,
                        Title = m.Title
                    }).ToList();
        }
        private double GetAveregeMark(List<Mark> marks)
        {
            if (marks.Count() == 0)
            {
                return 0;
            }
            else 
            {
                double result = marks.Where(m => m.Id != 13 && m.Id != 14).Average(m => m.Id);
                return result;
            }            
        }
    }
}

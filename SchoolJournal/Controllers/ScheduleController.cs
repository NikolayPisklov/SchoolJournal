using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolJournal.ViewModels;

namespace SchoolJournal.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly SchoolJournalContext _db;

        public ScheduleController(SchoolJournalContext db)
        {
            _db = db;
        }

        public IActionResult StudentSchedule(int? pageNumber)
        {
            int? fkClass = HttpContext.Session.GetInt32("FkClass");
            List<DateTime> schoolYearDays = GetSchoolYearDays();
            ViewBag.Content = GetScheduleContentForStudent((int)fkClass);
            ViewBag.LessonTimes = _db.LessonTimes.OrderBy(lt => lt.StartTime.Length).ThenBy(lt => lt.StartTime).ToList();
            return View("Schedule", Paging<DateTime>.Create(schoolYearDays, pageNumber ?? GetCurrentPage(schoolYearDays), 7));
        }
        public IActionResult TeacherSchedule(int? pageNumber)
        {
            int? teacherId = HttpContext.Session.GetInt32("TeacherId");
            List<DateTime> schoolYearDays = GetSchoolYearDays();
            ViewBag.Content = GetScheduleContentForTeacher((int)teacherId);
            ViewBag.LessonTimes = _db.LessonTimes.OrderBy(lt => lt.StartTime.Length).ThenBy(lt => lt.StartTime).ToList();
            return View("Schedule", Paging<DateTime>.Create(schoolYearDays, pageNumber ?? GetCurrentPage(schoolYearDays), 7));
        }
        public IActionResult AdminSchedule(int? pageNumber, int fkClass)
        {
            List<DateTime> schoolYearDays = GetSchoolYearDays();
            ViewBag.Content = GetScheduleContentForStudent(fkClass);
            ViewBag.LessonTimes = _db.LessonTimes.OrderBy(lt => lt.StartTime.Length).ThenBy(lt => lt.StartTime).ToList();
            ViewBag.FkClass = fkClass;
            HttpContext.Session.SetInt32("schedulePage", pageNumber ?? GetCurrentPage(schoolYearDays));
            return View("Schedule", Paging<DateTime>.Create(schoolYearDays, pageNumber ?? GetCurrentPage(schoolYearDays), 7));            
        }
        public IActionResult ClassesSchedules()
        {
            List<Class> classes = _db.Classes.Where(c => c.ReleaseDate > DateTime.Now)
                .OrderBy(c => c.Title.Length).ThenBy(c => c.Title).ToList();
            return View(classes);
        }
        [HttpGet]
        public IActionResult AddLesson(int fkClass, int fkLessonTime, DateTime lessonDate)
        {
            Lesson newLesson = new Lesson();
            newLesson.FkLessonTime = fkLessonTime;
            newLesson.Date = lessonDate;
            ViewBag.FkClass = fkClass;
            ViewBag.JournalsSelectList = GetJournalJournalsSelectList(fkClass);
            return View(newLesson);
        }
        [HttpPost]
        public IActionResult AddLesson(Lesson newLesson, int fkClass)
        {
            if (ModelState.IsValid)
            {
                _db.Add(newLesson);
                _db.SaveChanges();
                return RedirectToRoute(new { action = "AdminSchedule", 
                    pageNumber = HttpContext.Session.GetInt32("schedulePage"), fkClass = fkClass });

            }
            else
            {
                ViewBag.FkClass = fkClass;
                ViewBag.JournalsSelectList = GetJournalJournalsSelectList(fkClass);
                return View();
            }
        }
        [HttpGet]
        public IActionResult EditLesson(int fkJournal, int lessonId) 
        {
            int fkClass = _db.Journals.Where(j => j.Id == fkJournal).Select(j => j.FkClass).First(); 
            ViewBag.FkClass = fkClass;
            ViewBag.JournalsSelectList = GetJournalJournalsSelectList(fkClass);
            Lesson lesson = _db.Lessons.Find(lessonId);
            ViewBag.OldFkJournal = lesson.FkJournal;
            return View(lesson);
        }
        [HttpPost]
        public IActionResult EditLesson(int fkTime, int fkJournal, int fkClass, Lesson lesson) 
        {
            if (ModelState.IsValid)
            {
                Journal journal = _db.Journals.Find(fkJournal);
                lesson.FkJournalNavigation = _db.Journals.Find(fkJournal);
                journal.Lessons.Add(lesson);
                _db.Update(lesson);
                _db.SaveChanges();
                return RedirectToRoute(new { action = "AdminSchedule", pageNumber = HttpContext.Session.GetInt32("schedulePage"), fkClass });

            }
            else
            {
                ViewBag.FkClass = fkClass;
                ViewBag.JournalsSelectList = GetJournalJournalsSelectList(fkClass);
                return View();
            }
        }
        [HttpPost]
        public IActionResult DeleteLesson(Lesson lesson, int oldFkJournal, int fkClass) 
        {
            lesson.FkJournal = oldFkJournal;
            _db.Remove(lesson);
            _db.SaveChanges();
            return RedirectToRoute(new { action = "AdminSchedule", pageNumber = HttpContext.Session.GetInt32("schedulePage"), fkClass });
        }

        private List<ScheduleContent> GetScheduleContentForStudent(int fkClass) 
        {
            return (from j in _db.Journals
                    join l in _db.Lessons on j.Id equals l.FkJournal
                    join s in _db.Subjects on j.FkSubject equals s.Id
                    join t in _db.Teachers on j.FkTeacher equals t.Id
                    join c in _db.Classes on j.FkClass equals c.Id
                    where j.FkClass == fkClass && j.FkSchoolYear == SchoolDateTime.GetCurrentYearId(_db)
                    select new ScheduleContent { Journal = j, Lesson = l, Subject = s, Teacher = t, Class = c }).ToList();
        }
        private List<ScheduleContent> GetScheduleContentForTeacher(int teacherId)
        {
            return (from j in _db.Journals
                    join l in _db.Lessons on j.Id equals l.FkJournal
                    join s in _db.Subjects on j.FkSubject equals s.Id
                    join t in _db.Teachers on j.FkTeacher equals t.Id
                    join c in _db.Classes on j.FkClass equals c.Id
                    where j.FkTeacher == teacherId && j.FkSchoolYear == SchoolDateTime.GetCurrentYearId(_db)
                    select new ScheduleContent { Journal = j, Lesson = l, Subject = s, Teacher = t, Class = c }).ToList();
        }
        private List<DateTime> GetSchoolYearDays() 
        {
            List<DateTime> days = new List<DateTime>();
            DateTime startDate = _db.SchoolYears.
                Where(y => y.Id == SchoolDateTime.GetCurrentYearId(_db)).Select(y => y.StartDate).FirstOrDefault().AddDays(89);
            days.Add(startDate);
            for (int i = 1; i <= 279; i++)
            {
                var date = startDate.AddDays(i);
                days.Add(date);
            }
            return days;
        }
        private SelectList GetJournalJournalsSelectList(int fkClass) 
        {
            List<ScheduleContent> scheduleContents = (from j in _db.Journals
                                                      join s in _db.Subjects on j.FkSubject equals s.Id
                                                      join t in _db.Teachers on j.FkTeacher equals t.Id
                                                      join c in _db.Classes on j.FkClass equals c.Id
                                                      where j.FkClass == fkClass && j.FkSchoolYear == SchoolDateTime.GetCurrentYearId(_db)
                                                      select new ScheduleContent 
                                                      { Journal = j, Subject = s, Teacher = t, Class = c }).ToList();
            Dictionary<int, string> journals = new Dictionary<int, string>();
            foreach (ScheduleContent sc in scheduleContents) 
            {
                string pointTitle = $"{sc.Subject.Title} - {sc.Teacher.Surname} {sc.Teacher.Name[0]}. {sc.Teacher.Middlename[0]}.";
                journals.Add(sc.Journal.Id, pointTitle);
            }
            return new SelectList(journals, "Key", "Value"); ;
        }
        private int GetCurrentPage(List<DateTime> days) 
        {
            DateTime currentDay = days.Where(d => d.Date == DateTime.Now.Date).FirstOrDefault();
            double dayIndex = days.IndexOf(currentDay) + 1;
            double result = dayIndex / 7;
            if (result <= 1)
            {
                return 1;
            }
            else 
            {
                return (int)Math.Truncate(result) + 1;
            }
        }
    }
}

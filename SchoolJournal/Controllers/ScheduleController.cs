using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolJournal.ViewModels;
using System.Text.Json;

namespace SchoolJournal.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly SchoolJournalContext _db;
        private int _currentPage = 1;//To do: change to selecting page to datetime.now
        public ScheduleController(SchoolJournalContext db)
        {
            _db = db;
        }

        public IActionResult StudentSchedule(int? pageNumber)
        {
            User user = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("UserObject"));
            List<DateTime> schoolYearDays = GetSchoolYearDays();
            ViewBag.Content = GetScheduleContentForStudent(user.FkClass);
            ViewBag.LessonTimes = _db.LessonTimes.OrderBy(lt => lt.StartTime.Length).ThenBy(lt => lt.StartTime).ToList();
            return View("Schedule", Paging<DateTime>.Create(schoolYearDays, pageNumber ?? _currentPage, 7));
        }
        public IActionResult TeacherSchedule(int? pageNumber)
        {
            User user = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("UserObject"));
            List<DateTime> schoolYearDays = GetSchoolYearDays();
            ViewBag.Content = GetScheduleContentForTeacher(user.Id);
            ViewBag.LessonTimes = _db.LessonTimes.OrderBy(lt => lt.StartTime.Length).ThenBy(lt => lt.StartTime).ToList();
            return View("Schedule", Paging<DateTime>.Create(schoolYearDays, pageNumber ?? _currentPage, 7));
        }
        public IActionResult AdminSchedule(int? pageNumber, int fkClass)
        {
            List<DateTime> schoolYearDays = GetSchoolYearDays();
            ViewBag.Content = GetScheduleContentForStudent(fkClass);
            ViewBag.LessonTimes = _db.LessonTimes.OrderBy(lt => lt.StartTime.Length).ThenBy(lt => lt.StartTime).ToList();
            ViewBag.FkClass = fkClass;
            return View("Schedule", Paging<DateTime>.Create(schoolYearDays, pageNumber ?? _currentPage, 7));
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
                return RedirectToAction("AdminSchedule", new { _currentPage, fkClass });

            }
            else
            {
                ViewBag.FkClass = fkClass;
                ViewBag.JournalsSelectList = GetJournalJournalsSelectList(fkClass);
                return View();
            }
        }
        [HttpGet]
        public IActionResult EditLesson(Lesson lesson) 
        {
            int fkClass = _db.Journals.Where(j => j.Id == lesson.FkJournal).Select(j => j.FkClass).First(); 
            ViewBag.FkClass = fkClass;
            ViewBag.JournalsSelectList = GetJournalJournalsSelectList(fkClass);
            return View(lesson);
        }
        [HttpPost]
        public IActionResult EditLesson(int fkClass, Lesson lesson) 
        {

            return View();
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
    }
}

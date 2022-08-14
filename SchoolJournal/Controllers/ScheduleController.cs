using Microsoft.AspNetCore.Mvc;
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
            //User user = JsonSerializer.Deserialize<User>(HttpContext.Session.GetString("UserObject"));
            List<DateTime> schoolYearDays = GetSchoolYearDays();
            ViewBag.Content = GetScheduleContent(/*user.FkClass*/1);
            ViewBag.LessonTimes = _db.LessonTimes.OrderBy(lt => lt.StartTime.Length).ThenBy(lt => lt.StartTime).ToList();
            return View("Schedule", Paging<DateTime>.Create(schoolYearDays, pageNumber ?? _currentPage, 7));
        }

        private List<ScheduleContent> GetScheduleContent(int fkClass) 
        {
            return (from j in _db.Journals
                    join l in _db.Lessons on j.Id equals l.FkJournal
                    join s in _db.Subjects on j.FkSubject equals s.Id
                    join t in _db.Teachers on j.FkTeacher equals t.Id
                    join c in _db.Classes on j.FkClass equals c.Id
                    where j.FkClass == fkClass && j.FkSchoolYear == SchoolDateTime.GetCurrentYearId(_db)
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
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.ViewModels;
using Korzh.EasyQuery.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolJournal.Classes;

namespace SchoolJournal.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly SchoolJournalContext _db;

        public AdministrationController(SchoolJournalContext db)
        {
            _db = db;
        }

        public IActionResult Home()
        {
            return View();
        }


        [HttpGet]
        public IActionResult TeacherList()
        {
            var teachers = _db.Teachers.Where(t => t.FireDate == null)
                .OrderBy(t => t.Surname).ToList();
            ViewBag.Teachers = teachers;
            return View();
        }
        [HttpPost]
        public IActionResult TeacherList(SearchString searchString)
        {
            if (!string.IsNullOrEmpty(searchString.SearchValue))
            {
                var teachers = _db.Teachers.Where(t => t.FireDate == null).OrderBy(t => t.Surname)
                    .FullTextSearchQuery(searchString.SearchValue);
                ViewBag.Teachers = teachers;
                if (teachers.Count() == 0)
                {
                    ViewBag.Message = $"За запитом '{searchString.SearchValue}' нічого не знайдено!";
                    return View(searchString);
                }
                else
                {
                    return View(searchString);
                }
            }
            else
            {
                var teachers = _db.Teachers.Where(t => t.FireDate == null).OrderBy(t => t.Surname);
                ViewBag.Teachers = teachers;
                return View(searchString);
            }
        }
        [HttpGet]
        public IActionResult AddTeacher()
        {
            ViewBag.IsEdit = false;
            return View("Teacher");
        }
        [HttpPost]
        public IActionResult AddTeacher(Teacher newTeacher)
        {
            ViewBag.IsEdit = false;
            if (IsLoginExist(newTeacher.Login))
            {
                ViewBag.Login = "Такий логін вже існує!";
                return View("Teacher");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    _db.Add(newTeacher);
                    _db.SaveChanges();
                    ViewBag.Success = "Ви вдало додали вчителя!";
                    return View("Teacher");
                }
                else
                {
                    return View("Teacher");
                }
            }

        }
        [HttpGet]
        public IActionResult EditTeacher(int teacherId)
        {
            ViewBag.IsEdit = true;
            Teacher teacher = _db.Teachers.Find(teacherId);
            return View("Teacher", teacher);
        }
        [HttpPost]
        public IActionResult EditTeacher(Teacher editedTeacher)
        {
            ViewBag.IsEdit = true;
            string oldLogin = _db.Teachers.Where(t => t.Id == editedTeacher.Id).Select(t => t.Login).First();
            if (IsLoginExist(editedTeacher.Login) && (editedTeacher.Login != oldLogin))
            {
                ViewBag.Login = "Такий логін вже існує!";
                return View("Teacher");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    _db.ChangeTracker.Clear();
                    _db.Update(editedTeacher);
                    _db.SaveChanges();
                    ViewBag.Success = "Ви вдало відредагували вчителя!";
                    return View("Teacher");
                }
                else
                {
                    return View("Teacher");
                }
            }
        }
        [HttpGet]
        public IActionResult TeacherSubjectEdit(int teacherId) 
        {
            TeacherSubject teacherSubject = new TeacherSubject();
            teacherSubject.FkTeacher = teacherId;
            SetViewBagsForTeacherSubjectEdit(teacherId);
            return View(teacherSubject);
        }
        [HttpPost]
        public IActionResult TeacherSubjectEdit(TeacherSubject teacherSubject) 
        {
            ModelState.Remove("FkSubjectNavigation");
            ModelState.Remove("FkTeacherNavigation");
            ModelState.Remove("Journals");
            if (ModelState.IsValid)
            {
                _db.Add(teacherSubject);
                _db.SaveChanges();
                return RedirectToRoute(new { action = "TeacherSubjectEdit", teacherId = teacherSubject.FkTeacher });
            }
            else 
            {
                SetViewBagsForTeacherSubjectEdit(teacherSubject.FkTeacher);
                return View("TeacherSubjectEdit", teacherSubject);
            }
        }
        public IActionResult DeleteTeacherSubject(int teacherId, int subjectId) 
        {
            TeacherSubject teacherSubject = _db.TeacherSubjects
                .Where(t => t.FkSubject == subjectId && t.FkTeacher == teacherId).First();
            if (_db.Journals.Any(t => t.FkTeacherSubject == teacherSubject.Id 
                && t.FkSchoolYear == SchoolDateTime.GetCurrentYearId(_db)))
            {
                SetViewBagsForTeacherSubjectEdit(teacherId);
                string subjectTitle = _db.Subjects.Where(s => s.Id == subjectId).Select(s => s.Title).First();
                ViewBag.Message = $"Вчитель в даний момент читає предмет '{subjectTitle}'!";
                return View("TeacherSubjectEdit", teacherSubject);
            }
            else 
            {
                _db.Remove(teacherSubject);
                _db.SaveChanges();
                return RedirectToRoute(new { action = "TeacherSubjectEdit", teacherId = teacherSubject.FkTeacher });
            }           
        }


        public IActionResult StudentsList()
        {
            List<Student> students = _db.Students.Where(s => s.FkClassNavigation.ReleaseDate >= DateTime.Now)
                .OrderBy(s => s.Surname).ToList();
            ViewBag.Students = students;
            return View();
        }
        [HttpPost]
        public IActionResult StudentsList(SearchString searchString)
        {
            if (!string.IsNullOrEmpty(searchString.SearchValue))
            {
                ViewBag.Students = _db.Students.Where(s => s.FkClassNavigation.ReleaseDate >= DateTime.Now)
                    .OrderBy(s => s.Surname)
                    .FullTextSearchQuery(searchString.SearchValue).ToList();
                List<Student> students = ViewBag.Students;
                if (students.Count() == 0)
                {
                    ViewBag.Message = $"За запитом '{searchString.SearchValue}' нічого не знайдено!";
                    return View(searchString);
                }
                else
                {
                    return View(searchString);
                }
            }
            else
            {
                var students = _db.Students.Where(s => s.FkClassNavigation.ReleaseDate >= DateTime.Now)
                    .OrderBy(s => s.Surname).ToList();
                ViewBag.Students = students;
                return View(searchString);
            }
        }
        [HttpGet]
        public IActionResult AddStudent()
        {
            ViewBag.IsEdit = false;
            ViewBag.ClassesSelectList = GetClassSelectList();
            return View("Student");
        }
        [HttpPost]
        public IActionResult AddStudent(Student newStudent)
        {
            ViewBag.IsEdit = false;
            ViewBag.ClassesSelectList = GetClassSelectList();
            ModelState.Remove("FkClassNavigation");
            ModelState.Remove("Id");
            if (IsLoginExist(newStudent.Login))
            {
                ViewBag.Login = "Такий логін вже існує!";
                return View("Student");
            }
            if (ModelState.IsValid)
            {
                _db.Add(newStudent);
                _db.SaveChanges();
                ViewBag.Success = "Ви вдало додали учня!";
                return View("Student");
            }
            else
            {
                return View("Student");
            }
        }
        [HttpGet]
        public IActionResult EditStudent(int studentId)
        {
            ViewBag.IsEdit = true;
            ViewBag.ClassesSelectList = GetClassSelectList();
            Student student = _db.Students.Find(studentId);
            return View("Student", student);
        }
        [HttpPost]
        public IActionResult EditStudent(Student student)
        {
            ViewBag.IsEdit = true;
            ViewBag.ClassesSelectList = GetClassSelectList();
            string oldLogin = _db.Students.Where(s => s.Id == student.Id).Select(s => s.Login).First();
            ModelState.Remove("FkClassNavigation");
            if (IsLoginExist(student.Login) && (student.Login != oldLogin))
            {
                ViewBag.Login = "Такий логін вже існує!";
                return View("Student");
            }
            if (ModelState.IsValid)
            {
                _db.ChangeTracker.Clear();
                _db.Update(student);
                _db.SaveChanges();
                ViewBag.Success = "Ви вдало відредагували учня!";
                return View("Student");
            }
            else
            {
                return View("Student");
            }
        }


        [HttpGet]
        public IActionResult JournalsList()
        {
            List<Journal> journals = _db.Journals.Where(j => j.FkSchoolYear ==
                SchoolDateTime.GetCurrentYearId(_db)).ToList();
            JournalsFilter filter = new JournalsFilter(journals);
            SetJournalFiltersViewBags();
            return View(filter.Journals);
        }
        [HttpPost]
        public IActionResult JournalsList(int? subjectId, int? classRankId)
        {
            List<Journal> journals = _db.Journals.Where(j => j.FkSchoolYear ==
                SchoolDateTime.GetCurrentYearId(_db)).ToList();
            JournalsFilter filter = new JournalsFilter(journals);
            SetJournalFiltersViewBags();
            return View(filter.FilterJournals(subjectId, classRankId));
        }


        [HttpGet]
        public IActionResult AddJournalClassSelect()
        {
            ViewBag.ClassSelect = true;
            ViewBag.Classes = GetClassesList();
            Journal journal = new Journal();
            journal.FkSchoolYear = SchoolDateTime.GetCurrentYearId(_db);
            return View("AddJournal", journal);
        }

        [HttpPost]
        public IActionResult AddJournalSubjectSelect(Journal journal, int classId)
        {
            journal.FkClass = classId;
            ViewBag.SubjectSelect = true;
            Class clas = _db.Classes.Find(classId);
            List<Subject> subjects = _db.Subjects.ToList();
            if (clas.FkClassRank == 1)
            {
                ViewBag.Subjects = GetSubjectsList(subjects, SubjectsRank.Beginner);
                return View("AddJournal", journal);
            }
            else if (clas.FkClassRank == 2)
            {
                ViewBag.Subjects = GetSubjectsList(subjects, SubjectsRank.Middle);
                return View("AddJournal", journal);
            }
            else if (clas.FkClassRank == 3)
            {
                ViewBag.Subjects = GetSubjectsList(subjects, SubjectsRank.Senior);
                return View("AddJournal", journal);
            }
            else 
            {
                ViewBag.Subjects = GetSubjectsList(subjects, SubjectsRank.None);
                return View("AddJournal", journal);
            }
                
        }
        [HttpPost]
        public IActionResult AddJournalTeacherSelect(Journal journal, int subjectId)
        {
            ViewBag.FkSubject = subjectId; 
            ViewBag.TeacherSelect = true;
            ViewBag.Teachers = GetTeachersList(subjectId);
            return View("AddJournal", journal);
        }
        [HttpPost]
        public IActionResult AddJournalPreview(Journal journal, int teacherId, int subjectId) 
        {
            TeacherSubject teacherSubject = _db.TeacherSubjects.Where(ts => ts.FkSubject == subjectId &&
                ts.FkTeacher == teacherId).First();
            journal.FkTeacherSubject = teacherSubject.Id;
            ViewBag.Preview = true;
            if (IsJournalExist(journal)) 
            {
                ViewBag.Message = "Такий журнал вже існує!";
            }
            ViewBag.Teacher = _db.Teachers.Find(teacherSubject.FkTeacher);
            ViewBag.Subject = _db.Subjects.Find(teacherSubject.FkSubject);
            ViewBag.Class = _db.Classes.Find(journal.FkClass);
            ViewBag.SchoolYear = _db.SchoolYears.Find(SchoolDateTime.GetCurrentYearId(_db));
            return View("AddJournal", journal);
        }
        [HttpPost]
        public IActionResult AddJournalSave(Journal journal) 
        {
            _db.Add(journal);
            _db.SaveChanges();
            return View("Success");
        }

        private bool IsLoginExist(string login)
        {
            var student = _db.Students.Where(s => s.Login == login).FirstOrDefault();
            var teacher = _db.Teachers.Where(t => t.Login == login).FirstOrDefault();
            var admin = _db.Administrators.Where(a => a.Login == login).FirstOrDefault();
            if (student == null && teacher == null && admin == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private SelectList GetClassSelectList()
        {
            Dictionary<int, string> classesDict = new Dictionary<int, string>();
            List<Class> classes = _db.Classes.Where(c => c.ReleaseDate > DateTime.Now)
                .OrderBy(c => c.Title.Length).ThenBy(c => c.Title).ToList();
            foreach (Class c in classes)
            {
                classesDict.Add(c.Id, c.Title);
            }
            return new SelectList(classesDict, "Key", "Value");
        }
        private List<Class> GetClassesList() 
        {
            return _db.Classes.OrderBy(c => c.Title.Length).ThenBy(c => c.Title).ToList();
        }
        private List<Subject> GetSubjectsList(List<Subject> subjects, SubjectsRank rank) 
        {
            SubjectsList subjectsList = new SubjectsList(subjects, rank);
            return subjectsList.GetSubjectsByRank();
        }
        private List<Teacher> GetTeachersList(int fkSubject) 
        {
            var teacherSubject = _db.TeacherSubjects.Where(x => x.FkSubject == fkSubject).ToList();
            List<Teacher> teachers = new List<Teacher>();
            foreach (TeacherSubject ts in teacherSubject) 
            {
                teachers.Add(_db.Teachers.Find(ts.FkTeacher));
            }
            return teachers;
        }
       
        private void SetJournalFiltersViewBags() 
        {
            SubjectSelectList subjectSelectList =
                new SubjectSelectList(_db.Subjects.OrderBy(s => s.Title));
            ClassRankSelectList classRankSelectList =
                new ClassRankSelectList(_db.ClassRanks.OrderBy(c => c.Title));
            ViewBag.SubjectsSelectList = subjectSelectList.GetSelectList();
            ViewBag.ClassRanksSelectList = classRankSelectList.GetSelectList();
        }
        private bool IsJournalExist(Journal journal) 
        {
            var check = _db.Journals.Where(j => j.FkClass == journal.FkClass && 
                j.FkTeacherSubject == journal.FkTeacherSubject &&
                j.FkSchoolYear == journal.FkSchoolYear).FirstOrDefault();
            if (check == null)
            {
                return false;
            }
            else 
            {
                return true;
            }
        }
        private void SetViewBagsForTeacherSubjectEdit(int teacherId) 
        {
            List<TeacherSubject> teacherSubjects = _db.Teachers.Find(teacherId).TeacherSubjects
                .OrderBy(ts => ts.FkSubjectNavigation.Title).ToList();
            List<Subject> readingSubjects = new List<Subject>();
            foreach (TeacherSubject ts in teacherSubjects)
            {
                readingSubjects.Add(ts.FkSubjectNavigation);
            }
            IQueryable<Subject> subjects = _db.Subjects.WhereBulkNotContains(readingSubjects)
                .OrderBy(s => s.Title);
            SubjectSelectList subjectSelectList = new SubjectSelectList(subjects);
            ViewBag.TeacherSubjects = teacherSubjects;
            ViewBag.SubjectSelectList = subjectSelectList.GetSelectList();
        }
    }
}

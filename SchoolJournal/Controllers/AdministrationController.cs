using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.ViewModels;
using Korzh.EasyQuery.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var teachers = _db.Teachers.Where(t => t.FireDate == null).OrderBy(t => t.Surname).ToList();
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
            List<Class> classes = _db.Classes.Where(c => c.ReleaseDate > DateTime.Now).ToList();
            foreach (Class c in classes)
            {
                classesDict.Add(c.Id, c.Title);
            }
            return new SelectList(classesDict, "Key", "Value");
        }
    }
}

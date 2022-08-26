using Microsoft.AspNetCore.Mvc;
using SchoolJournal.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace SchoolJournal.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly SchoolJournalContext _db;

        public AuthorizationController(SchoolJournalContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Authorization()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Authorization(User user) 
        {
            if (IsStudent(user))
            {
                SetStudentProperties(user);
                SetSessionVariablesForStudent(user);
                return RedirectToAction("StudentHome", "Home", new { fkClass = user.FkClass});
            }
            else if (IsTeacher(user))
            {
                SetTeacherProperties(user);
                SetSessionVariablesForTeacher(user);
                return RedirectToAction("TeacherHome", "Home", new { teacherId = user.Id });
            }
            else if (IsAdmin(user))
            {
                SetAdminProperties(user);
                SetSessionVariablesForAdmin(user);
                return RedirectToAction("Home", "Home");
            }
            else 
            {
                ViewBag.Message = "Невірний логін або пароль!";
                return View();
            }        
        }

        private bool IsAdmin(User user) 
        {        
            var admin = _db.Administrators.Where(a => a.Login == user.Login && a.Password == user.Password).FirstOrDefault();
            if (admin == null)
            {
                return false;
            }
            else 
            {
                return true;
            }
        }
        private bool IsTeacher(User user) 
        {
            var teacher = _db.Teachers.Where(t => t.Login == user.Login && t.Password == user.Password).FirstOrDefault();
            if (teacher == null)
            {
                return false;
            }
            else 
            {
                return true;
            }
        }
        private bool IsStudent(User user) 
        {
            var student = _db.Students.Where(s => s.Login == user.Login && s.Password == user.Password).FirstOrDefault();
            if (student == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void SetSessionVariablesForAdmin(User user) 
        {
            HttpContext.Session.SetString("Status", "Admin");
        }
        private void SetSessionVariablesForTeacher(User user) 
        {
            HttpContext.Session.SetString("Status", "Teacher");
            HttpContext.Session.SetInt32("TeacherId", user.Id);
        }
        private void SetSessionVariablesForStudent(User user) 
        {
            HttpContext.Session.SetString("Status", "Student");
            HttpContext.Session.SetInt32("StudentId", user.Id);
            HttpContext.Session.SetInt32("FkClass", user.FkClass);
        }
        private void SetStudentProperties(User user) 
        {
            Student student = _db.Students.Where(s => s.Login == user.Login).First();
            user.Id = student.Id;
            user.FkClass = (int)student.FkClass;
            user.ParrentEmail = student.ParrentEmail;
            user.Surname = student.Surname;
            user.Name = student.Name;
            user.Middlename = student.Middlename;            
        }
        private void SetTeacherProperties(User user) 
        {
            Teacher teacher = _db.Teachers.Where(t => t.Login == user.Login).First();
            user.Id = teacher.Id;
            user.Surname = teacher.Surname;
            user.Name = teacher.Name;
            user.Middlename = teacher.Middlename;
            user.HireDate = teacher.HireDate;
            user.FireDate = teacher.FireDate;
        }
        private void SetAdminProperties(User user)
        {
            Administrator administrator = _db.Administrators.Where(a => a.Login == user.Login).First();
            user.Id = administrator.Id;
            user.Surname = administrator.Surname;
            user.Name = administrator.Name;
            user.Middlename = administrator.Middlename;
            user.HireDate = administrator.HireDate;
            user.FireDate = administrator.FireDate;
        }
    }
}

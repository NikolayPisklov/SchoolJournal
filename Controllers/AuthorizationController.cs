using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolJournal.Models;
using SchoolJournal.ViewModels;
using System.Security.Claims;

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
        public async Task<IActionResult> Authorization(UserLoginDetails details) 
        {
            var user = _db.Users.Include(x => x.PersonClasses)
                .FirstOrDefault(x => x.Login == details.Login 
                && x.Password == details.Password);
            if (user == null)
            {
                ViewBag.Message = "Невірний логін або пароль!";
                return View(details);
            }
            else 
            {
                var claims = GetUserClaims(user);
                var claimsIdentity = GetClaimsIdentity(claims);
                
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("RediretByStatus", new { fkStatus = user.FkStatus});
            }            
        }
        public IActionResult RediretByStatus(int fkStatus) 
        {
            if(fkStatus == 3) 
            {
                return RedirectToAction("AdminHome", "Home");
            }
            else if(fkStatus == 2) 
            {
                return RedirectToAction("TeacherHome", "Home");
            }
            else 
            {
                return RedirectToAction("StudentHome", "Home"); 
            }           
        }

        private List<Claim> GetUserClaims(User user) 
        {
            var fullName = $"{user.Surname} {user.Name} {user.Middlename}";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim("FullName", fullName),
                new Claim(ClaimTypes.Role, user.FkStatus.ToString())
            };
            if (user.FkStatus == 1) 
            {
                var fkClass = user.PersonClasses.MaxBy(x => x.Id).FkClass.ToString();
                var classClaim = new Claim("Class", fkClass);
                claims.Add(classClaim);
            }
            return claims;
        }
        private ClaimsIdentity GetClaimsIdentity(List<Claim> claims) 
        {
            return new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
    
}

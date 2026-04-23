using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.DbServices.Interfaces;
using System.Data.Common;
using System.Threading.Tasks;

namespace SchoolJournalApi.Services.DbServices
{
    public class UserDbService : DbService, IUsersDbService
    {
        public UserDbService(SchoolJournalDbContext db) : base(db) { _db = db; }


        public IQueryable<User> GetAllUsers() 
        {
            return _db.Users.AsNoTracking();
        }
        public IQueryable<Status> GetUserStatuses() 
        {
            return _db.Statuses.AsNoTracking();
        }
        public void AddUser(User user) 
        {
             _db.Users.Add(user);
        }
        public void DeleteUser(User user) 
        {
            _db.Users.Remove(user);
        }

        public async Task<StudentClass?> FindClassOfStudentAsync(int userId)
        {
            var studentClass = await _db.StudentClasses.Include(s => s.Class)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);
            return studentClass;
        }
        public async Task<User?> FindUserAsync(int userId) 
        {
            try
            {
                return await _db.Users.FindAsync(userId);
            }
            catch (DbException ex)
            {
                throw new EfDbException("An error has occur while reading data!", ex);
            }            
        }
        public IQueryable<User> FilterUsersByStatus(IQueryable<User> users, int statusId) 
        {
            return users.Where(x => x.StatusId == statusId);
        }
        public IQueryable<User> FilterUsersByName(IQueryable<User> users, string searchString) 
        {
            var parts = searchString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            users = users.Where(x => parts
                .All(p =>
                    EF.Functions.Like(x.LastName, "%" + p + "%") ||
                    EF.Functions.Like(x.FirstName, "%" + p + "%") ||
                    EF.Functions.Like(x.MiddleName, "%" + p + "%")
                )
            );
            return users;
        }
        public async Task<bool> IsThereUserWithSameEmailAsync(string email, int? userId)
        {
            try
            {
                return await _db.Users.AnyAsync(x => email.Equals(x.Email) && x.Id != userId);
            }
            catch (DbException ex)
            {
                throw new EfDbException("An error has occur while reading data!", ex);
            }
        }
        public async Task<bool> IsThereUserWithSameEmailAsync(string email)
        {
            try
            {
                return await _db.Users.AnyAsync(x => email.Equals(x.Email));
            }
            catch (DbException ex)
            {
                throw new EfDbException("An error has occur while reading data!", ex);
            }
        }
        public async Task<bool> IsThereUserWithSameLoginAsync(string login, int? userId)
        {
            try
            {
                return await _db.Users.AnyAsync(x => login.Equals(x.Login) && x.Id != userId);
            }
            catch (DbException ex)
            {
                throw new EfDbException("An error has occur while reading data!", ex);
            }                   
        }
        public async Task<bool> IsThereUserWithSameLoginAsync(string login)
        {
            try
            {
                return await _db.Users.AnyAsync(x => login.Equals(x.Login));
            }
            catch (DbException ex)
            {
                throw new EfDbException("An error has occur while reading data!", ex);
            }           
        }
        public async Task<int> SelectUserStatusId(int userId)
        {
            try
            {
                var statusId = await _db.Users.Where(u => u.Id == userId).Select(u => u.StatusId).SingleAsync();
                return statusId;
            }
            catch (DbException ex) 
            {
                throw new EfDbException("An error has occur while reading data!", ex);
            }           
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Dtos.User;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.DbServices.Interfaces;

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
            return _db.Statuses;
        }
        public async Task<UserUpdateDto?> GetUserDetailsAsync(int userId) 
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if(user is null) 
            {
                return null;
            }
            var dto = new UserUpdateDto
            {
                Id = userId,
                Login = user.Login,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Email = user.Email
            };
            return dto;
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
            return await _db.Users.FindAsync(userId);
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
        public async Task<bool> IsThereUserWithSameLoginAsync(string login, int? userId)
        {
            return await _db.Users.AnyAsync(x => login.Equals(x.Login) && x.Id != userId);             
        }
        public async Task<bool> IsThereUserWithSameLoginAsync(string login)
        {
            return await _db.Users.AnyAsync(x => login.Equals(x.Login));
        }
    }
}

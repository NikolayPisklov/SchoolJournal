using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services.DbServices.Interfaces
{
    public interface IUsersDbService
    {
        void AddUser(User user);
        void DeleteUser(User user);
        IQueryable<Status> GetUserStatuses();
        Task<User?> FindUserAsync(int userId);
        Task<bool> IsThereUserWithSameLoginAsync(string login, int? userId);
        Task<bool> IsThereUserWithSameLoginAsync(string login);
        IQueryable<User> GetAllUsers();
        IQueryable<User> FilterUsersByStatus(IQueryable<User> users, int statusId);
        IQueryable<User> FilterUsersByName(IQueryable<User> users, string searchString);
        Task<StudentClass?> FindClassOfStudentAsync(int userId);
        Task<bool> IsThereUserWithSameEmailAsync(string email, int? userId);
        Task<bool> IsThereUserWithSameEmailAsync(string email);
        Task<int> SelectUserStatusId(int userId);
    }
}

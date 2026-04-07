using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services.DbServices.Interfaces
{
    public interface IClassDbService
    {
        Task AddClassAsync(Class newClass);
        Task<Class?> FindClassAsync(int classId);
        Task SaveChangesAsync();
        Task DeleteClassAsync(Class classEntity);
        IQueryable<Class> GetClasses();
    }
}

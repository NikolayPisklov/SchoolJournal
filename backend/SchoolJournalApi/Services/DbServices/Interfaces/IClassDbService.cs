using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services.DbServices.Interfaces
{
    public interface IClassDbService
    {
        void AddClass(Class newClass);
        Task<Class?> FindClassAsync(int classId);
        void DeleteClass(Class classEntity);
        IQueryable<Class> GetClasses();
        Task<bool> IsThereStudentsInClass(int classId);
        Task<bool> IsThereJournalsForClass(int classId, int year);
    }
}

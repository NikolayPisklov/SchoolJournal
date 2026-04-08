using Microsoft.EntityFrameworkCore.Storage;
using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services.DbServices.Interfaces
{
    public interface IStudentClassDbService
    {
        IQueryable<StudentClass> GetStudentClassForClass(int classId);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<StudentClass?> FindStudentClassAsync(int studentId);
        Task AddStudentClassAsync(StudentClass studentClass);
        Task<bool> IsStudent(int userId);
    }
}

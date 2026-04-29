using Microsoft.EntityFrameworkCore.Storage;
using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services.DbServices.Interfaces
{
    public interface IStudentClassDbService
    {
        IQueryable<StudentClass> GetStudentClassForClass(int classId);
        IQueryable<StudentClass> GetStudentsForJournal(int journalId);
        Task<StudentClass?> FindStudentClassAsync(int studentId);
        void AddStudentClass(StudentClass studentClass);
        Task<bool> IsStudent(int userId);
    }
}

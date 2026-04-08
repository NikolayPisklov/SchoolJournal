using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services.DbServices.Interfaces
{
    public interface ITeacherSubjectDbService
    {
        Task<bool> IsTeacherAsync(int userId);
        Task AddTeacherSubjectAsync(TeacherSubject teacherSubject);
        Task<TeacherSubject?> FindTeacherSubjectAsync(int teacherSubjectId);
        Task DeleteTeacherSubjectAsync(TeacherSubject teacherSubject);
        IQueryable<TeacherSubject> GetTeacherSubjectsForTeacher(int userId);
        IQueryable<Subject> GetSubjects(int? educationalLevelId);
        IQueryable<TeacherSubject> GetTeacherSubjects(int? educationalLevelId);
    }
}

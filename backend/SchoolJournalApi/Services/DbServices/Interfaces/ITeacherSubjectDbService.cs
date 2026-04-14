using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services.DbServices.Interfaces
{
    public interface ITeacherSubjectDbService
    {
        Task<bool> IsTeacherAsync(int userId);
        void AddTeacherSubject(TeacherSubject teacherSubject);
        Task<TeacherSubject?> FindTeacherSubjectAsync(int teacherSubjectId);
        void DeleteTeacherSubject(TeacherSubject teacherSubject);
        IQueryable<TeacherSubject> GetTeacherSubjectsForTeacher(int userId);
        IQueryable<Subject> GetSubjects(int? educationalLevelId);
        IQueryable<TeacherSubject> GetTeacherSubjects(int? educationalLevelId);
    }
}

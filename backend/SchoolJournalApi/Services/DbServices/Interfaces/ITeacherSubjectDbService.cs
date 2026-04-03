using SchoolJournalApi.Dto_s;

namespace SchoolJournalApi.Services.DbServices.Interfaces
{
    public interface ITeacherSubjectDbService
    {
        Task<List<TeacherSubjectsDto>> GetSubjectsForTeacherAsync(int userId);
        Task AddTeacherSubjectAsync(int userId, int subjectId);
        Task DeleteTeacherSubjectAsync(int teacherSubjectId);
        Task<List<SubjectDto>> GetSubjectsAsync(int? educationalLevelId);
        Task<List<TeacherSubjectsDto>> GetAllTeacherSubjectsAsync(int? eduLevelId);
    }
}

using SchoolJournalApi.Dto_s;

namespace SchoolJournalApi.Services.AppServices.Interfaces
{
    public interface ITeacherSubjectService
    {
        Task<List<TeacherSubjectsDto>> GetSubjectsForTeacherAsync(int userId);
        Task AddTeacherSubjectAsync(int userId, int subjectId);
        Task DeleteTeacherSubjectAsync(int teacherSubjectId);
        Task<List<SubjectDto>> GetSubjectsAsync(int? educationalLevelId);
        Task<List<TeacherSubjectsDto>> GetTeacherSubjectsAsync(int? eduLevelId);
    }
}

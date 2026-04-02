using SchoolJournalApi.Dtos;
using SchoolJournalApi.Dtos.Progress;

namespace SchoolJournalApi.Services
{
    public interface IProgressDbService
    {
        Task<List<MarkDto>> GetAllMarksAsync();
        Task<List<AttendanceDto>> GetAllAttendancesAsync();
        Task AddProgressAsync(AddProgressDto dto);
        Task UpdateProgressAsync(ProgressDto dto);
        Task<List<JournalProgressDto>> GetProgressesForJournalAsync(int journalId);
        Task<ProgressDetailsDto>GetProgressDetailsAsync(int progressId);
        Task<StudentStaticticDto>GetStudentStatisticAsync(int studentId, int journalId);
    }
}

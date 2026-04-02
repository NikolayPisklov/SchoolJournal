using SchoolJournalApi.Dtos.Journal;

namespace SchoolJournalApi.Services
{
    public interface IJournalDbService
    {
        Task AddJournalAsync(JournalCreationDto dto);
        Task<List<JournalInListDto>> GetJournalsForClassAsync(int classId);
        Task DeleteJournalAsync(int journalId);
        Task<List<JournalGroupDto>> GetJournalsForTeacherAsync(int teacherId);
        Task<List<JournalInListDto>> GetJournalsForStudent(int studentId);
        Task<JournalDetailsDto> GetJournalDetailsAsync(int journalId);
        Task<JournalTitleDto> GetJournalTitleAsync(int journalId);
        Task<JournalDetailsDto> GetJournalDetailsForStudent(int journalId, int studentId);
    }
}

using SchoolJournalApi.Dtos.Journal;

namespace SchoolJournalApi.Services.AppServices.Interfaces
{
    public interface IJournalService
    {
        Task AddJournalAsync(JournalCreationDto dto);
        Task<List<JournalInListDto>> GetJournalsForClassAsync(int classId);
        Task DeleteJournalAsync(int journalId);
        Task<List<JournalGroupDto>> GetJournalsForTeacherAsync(int teacherId);
        Task<List<JournalInListDto>> GetJournalsForStudent(int studentId);
        Task<JournalDetailsDto> GetJournalDetailsAsync(int journalId);
        Task<JournalDetailsDto> GetJournalDetailsForStudentAsync(int journalId);
        Task<JournalTitleDto> GetJournalTitleAsync(int journalId);
    }
}


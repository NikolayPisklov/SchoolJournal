using SchoolJournalApi.Dtos.Lesson;

namespace SchoolJournalApi.Services
{
    public interface ILessonDbService
    {
        Task AddLessonAsync(AddLessonDto lessonDto);
        Task<List<LessonDto>> GetLessonsForJournal(int journalId, int month, int journalYear);
        Task UpdateLessonDetailsAsync(int lessonId, LessonDetailsUpdateDto detailsDto);
        Task DeleteLessonAsync(int lessonId);
    }
}

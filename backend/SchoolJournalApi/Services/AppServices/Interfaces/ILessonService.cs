using SchoolJournalApi.Dtos.Lesson;

namespace SchoolJournalApi.Services.AppServices.Interfaces
{
    public interface ILessonService
    {
        Task AddLessonAsync(AddLessonDto lessonDto);
        Task<List<LessonDto>> GetLessonsForJournalAsync(int journalId, int month, int journalYear);
        Task UpdateLessonDetailsAsync(int lessonId, LessonDetailsUpdateDto detailsDto);
        Task DeleteLessonAsync(int lessonId);
    }
}

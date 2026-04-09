using SchoolJournalApi.Dtos.Lesson;
using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services.DbServices.Interfaces
{
    public interface ILessonDbService
    {
        Task AddLessonAsync(Lesson lesson);
        Task<Lesson?> FindLessonAsync(int lessonId);
        Task DeleteLessonAsync(Lesson lesson);
        IQueryable<Lesson> GetLessonsForJournal(int journalId, int month, int journalYear);
        Task SaveChangesAsync();
    }
}

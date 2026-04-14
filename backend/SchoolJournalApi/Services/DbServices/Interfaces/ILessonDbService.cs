using SchoolJournalApi.Dtos.Lesson;
using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services.DbServices.Interfaces
{
    public interface ILessonDbService
    {
        void AddLesson(Lesson lesson);
        Task<Lesson?> FindLessonAsync(int lessonId);
        void DeleteLesson(Lesson lesson);
        IQueryable<Lesson> GetLessonsForJournal(int journalId, int month, int journalYear);
    }
}

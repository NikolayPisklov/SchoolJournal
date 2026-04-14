using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services.DbServices.Interfaces
{
    public interface IProgressDbService
    {
        IQueryable<Mark> GetMarks();
        IQueryable<Attendance> GetAttendances();
        IQueryable<Progress> GetProgressesForJournal(int journalId);
        IQueryable<Progress> GetProgressesForStudentStatistic(int studentId, int journalId);
        void AddProgress(Progress progress);
        Task<Progress?> FindProgressAsync(int progressId);
        IQueryable<Progress> GetProgressHistory(int userId, int lessonId);
    }
}

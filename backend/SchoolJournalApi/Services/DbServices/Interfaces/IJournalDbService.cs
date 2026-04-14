using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services.DbServices.Interfaces
{
    public interface IJournalDbService
    {
        Task<bool> IsThereSameJournal(int classId, int teacherSubjectId, int year);
        void AddJournal(Journal journal);
        Task<Journal?> FindJournalAsync(int journalId);
        Task<Journal?> FindJournalWithIncludes(int journalId);
        Task<StudentClass?> FindStudentClassAsync(int studentId);
        void DeleteJournal(Journal journal);
        IQueryable<StudentClass> GetStudentsForJournal(int journalId);  
        IQueryable<Lesson> GetLessonsForJournal(int journalId, int journalYear);  
        IQueryable<Progress> GetProgressesForJournal(int journalId);
        IQueryable<Journal> GetJournalsForClass(int classId);
        IQueryable<Journal> GetJournalsForTeacher(int teacherId);
    }
}

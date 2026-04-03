using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dtos.Lesson;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.DbServices.Interfaces;

namespace SchoolJournalApi.Services.DbServices
{
    public class LessonDbService : DbService<LessonDto>, ILessonDbService
    {
        public LessonDbService(SchoolJournalDbContext db) : base(db) { }
        public async Task AddLessonAsync(AddLessonDto lessonDto)
        {
            try
            {
                var journal = await _db.Journals.FindAsync(lessonDto.JournalId);
                DateOnly notNullDate = (DateOnly)lessonDto.LessonDate!;
                if (journal is null)
                    throw new EntityNotFoundException("Journal of Lesson");
                if (!IsLessonDateValidToJournalYear(journal.Year, (DateOnly)lessonDto.LessonDate!))
                    throw new EntityHasBusinessLogicConflictException("Lesson date is out of range of Journal's year.");
                if (notNullDate.DayOfWeek == DayOfWeek.Sunday || notNullDate.DayOfWeek == DayOfWeek.Saturday)
                    throw new EntityHasBusinessLogicConflictException("Lessons can't be on weekends");
                Lesson newLesson = new Lesson();
                newLesson.JournalId = lessonDto.JournalId;
                newLesson.LessonDate = (DateOnly)lessonDto.LessonDate!;
                _db.Add(newLesson);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            {
                throw new EntityAddingException("An error has occurred while adding a Lesson entity!", ex);
            }
        }

        public async Task DeleteLessonAsync(int lessonId)
        {
            var lesson = await _db.Lessons.FindAsync(lessonId);
            if(lesson is null)
            {
                throw new EntityNotFoundException("Lesson");
            }
            if(lesson.LessonDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                throw new EntityHasBusinessLogicConflictException("Cannot delete lesson after it is being teached");
            }
            lesson.IsDeleted = true;
            await _db.SaveChangesAsync();
        }

        public async Task<List<LessonDto>> GetLessonsForJournal(int journalId, int month, int journalYear)
        {
            int realYear = month >= 9 ? journalYear : journalYear + 1;
            var start = new DateOnly(realYear, month, 1);
            var end = start.AddMonths(1);
            var lessons = await _db.Lessons.AsNoTracking()
                .Where(l => l.JournalId == journalId && l.LessonDate >= start && l.LessonDate < end && l.IsDeleted == false)
                .OrderBy(l  => l.LessonDate)
                .Select(l => new LessonDto
                {
                    Id = l.Id,
                    JournalId = l.JournalId,
                    LessonDate = l.LessonDate,
                    Theme = l.Theme,
                    Homework = l.Homework,
                }).ToListAsync();
            return lessons;
        }

        public async Task UpdateLessonDetailsAsync(int lessonId, LessonDetailsUpdateDto detailsDto)
        {
            try
            {
                var lesson = await _db.Lessons.FindAsync(lessonId);
                if (lesson is null)
                {
                    throw new EntityNotFoundException($"Урок с Id:{lessonId} не найден");
                }
                lesson.Homework = detailsDto.Homework;
                lesson.Theme = detailsDto.Theme;
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            {
                throw new EntityUpdateException("An error has occurred while updating a Lesson entity!", ex);
            }           
        }

        private bool IsLessonDateValidToJournalYear(int journalYear, DateOnly lessonDate)
        {
            var yearStart = new DateOnly(journalYear, 9, 1);
            var yearEnd = new DateOnly(journalYear + 1, 5, 31);
            return lessonDate >= yearStart && lessonDate <= yearEnd;
        }
    }
}

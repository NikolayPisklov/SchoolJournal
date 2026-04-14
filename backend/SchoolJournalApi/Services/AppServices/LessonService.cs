using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dtos.Lesson;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.AppServices.Interfaces;
using SchoolJournalApi.Services.DbServices.Interfaces;
using System.Data.Common;

namespace SchoolJournalApi.Services.AppServices
{
    public class LessonService : ILessonService
    {
        private readonly ILessonDbService _lessonDbService;
        private readonly IJournalDbService _journalDbService;
        private readonly IContextService _contextService;


        public LessonService(ILessonDbService lessonDbService, IJournalDbService journalDbService, 
            IContextService contextService)
        {
            _lessonDbService = lessonDbService;
            _journalDbService = journalDbService;
            _contextService = contextService;
        }

        public async Task AddLessonAsync(AddLessonDto lessonDto)
        {
            var journal = await _journalDbService.FindJournalAsync(lessonDto.JournalId);
            if (journal == null) 
            {
                throw new EntityNotFoundException("Journal is not found!");
            }
            ValidateLessonDate(lessonDto.LessonDate, journal.Year);
            var newLesson = new Lesson
            {
                JournalId = lessonDto.JournalId,
                LessonDate = (DateOnly)lessonDto.LessonDate!
            };
            _lessonDbService.AddLesson(newLesson);
            await _contextService.SaveChangesAsync();
        }

        public async Task DeleteLessonAsync(int lessonId)
        {
            var lesson = await _lessonDbService.FindLessonAsync(lessonId);
            if(lesson is null)
            {
                throw new EntityNotFoundException($"Lesson with Id: {lessonId} is not found!");
            }
            if (lesson.LessonDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                throw new BusinessLogicException("Cannot delete lesson after it is being teached");
            }
            _lessonDbService.DeleteLesson(lesson);
            await _contextService.SaveChangesAsync();
        }

        public async Task<List<LessonDto>> GetLessonsForJournalAsync(int journalId, int month, int journalYear)
        {
            try
            {
                int realYear = month >= 9 ? journalYear : journalYear + 1;
                var lessonsQuery = _lessonDbService.GetLessonsForJournal(journalId, month, realYear);
                return await lessonsQuery.Select(l => new LessonDto
                {
                    Id = l.Id,
                    JournalId = l.JournalId,
                    LessonDate = l.LessonDate,
                    Theme = l.Theme,
                    Homework = l.Homework,
                }).ToListAsync();
            }
            catch (DbException ex) 
            {
                throw new EfDbException("Error while reading data!", ex);
            }            
        }

        public async Task UpdateLessonDetailsAsync(int lessonId, LessonDetailsUpdateDto detailsDto)
        {
            var lesson = await _lessonDbService.FindLessonAsync(lessonId);
            if (lesson is null)
            {
                throw new EntityNotFoundException($"Lesson with Id: {lessonId} is not found!");
            }
            if (lesson.LessonDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                throw new BusinessLogicException("Cannot update lesson after it is being teached");
            }
            lesson.Homework = detailsDto.Homework;
            lesson.Theme = detailsDto.Theme;
            await _contextService.SaveChangesAsync();
        }

        private bool IsLessonDateValidToJournalYear(int journalYear, DateOnly lessonDate)
        {
            var yearStart = new DateOnly(journalYear, 9, 1);
            var yearEnd = new DateOnly(journalYear + 1, 5, 31);
            return lessonDate >= yearStart && lessonDate <= yearEnd;
        }
        private void ValidateLessonDate(DateOnly? lessonDate, int journalYear) 
        {
            if (lessonDate is null
                || !IsLessonDateValidToJournalYear(journalYear, (DateOnly)lessonDate))
            {
                throw new BusinessLogicException("Lesson date is out of range of journal's academic year!");
            }
            if (lessonDate.Value.DayOfWeek == DayOfWeek.Sunday
                || lessonDate.Value.DayOfWeek == DayOfWeek.Saturday)
            {
                throw new BusinessLogicException("Lessons can't be teached on weekends!");
            }
        }
    }
}

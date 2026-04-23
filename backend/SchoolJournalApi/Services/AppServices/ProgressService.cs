using EntityFramework.Exceptions.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dtos;
using SchoolJournalApi.Dtos.Progress;
using SchoolJournalApi.Enums;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.AppServices.Interfaces;
using SchoolJournalApi.Services.DbServices.Interfaces;
using System.Data.Common;

namespace SchoolJournalApi.Services.AppServices
{
    public class ProgressService : IProgressService
    {
        private readonly IProgressDbService _progressDbService;
        private readonly IContextService _contextService;
        private readonly ILessonDbService _lessonDbService;

        public ProgressService(IProgressDbService progressDbService, IContextService contextService,
            ILessonDbService lessonDbService)
        {
            _progressDbService = progressDbService;
            _contextService = contextService;
            _lessonDbService = lessonDbService;
        }

        public async Task AddProgressAsync(AddProgressDto dto)
        {
            try
            {
                await ValidateProgressAsync(dto.AttendanceId, dto.MarkId, dto.LessonId);
                var newProgress = new Progress
                {
                    UserId = dto.UserId,
                    LessonId = dto.LessonId,
                    MarkId = dto.MarkId,
                    AttendanceId = (int)dto.AttendanceId!,
                    ProgressUpdateDate = dto.ProgressUpdateDate
                };
                _progressDbService.AddProgress(newProgress);
                await _contextService.SaveChangesAsync();
            }
            catch (ReferenceConstraintException ex)
            {
                throw new EntityAddingException("An error has occured while adding Progress entity to DB.", ex);
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occurred while connecting to DB!", ex);
            }            
        }

        public Task<List<AttendanceDto>> GetAllAttendancesAsync()
        {
            try
            {
                var attendanceQuery = _progressDbService.GetAttendances();
                return attendanceQuery.Select(m => new AttendanceDto
                {
                    Id = m.Id,
                    Value = m.Value
                }).ToListAsync();
            }
            catch(SqlException ex) 
            {
                throw new EfDbException("An error has occurred while reading data from DB!", ex);
            }
        }
        public Task<List<MarkDto>> GetAllMarksAsync()
        {
            try
            {
                var marksQuery = _progressDbService.GetMarks();
                return marksQuery.Select(m => new MarkDto
                {
                    Id = m.Id,
                    Value = m.Value
                }).ToListAsync();
            }
            catch(SqlException ex) 
            {
                throw new EfDbException("An error has occured while reading data from DB!", ex);
            }
        }
        public async Task<ProgressDetailsDto> GetProgressDetailsAsync(int progressId)
        {
            try
            {
                var progress = await _progressDbService.FindProgressAsync(progressId);
                if (progress is null)
                {
                    throw new EntityNotFoundException($"Progress with Id: {progressId} is not found!");
                }
                return new ProgressDetailsDto
                {
                    Id = progress.Id,
                    UserId = progress.UserId,
                    LessonId = progress.LessonId,
                    MarkId = progress.MarkId,
                    AttendanceId = progress.AttendanceId,
                    ProgressUpdateDate = progress.ProgressUpdateDate,
                    LessonDate = progress.Lesson.LessonDate,
                    ProgressChangeHistory = await GetProgressHistoryAsync(progress.UserId, progress.LessonId)
                };
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occurred while reading data from DB!", ex);
            }           
        }
        private async Task<List<JournalProgressDto>> GetProgressHistoryAsync(int userId, int lessonId) 
        {
            try
            {
                return await _progressDbService
                    .GetProgressHistory(userId, lessonId).Select(p => new JournalProgressDto
                    {
                        Id = p.Id,
                        UserId = p.UserId,
                        LessonId = p.LessonId,
                        MarkValue = p.Mark == null ? null : p.Mark.Value,
                        AttendanceValue = p.Attendance.Value,
                        ProgressUpdateTime = p.ProgressUpdateDate
                    }).ToListAsync();
            }
            catch (SqlException ex) 
            {
                throw new EfDbException("An error has occured while reading data from DB!", ex);
            }            
        }
        public async Task<List<JournalProgressDto>> GetProgressesForJournalAsync(int journalId)
        {
            var progressesQuery = _progressDbService.GetProgressesForJournal(journalId);
            try
            {
                return await progressesQuery.Select(p => new JournalProgressDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    LessonId = p.LessonId,
                    MarkValue = p.Mark == null ? null : p.Mark.Value,
                    AttendanceValue = p.Attendance!.Value,
                    ProgressUpdateTime = p.ProgressUpdateDate
                }).ToListAsync();
            }
            catch (SqlException ex) 
            {
                throw new EfDbException("An error has occured while reading data from DB!", ex);
            }
        }

        public async Task<StudentStaticticDto> GetStudentStatisticAsync(int studentId, int journalId)
        {
            try
            {
                var progressesQuery = _progressDbService.GetProgressesForStudentStatistic(studentId, journalId);
                var data = await progressesQuery.Select(p => new {
                    Date = p.Lesson.LessonDate,
                    Mark = (double)p.Mark!.Value
                }).ToListAsync();
                var dates = data.Select(x => x.Date).ToList();
                var factMarks = data.Select(x => x.Mark).ToList();
                return CreateStudentStatisticDto(factMarks, dates);
            }
            catch (SqlException ex) 
            {
                throw new EfDbException("An error has occured while reading data from DB!", ex);
            }            
        }
        public async Task UpdateProgressAsync(ProgressDto dto)
        {
            var progress = await _progressDbService.FindProgressAsync(dto.Id);
            if (progress is null) 
            {
                throw new EntityNotFoundException($"Progress with Id: {dto.Id} is not found!");
            }
            await ValidateProgressAsync(dto.AttendanceId, dto.MarkId, dto.LessonId);
            if(IsProgressDtoEqualToProgress(progress, dto))
            {
                return;
            }
            progress.IsUpdated = true;
            var newProgress = new Progress();
            newProgress.UserId = dto.UserId;
            newProgress.LessonId = dto.LessonId;
            newProgress.MarkId = dto.MarkId;
            newProgress.AttendanceId = (int)dto.AttendanceId!;
            newProgress.ProgressUpdateDate = dto.ProgressUpdateDate;
            _progressDbService.AddProgress(newProgress);
            await _contextService.SaveChangesAsync();
        }


        private StudentStaticticDto CreateStudentStatisticDto(List<double> factMarks, List<DateOnly> dates) 
        {
            List<double> avgMarks = new List<double>();
            double sum = 0;
            for (int i = 0; i < factMarks.Count; i++)
            {
                sum += factMarks[i];
                double avg = sum / (i + 1);
                avgMarks.Add(avg);
            }
            return new StudentStaticticDto()
            {
                AvgMarks = avgMarks,
                DateLabels = dates,
                FactMarks = factMarks
            };
        }
        private bool IsProgressDtoEqualToProgress(Progress progress, ProgressDto dto)
        {
            if (progress.UserId == dto.UserId && progress.LessonId == dto.LessonId &&
                progress.MarkId == dto.MarkId && progress.AttendanceId == (int)dto.AttendanceId! && progress.Id == dto.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async Task ValidateProgressAsync(int? attendanceId, int? markId, int lessonId)
        {
            try
            {
                if ((attendanceId == (int)Attendances.Absent || attendanceId == (int)Attendances.AbsentWithReason)
                    && markId is not null)
                {
                    throw new BusinessLogicException("Student can't be absent and recieve a mark!");
                }
                var lesson = await _lessonDbService.FindLessonAsync(lessonId);
                if (lesson is null)
                {
                    throw new EntityNotFoundException("Lesson fow progress is not found!");
                }
                var dateNow = DateOnly.Parse(DateTime.Now.ToString());
                if (lesson.LessonDate > dateNow)
                {
                    throw new BusinessLogicException("Can't edit progress before a lesson has been taught.");
                }
                if (lesson.LessonDate.AddDays(30) < dateNow)
                {
                    throw new BusinessLogicException("Can't edit progress after 30 days since lesson has been taught.");
                }
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occured while reading data from DB!", ex);
            }
        }
    }
}

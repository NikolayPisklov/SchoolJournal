using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dtos;
using SchoolJournalApi.Dtos.Progress;
using SchoolJournalApi.Enums;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.DbServices.Interfaces;
using System.Data.Common;

namespace SchoolJournalApi.Services.DbServices
{
    public class ProgressDbService : DbService<JournalProgressDto>, IProgressDbService
    {
        public ProgressDbService(SchoolJournalDbContext db): base(db) { }

        public async Task AddProgressAsync(AddProgressDto dto)
        {
            try
            {
                if((dto.AttendanceId == (int)Attendances.Absent || dto.AttendanceId == (int)Attendances.AbsentWithReason)
                    && dto.MarkId is not null)
                {
                    throw new EntityHasBusinessLogicConflictException("Student can't be absent and recieve a mark!");
                }
                Progress progress = new Progress();
                progress.UserId = dto.UserId;
                progress.LessonId = dto.LessonId;
                progress.MarkId = dto.MarkId;
                progress.AttendanceId = (int)dto.AttendanceId!;
                progress.ProgressUpdateDate = dto.ProgressUpdateDate;
                await _db.AddAsync(progress);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            {
                throw new EntityAddingException("An error has occured while adding Progress entity!", ex);
            }            
        }
        public async Task UpdateProgressAsync(ProgressDto dto)
        {
            try
            {
                if ((dto.AttendanceId == (int)Attendances.Absent || dto.AttendanceId == (int)Attendances.AbsentWithReason)
                    && dto.MarkId is not null)
                {
                    throw new EntityHasBusinessLogicConflictException("Student can't be absent and recieve a mark!");
                }
                var progress = await _db.Progresses.FindAsync(dto.Id);
                if(progress is null)
                {
                    throw new EntityNotFoundException("Progress");
                }
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
                _db.Add(newProgress);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            {
                throw new EntityUpdateException("An error has occured while updating Progress entity!", ex);
            }
        }
        public async Task<List<AttendanceDto>> GetAllAttendancesAsync()
        {
            var attendances = await _db.Attendances
                .Select(a => new AttendanceDto
                {
                    Id = a.Id,
                    Value = a.Value
                }).ToListAsync();
            return attendances;
        }

        public async Task<List<MarkDto>> GetAllMarksAsync()
        {
            var marks = await _db.Marks
                .Select(m => new MarkDto 
                { 
                    Id = m.Id, 
                    Value = m.Value 
                }).ToListAsync();
            return marks;
        }

        public async Task<List<JournalProgressDto>> GetProgressesForJournalAsync(int journalId)
        {
            var progresses = await _db.Progresses.AsNoTracking()
                .Where(p => p.Lesson!.JournalId == journalId && p.IsUpdated == false)
                .Select(p => new JournalProgressDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    LessonId = p.LessonId,
                    MarkValue = p.Mark == null ? null : p.Mark.Value,
                    AttendanceValue = p.Attendance!.Value,
                    ProgressUpdateTime = p.ProgressUpdateDate
                }).ToListAsync();
            return progresses;
        }

        public async Task<ProgressDetailsDto> GetProgressDetailsAsync(int progressId)
        {
            var progress = await _db.Progresses.FindAsync(progressId);
            if(progress is null) 
            {
                throw new EntityNotFoundException("Progress");
            }
            var dto = new ProgressDetailsDto();
            dto.Id = progress.Id;
            dto.UserId = progress.UserId;
            dto.LessonId = progress.LessonId;
            dto.MarkId = progress.MarkId;
            dto.AttendanceId = progress.AttendanceId;
            dto.ProgressUpdateDate = progress.ProgressUpdateDate;
            dto.ProgressChangeHistory = await _db.Progresses.Where(p => p.UserId == dto.UserId && p.LessonId == dto.LessonId
                && p.IsUpdated == true).Select(p => new JournalProgressDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    LessonId = p.LessonId,
                    MarkValue = p.Mark == null ? null : p.Mark.Value,
                    AttendanceValue = p.Attendance.Value,
                    ProgressUpdateTime = p.ProgressUpdateDate
                }).ToListAsync();
            return dto;
        }

        public async Task<StudentStaticticDto> GetStudentStatisticAsync(int studentId, int journalId)
        {
            try
            {
                StudentStaticticDto result = new StudentStaticticDto();
                var data = await _db.Progresses
                    .Where(p => p.UserId == studentId && p.Lesson.JournalId == journalId && p.MarkId != null && p.IsUpdated == false)
                    .OrderBy(p => p.Lesson.LessonDate)
                    .Select(p => new {
                        Date = p.Lesson.LessonDate,
                        Mark = (double)p.Mark!.Value
                    })
                    .ToListAsync();
                var dates = data.Select(x => x.Date).ToList();
                var factMarks = data.Select(x => x.Mark).ToList();
                List<double> avgMarks = new List<double>();
                double sum = 0;
                //Optimize?
                foreach (var mark in factMarks)
                {
                    if (sum == 0)
                    {
                        avgMarks.Add(mark);
                        sum += mark;
                    }
                    else
                    {
                        sum += mark;
                        double avg = sum / (avgMarks.Count + 1);
                        avgMarks.Add(avg);
                    }
                }
                result.AvgMarks = avgMarks;
                result.DateLabels = dates;
                result.FactMarks = factMarks;
                return result;
            }
            catch (DbException ex)
            {
                throw new EfDbException("Failed to select data from database!", ex);
            }            
        }
        private bool IsProgressDtoEqualToProgress(Progress progress, ProgressDto dto)
        {
            if(progress.UserId == dto.UserId && progress.LessonId == dto.LessonId &&
                progress.MarkId == dto.MarkId && progress.AttendanceId == (int)dto.AttendanceId! && progress.Id == dto.Id) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

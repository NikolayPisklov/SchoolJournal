using EntityFramework.Exceptions.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dtos.Journal;
using SchoolJournalApi.Dtos.Lesson;
using SchoolJournalApi.Dtos.Progress;
using SchoolJournalApi.Dtos.User;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.AppServices.Interfaces;
using SchoolJournalApi.Services.DbServices.Interfaces;

namespace SchoolJournalApi.Services.AppServices
{
    public class JournalService : IJournalService
    {
        private readonly IJournalDbService _journalDbService;
        private readonly IContextService _contextService;
        private readonly ILessonDbService _lessonDbService;
        private readonly IProgressDbService _progressDbService;
        private readonly IStudentClassDbService _studentClassDbService;

        public JournalService(IJournalDbService journalDbService, IContextService contextService, 
            ILessonDbService lessonDbService, IProgressDbService progressDbService, 
            IStudentClassDbService studentClassDbService)
        {
            _journalDbService = journalDbService;
            _contextService = contextService;
            _lessonDbService = lessonDbService;
            _progressDbService = progressDbService;
            _studentClassDbService = studentClassDbService;
        }


        public async Task AddJournalAsync(JournalCreationDto dto)
        {
            try
            {
                int schoolYear = SchoolYearService.GetCurrentSchoolYear();
                if (await _journalDbService.IsThereSameJournal(dto.ClassId, dto.TeacherSubjectId, schoolYear))
                {
                    throw new EntityAlreadyExistsException("Entity Journal with this parameters for this year already exists!");
                }
                var newJournal = new Journal();
                newJournal.ClassId = dto.ClassId;
                newJournal.TeacherSubjectId = dto.TeacherSubjectId;
                newJournal.Year = schoolYear;
                _journalDbService.AddJournal(newJournal);
                await _contextService.SaveChangesAsync();
            }
            catch(ReferenceConstraintException ex)
            {
                throw new EntityAddingException("An error has occured while adding Journal entity to DB.", ex);
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occurred while reading data from DB!", ex);
            }            
        }
        public async Task DeleteJournalAsync(int journalId)
        {
            try
            {
                var journal = await _journalDbService.FindJournalAsync(journalId);
                if (journal is null)
                {
                    throw new EntityNotFoundException($"Journal entity with Id: {journalId} is not found!");
                }
                _journalDbService.DeleteJournal(journal);
                await _contextService.SaveChangesAsync();
            }
            catch (ReferenceConstraintException ex)
            {
                throw new EntityInUseException("An error has occured while deleting Journal entity from DB.", ex);
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occurred while reading data from DB!", ex);
            }
        }
        public async Task<JournalDetailsDto> GetJournalDetailsAsync(int journalId)
        {
            try
            {
                var journal = await _journalDbService.FindJournalAsync(journalId);
                if (journal is null)
                {
                    throw new EntityNotFoundException($"Journal entity with Id: {journalId} is not found!");
                }
                var studentsQuery = _studentClassDbService.GetStudentsForJournal(journal.ClassId);
                var lessonsQuery = _lessonDbService.GetLessonsForJournal(journalId, 9, journal.Year);
                var progressesQuery = _progressDbService.GetProgressesForJournal(journal.Id);
                return new JournalDetailsDto
                {
                    Students = await SelectUsersAsync(studentsQuery),
                    Lessons = await SelectLessonsAsync(lessonsQuery),
                    Progresses = await SelectProgressesAsync(progressesQuery),
                    JournalYear = journal.Year
                };
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occurred while reading data from DB!", ex);
            }
        }
        public async Task<JournalDetailsDto> GetJournalDetailsForStudentAsync(int journalId, int studentId)
        {
            try
            {
                var journal = await _journalDbService.FindJournalAsync(journalId);
                if (journal is null)
                {
                    throw new EntityNotFoundException($"Journal entity with Id: {journalId} is not found!");
                }
                var lessonsQuery = _lessonDbService.GetAllLessonsForJournal(journalId);
                var progressesQuery = _progressDbService.GetProgressesForStudentJournal(journalId, studentId);
                return new JournalDetailsDto
                {
                    Lessons = await SelectLessonsAsync(lessonsQuery),
                    Progresses = await SelectProgressesAsync(progressesQuery),
                    JournalYear = journal.Year
                };
            }
            catch(SqlException ex)
            {
                throw new EfDbException("An error has occurred while reading data from DB!", ex);
            }
        }
        public async Task<List<JournalInListDto>> GetJournalsForClassAsync(int classId)
        {
            var journalsQuery = _journalDbService.GetJournalsForClass(classId);
            try 
            {
                return await journalsQuery.Select(j => new JournalInListDto()
                {
                    Id = j.Id,
                    ClassId = j.ClassId,
                    TeacherSubjectId = j.TeacherSubjectId,
                    SubjectTitle = j.TeacherSubject.Subject.Title,
                    TeacherFirstName = j.TeacherSubject.Teacher.FirstName,
                    TeacherLastName = j.TeacherSubject.Teacher.LastName,
                    TeacherMiddleName = j.TeacherSubject.Teacher.MiddleName,
                    ClassTitle = j.Class.Title,
                    ClassYear = j.Class.Year,
                    JournalYear = j.Year
                }).ToListAsync();
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occurred while reading data from DB!", ex);
            }
        }
        public async Task<List<JournalInListDto>> GetJournalsForStudent(int studentId)
        {
            try
            {
                var studentClass = await _studentClassDbService.FindStudentClassAsync(studentId);
                if (studentClass is null)
                {
                    throw new EntityNotFoundException($"Class for student with Id: {studentId} is not found!");
                }
                return await GetJournalsForClassAsync(studentClass.ClassId);
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occurred while reading data from DB!", ex);
            }
        }
        public Task<List<JournalGroupDto>> GetJournalsForTeacherAsync(int teacherId)
        {
            try
            {
                var journalsQuery = _journalDbService.GetJournalsForTeacher(teacherId);
                return journalsQuery.OrderBy(j => j.Class.Year)
                   .ThenBy(j => j.Class.Title)
                   .GroupBy(j => new
                   {
                       j.ClassId,
                       j.Class.Title,
                       j.Class.Year
                   })
                   .Select(g => new JournalGroupDto
                   {
                       ClassId = g.Key.ClassId,
                       ClassTitle = g.Key.Title,
                       ClassYear = g.Key.Year,

                       JournalsOfClass = g.Select(j => new JournalInListDto
                       {
                           Id = j.Id,
                           SubjectTitle = j.TeacherSubject.Subject.Title,
                           TeacherFirstName = j.TeacherSubject.Teacher.FirstName,
                           TeacherLastName = j.TeacherSubject.Teacher.LastName,
                           TeacherMiddleName = j.TeacherSubject.Teacher.MiddleName,
                           JournalYear = j.Year
                       }).ToList()
                   }).ToListAsync();
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occurred while reading data from DB!", ex);
            }           
        }
        public async Task<JournalTitleDto> GetJournalTitleAsync(int journalId)
        {
            try
            {
                var journal = await _journalDbService.FindJournalWithIncludes(journalId);
                if (journal is null)
                {
                    throw new EntityNotFoundException($"Journal entity with Id: {journalId} is not found!");
                }
                return new JournalTitleDto
                {
                    ClassTitle = journal.Class.Title,
                    ClassYear = journal.Class.Year,
                    SubjectTitle = journal.TeacherSubject.Subject.Title
                };
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occurred while reading data from DB!", ex);
            }
        }


        private async Task<List<JournalProgressDto>> SelectProgressesAsync(IQueryable<Progress> progressesQuery)
        {
            try
            {
                return await progressesQuery.Select(p => new JournalProgressDto
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
                throw new EfDbException("An error has occurred while reading data from DB!", ex);
            }
        }
        private async Task<List<LessonDto>> SelectLessonsAsync(IQueryable<Lesson> lessonsQuery) 
        {
            try
            {
                return await lessonsQuery.Select(l => new LessonDto
                {
                    Id = l.Id,
                    JournalId = l.JournalId,
                    LessonDate = l.LessonDate,
                    Theme = l.Theme,
                    Homework = l.Homework,
                }).ToListAsync();
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occurred while reading data from DB!", ex);
            }
        }
        private async Task<List<ListedStudentDto>> SelectUsersAsync(IQueryable<StudentClass> studentsQuery) 
        {
            try
            {
                return await studentsQuery.Select(sc => new ListedStudentDto
                {
                    Id = sc.Student.Id,
                    FirstName = sc.Student.FirstName,
                    LastName = sc.Student.LastName,
                    MiddleName = sc.Student.MiddleName
                }).ToListAsync();
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occurred while reading data from DB!", ex);
            }
        }
    }
}

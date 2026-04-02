using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Models;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Dtos;
using SchoolJournalApi.Dtos.User;
using SchoolJournalApi.Dtos.Progress;
using SchoolJournalApi.Dtos.Journal;
using SchoolJournalApi.Dtos.Lesson;
using SchoolJournalApi.Enum_s;

namespace SchoolJournalApi.Services
{
    public class JournalDbService : DbService<Journal>, IJournalDbService
    {
        public JournalDbService(SchoolJournalDbContext db) : base(db)
        {
        }
        public async Task AddJournalAsync(JournalCreationDto dto)
        {
            try
            {
                var isThereSameJournal = await _db.Journals.AnyAsync(j=> j.ClassId == dto.ClassId 
                    && j.TeacherSubjectId == dto.TeacherSubjectId && j.Year == DateTime.Now.Year);
                if (isThereSameJournal) 
                {
                    throw new EntityAlreadyExistsException("Сущьность журнала стакими параметрами уже существует!");
                }
                var newJournal = new Journal();
                newJournal.ClassId = dto.ClassId;
                newJournal.TeacherSubjectId = dto.TeacherSubjectId;
                newJournal.Year = DateTime.Now.Year;
                await _db.AddAsync(newJournal);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException) 
            {
                throw new EntityAddingException("Journal", "Ошибка при добавлении журнала");
            }
        }

        public async Task DeleteJournalAsync(int journalId)
        {
            try
            {
                var journal = await _db.Journals.FindAsync(journalId);
                if (journal is null)
                {
                    throw new EntityNotFoundException("Journal");
                }
                _db.Remove(journal);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException) 
            {
                throw new EntityInUseException("Journal", journalId);
            }
        }

        public async Task<JournalDetailsDto> GetJournalDetailsAsync(int journalId)
        {
            var journalDto = new JournalDetailsDto();
            var journal = await _db.Journals.FindAsync(journalId);
            if (journal is null)
                throw new EntityNotFoundException("Journal");
            var students = await _db.StudentClasses.AsNoTracking()
                .Where(sc => sc.ClassId == journal.ClassId)
                .Select(sc => new ListedStudentDto 
                { 
                    Id = sc.Student!.Id,
                    FirstName = sc.Student.FirstName,
                    LastName = sc.Student.LastName,
                    MiddleName = sc.Student.MiddleName
                }).ToListAsync();
            var start = new DateOnly(journal.Year, 9, 1);
            var end = start.AddMonths(1);
            var lessons = await _db.Lessons.AsNoTracking()
                .Where(l => l.JournalId == journalId && l.LessonDate >= start 
                    && l.LessonDate < end && l.IsDeleted == false)
                .OrderBy(l => l.LessonDate)
                .Select(l => new LessonDto
                {
                    Id = l.Id,
                    JournalId = l.JournalId,
                    LessonDate = l.LessonDate,
                    Theme = l.Theme,
                    Homework = l.Homework,
                }).ToListAsync();
            var progresses = await _db.Progresses.AsNoTracking()
                .Where(prog => prog.Lesson.JournalId == journalId
                    && prog.IsUpdated == false)
                .Select(p => new JournalProgressDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    LessonId = p.LessonId,
                    MarkValue = p.Mark == null ? null : p.Mark.Value,
                    AttendanceValue = p.Attendance!.Value,
                    ProgressUpdateTime = p.ProgressUpdateDate
                }).ToListAsync();
            journalDto.Students = students;
            journalDto.Lessons = lessons;
            journalDto.Progresses = progresses;
            journalDto.JournalYear = journal.Year;
            return journalDto;
        }

        public async Task<JournalDetailsDto> GetJournalDetailsForStudent(int journalId, int studentId)
        {
            var journalDto = new JournalDetailsDto();
            var journal = await _db.Journals.FindAsync(journalId);
            if (journal is null)
                throw new EntityNotFoundException("Journal");
            var lessons = await _db.Lessons.AsNoTracking()
                .Where(l => l.JournalId == journalId && l.IsDeleted == false)
                .OrderBy(l => l.LessonDate)
                .Select(l => new LessonDto
                {
                    Id = l.Id,
                    JournalId = l.JournalId,
                    LessonDate = l.LessonDate,
                    Theme = l.Theme,
                    Homework = l.Homework,
                }).ToListAsync();
            var progresses = await _db.Progresses.AsNoTracking()
                .Where(prog => prog.Lesson.JournalId == journalId
                    && prog.IsUpdated == false && prog.UserId == studentId)
                .Select(p => new JournalProgressDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    LessonId = p.LessonId,
                    MarkValue = p.Mark == null ? null : p.Mark.Value,
                    AttendanceValue = p.Attendance!.Value,
                    ProgressUpdateTime = p.ProgressUpdateDate
                }).ToListAsync();            
            journalDto.Lessons = lessons;
            journalDto.Progresses = progresses;
            journalDto.JournalYear = journal.Year;
            return journalDto;
        }

        public async Task<List<JournalInListDto>> GetJournalsForClassAsync(int classId)
        {
            var journals = await _db.Journals.AsNoTracking()
                .Where(j => j.ClassId == classId)
                .Select(j => new JournalInListDto()
                {
                    Id = j.Id,
                    ClassId = j.ClassId,
                    TeacherSubjectId = j.TeacherSubjectId,
                    SubjectTitle = j.TeacherSubject!.Subject!.Title,
                    TeacherFirstName = j.TeacherSubject!.Teacher!.FirstName,
                    TeacherLastName = j.TeacherSubject.Teacher.LastName,
                    TeacherMiddleName = j.TeacherSubject.Teacher.MiddleName,
                    ClassTitle = j.Class!.Title,
                    ClassYear = j.Class.Year,
                    JournalYear = j.Year
                }).ToListAsync();
            return journals;
        }

        public async Task<List<JournalInListDto>> GetJournalsForStudent(int studentId)
        {
            var studentClass = await _db.StudentClasses.AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == studentId && s.IsActive);
            if(studentClass is null)
            {
                throw new EntityNotFoundException($"Active class for student with Id: {studentId} is not found");
            }
            var journals = await _db.Journals.AsNoTracking()
                .Where(j => j.ClassId == studentClass.ClassId && j.Year == DateTime.Now.Year)
                .Select(j => new JournalInListDto{
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
            return journals;
        }

        public async Task<List<JournalGroupDto>> GetJournalsForTeacherAsync(int teacherId)
        {
            var journals = await _db.Journals.AsNoTracking()
               .Where(j => j.TeacherSubject!.Teacher!.Id == teacherId)
               .OrderBy(j => j.Class!.Year)
               .ThenBy(j => j.Class!.Title)
               .GroupBy(j => new
               {
                   j.ClassId,
                   j.Class!.Title,
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
            return journals;
        }

        public async Task<JournalTitleDto> GetJournalTitleAsync(int journalId)
        {
            var dto = await _db.Journals.Where(j => j.Id == journalId)
                .Select(j => new JournalTitleDto 
                {
                    ClassTitle = j.Class!.Title,
                    ClassYear = j.Class.Year,
                    SubjectTitle = j.TeacherSubject.Subject.Title
                }).FirstAsync();
            return dto;
        }
    }
}

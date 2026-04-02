using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Enum_s;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services
{
    public class TeacherSubjectDbService : DbService<TeacherSubject>, ITeacherSubjectDbService
    {
        public TeacherSubjectDbService(SchoolJournalDbContext db) : base(db) { } 

        public async Task AddTeacherSubjectAsync(int userId, int subjectId)
        {
            var isUserTeacher = await _db.Users.AnyAsync(u => u.Id == userId 
                && u.StatusId == (int)UserStatuses.Teacher);
            if (!isUserTeacher) {
                throw new EntityHasStatusDiscrepancyException(userId, "User is not a teacher and can't teach a subject!");
            }
            var newTS = new TeacherSubject
            {
                SubjectId = subjectId,
                UserId = userId
            };
            _db.Add(newTS);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteTeacherSubjectAsync(int teacherSubjectId)
        {
            try
            {
                var ts = await _db.TeacherSubjects.FindAsync(teacherSubjectId);
                if (ts == null) 
                {
                    throw new EntityNotFoundException("Teacher SUbject");
                }
                _db.Remove(ts);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException) 
            {
                throw new EntityInUseException("Teacher Subject", teacherSubjectId);
            }
        }
        public async Task<List<TeacherSubjectsDto>> GetAllTeacherSubjectsAsync(int? eduLevelId)
        {
            var list = _db.TeacherSubjects
                .Include(t => t.Subject)
                .Include(t => t.Teacher).AsNoTracking();
            if (eduLevelId is not null) 
            {

                list = list.Where(ts => ts.Subject!.EducationalLevelId == eduLevelId);
            }
            return await list.Select(t => new TeacherSubjectsDto 
            {
                Id = t.Id,
                SubjectTitle = t.Subject!.Title,
                TeacherFirstName = t.Teacher!.FirstName,
                TeacherLastName = t.Teacher.LastName,
                TeacherMiddleName = t.Teacher.MiddleName
            }).ToListAsync();
        }
        public async Task<List<SubjectDto>> GetSubjectsAsync(int? educationalLevelId)
        {
            var subjects = _db.Subjects.AsNoTracking();
            if(educationalLevelId is not null) 
            {
                subjects = subjects.Where(s => s.EducationalLevelId == educationalLevelId);
            }
            return await subjects.Select(s => new SubjectDto
            {
                Id = s.Id,
                Title = s.Title,
                EducationalLevelId = s.EducationalLevelId
            }).ToListAsync();
        }

        public async Task<List<TeacherSubjectsDto>> GetSubjectsForTeacherAsync(int userId)
        {
            var tsForTeacher = _db.TeacherSubjects.Where(s => s.UserId == userId).AsNoTracking();
            return await tsForTeacher.Select(t => new TeacherSubjectsDto
            {
                Id = t.Id,
                SubjectEducationalLevelId = t.Subject!.EducationalLevelId,
                SubjectId = t.SubjectId,
                SubjectTitle = t.Subject!.Title,
                TeacherId = t.UserId
            }).ToListAsync();
        }
    }
}

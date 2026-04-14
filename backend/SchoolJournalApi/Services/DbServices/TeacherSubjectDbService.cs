using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Enum_s;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.DbServices.Interfaces;
using System.Data.Common;

namespace SchoolJournalApi.Services.DbServices
{
    public class TeacherSubjectDbService : DbService, ITeacherSubjectDbService
    {
        public TeacherSubjectDbService(SchoolJournalDbContext db) : base(db) { } 

        public async Task<bool> IsTeacherAsync(int userId) 
        {
            try
            {
                return await _db.Users.AnyAsync(u => u.StatusId == (int)UserStatuses.Teacher && u.Id == userId);
            }
            catch (DbException ex)
            {
                throw new EfDbException("An error has occur while reading data from DB!", ex);
            }   
        }
        public async Task<TeacherSubject?> FindTeacherSubjectAsync(int teacherSubjectId) 
        {
            try
            {
                return await _db.TeacherSubjects.FindAsync(teacherSubjectId);
            }
            catch (DbException ex) 
            {
                throw new EfDbException("An error has occur while reading data from DB!", ex);
            }
        }
        public void DeleteTeacherSubject(TeacherSubject teacherSubject) 
        {
             _db.Remove(teacherSubject);
        }
        public void AddTeacherSubject(TeacherSubject teacherSubject) 
        {
            _db.Add(teacherSubject);        
        }
        public IQueryable<TeacherSubject> GetTeacherSubjectsForTeacher(int userId) 
        {
            return _db.TeacherSubjects.Where(x => x.UserId == userId).AsNoTracking();
        }
        public IQueryable<Subject> GetSubjects(int? educationalLevelId) 
        {
            var subjects = _db.Subjects.AsNoTracking();
            if (educationalLevelId is not null)
            {
                subjects = subjects.Where(s => s.EducationalLevelId == educationalLevelId);
            }
            return subjects;
        }
        public IQueryable<TeacherSubject> GetTeacherSubjects(int? educationalLevelId) 
        {
            var teacherSubjects = _db.TeacherSubjects.AsNoTracking();
            if (educationalLevelId is not null)
            {
                teacherSubjects = teacherSubjects.Where(ts => ts.Subject!.EducationalLevelId == educationalLevelId);
            }
            return teacherSubjects;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Enum_s;
using SchoolJournalApi.Models;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Services.DbServices.Interfaces;
using System.Data.Common;

namespace SchoolJournalApi.Services.DbServices
{
    public class StudentClassDbService : DbService, IStudentClassDbService
    {
        public StudentClassDbService(SchoolJournalDbContext db) : base(db) { }

        public async Task<bool> IsStudent(int userId) 
        {
            return await _db.Users.AnyAsync(u => u.Id == userId && u.StatusId == (int)UserStatuses.Student);
        }
        public IQueryable<StudentClass> GetStudentClassForClass(int classId) 
        {
            try
            {
                return _db.StudentClasses.AsNoTracking().Where(s => s.IsActive && s.ClassId == classId);
            }
            catch(DbException ex)
            {
                throw new EfDbException("An error has occured while reading data from DB!", ex);
            }
        }
        public async Task<StudentClass?> FindStudentClassAsync(int studentId)
        {
            try
            {
                return await _db.StudentClasses.FirstOrDefaultAsync(c => c.UserId == studentId && c.IsActive);
            }
            catch (DbException ex) 
            {
                throw new EfDbException("An error has occured while reading data from DB!", ex);
            }
        }
        public void AddStudentClass(StudentClass studentClass) 
        {
            _db.Add(studentClass);
        }
    }
}

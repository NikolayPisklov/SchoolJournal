using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Enum_s;
using SchoolJournalApi.Models;
using SchoolJournalApi.Exceptions;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using SchoolJournalApi.Dtos.User;
using SchoolJournalApi.Services.DbServices.Interfaces;

namespace SchoolJournalApi.Services.DbServices
{
    public class StudentClassService : DbService<StudentClass>, IStudentClassService
    {
        public StudentClassService(SchoolJournalDbContext db) : base(db) { }

        public async Task DeleteStudentClassAsync(int studentClassId)
        {
            try 
            {
                var studentClass = await _db.StudentClasses.FindAsync(studentClassId);
                if (studentClass is null)
                {
                    throw new EntityNotFoundException("Student-Class");
                }
                _db.Remove(studentClass);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            {
                throw new EntityInUseException($"Entity Student-Class with Id: {studentClassId} is in use and can't be deleted!", ex);
            }
        }
        public async Task<List<ListedStudentDto>> GetStudentsInClassAsync(int classId)
        {
            var students = await _db.StudentClasses.Where(s => s.IsActive &&  s.ClassId == classId)
                .Select(s => s.Student).Select(s => new ListedStudentDto 
                {
                    Id = s!.Id,
                    FirstName = s.FirstName, 
                    LastName = s.LastName, 
                    MiddleName = s.MiddleName
                }).ToListAsync();
            return students;
        }

        public async Task TransferStudentToAnotherCLassAsync(int studentId, int newClassId, int? oldClassId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try 
            {
                if(oldClassId is null) 
                { 
                    await AddStudentToClassAsync(newClassId, studentId, transaction);
                    return;
                }
                var studentClass = await _db.StudentClasses.FirstOrDefaultAsync(c => c.ClassId == oldClassId 
                    && c.UserId == studentId && c.IsActive);
                if (studentClass is null)
                {
                    await transaction.RollbackAsync();
                    throw new EntityNotFoundException("Student-Class");
                }
                if (studentClass.ClassId == newClassId) 
                {
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException("Student already in this class");
                }
                var newStudentClass = new StudentClass()
                {
                    UserId = studentClass.UserId,
                    ClassId = newClassId,
                    IsActive = true
                };
                studentClass.IsActive = false;
                await _db.AddAsync(newStudentClass);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch(DbUpdateException ex) 
            {
                await transaction.RollbackAsync();
                throw new EntityUpdateException("An error has occurred while updating a StudentClass entity!", ex);
            }
        }


        private async Task AddStudentToClassAsync(int classId, int userId, IDbContextTransaction transaction)
        {
            try
            {
                if (!await _db.Users.AnyAsync(u => u.Id == userId && u.StatusId == (int)UserStatuses.Student))
                {
                    await transaction.RollbackAsync();
                    throw new EntityHasStatusDiscrepancyException("Provided user is not a student and can't be assigned to a class.");
                }
                if (await _db.StudentClasses.AnyAsync(s => s.UserId == userId && s.IsActive))
                {
                    await transaction.RollbackAsync();
                    throw new BusinessLogicException("Student already associated with another class.");
                }
                var newStudentClass = new StudentClass();
                newStudentClass.ClassId = classId;
                newStudentClass.UserId = userId;
                newStudentClass.IsActive = true;
                _db.Add(newStudentClass);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                throw new EntityAddingException("An error has occurred while updating a StudentClass entity!", ex);
            }
        }
    }
}

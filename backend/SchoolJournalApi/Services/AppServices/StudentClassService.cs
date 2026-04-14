using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dtos.User;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.AppServices.Interfaces;
using SchoolJournalApi.Services.DbServices.Interfaces;
using System.Data.Common;

namespace SchoolJournalApi.Services.AppServices
{
    public class StudentClassService : IStudentClassService
    {
        private readonly IStudentClassDbService _studentClassDbService;
        private readonly IContextService _contextService;


        public StudentClassService(IStudentClassDbService studentClassDbService, IContextService contextService)
        {
            _studentClassDbService = studentClassDbService;
            _contextService = contextService;
        }

        public async Task<List<ListedStudentDto>> GetStudentsInClassAsync(int classId)
        {
            try
            {
                var studentClasses = _studentClassDbService.GetStudentClassForClass(classId);
                return await studentClasses.Select(s => s.Student)
                    .Select(s => new ListedStudentDto
                    {
                        Id = s!.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        MiddleName = s.MiddleName
                    }).ToListAsync();
            }
            catch (DbException ex)
            {
                throw new EfDbException("An error has occured while reading data from DB!", ex);
            }

        }
        public async Task TransferStudentToAnotherCLassAsync(int studentId, int newClassId)
        {
            if (!await _studentClassDbService.IsStudent(studentId)) 
            {
                throw new BusinessLogicException("Selected user is not a student!");
            }
            using var transaction = await _contextService.BeginTransactionAsync();
            var studentClass = await _studentClassDbService.FindStudentClassAsync(studentId);
            if (studentClass is null)
            {
                TransferStudentWithoutCurrentClassAsync(studentId, newClassId);
                await transaction.CommitAsync();
                return;
            }
            if (studentClass.ClassId == newClassId)
            {
                await transaction.RollbackAsync();
                throw new BusinessLogicException("Student can't be transfered to the same class!");
            }
            var newStudentClass = CreateStudentClassEntity(studentId, newClassId);
            studentClass.IsActive = false;
            _studentClassDbService.AddStudentClass(newStudentClass);
            await transaction.CommitAsync();
        }

        private StudentClass CreateStudentClassEntity(int studentId, int newClassId) 
        {
            return new StudentClass()
            {
                UserId = studentId,
                ClassId = newClassId,
                IsActive = true
            };
        }
        private void TransferStudentWithoutCurrentClassAsync(int newClassId, int studentId) 
        {
            var newStudentClass = CreateStudentClassEntity(studentId, newClassId);
            _studentClassDbService.AddStudentClass(newStudentClass);
        }
    }
}

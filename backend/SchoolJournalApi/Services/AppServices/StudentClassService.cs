using EntityFramework.Exceptions.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dtos.User;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.AppServices.Interfaces;
using SchoolJournalApi.Services.DbServices.Interfaces;

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
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            }

        }
        public async Task TransferStudentToAnotherCLassAsync(int studentId, int newClassId)
        {
            try
            {
                if (!await _studentClassDbService.IsStudent(studentId))
                {
                    throw new BusinessLogicException("Selected user is not a student!");
                }
                var studentClass = await _studentClassDbService.FindStudentClassAsync(studentId);
                if (studentClass is null)
                {
                    TransferStudentWithoutCurrentClass(newClassId, studentId);
                    await _contextService.SaveChangesAsync();
                    return;
                }
                var newStudentClass = CreateStudentClassEntity(studentId, newClassId);
                studentClass.IsActive = false;
                _studentClassDbService.AddStudentClass(newStudentClass);
                await _contextService.SaveChangesAsync();
            }
            catch(ReferenceConstraintException ex)
            {
                throw new EfDbException("An error has occured while transfering student from class to class.", ex);
            }
            catch(UniqueConstraintException)
            {
                throw new BusinessLogicException("Student can't be transfered to the same class!");
            }
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
        private void TransferStudentWithoutCurrentClass(int newClassId, int studentId) 
        {
            var newStudentClass = CreateStudentClassEntity(studentId, newClassId);
            _studentClassDbService.AddStudentClass(newStudentClass);
        }
    }
}

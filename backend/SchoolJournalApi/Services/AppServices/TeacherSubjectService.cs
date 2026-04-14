using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.AppServices.Interfaces;
using SchoolJournalApi.Services.DbServices.Interfaces;
using System.Data.Common;

namespace SchoolJournalApi.Services.AppServices
{
    public class TeacherSubjectService : ITeacherSubjectService
    {
        private readonly ITeacherSubjectDbService _teacherSubjectDbService;
        private readonly IContextService _contextService;


        public TeacherSubjectService(ITeacherSubjectDbService teacherSubjectDbService, IContextService contextService) 
        {
            _contextService = contextService;
            _teacherSubjectDbService = teacherSubjectDbService;
        }

        public async Task AddTeacherSubjectAsync(int userId, int subjectId)
        {
            if (!await _teacherSubjectDbService.IsTeacherAsync(userId)) 
            {
                throw new BusinessLogicException("User is not a teacher and con't teach a subject!");
            }
            var newTeacherSubject = new TeacherSubject 
            {
                SubjectId = subjectId, 
                UserId = userId
            };
            _teacherSubjectDbService.AddTeacherSubject(newTeacherSubject);
            await _contextService.SaveChangesAsync();
        }
        public async Task DeleteTeacherSubjectAsync(int teacherSubjectId)
        {
            var teacherSubject = await _teacherSubjectDbService.FindTeacherSubjectAsync(teacherSubjectId);
            if (teacherSubject is null) 
            {
                throw new EntityNotFoundException($"Entity TeacherSubject with Id: {teacherSubjectId} can't be found!");
            }
            _teacherSubjectDbService.DeleteTeacherSubject(teacherSubject);
            await _contextService.SaveChangesAsync();
        }

        public async Task<List<SubjectDto>> GetSubjectsAsync(int? educationalLevelId)
        {
            try 
            {
                var subjects = _teacherSubjectDbService.GetSubjects(educationalLevelId);
                return await subjects.Select(s => new SubjectDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    EducationalLevelId = s.EducationalLevelId
                }).ToListAsync();
            }
            catch (DbException ex)
            {
                throw new EfDbException("An error has occur while reading data from DB!", ex);
            }            
        }

        public async Task<List<TeacherSubjectsDto>> GetSubjectsForTeacherAsync(int userId)
        {
            try
            {
                var teachersSubjects = _teacherSubjectDbService.GetTeacherSubjectsForTeacher(userId);
                return await teachersSubjects.Select(t => new TeacherSubjectsDto
                {
                    Id = t.Id,
                    SubjectEducationalLevelId = t.Subject!.EducationalLevelId,
                    SubjectId = t.SubjectId,
                    SubjectTitle = t.Subject!.Title,
                    TeacherId = t.UserId
                }).ToListAsync();
            }
            catch(DbException ex) 
            {
                throw new EfDbException("An error has occur while reading data from DB!", ex);
            }            
        }

        public async Task<List<TeacherSubjectsDto>> GetTeacherSubjectsAsync(int? eduLevelId)
        {
            try 
            {
                var teacherSubjects = _teacherSubjectDbService.GetTeacherSubjects(eduLevelId);
                return await teacherSubjects.Select(t => new TeacherSubjectsDto
                {
                    Id = t.Id,
                    SubjectTitle = t.Subject!.Title,
                    TeacherFirstName = t.Teacher!.FirstName,
                    TeacherLastName = t.Teacher.LastName,
                    TeacherMiddleName = t.Teacher.MiddleName
                }).ToListAsync();
            }
            catch(DbException ex) 
            {
                throw new EfDbException("An error has occur while reading data from DB!", ex);
            }
        }
    }
}

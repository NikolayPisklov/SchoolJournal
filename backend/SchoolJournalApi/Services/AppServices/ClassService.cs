using EntityFramework.Exceptions.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Enum_s;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.AppServices.Interfaces;
using SchoolJournalApi.Services.DbServices.Interfaces;

namespace SchoolJournalApi.Services.AppServices
{
    public class ClassService : IClassService
    {
        private readonly IClassDbService _classDbService;
        private readonly IContextService _contextService;


        public ClassService(IClassDbService classDbService, IContextService contextService) 
        {
            _contextService = contextService;
            _classDbService = classDbService;
        }


        public async Task AddClassAsync(ClassCreationDto classDto)
        {
            try
            {
                int number = DateTime.Now.Year - (int)classDto.Year!;
                int eduLevel = GetEducationalLevel(number);
                Class newClass = new Class
                {
                    Title = classDto.Title,
                    Year = (int)classDto.Year,
                    EducationalLevelId = eduLevel
                };
                _classDbService.AddClass(newClass);
                await _contextService.SaveChangesAsync();
            }
            catch (ReferenceConstraintException ex)
            {
                throw new EntityAddingException("An error has occured while adding Class entity.", ex);
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            }
        }
        public async Task UpdateClassAsync(ClassDto classDto)
        {
            try
            {
                var classEntity = await _classDbService.FindClassAsync(classDto.Id);
                if (classEntity is null)
                {
                    throw new EntityNotFoundException("Entity Class can't be found!");
                }
                int number = DateTime.Now.Year - (int)classDto.Year!;
                int eduLevel = GetEducationalLevel(number);

                classEntity.Title = classDto.Title;
                classEntity.Year = (int)classDto.Year!;
                classDto.EducationalLevelId = eduLevel;
                await _contextService.SaveChangesAsync();
            }
            catch (ReferenceConstraintException ex)
            {
                throw new EntityInUseException("An error has occured while updating Class entity.", ex);
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            }            
        }
        public async Task DeleteClassAsync(int classId)
        {
            try
            {
                var classEntity = await _classDbService.FindClassAsync(classId);
                if (classEntity is null)
                {
                    throw new EntityNotFoundException("Entity Class can't be found!");
                }
                _classDbService.DeleteClass(classEntity);
                await _contextService.SaveChangesAsync();
            }
            catch (ReferenceConstraintException ex)
            {
                throw new EntityInUseException("An error has occured while deleting Class entity.", ex);
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            }
            
        }
        public async Task<ClassDto> GetClassDtoAsync(int classId)
        {            
            try
            {
                var classEntity = await _classDbService.FindClassAsync(classId);
                if (classEntity is null)
                {
                    throw new EntityNotFoundException("Entity Class can't be found!");
                }
                ClassDto classDto = new ClassDto();
                MapClassToClassDto(classEntity, classDto);
                return classDto;
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            }
        }
        public async Task<PagingResultDto<ClassDto>> GetClassesOnPageAsync(int pageSize, int? educationalLevelId, int page = 1)
        {
            try
            {
                var classes = _classDbService.GetClasses();
                classes = FilterClasses(classes, educationalLevelId);
                int numberOfPages = await CalculateNumberOfPagesAsync(classes, pageSize);
                List<ClassDto> classesDto = await classes.OrderBy(c => c.Year).Skip((page - 1) * pageSize).Take(pageSize)
                    .Select(c => new ClassDto
                    {
                        Id = c.Id,
                        EducationalLevelId = c.EducationalLevelId,
                        Title = c.Title,
                        Year = c.Year
                    }).ToListAsync();
                return new PagingResultDto<ClassDto> { Items = classesDto, NumberOfPages = numberOfPages };
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            }           
        }
        public async Task<List<ClassDto>> GetAllClassesAsync() 
        {
            try
            {
                var classes = _classDbService.GetClasses();
                return await classes.OrderByDescending(c => c.Year)
                    .ThenBy(c => c.Title).Select(c => new ClassDto
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Year = c.Year,
                        EducationalLevelId = c.EducationalLevelId
                    }).ToListAsync();
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            }
        }

        private int GetEducationalLevel(int classYear) 
        {
            if (classYear < 5)
            {
                return (int)EducationalLevels.Junior;
            }
            else if (classYear >= 5 && classYear < 10)
            {
                return (int)EducationalLevels.Middle;
            }
            else
            {
                return (int)EducationalLevels.Senior;
            }
        }
        private void MapClassToClassDto(Class classEntity, ClassDto dto) 
        {
            dto.Id = classEntity.Id;
            dto.EducationalLevelId = classEntity.EducationalLevelId;
            dto.Title = classEntity.Title;
            dto.Year = classEntity.Year;
        }
        private IQueryable<Class> FilterClasses(IQueryable<Class> classes, int? educationalLevelId)
        {
            if (educationalLevelId is null)
            {
                return classes;
            }
            classes = classes.Where(c => c.EducationalLevelId == educationalLevelId);
            return classes;
        }
        private async Task<int> CalculateNumberOfPagesAsync(IQueryable<Class> classes, int pageSize)
        {
            double classesCount = await classes.CountAsync();
            if (classesCount == 0)
            {
                return 0;
            }
            double result = classesCount / pageSize;
            return (int)Math.Ceiling(result);
        }
    }
}

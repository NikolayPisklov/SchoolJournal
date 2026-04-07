using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Enum_s;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.AppServices.Interfaces;
using SchoolJournalApi.Services.DbServices.Interfaces;
using System.Formats.Asn1;

namespace SchoolJournalApi.Services.AppServices
{
    public class ClassService : IClassService
    {
        private readonly IClassDbService _classDbService;


        public ClassService(IClassDbService classDbService) 
        {
            _classDbService = classDbService;
        }


        public async Task AddClassAsync(ClassCreationDto classDto)
        {
            int number = DateTime.Now.Year - (int)classDto.Year!;
            int eduLevel = GetEducationalLevel(number);
            Class newClass = new Class
            {
                Title = classDto.Title,
                Year = (int)classDto.Year,
                EducationalLevelId = eduLevel
            };
            await _classDbService.AddClassAsync(newClass);
        }
        public async Task UpdateClassAsync(ClassDto classDto)
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
            await _classDbService.SaveChangesAsync();
        }
        public async Task DeleteClassAsync(int classId)
        {
            var classEntity = await _classDbService.FindClassAsync(classId);
            if (classEntity is null)
            {
                throw new EntityNotFoundException("Entity Class can't be found!");
            }
            await _classDbService.DeleteClassAsync(classEntity);
        }
        public async Task<ClassDto> GetClassDtoAsync(int classId)
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
        public async Task<PagingResultDto<ClassDto>> GetClassesOnPageAsync(int pageSize, int? educationalLevelId, int page = 1)
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

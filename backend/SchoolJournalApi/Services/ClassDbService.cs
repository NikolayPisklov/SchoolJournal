using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Enum_s;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services
{
    public class ClassDbService : DbService<Class>, IClassDbService
    {
        public ClassDbService (SchoolJournalDbContext db) : base (db)
        {
          
        }
        public async Task AddClassAsync(ClassCreationDto classDto)
        {
            try
            {
                int number = DateTime.Now.Year - (int)classDto.Year!;
                int eduLevel;
                if (number < 5)
                {
                    eduLevel = (int)EducationalLevels.Junior;
                }
                else if (number >= 5 && number < 10) 
                {
                    eduLevel = (int)EducationalLevels.Middle;
                }
                else 
                {
                    eduLevel = (int)EducationalLevels.Senior;
                }
                    Class newClass = new Class
                    {
                        Title = classDto.Title,
                        Year = (int)classDto.Year,
                        EducationalLevelId = eduLevel
                    };
                await _db.AddAsync(newClass);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException) 
            {
                throw new EntityAddingException("Class");
            }
        }
        public async Task DeleteClassAsync(int classId)
        {
            try
            {
                var entity = await _db.Classes.FindAsync(classId);
                if (entity is null)
                {
                    throw new EntityNotFoundException("Class");
                }
                _db.Remove(entity);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException) 
            {
                throw new EntityInUseException("Class", classId);
            }
        }

        public async Task<ClassDto> GetClassDtoAsync(int classId)
        {
            var theClass = await _db.Classes.FirstOrDefaultAsync(c => c.Id == classId);
            if (theClass is null) 
            {
                throw new EntityNotFoundException("Class");
            }
            ClassDto dto = new ClassDto();
            dto.Id = theClass.Id;
            dto.EducationalLevelId = theClass.EducationalLevelId;
            dto.Title = theClass.Title;
            dto.Year = theClass.Year;
            return dto;
        }

        public async Task<List<ClassDto>> GetClassesAsync()
        {
            List<ClassDto> classes = await _db.Classes.OrderBy(c => c.Year)
                .Select(c => new ClassDto {
                    Id = c.Id,
                    EducationalLevelId = c.EducationalLevelId,
                    Title = c.Title,
                    Year = c.Year
                }).ToListAsync();
            if(classes.Count == 0) 
            {
                throw new EntityNotFoundException("Classes not found");
            }
            return classes;
        }

        public async Task<PagingResultDto<ClassDto>> GetClassesOnPageAsync(int pageSize, int? educationalLevelId, int page = 1)
        {
            var classes = _db.Classes.AsNoTracking();
            classes = FilterClasses(classes, educationalLevelId);
            int numberOfPages = await GetNumberOfItemsPagesAsync(classes, pageSize);
            List<ClassDto> classesDto = classes.OrderBy(c => c.Year).Skip((page - 1) * pageSize).Take(pageSize)
                .Select(c => new ClassDto
                {
                    Id = c.Id,
                    EducationalLevelId = c.EducationalLevelId,
                    Title = c.Title,
                    Year = c.Year
                }).ToList();
            return new PagingResultDto<ClassDto> {Items = classesDto, NumberOfPages = numberOfPages};
        }
        public async Task UpdateClassAsync(ClassDto classDto)
        {
            try
            {
                var oldClass = _db.Classes.FirstOrDefault(c => c.Id == classDto.Id);
                if (oldClass is null)
                {
                    throw new EntityNotFoundException("Class");
                }
                oldClass.Title = classDto.Title;
                oldClass.Year = (int)classDto.Year!;
                //may be automatic edu level change
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new EntityUpdateException("Class");
            }
        }


        private IQueryable<Class> FilterClasses(IQueryable<Class> classes, int? educationalLevelId)
        {
            if(educationalLevelId is null) 
            {
                return classes;
            }
            classes = classes.Where(c => c.EducationalLevelId == educationalLevelId);
            return classes;
        }
    }
}

using SchoolJournalApi.Dto_s;

namespace SchoolJournalApi.Services
{
    public interface IClassDbService
    {
        Task<PagingResultDto<ClassDto>> GetClassesOnPageAsync(int pageSize, int? educationalLevelId, int page = 1);
        Task AddClassAsync(ClassCreationDto classDto);
        Task UpdateClassAsync(ClassDto classDto);
        Task DeleteClassAsync(int classId);
        Task<List<ClassDto>> GetClassesAsync();
        Task<ClassDto> GetClassDtoAsync(int classId);
    }
}

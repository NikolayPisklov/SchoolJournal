using SchoolJournalApi.Dto_s;

namespace SchoolJournalApi.Services.AppServices.Interfaces
{
    public interface IClassService
    {
        Task AddClassAsync(ClassCreationDto classDto);
        Task UpdateClassAsync(ClassDto classDto);
        Task DeleteClassAsync(int classId);
        Task<ClassDto> GetClassDtoAsync(int classId);
        Task<PagingResultDto<ClassDto>> GetClassesOnPageAsync(int pageSize, int? educationalLevelId, int page = 1);
    }
}

using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Dtos.User;

namespace SchoolJournalApi.Services
{
    public interface IUsersDbService
    {
        Task<List<StatusDto>> GetUserStatusesAsync();
        Task<UserDetailsForUpdateDto?> GetUserDetailsAsync(int userId);
        Task<bool> TryUpdateUserDetailsAsync(UserDetailsForUpdateDto userDetails);
        Task<bool> TryAddUserAsync(UserDetailsForCreationDto userDetails);
        Task<PagingResultDto<ListedUserDto>> GetUsersOnPageAsync(int? statusId, string? search, int pageSize, int page = 1);
        Task<bool> TryDeleteUserAsync(int userId);
        Task<int> GetUserStatusAsync(int userId);
        Task<ClassDto> GetStudentsClassAsync(int userId);
    }
}

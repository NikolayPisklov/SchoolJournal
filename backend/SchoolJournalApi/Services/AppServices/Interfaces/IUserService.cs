using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Dtos.User;

namespace SchoolJournalApi.Services.AppServices.Interfaces
{
    public interface IUserService
    {
        Task UpdateUserAsync(UserUpdateDto dto);
        Task AddUserAsync(UserCreationDto dto);
        Task DeleteUserAsync(int userId);
        Task<List<StatusDto>> GetUserStatusesAsync();
        Task<UserUpdateDto> GetUserDetailsAsync(int userId);
        Task<PagingResultDto<ListedUserDto>> GetUsersOnPageAsync(int? statusId, string? nameSearch, int pageSize, int page = 1);
        Task<int> GetUserStatusAsync(int userId);
        Task<ClassDto> GetClassOfStudentAsync(int userId);
    }
}

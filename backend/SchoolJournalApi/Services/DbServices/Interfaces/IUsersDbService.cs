using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Dtos.User;
using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services.DbServices.Interfaces
{
    public interface IUsersDbService
    {
        Task<List<StatusDto>> GetUserStatusesAsync();
        Task<UserUpdateDto?> GetUserDetailsAsync(int userId);
        Task SaveChangesAsync();
        Task<bool> TryAddUserAsync(UserCreationDto userDetails);
        Task<PagingResultDto<ListedUserDto>> GetUsersOnPageAsync(int? statusId, string? search, int pageSize, int page = 1);
        Task<bool> TryDeleteUserAsync(int userId);
        Task<int> GetUserStatusAsync(int userId);
        Task<ClassDto> GetStudentsClassAsync(int userId);
        Task<User?> FindUserAsync(int userId);
        Task<bool> IsThereUserWithSameLoginAsync(string login, int? userId);
    }
}

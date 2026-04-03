using SchoolJournalApi.Dtos.User;

namespace SchoolJournalApi.Services.AppServices.Interfaces
{
    public interface IUserService
    {
        Task UpdateUserAsync(UserUpdateDto dto);
    }
}

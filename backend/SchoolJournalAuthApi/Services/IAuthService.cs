using SchoolJournalAuthApi.Entities;
using SchoolJournalAuthApi.Models;

namespace SchoolJournalAuthApi.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserRegisterDto request);
        Task<TokenResponseDto?> LoginAsync(UserLoginDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(int userId, string refreshToken);
        Task<LoggedInUserDto?> GetLoggedInUserDetailsAsync(int userId);
    }
}

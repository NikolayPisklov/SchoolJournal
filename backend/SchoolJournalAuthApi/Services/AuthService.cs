using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolJournalAuthApi.Entities;
using SchoolJournalAuthApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SchoolJournalAuthApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly AuthDbContext _db;

        public AuthService(IConfiguration configuration, AuthDbContext db) 
        {
            _configuration = configuration;
            _db = db;
        }

        public async Task<TokenResponseDto?> LoginAsync(UserLoginDto request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Login == request.Login);
            if (user == null) 
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            var response = new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user)
            };
            return response;
        }
        
        public async Task<User?> RegisterAsync(UserRegisterDto request)
        {
            if (await _db.Users.AnyAsync(x => x.Login == request.Login)) 
            {
                return null;
            }
            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            user.StatusId = request.StatusId;
            user.Login = request.Login;
            user.PasswordHash = hashedPassword;
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.MiddleName = request.MiddleName;
            user.RefreshToken = "";

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }
        public async Task<TokenResponseDto?> RefreshTokensAsync(int userId, string refreshToken) 
        {
            var user = await ValidateRefreshTokenAsync(userId, refreshToken);
            if (user is null) 
            {
                return null;
            }
            var response = new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user)
            };
            return response;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.StatusId.ToString())
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        private string GenerateRefreshToken() 
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private async Task<string> GenerateAndSaveRefreshToken(User user) 
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _db.SaveChangesAsync();
            return refreshToken;
        }
        private async Task<User?> ValidateRefreshTokenAsync(int userId, string refreshToken) 
        {
            var user = await _db.Users.FindAsync(userId);
            if (user is null || user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime < DateTime.UtcNow) 
            {
                return null;
            }
            return user;
        }

        public async Task<LoggedInUserDto?> GetLoggedInUserDetailsAsync(int userId)
        {
            var user = await _db.Users.FindAsync(userId);
            string role = string.Empty;
            switch (user!.StatusId) 
            {
                case 1:
                    role = "admin";
                    break;
                case 2:
                    role = "teacher";
                    break;
                case 3:
                    role = "student";
                    break;
                default:
                    return null;
            }
            return new LoggedInUserDto
            {
                Id = userId,
                Role = role,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName
            };
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolJournalAuthApi.Entities;
using SchoolJournalAuthApi.Models;
using SchoolJournalAuthApi.Services;
using System.Security.Claims;
namespace SchoolJournalAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) 
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>>Register(UserRegisterDto request) 
        {
            var user = await _authService.RegisterAsync(request);
            if (user == null) 
            {
                return BadRequest("Пользователь с таким логином уже существует.");
            }
            return Ok(user);
        }
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>>Login(UserLoginDto request) 
        {
            var response = await _authService.LoginAsync(request);
            if (response == null) 
            {
                return BadRequest("Неверный логин или пароль");
            }
            CreateCookies(response);
            return Ok("Logged in!");
        }
        [Authorize]
        [HttpGet("get-logged-in-user-details")]
        public async Task<ActionResult<LoggedInUserDto>> GetLoggedInUserDetails() 
        {
            var accessToken = Request.Cookies["accessToken"];
            if(accessToken is null) 
            {
                return BadRequest("Время сессии истекло!");
            }
            var userId = JwtClaimReader.GetClaim(accessToken, ClaimTypes.NameIdentifier);
            if(!Int32.TryParse(userId, out int userIdParsed)) 
            {
                return BadRequest("Ошибка авторизации! Невалидный токен.");
            }
            var details = await _authService.GetLoggedInUserDetailsAsync(userIdParsed);
            return Ok(details); 
        }
        [HttpPost("refresh-tokens")]
        public async Task<ActionResult<TokenResponseDto>> RefreshTokens() 
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var accessToken = Request.Cookies["accessToken"];
            if (accessToken is null || refreshToken is null) 
            {
                return BadRequest("Session has expired!");
            }
            var userId = JwtClaimReader.GetClaim(accessToken, ClaimTypes.NameIdentifier);
            if (userId is null || !Int32.TryParse(userId, out int userIdParsed)) 
            {
                return BadRequest("Invalid access token!");
            }
            var result = await _authService.RefreshTokensAsync(userIdParsed, refreshToken);
            if (result is null || result.AccessToken is null || result.RefreshToken is null) 
            { 
                return Unauthorized("Invalid refresh token!");
            } 
            CreateCookies(result);
            return Ok("Tokens has been refreshed!");  
        }
        [Authorize]
        [HttpPost("logout")]
        public ActionResult Logout() 
        {
            Response.Cookies.Delete("refreshToken");
            Response.Cookies.Delete("accessToken");
            return Ok("Вы успешно вышли из системы!");
        }

        private void CreateCookies(TokenResponseDto tokens) 
        {
            Response.Cookies.Append("accessToken", tokens.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }
       
    }
}

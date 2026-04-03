using Microsoft.AspNetCore.Identity;
using SchoolJournalApi.Dtos.User;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;
using SchoolJournalApi.Services.AppServices.Interfaces;
using SchoolJournalApi.Services.DbServices.Interfaces;

namespace SchoolJournalApi.Services.AppServices
{
    public class UserService : IUserService
    {
        private readonly IUsersDbService _dbService;


        public UserService (IUsersDbService usersDbService)
        {
            _dbService = usersDbService;
        }


        public async Task UpdateUserAsync(UserUpdateDto dto) 
        {
            var user = await _dbService.FindUserAsync(dto.Id);
            if (user is null)
            {
                throw new EntityNotFoundException($"Entity User with Id: {dto.Id} is not found!");
            }
            if (await _dbService.IsThereUserWithSameLoginAsync(dto.Login, dto.Id))
            {
                throw new EntityAlreadyExistsException($"Entity User with Login {dto.Login} is already exists!");
            }
            MapUserToUpdateDto(user, dto);
            await _dbService.SaveChangesAsync();            
        }


        private void MapUserToUpdateDto(User user, UserUpdateDto dto) 
        {
            user.Login = dto.Login;
            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.MiddleName = dto.MiddleName;
            if (dto.Password is not null)
            {
                user.PasswordHash = new PasswordHasher<User>().HashPassword(user, dto.Password);
            }
        }
    }
}

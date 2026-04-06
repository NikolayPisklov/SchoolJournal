using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dto_s;
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
        public async Task AddUserAsync(UserCreationDto dto) 
        {
            if (await _dbService.IsThereUserWithSameLoginAsync(dto.Login)) 
            {
                throw new EntityAlreadyExistsException($"Entity User with Login {dto.Login} is already exists!");
            }
            User user = new User();
            MapUserToCreationDto(user, dto);
            await _dbService.AddUserAsync(user);
        }
        public async Task DeleteUserAsync(int userId) 
        {
            var user = await _dbService.FindUserAsync(userId);
            if(user is null) 
            {
                throw new EntityNotFoundException($"User with Id: {userId} is not found!");
            }
            await _dbService.DeleteUserAsync(user);
        }
        public async Task<List<StatusDto>> GetUserStatusesAsync() 
        {
            var statuses = _dbService.GetUserStatuses();
            var dtos = await statuses.Select(s => new StatusDto
            {
                Id = s.Id,
                Title = s.Title
            }).ToListAsync();
            return dtos;
        }
        public async Task<UserUpdateDto> GetUserDetailsAsync(int userId) 
        {
            var user = await _dbService.FindUserAsync(userId);
            if( user is null)
            {
                throw new EntityNotFoundException($"User with Id: {userId} is not found!");
            }
            UserUpdateDto dto = new UserUpdateDto
            {
                Id = user.Id,
                Login = user.Login,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MiddleName = user.MiddleName
            };
            return dto;
        }
        public async Task<PagingResultDto<ListedUserDto>> GetUsersOnPageAsync(int? statusId, string? nameSearch, int pageSize, int page = 1) 
        {
            var users = _dbService.GetAllUsers();
            users = FilterUsers(users, statusId, nameSearch);
            int numberOfPages = await CalculateNumberOfPagesAsync(users, pageSize);
            List<ListedUserDto> dtoUsersList = await users.OrderBy(x => x.LastName)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .Select(x => new ListedUserDto
               {
                   Id = x.Id,
                   StatusId = x.StatusId,
                   FirstName = x.FirstName,
                   LastName = x.LastName,
                   MiddleName = x.MiddleName,
                   ClassTitle = x.StudentClasses.Where(x => x.IsActive).Select(x => x.Class.Title).FirstOrDefault(),
                   ClassYear = x.StudentClasses.Where(x => x.IsActive).Select(x => x.Class.Year).FirstOrDefault(),
               }).ToListAsync();
            return new PagingResultDto<ListedUserDto> { Items = dtoUsersList, NumberOfPages = numberOfPages };
        }
        public async Task<int> GetUserStatusAsync(int userId) 
        {
            var userStatus = await _dbService.FindUserAsync(userId);
            if (userStatus is null)
            {
                throw new EntityNotFoundException($"User with Id: {userId} can't be found!");
            }
            return userStatus.StatusId;
        }
        public async Task<ClassDto> GetClassOfStudentAsync(int userId) 
        {
            var studentClass = await _dbService.FindClassOfStudentAsync(userId);
            if (studentClass is null)
            {
                throw new EntityNotFoundException($"Student-Class for student with Id: {userId} is not found!");
            }
            var clas = studentClass.Class;
            if (clas is null)
            {
                throw new EntityNotFoundException($"Student with Id: {userId} is not in class!");
            }
            return new ClassDto
            {
                Id = clas.Id,
                Title = clas.Title,
                EducationalLevelId = clas.EducationalLevelId,
                Year = clas.Year
            };
        }


        private IQueryable<User> FilterUsers(IQueryable<User> users, int? statusId, string? nameSearch) 
        {
            if(statusId is not null) 
            {
                users = _dbService.FilterUsersByStatus(users, (int)statusId);
            }
            if (!string.IsNullOrEmpty(nameSearch)) 
            {
                users = _dbService.FilterUsersByName(users, nameSearch);
            }
            return users;
        }
        private void MapUserToCreationDto(User user, UserCreationDto dto) 
        {
            user.StatusId = (int)dto.StatusId!;
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
        private async Task<int> CalculateNumberOfPagesAsync(IQueryable<User> users, int pageSize)
        {
            double usersCount = await users.CountAsync();
            if (usersCount == 0)
            {
                return 0;
            }
            double result = usersCount / pageSize;
            return (int)Math.Ceiling(result);
        }
    }
}

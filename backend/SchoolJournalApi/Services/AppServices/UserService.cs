using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
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
        private readonly IContextService _contextService;


        public UserService (IUsersDbService usersDbService, IContextService contextService)
        {
            _contextService = contextService;
            _dbService = usersDbService;
        }


        public async Task UpdateUserAsync(UserUpdateDto dto) 
        {
            try
            {
                var user = await _dbService.FindUserAsync(dto.Id);
                if (user is null)
                {
                    throw new EntityNotFoundException($"Entity User with Id: {dto.Id} is not found!");
                }
                if (await _dbService.IsThereUserWithSameLoginAsync(dto.Login, dto.Id))
                {
                    throw new EntityAlreadyExistsException($"User with the same login already exists!");
                }
                if (await _dbService.IsThereUserWithSameEmailAsync(dto.Email, dto.Id))
                {
                    throw new EntityAlreadyExistsException($"User with the same email already exists!");
                }
                MapUserToUpdateDto(user, dto);
                await _contextService.SaveChangesAsync();
            }
            catch (ReferenceConstraintException ex)
            {
                throw new EntityInUseException("An error has occured while updating user.", ex);
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            }                        
        }
        public async Task AddUserAsync(UserCreationDto dto) 
        {
            try
            {
                if (await _dbService.IsThereUserWithSameLoginAsync(dto.Login))
                {
                    throw new EntityAlreadyExistsException($"User with the same login already exists!");
                }
                if (await _dbService.IsThereUserWithSameEmailAsync(dto.Email))
                {
                    throw new EntityAlreadyExistsException($"User with the same email already exists!");
                }
                User user = new User();
                MapUserToCreationDto(user, dto);
                _dbService.AddUser(user);
                await _contextService.SaveChangesAsync();
            }
            catch (ReferenceConstraintException ex)
            {
                throw new EntityInUseException("An error has occured while adding user.", ex);
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            }            
        }
        public async Task DeleteUserAsync(int userId) 
        {
            try
            {
                var user = await _dbService.FindUserAsync(userId);
                if (user is null)
                {
                    throw new EntityNotFoundException($"User with Id: {userId} is not found!");
                }
                _dbService.DeleteUser(user);
                await _contextService.SaveChangesAsync();
            }
            catch(ReferenceConstraintException ex)
            {
                throw new EntityInUseException("User are bound to other records and can't be deleted.", ex);
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            }
        }
        public async Task<List<StatusDto>> GetUserStatusesAsync() 
        {
            try
            {
                var statuses = _dbService.GetUserStatuses();
                var dtos = await statuses.Select(s => new StatusDto
                {
                    Id = s.Id,
                    Title = s.Title
                }).ToListAsync();
                return dtos;
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            }            
        }
        public async Task<UserUpdateDto> GetUserDetailsAsync(int userId) 
        {
            try
            {
                var user = await _dbService.FindUserAsync(userId);
                if (user is null)
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
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            } 
        }
        public async Task<PagingResultDto<ListedUserDto>> GetUsersOnPageAsync(int? statusId, string? nameSearch, int pageSize, int page = 1) 
        {
            try
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
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            }
        }
        public async Task<int> GetUserStatusAsync(int userId) 
        {
            try
            {
                return await _dbService.SelectUserStatusId(userId);
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            }           
        }
        public async Task<ClassDto> GetClassOfStudentAsync(int userId) 
        {
            try
            {
                var studentClass = await _dbService.FindClassOfStudentAsync(userId);
                if (studentClass is null)
                {
                    throw new EntityNotFoundException($"Student-Class for student with Id: {userId} is not found!");
                }
                var clas = studentClass.Class;
                return new ClassDto
                {
                    Id = clas.Id,
                    Title = clas.Title,
                    EducationalLevelId = clas.EducationalLevelId,
                    Year = clas.Year
                };
            }
            catch (SqlException ex)
            {
                throw new EfDbException("An error has occur while connecting to DB!", ex);
            }
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

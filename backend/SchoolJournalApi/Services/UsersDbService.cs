using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Dtos.User;
using SchoolJournalApi.Exceptions;
using SchoolJournalApi.Models;

namespace SchoolJournalApi.Services
{
    public class UsersDbService : DbService<User>, IUsersDbService
    {
        private readonly SchoolJournalDbContext _db;

        public UsersDbService(SchoolJournalDbContext db) : base(db) { _db = db; }

        public async Task<PagingResultDto<ListedUserDto>> GetUsersOnPageAsync(int? statusId, string? search, int pageSize, int page = 1) 
        {
            var users = _db.Users.AsNoTracking();
            users = FilterUsers(users, statusId, search);
            int numberOfPages = await GetNumberOfItemsPagesAsync(users, pageSize);
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
                   ClassTitle = x.StudentClasses.Where(x => x.IsActive).Select(x => x.Class!.Title).FirstOrDefault(),
                   ClassYear = x.StudentClasses.Where(x => x.IsActive).Select(x => x.Class!.Year).FirstOrDefault(),
               }).ToListAsync();
            return new PagingResultDto<ListedUserDto> { Items = dtoUsersList, NumberOfPages = numberOfPages };
        }
        
        public async Task<List<StatusDto>> GetUserStatusesAsync() 
        {
            var statuses = await _db.Statuses.Select(s => new StatusDto
            {
                Id = s.Id,
                Title = s.Title
            }).ToListAsync();
            return statuses;
        }
        public async Task<UserDetailsForUpdateDto?> GetUserDetailsAsync(int userId) 
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if(user is null) 
            {
                return null;
            }
            var dto = new UserDetailsForUpdateDto
            {
                Id = userId,
                Login = user.Login,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Email = user.Email
            };
            return dto;
        }
        public async Task<bool> TryUpdateUserDetailsAsync(UserDetailsForUpdateDto userDetails) 
        {
            var user = await _db.Users.FindAsync(userDetails.Id);
            if (user is null) 
            {
                return false;
            }
            if(await _db.Users.AnyAsync(x => x.Login == userDetails.Login) 
                && !userDetails.Login.Equals(user.Login))
            {
                return false;
            }
            user.Login = userDetails.Login;
            user.Email = userDetails.Email;
            user.FirstName = userDetails.FirstName;
            user.LastName = userDetails.LastName;
            user.MiddleName = userDetails.MiddleName;
            if (userDetails.Password is not null) 
            {
                user.PasswordHash = new PasswordHasher<User>().HashPassword(user, userDetails.Password);
            }
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> TryAddUserAsync(UserDetailsForCreationDto userDetails) 
        {
            if(await _db.Users.AnyAsync(x => x.Login == userDetails.Login)) 
            {
                return false;
            }
            var newUser = new User()
            {
                StatusId = (int)userDetails.StatusId!,
                Login = userDetails.Login,
                Email = userDetails.Email,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                MiddleName = userDetails.MiddleName,
            };
            newUser.PasswordHash = new PasswordHasher<User>().HashPassword(newUser, userDetails.Password);
            _db.Add(newUser);
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> TryDeleteUserAsync(int userId) 
        {
            try
            {
                var user = _db.Users.FirstOrDefault(x => x.Id == userId);
                if (user is null)
                {
                    return false;
                }
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException) 
            {
                throw  new EntityNotFoundException($"User");
            }
            catch (DbUpdateException) 
            {
                throw new EntityInUseException("User", userId);
            }
        }
        public async Task<int> GetUserStatusAsync(int userId)
        {
            var userStatus = await _db.Users.FindAsync(userId);
            if (userStatus is null) 
            {
                throw new EntityNotFoundException("User");
            }
            return userStatus.StatusId;
        }

        public async Task<ClassDto> GetStudentsClassAsync(int userId)
        {
            var studentClass = await _db.StudentClasses.Include(s => s.Class)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);
            if(studentClass is null) 
            {
                throw new EntityNotFoundException("Student-Class");
            }
            var clas = studentClass.Class;
            if (clas is null) 
            {
                throw new EntityNotFoundException("Class");
            }
            return new ClassDto
            {
                Id = clas.Id,
                Title = clas.Title,
                EducationalLevelId = clas.EducationalLevelId,
                Year = clas.Year
            };
        }

        private IQueryable<User> FilterUsers(IQueryable<User> users, int? statusId, string? search) 
        {
            if (statusId is not null)
            {
                users = users.Where(x => x.StatusId == statusId);
            }
            if (!string.IsNullOrEmpty(search)) 
            {
                var parts = search
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                users = users.Where(x =>
                    parts.All(p => 
                        EF.Functions.Like(x.LastName, "%" + p + "%") ||
                        EF.Functions.Like(x.FirstName, "%" + p + "%") ||
                        EF.Functions.Like(x.MiddleName, "%" + p + "%")
                    )
                );
            }
            return users;
        }
    }
}

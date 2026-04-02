using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Dtos.Journal;
using SchoolJournalApi.Dtos.User;
using SchoolJournalApi.Enum_s;
using SchoolJournalApi.Services;

namespace SchoolJournalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserStatusesNames.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IUsersDbService _dbService;
        private readonly IClassDbService _classDbService;
        private readonly ITeacherSubjectDbService _teacherSubjectDbService;
        private readonly IStudentClassService _studentClassService;
        private readonly IJournalDbService _journalDbService;

        public AdminController(IUsersDbService dbService, IClassDbService classDbService,
            ITeacherSubjectDbService teacherSubjectDbService, IStudentClassService studentClassService,
            IJournalDbService journalDbService)
        {
            _dbService = dbService;
            _classDbService = classDbService;
            _teacherSubjectDbService = teacherSubjectDbService;
            _studentClassService = studentClassService;
            _journalDbService = journalDbService;
        }
        [HttpGet("get-users-on-page")]
        public async Task<IActionResult> GetUsersOnPage(int? status, string? search, int pageSize, int page) 
        {
            var result = await _dbService.GetUsersOnPageAsync(status, search, pageSize, page);
            if (result.Items is null || result.Items.Count() == 0) 
            {
                return NotFound();
            }
            else 
            {
                return Ok(result);
            }
        }
        [HttpGet("get-user-details")]
        public async Task<IActionResult> GetUserDetails(int id)
        {
            var user = await _dbService.GetUserDetailsAsync(id);
            if (user is null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpGet("get-user-statuses")]
        public async Task<IActionResult> GetUserStatuses() 
        {
            var statuses = await _dbService.GetUserStatusesAsync();
            return Ok(statuses);
        }
        [HttpPut("update-user-details")]
        public async Task<IActionResult> UpdateUserDetails(UserDetailsForUpdateDto? updatedUser) 
        {
            if (!ModelState.IsValid || updatedUser is null) 
            {
                return BadRequest(ModelState);
            }
            if(await _dbService.TryUpdateUserDetailsAsync(updatedUser)) 
            {
                return Ok("Данные пользователя успешно обновлены!");
            }
            else 
            {
                return Conflict("Пользователь с таким логином уже существует.");
            }                
        }
        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser(UserDetailsForCreationDto? userDetails) 
        {
            if(!ModelState.IsValid || userDetails is null) 
            {
                return BadRequest(ModelState);
            }
            if(await _dbService.TryAddUserAsync(userDetails)) 
            {
                return Ok("Пользователь успешно добавлен в систему!");
            }
            else 
            {
                return Conflict("Пользователь с таким логином уже существует.");
            }
        }
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            if (await _dbService.TryDeleteUserAsync(userId))
            {
                return Ok("Пользователь успешно удалён.");
            }
            else
            {
                return NotFound("Пользователь не найден в базе данных.");
            }       
        }
        [HttpGet("get-user-status-id")]
        public async Task<IActionResult> GetUserStatusId(int id) 
        {
            var result = await _dbService.GetUserStatusAsync(id);
            return Ok(result);
        }
        [HttpGet("get-class-of-student")]
        public async Task<IActionResult> GetStudentsClass(int userId) 
        {
            var dto = await _dbService.GetStudentsClassAsync(userId);
            return Ok(dto);
        }
        //Classes-----------------------------------------------------------------------------------------------------------------
        [HttpGet("get-classes-on-page")]
        public async Task<IActionResult> GetClassesOnPage(int pageSize, int? educationalLevel, int page = 1) 
        {
            var classes = await _classDbService.GetClassesOnPageAsync(pageSize, educationalLevel, page);
            if(classes.Items is null || !classes.Items.Any()) 
            {
                return NotFound();
            }
            return Ok(classes);
        }
        [HttpPost("add-class")]
        public async Task<IActionResult> AddClass(ClassCreationDto? dto) 
        {
            if (!ModelState.IsValid || dto is null)
            {
                return BadRequest(ModelState);
            }
            await _classDbService.AddClassAsync(dto);
            return Ok();
        }
        [HttpPut("update-class")]
        public async Task<IActionResult> UpdateClass(ClassDto dto) 
        {
            await _classDbService.UpdateClassAsync(dto);
            return Ok();
        }
        [HttpDelete("delete-class")]
        public async Task<IActionResult> DeleteClass(int id) 
        {
            await _classDbService.DeleteClassAsync(id);
            return Ok();
        }
        [HttpGet("get-classes")]
        public async Task<IActionResult> GetClasses() 
        {
            var classes = await _classDbService.GetClassesAsync();
            return Ok(classes);
        }
        [HttpGet("get-class-details")]
        public async Task<IActionResult> GetClassDetails(int id) 
        {
            var dto = await _classDbService.GetClassDtoAsync(id);
            return Ok(dto);
        }
        //TeacherSubject----------------------------------------------------------------------------------------------------------
        [HttpGet("get-subjects")]
        public async Task<IActionResult> GetSubjects(int? eduLevelId) 
        {
            var subjects = await _teacherSubjectDbService.GetSubjectsAsync(eduLevelId);
            if (!subjects.Any()) 
            {  
                return NotFound(); 
            }
            return Ok(subjects);
        }
        [HttpGet("get-teacher-subject")]
        public async Task<IActionResult> GetTeacherSubjectForTeacher(int id)
        {
            var subjects = await _teacherSubjectDbService.GetSubjectsForTeacherAsync(id);
            if (!subjects.Any())
            {
                return NotFound();
            }
            return Ok(subjects);
        }
        [HttpDelete("delete-teacher-subject")]
        public async Task<IActionResult> DeleteTeacherSubject(int id) 
        {
            await _teacherSubjectDbService.DeleteTeacherSubjectAsync(id);
            return Ok();
        }
        [HttpPost("add-teacher-subject")]
        public async Task<IActionResult> AddTeacherSubject(NewTeacherSubjectDto dto) 
        {
            await _teacherSubjectDbService.AddTeacherSubjectAsync(dto.UserId, dto.SubjectId);
            return Ok();
        }
        [HttpGet("get-all-teachers-subjects")]
        public async Task<IActionResult> GetAllTeachersSubjects(int? eduLevelId) 
        {
            var result = await _teacherSubjectDbService.GetAllTeacherSubjectsAsync(eduLevelId);
            return Ok(result);
        }
        //Student-Class-----------------------------------------------------------------------------------------------------------
        [HttpGet("get-students-in-class")]
        public async Task<IActionResult> GetStudentsInClass(int classId) 
        {
            var students = await _studentClassService.GetStudentsInClassAsync(classId);
            return Ok(students);
        }
        [HttpPost("transfer-student")]
        public async Task<IActionResult> TransferStudent(TransferStudentDto dto) 
        {
            await _studentClassService.TransferStudentToAnotherCLassAsync(dto.StudentId, dto.NewClassId, dto.OldClassId);
            return Ok();
        }
        //Journals
        [HttpGet("get-journals-for-class")]
        public async Task<IActionResult> GetJournalsForClass(int classId)
        {
            var journals = await _journalDbService.GetJournalsForClassAsync(classId);
            if (journals.Count == 0) 
            {
                return NotFound();
            }
            return Ok(journals);
        }
        [HttpPost("add-journal")]
        public async Task<IActionResult> AddJournalToClass(JournalCreationDto dto) 
        {
            await _journalDbService.AddJournalAsync(dto);
            return Ok();
        }
        [HttpDelete("delete-journal")]
        public async Task<IActionResult> DeleteJournal(int journalId) 
        {
            await _journalDbService.DeleteJournalAsync(journalId);
            return Ok();
        }
    }
}

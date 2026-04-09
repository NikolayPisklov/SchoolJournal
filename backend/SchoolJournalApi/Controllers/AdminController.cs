using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolJournalApi.Dto_s;
using SchoolJournalApi.Dtos.Journal;
using SchoolJournalApi.Dtos.User;
using SchoolJournalApi.Enum_s;
using SchoolJournalApi.Services.AppServices.Interfaces;
using SchoolJournalApi.Services.DbServices.Interfaces;

namespace SchoolJournalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserStatusesNames.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IClassService _classService;
        private readonly ITeacherSubjectService _teacherSubjectService;
        private readonly IStudentClassService _studentClassService;
        private readonly IJournalService _journalService;

        public AdminController(IClassService classService,
            ITeacherSubjectService teacherSubjectService, IStudentClassService studentClassService,
            IJournalService journalService, IUserService userService)
        {
            _classService = classService;
            _teacherSubjectService = teacherSubjectService;
            _studentClassService = studentClassService;
            _journalService = journalService;
            _userService = userService;
        }
        [HttpGet("get-users-on-page")]
        public async Task<IActionResult> GetUsersOnPage(int? status, string? search, int pageSize, int page) 
        {
            var result = await _userService.GetUsersOnPageAsync(status, search, pageSize, page);
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
            var user = await _userService.GetUserDetailsAsync(id);
            return Ok(user);
        }
        [HttpGet("get-user-statuses")]
        public async Task<IActionResult> GetUserStatuses() 
        {
            var statuses = await _userService.GetUserStatusesAsync();
            return Ok(statuses);
        }
        [HttpPut("update-user-details")]
        public async Task<IActionResult> UpdateUserDetails(UserUpdateDto updatedUser) 
        {
            await _userService.UpdateUserAsync(updatedUser);
            return Ok();
        }
        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser(UserCreationDto userDetails) 
        {
            await _userService.AddUserAsync(userDetails);
            return Ok();
        }
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            await _userService.DeleteUserAsync(userId);
            return Ok();
        }
        [HttpGet("get-user-status-id")]
        public async Task<IActionResult> GetUserStatusId(int id) 
        {
            var result = await _userService.GetUserStatusAsync(id);
            return Ok(result);
        }
        [HttpGet("get-class-of-student")]
        public async Task<IActionResult> GetStudentsClass(int userId) 
        {
            var dto = await _userService.GetClassOfStudentAsync(userId);
            return Ok(dto);
        }
        //Classes-----------------------------------------------------------------------------------------------------------------
        [HttpGet("get-classes-on-page")]
        public async Task<IActionResult> GetClassesOnPage(int pageSize, int? educationalLevel, int page = 1) 
        {
            var classes = await _classService.GetClassesOnPageAsync(pageSize, educationalLevel, page);
            if(classes.Items is null || !classes.Items.Any()) 
            {
                return NotFound();
            }
            return Ok(classes);
        }
        [HttpPost("add-class")]
        public async Task<IActionResult> AddClass(ClassCreationDto dto) 
        {
            await _classService.AddClassAsync(dto);
            return Ok();
        }
        [HttpPut("update-class")]
        public async Task<IActionResult> UpdateClass(ClassDto dto) 
        {
            await _classService.UpdateClassAsync(dto);
            return Ok();
        }
        [HttpDelete("delete-class")]
        public async Task<IActionResult> DeleteClass(int id) 
        {
            await _classService.DeleteClassAsync(id);
            return Ok();
        }
        [HttpGet("get-class-details")]
        public async Task<IActionResult> GetClassDetails(int id) 
        {
            var dto = await _classService.GetClassDtoAsync(id);
            return Ok(dto);
        }
        //TeacherSubject----------------------------------------------------------------------------------------------------------
        [HttpGet("get-subjects")]
        public async Task<IActionResult> GetSubjects(int? eduLevelId) 
        {
            var subjects = await _teacherSubjectService.GetSubjectsAsync(eduLevelId);
            if (!subjects.Any()) 
            {  
                return NotFound(); 
            }
            return Ok(subjects);
        }
        [HttpGet("get-teacher-subject")]
        public async Task<IActionResult> GetTeacherSubjectForTeacher(int id)
        {
            var subjects = await _teacherSubjectService.GetSubjectsForTeacherAsync(id);
            if (!subjects.Any())
            {
                return NotFound();
            }
            return Ok(subjects);
        }
        [HttpDelete("delete-teacher-subject")]
        public async Task<IActionResult> DeleteTeacherSubject(int id) 
        {
            await _teacherSubjectService.DeleteTeacherSubjectAsync(id);
            return Ok();
        }
        [HttpPost("add-teacher-subject")]
        public async Task<IActionResult> AddTeacherSubject(NewTeacherSubjectDto dto) 
        {
            await _teacherSubjectService.AddTeacherSubjectAsync(dto.UserId, dto.SubjectId);
            return Ok();
        }
        [HttpGet("get-all-teachers-subjects")]
        public async Task<IActionResult> GetAllTeachersSubjects(int? eduLevelId) 
        {
            var result = await _teacherSubjectService.GetTeacherSubjectsAsync(eduLevelId);
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
            await _studentClassService.TransferStudentToAnotherCLassAsync(dto.StudentId, dto.NewClassId);
            return Ok();
        }
        //Journals
        [HttpGet("get-journals-for-class")]
        public async Task<IActionResult> GetJournalsForClass(int classId)
        {
            var journals = await _journalService.GetJournalsForClassAsync(classId);
            if (journals.Count == 0) 
            {
                return NotFound();
            }
            return Ok(journals);
        }
        [HttpPost("add-journal")]
        public async Task<IActionResult> AddJournalToClass(JournalCreationDto dto) 
        {
            await _journalService.AddJournalAsync(dto);
            return Ok();
        }
        [HttpDelete("delete-journal")]
        public async Task<IActionResult> DeleteJournal(int journalId) 
        {
            await _journalService.DeleteJournalAsync(journalId);
            return Ok();
        }
    }
}

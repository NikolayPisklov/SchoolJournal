using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SchoolJournalApi.Enum_s;
using SchoolJournalApi.Services.AppServices.Interfaces;
using SchoolJournalApi.Services.DbServices.Interfaces;

namespace SchoolJournalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserStatusesNames.Student)]
    public class StudentController : ControllerBase
    {
        private readonly IJournalService _journalService;
        public StudentController(IJournalService journalService) 
        {
            _journalService = journalService;
        }

        [HttpGet("get-journals-for-student")]
        public async Task<IActionResult> GetJournalsForStudent(int studentId)
        {
            var journals = await _journalService.GetJournalsForStudent(studentId);
            return Ok(journals);
        }
        [HttpGet("get-journal-details-for-student")]
        public async Task<IActionResult> GetJournalDetailsForStudent(int journalId, int studentId)
        {
            var details = await _journalService.GetJournalDetailsForStudentAsync(journalId);
            return Ok(details);
        }
    }
}

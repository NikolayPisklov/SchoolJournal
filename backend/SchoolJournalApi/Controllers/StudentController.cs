using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SchoolJournalApi.Enum_s;
using SchoolJournalApi.Services.DbServices.Interfaces;

namespace SchoolJournalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserStatusesNames.Student)]
    public class StudentController : ControllerBase
    {
        private readonly IJournalDbService _journalDbService;
        public StudentController(IJournalDbService journalDbService) 
        {
            _journalDbService = journalDbService;
        }

        [HttpGet("get-journals-for-student")]
        public async Task<IActionResult> GetJournalsForStudent(int studentId)
        {
            var journals = await _journalDbService.GetJournalsForStudent(studentId);
            return Ok(journals);
        }
        [HttpGet("get-journal-details-for-student")]
        public async Task<IActionResult> GetJournalDetailsForStudent(int journalId, int studentId)
        {
            var details = await _journalDbService.GetJournalDetailsForStudent(journalId, studentId);
            return Ok(details);
        }
    }
}

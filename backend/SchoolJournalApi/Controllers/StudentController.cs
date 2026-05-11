using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolJournalApi.Enum_s;
using SchoolJournalApi.Services.AppServices.Interfaces;

namespace SchoolJournalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserStatusesNames.Student)]
    public class StudentController : ControllerBase
    {
        private readonly IJournalService _journalService;
        private readonly IProgressService _progressService;

        public StudentController(IJournalService journalService, IProgressService progressService) 
        {
            _progressService = progressService;
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
            var details = await _journalService.GetJournalDetailsForStudentAsync(journalId, studentId);
            return Ok(details);
        }
        [HttpGet("get-progress-history-for-student")]
        public async Task<IActionResult> GetProgressHistoryForStudent(int studentId, int lessonId)
        {
            var result = await _progressService.GetProgressHistoryForStudent(studentId, lessonId);
            return Ok(result);
        }
    }
}

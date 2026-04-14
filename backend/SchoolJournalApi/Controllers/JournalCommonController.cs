using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolJournalApi.Services.AppServices.Interfaces;
using SchoolJournalApi.Services.DbServices.Interfaces;

namespace SchoolJournalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JournalCommonController : ControllerBase
    {
        private readonly IJournalService _journalDbService;
        private readonly IProgressService _progressService;

        public JournalCommonController(IJournalService journalDbService, IProgressService progressService) 
        {
            _journalDbService = journalDbService;
            _progressService = progressService;
        }

        [HttpGet("get-journal-title")]
        public async Task<IActionResult> GetJournalTitle(int journalId) 
        {
            var dto = await _journalDbService.GetJournalTitleAsync(journalId);
            return Ok(dto);
        }
        [HttpGet("get-student-statictic")]
        public async Task<IActionResult> GetStudentStatistic(int studentId, int journalId)
        {
            var dto = await _progressService.GetStudentStatisticAsync(studentId, journalId);
            return Ok(dto);
        }
    }
}

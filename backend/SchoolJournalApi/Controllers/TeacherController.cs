using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolJournalApi.Dtos.Lesson;
using SchoolJournalApi.Dtos.Progress;
using SchoolJournalApi.Enum_s;
using SchoolJournalApi.Services.DbServices.Interfaces;

namespace SchoolJournalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{UserStatusesNames.Teacher}, {UserStatusesNames.Admin}")]
    public class TeacherController : ControllerBase
    {
        private readonly IJournalDbService _journalDbService;
        private readonly IProgressDbService _progressDbService;
        private readonly ILessonDbService _lessonDbService;

        public TeacherController(IJournalDbService journalDbService, IProgressDbService progressDbService, ILessonDbService lessonDbService) 
        {
            _lessonDbService = lessonDbService;
            _journalDbService = journalDbService;
            _progressDbService = progressDbService;
        }

        [HttpGet("get-journals-for-teacher")]
        public async Task<IActionResult> GetJournalsForTeacher(int teacherId)
        {
            var journals = await _journalDbService.GetJournalsForTeacherAsync(teacherId);
            if (journals.Count == 0)
            {
                return NotFound();
            }
            return Ok(journals);
        }
        [HttpGet("get-journal-title")]
        public async Task<IActionResult> GetJournalTitle(int journalId)
        {
            var journalTitleDto = await _journalDbService.GetJournalTitleAsync(journalId);
            return Ok(journalTitleDto);
        }
        [HttpGet("get-journal-details")]
        public async Task<IActionResult> GetJournalDetails(int journalId)
        {
            var journalDto = await _journalDbService.GetJournalDetailsAsync(journalId);
            return Ok(journalDto);
        }
        [HttpGet("get-all-marks")]
        public async Task<IActionResult> GetAllMarks()
        {
            var marks = await _progressDbService.GetAllMarksAsync();
            if (marks.Count == 0) 
            {
                return NotFound();
            }
            return Ok(marks);
        }
        [HttpGet("get-all-attendances")]
        public async Task<IActionResult> GetAllAttendances() 
        {
            var att = await _progressDbService.GetAllAttendancesAsync();
            if (att.Count == 0)
            {
                return NotFound();
            }
            return Ok(att);
        }
        [HttpPost("add-progress")]
        public async Task<IActionResult> AddProgress(AddProgressDto dto)
        {
            await _progressDbService.AddProgressAsync(dto);
            return Ok();
        }
        [HttpPut("update-progress")]
        public async Task<IActionResult> UpdateProgress(ProgressDto dto) 
        {
            await _progressDbService.UpdateProgressAsync(dto);
            return Ok();
        }
        [HttpGet("get-progresses-of-journal")]
        public async Task<IActionResult> GetProgressOfJournal(int journalId) 
        {
            var progresses = await _progressDbService.GetProgressesForJournalAsync(journalId);
            if(progresses.Count == 0) 
            { 
                return NotFound(); 
            }
            return Ok(progresses);
        }
        [HttpGet("get-progress-details")]
        public async Task<IActionResult> GetSelectedProgressInfo(int progressId) 
        {
            var dto = await _progressDbService.GetProgressDetailsAsync(progressId);
            return Ok(dto);
        }
        [HttpPost("add-lesson")]
        public async Task<IActionResult> AddLessonToJournal(AddLessonDto addLessonDto)
        {
            await _lessonDbService.AddLessonAsync(addLessonDto);
            return Ok();
        }
        [HttpGet("get-lessons-for-journal")]
        public async Task<IActionResult> GetLessonsForJournal(int journalId, int month, int journalYear)
        {
            var lessons = await _lessonDbService.GetLessonsForJournal(journalId, month, journalYear);
            return Ok(lessons);
        }
        [HttpPatch("{lessonId}")]
        public async Task<IActionResult> UpdateLessonDetails(int lessonId, LessonDetailsUpdateDto dto)
        {
            await _lessonDbService.UpdateLessonDetailsAsync(lessonId, dto);
            return Ok();
        }
        [HttpGet("get-student-statictic")]
        public async Task<IActionResult> GetStudentStatistic(int studentId, int journalId)
        {
            var result = await _progressDbService.GetStudentStatisticAsync(studentId, journalId);
            return Ok(result);
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SchoolJournalApi.Dtos.Progress
{
    public class ProgressDetailsDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LessonId { get; set; }
        public int? MarkId { get; set; }
        public int AttendanceId { get; set; }
        public DateTime ProgressUpdateDate { get; set; }
        public DateOnly LessonDate { get; set; }
        public List<JournalProgressDto> ProgressChangeHistory { get; set; } = new List<JournalProgressDto>();
    }
}

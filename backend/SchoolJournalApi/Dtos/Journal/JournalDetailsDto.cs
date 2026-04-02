using SchoolJournalApi.Dtos.Lesson;
using SchoolJournalApi.Dtos.Progress;
using SchoolJournalApi.Dtos.User;

namespace SchoolJournalApi.Dtos.Journal
{
    public class JournalDetailsDto
    {
        public List<ListedStudentDto> Students { get; set; } = new List<ListedStudentDto>();
        public List<LessonDto> Lessons { get; set; } = new List<LessonDto>();
        public List<JournalProgressDto> Progresses { get; set; } = new List<JournalProgressDto>();
        public int JournalYear { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SchoolJournalApi.Dtos.Lesson
{
    public class AddLessonDto
    {
        [Required]
        public int JournalId { get; set; }
        [Required(ErrorMessage = "Дата проведения занятия обязательна для заполнения.")]
        //LessonDate check
        public DateOnly? LessonDate { get; set; }
    }
}

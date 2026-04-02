using SchoolJournalApi.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace SchoolJournalApi.Dtos.Progress
{
    public class ProgressDto
    {
        [Required(ErrorMessage = "Уникальный идентификатор оценки обязателен для заполнения")]
        public int Id { get; set; }
        [Required(ErrorMessage = "У/И Ученика обязателен!")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "У/И Урока обязателен!")]
        public int LessonId { get; set; }
        public int? MarkId { get; set; }
        [Required(ErrorMessage = "Поле посещаемости обязательно для заполнения!")]
        public int? AttendanceId { get; set; }
        //[ProgressDate]
        public DateTime ProgressUpdateDate { get; set; }
        public DateOnly LessonDate {  get; set; }
    }
}

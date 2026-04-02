using SchoolJournalApi.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace SchoolJournalApi.Dto_s
{
    public class ClassCreationDto
    {
        [Required(ErrorMessage = "Название класса обязательно для заполнения!")]
        [RegularExpression(@"^[А-ЯЁ]$", 
            ErrorMessage = "Название класса должно состоять из одной заглавной буквы кириллицы!")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Год набора класса обязателен для заполнения!")]
        [YearRange]
        public int? Year { get; set; }
    }
}

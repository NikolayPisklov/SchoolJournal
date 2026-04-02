using System.ComponentModel.DataAnnotations;

namespace SchoolJournalApi.Dtos.User
{
    public class UserDetailsForCreationDto
    {
        [Required(ErrorMessage = "Выбор статуса пользователя обязателен!")]
        public int? StatusId { get; set; }

        [Required(ErrorMessage = "Логин обязателен для заполнения!")]
        [RegularExpression(@"^[A-Za-z0-9]{2,}$",
            ErrorMessage = "Логин должен содержать только латинские буквы и цифры. Без спец. знаков!")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имя обязательно для заполнения!")]
        [RegularExpression(@"^[а-яА-ЯёЁ]{2,}$",
            ErrorMessage = "Имя должно содержать минимум две буквы и быть составлено на кириллице")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Фамилия обязательно для заполнения!")]
        [RegularExpression(@"^[а-яА-ЯёЁ]{2,}$",
            ErrorMessage = "Фамилия должно содержать минимум две буквы и быть составлено на кириллице")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Отчество обязательно для заполнения!")]
        [RegularExpression(@"^[а-яА-ЯёЁ]{2,}$",
            ErrorMessage = "Отчество должно содержать минимум две буквы и быть составлено на кириллице")]
        public string MiddleName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен для заполнения!")]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)\S{8,}$",
            ErrorMessage = "Пароль должен быть минимум 8 символов, без пробелов, содержать одну заглавную букву и одну цифру"
        )]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Эл. почта обязательна для заполнения!")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Адрес электронной почты должен содержать символ '@' и домен (например, example@mail.com")]
        public string Email { get; set; } = string.Empty;
    }
}

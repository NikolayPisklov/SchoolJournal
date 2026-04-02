using System.ComponentModel.DataAnnotations;

namespace SchoolJournalApi.ValidationAttributes
{
    public class YearRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Год обязателен");

            if (!int.TryParse(value.ToString(), out int year))
                return new ValidationResult("Некорректный год");

            int currentYear = DateTime.Now.Year;
            int minYear = currentYear - 11;

            if (year < minYear || year > currentYear)
            {
                return new ValidationResult(
                    $"Год должен быть в диапазоне {minYear} - {currentYear}");
            }

            return ValidationResult.Success;
        }
    }
}

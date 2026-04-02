using SchoolJournalApi.Dtos.Lesson;
using SchoolJournalApi.Dtos.Progress;
using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolJournalApi.ValidationAttributes
{
    public class ProgressDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) 
        {
            DateOnly lessonDate = new DateOnly();
            if (value is null)
            {
                return new ValidationResult("Дата обновления успеваемости обязательна!");
            }
            if (!DateTime.TryParse(value.ToString(), out var updateDate))
            {
                return new ValidationResult("Некорректное время!");
            }
            var type = validationContext.ObjectType;
            if (type.Equals(typeof(ProgressDto))) 
            {
                var instance = (LessonDto)validationContext.ObjectInstance;
                lessonDate = instance.LessonDate;
                if (DateOnly.FromDateTime(updateDate) > lessonDate.AddDays(30))
                {
                    return new ValidationResult("Время редактирования успеваемости за этот урок истекло!");
                }
                if (DateOnly.FromDateTime(updateDate) < lessonDate)
                {
                    return new ValidationResult("Нельзя редактировать успеваемость до проведения урока.");
                }
            }
            else if(type.Equals(typeof(AddProgressDto)))
            {
                var instance = (AddProgressDto)validationContext.ObjectInstance;
                lessonDate = instance.LessonDate;
                if (DateOnly.FromDateTime(updateDate) > lessonDate.AddDays(30))
                {
                    return new ValidationResult("Время редактирования успеваемости за этот урок истекло!");
                }
                if (DateOnly.FromDateTime(updateDate) < lessonDate)
                {
                    return new ValidationResult("Нельзя редактировать успеваемость до проведения урока.");
                }
            }             
            return ValidationResult.Success;
        }
    }
}

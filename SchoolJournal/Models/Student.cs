using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolJournal.Models
{
    public partial class Student
    {
        public Student()
        {
            Progresses = new HashSet<Progress>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Будь ласка, оберіть клас!")]
        public int? FkClass { get; set; }
        [Required(ErrorMessage = "Будь ласка, введіть логін учня!")]
        public string Login { get; set; } = null!;
        [Required(ErrorMessage = "Будь ласка, введіть пароль учня!")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Будь ласка, введіть e-mail одного з батьків!")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email адреса введена неправильно!")]
        public string ParrentEmail { get; set; } = null!;
        [Required(ErrorMessage = "Будь ласка, введіть ім'я учня!")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Будь ласка, введіть прізвище учня!")]
        public string Surname { get; set; } = null!;
        [Required(ErrorMessage = "Будь ласка, введіть по-батькові учня!")]
        public string Middlename { get; set; } = null!;

        public virtual Class? FkClassNavigation { get; set; }
        public virtual ICollection<Progress> Progresses { get; set; }
    }
}

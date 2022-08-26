using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolJournal.Models
{
    public partial class Teacher
    {
        public Teacher()
        {
            Journals = new HashSet<Journal>();
        }

        public int Id { get; set; }
        [Required (ErrorMessage = "Будь ласка, введіть логін!")]
        public string Login { get; set; } = null!;
        [Required(ErrorMessage = "Будь ласка, введіть пароль!")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Будь ласка, введіть ім'я!")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Будь ласка, введіть прізвище!")]
        public string Surname { get; set; } = null!;
        [Required(ErrorMessage = "Будь ласка, введіть по-батькові!")]
        public string Middlename { get; set; } = null!;
        [Required(ErrorMessage = "Будь ласка, введіть дату найму!")]
        public DateTime? HireDate { get; set; }
        public DateTime? FireDate { get; set; }

        public virtual ICollection<Journal> Journals { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolJournal.Models
{
    public partial class TeacherSubject
    {
        public TeacherSubject()
        {
            Journals = new HashSet<Journal>();
        }

        public int Id { get; set; }
        public int FkTeacher { get; set; }
        [Required(ErrorMessage = "Будь ласка, оберіть предмет!")]
        public int? FkSubject { get; set; }

        public virtual Subject FkSubjectNavigation { get; set; } = null!;
        public virtual Teacher FkTeacherNavigation { get; set; } = null!;
        public virtual ICollection<Journal> Journals { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolJournal.Models
{
    public partial class Lesson
    {
        public Lesson()
        {
            Progresses = new HashSet<Progress>();
        }

        public int Id { get; set; }
        [Required (ErrorMessage = "Будь ласка, оберіть журнал!")]
        public int? FkJournal { get; set; }
        public int FkLessonTime { get; set; }
        public DateTime Date { get; set; }
        public string? Theme { get; set; } 
        public string? Homework { get; set; }

        public virtual Journal? FkJournalNavigation { get; set; }
        public virtual LessonTime? FkLessonTimeNavigation { get; set; }
        public virtual ICollection<Progress> Progresses { get; set; }
    }
}

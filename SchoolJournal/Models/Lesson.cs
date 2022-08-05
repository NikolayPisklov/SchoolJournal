using System;
using System.Collections.Generic;

namespace SchoolJournal.Models
{
    public partial class Lesson
    {
        public Lesson()
        {
            Progresses = new HashSet<Progress>();
        }

        public int Id { get; set; }
        public int FkJournal { get; set; }
        public int FkLessonTime { get; set; }
        public DateTime Date { get; set; }
        public string Theme { get; set; } = null!;
        public string Homework { get; set; } = null!;

        public virtual Journal FkJournalNavigation { get; set; } = null!;
        public virtual LessonTime FkLessonTimeNavigation { get; set; } = null!;
        public virtual ICollection<Progress> Progresses { get; set; }
    }
}

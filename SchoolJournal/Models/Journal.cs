using System;
using System.Collections.Generic;

namespace SchoolJournal.Models
{
    public partial class Journal
    {
        public Journal()
        {
            Lessons = new HashSet<Lesson>();
        }

        public int Id { get; set; }
        public int FkSubject { get; set; }
        public int FkClass { get; set; }
        public int FkTeacher { get; set; }
        public int FkSchoolYear { get; set; }

        public virtual Class FkClassNavigation { get; set; } = null!;
        public virtual SchoolYear FkSchoolYearNavigation { get; set; } = null!;
        public virtual Subject FkSubjectNavigation { get; set; } = null!;
        public virtual Teacher FkTeacherNavigation { get; set; } = null!;
        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}

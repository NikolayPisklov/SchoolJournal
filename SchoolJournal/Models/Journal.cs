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
        public int FkClass { get; set; }
        public int FkTeacherSubject { get; set; }
        public int FkSchoolYear { get; set; }

        public virtual Class FkClassNavigation { get; set; } = null!;
        public virtual SchoolYear FkSchoolYearNavigation { get; set; } = null!;
        public virtual TeacherSubject FkTeacherSubjectNavigation { get; set; } = null!;
        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}

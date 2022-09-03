using System;
using System.Collections.Generic;

namespace SchoolJournal.Models
{
    public partial class Subject
    {
        public Subject()
        {
            TeacherSubjects = new HashSet<TeacherSubject>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool IsBeginner { get; set; }
        public bool IsMiddle { get; set; }
        public bool IsSenior { get; set; }

        public virtual ICollection<TeacherSubject> TeacherSubjects { get; set; }
    }
}

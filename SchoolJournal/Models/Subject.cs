using System;
using System.Collections.Generic;

namespace SchoolJournal.Models
{
    public partial class Subject
    {
        public Subject()
        {
            Journals = new HashSet<Journal>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool IsBeginner { get; set; }
        public bool IsMiddle { get; set; }
        public bool IsSenior { get; set; }

        public virtual ICollection<Journal> Journals { get; set; }
    }
}

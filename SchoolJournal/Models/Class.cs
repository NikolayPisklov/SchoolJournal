using System;
using System.Collections.Generic;

namespace SchoolJournal.Models
{
    public partial class Class
    {
        public Class()
        {
            Journals = new HashSet<Journal>();
            Students = new HashSet<Student>();
        }

        public int Id { get; set; }
        public int FkClassRank { get; set; }
        public string Title { get; set; } = null!;
        public DateTime RecruitmentDate { get; set; }
        public DateTime ReleaseDate { get; set; }

        public virtual ClassRank FkClassRankNavigation { get; set; } = null!;
        public virtual ICollection<Journal> Journals { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}

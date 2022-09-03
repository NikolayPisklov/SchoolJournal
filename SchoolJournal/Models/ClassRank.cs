using System;
using System.Collections.Generic;

namespace SchoolJournal.Models
{
    public partial class ClassRank
    {
        public ClassRank()
        {
            Classes = new HashSet<Class>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;

        public virtual ICollection<Class> Classes { get; set; }
    }
}

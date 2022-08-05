using System;
using System.Collections.Generic;

namespace SchoolJournal.Models
{
    public partial class Student
    {
        public Student()
        {
            Progresses = new HashSet<Progress>();
        }

        public int Id { get; set; }
        public int FkClass { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ParrentEmail { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Middlename { get; set; } = null!;

        public virtual Class FkClassNavigation { get; set; } = null!;
        public virtual ICollection<Progress> Progresses { get; set; }
    }
}

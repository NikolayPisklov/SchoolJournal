using System;
using System.Collections.Generic;

namespace SchoolJournal.Models;

public partial class TeacherSubject
{
    public int FkTeacher { get; set; }

    public int FkSubject { get; set; }

    public virtual Subject FkSubjectNavigation { get; set; } = null!;

    public virtual User FkTeacherNavigation { get; set; } = null!;

    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();
}

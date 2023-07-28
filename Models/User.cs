using System;
using System.Collections.Generic;

namespace SchoolJournal.Models;

public partial class User
{
    public int Id { get; set; }

    public int FkStatus { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Middlename { get; set; } = null!;

    public virtual Status FkStatusNavigation { get; set; } = null!;

    public virtual ICollection<PersonClass> PersonClasses { get; set; } = new List<PersonClass>();

    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();

    public virtual ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
}

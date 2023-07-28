using System;
using System.Collections.Generic;

namespace SchoolJournal.Models;

public partial class PersonClass
{
    public int Id { get; set; }

    public int FkClass { get; set; }

    public int FkPerson { get; set; }

    public virtual Class FkClassNavigation { get; set; } = null!;

    public virtual User FkPersonNavigation { get; set; } = null!;
}

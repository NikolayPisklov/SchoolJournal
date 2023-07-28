using System;
using System.Collections.Generic;

namespace SchoolJournal.Models;

public partial class SchoolYear
{
    public int Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();
}

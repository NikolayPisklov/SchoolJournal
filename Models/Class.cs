using System;
using System.Collections.Generic;

namespace SchoolJournal.Models;

public partial class Class
{
    public int Id { get; set; }

    public int FkRank { get; set; }

    public int Number { get; set; }

    public string Title { get; set; } = null!;

    public virtual Rank FkRankNavigation { get; set; } = null!;

    public virtual ICollection<Journal> Journals { get; set; } = new List<Journal>();

    public virtual ICollection<PersonClass> PersonClasses { get; set; } = new List<PersonClass>();
}

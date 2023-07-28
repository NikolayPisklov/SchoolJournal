using System;
using System.Collections.Generic;

namespace SchoolJournal.Models;

public partial class Mark
{
    public int Id { get; set; }

    public int Value { get; set; }

    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
}

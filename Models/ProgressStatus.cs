using System;
using System.Collections.Generic;

namespace SchoolJournal.Models;

public partial class ProgressStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
}

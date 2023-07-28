using System;
using System.Collections.Generic;

namespace SchoolJournal.Models;

public partial class Rank
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}

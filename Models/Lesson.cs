using System;
using System.Collections.Generic;

namespace SchoolJournal.Models;

public partial class Lesson
{
    public int Id { get; set; }

    public int FkJournal { get; set; }

    public DateTime LessonDateTime { get; set; }

    public string Theme { get; set; } = null!;

    public string Homework { get; set; } = null!;

    public string Link { get; set; } = null!;

    public virtual Journal FkJournalNavigation { get; set; } = null!;

    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
}

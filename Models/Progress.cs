using System;
using System.Collections.Generic;

namespace SchoolJournal.Models;

public partial class Progress
{
    public int Id { get; set; }

    public int FkStudent { get; set; }

    public int FkMark { get; set; }

    public int FkAttendance { get; set; }

    public int FkLesson { get; set; }

    public int FkProgressStatus { get; set; }

    public DateTime ProgressDateTime { get; set; }

    public virtual Attendance FkAttendanceNavigation { get; set; } = null!;

    public virtual Lesson FkLessonNavigation { get; set; } = null!;

    public virtual Mark FkMarkNavigation { get; set; } = null!;

    public virtual ProgressStatus FkProgressStatusNavigation { get; set; } = null!;

    public virtual User FkStudentNavigation { get; set; } = null!;
}

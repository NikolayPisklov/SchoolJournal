using System;
using System.Collections.Generic;

namespace SchoolJournal.Models
{
    public partial class Progress
    {
        public int Id { get; set; }
        public int FkStudent { get; set; }
        public int FkMark { get; set; }
        public int FkLesson { get; set; }

        public virtual Lesson FkLessonNavigation { get; set; } = null!;
        public virtual Mark FkMarkNavigation { get; set; } = null!;
        public virtual Student FkStudentNavigation { get; set; } = null!;
    }
}

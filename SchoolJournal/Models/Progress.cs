using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolJournal.Models
{
    public partial class Progress
    {
        public int Id { get; set; }
        public int FkStudent { get; set; }
        [Required(ErrorMessage = "Будь ласка, оберіть оцінку!")]
        public int? FkMark { get; set; }
        public int FkLesson { get; set; }

        public virtual Lesson? FkLessonNavigation { get; set; } 
        public virtual Mark? FkMarkNavigation { get; set; }
        public virtual Student? FkStudentNavigation { get; set; }
    }
}

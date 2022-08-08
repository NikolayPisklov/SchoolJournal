using Microsoft.EntityFrameworkCore;

namespace SchoolJournal.ViewModels
{
    public class JournalListContent
    {
        public Journal Journal { get; set; }
        public Teacher Teacher { get; set; }
        public Subject Subject { get; set; }
        public Class Class { get; set; } 

    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolJournal.ViewModels;

namespace SchoolJournal.Classes
{
    public class JournalsSelectList : SchoolSelectList
    {
        public IQueryable<JournalContent> JournalContents { get; set; }

        public JournalsSelectList(IQueryable<JournalContent> journalContents)
        {
            JournalContents = journalContents;
            foreach (JournalContent jc in JournalContents) 
            {
                KeyValuePairs.Add(jc.Journal.Id,
                    $"{jc.Subject.Title} - {jc.Teacher.Surname} {jc.Teacher.Name[0]}. {jc.Teacher.Middlename[0]}.");
            }
        }
    }
}

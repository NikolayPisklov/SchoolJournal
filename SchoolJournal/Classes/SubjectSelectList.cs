using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolJournal.Interfaces;

namespace SchoolJournal.Classes
{
    public class SubjectSelectList : SchoolSelectList
    {
        public IQueryable<Subject> Subjects { get; set; }

        public SubjectSelectList(IQueryable<Subject> subjects) 
        {
            Subjects = subjects; 
            foreach(Subject s in Subjects) 
            {
                KeyValuePairs.Add(s.Id, s.Title);
            }
        }
    }
}

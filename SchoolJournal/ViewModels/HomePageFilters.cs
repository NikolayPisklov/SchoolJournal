using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolJournal.ViewModels
{
    public class HomePageFilters
    {
        public Dictionary<int, string> ClassRangs;
        public Dictionary<int, string> Subjects;

        public HomePageFilters(List<Subject> subjects) 
        {
            ClassRangs = new Dictionary<int, string> {
                { 1, "Початкові класи"},
                { 2, "Середні класи"},
                { 3, "Старші класи"}
            };
            Subjects = new Dictionary<int, string>();
            foreach (Subject subject in subjects) 
            {
                Subjects.Add(subject.Id, subject.Title);
            }
        }

        public SelectList GetClassRangsSelecteList() 
        {
            return new SelectList(ClassRangs, "Key", "Value");
        }
        public SelectList GetSubjectsSelectList() 
        {
            return new SelectList(Subjects, "Key", "Value");
        }
    }
}

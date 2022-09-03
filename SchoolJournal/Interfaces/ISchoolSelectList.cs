using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolJournal.Interfaces
{
    public interface ISchoolSelectList
    {
        public Dictionary<int, string> KeyValuePairs { get; set; }

        public SelectList GetSelectList();
    }
}

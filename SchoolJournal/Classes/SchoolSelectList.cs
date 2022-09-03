using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolJournal.Classes
{
    public abstract class SchoolSelectList
    {
        public Dictionary<int, string> KeyValuePairs { get; set; } = new Dictionary<int, string>();

        public SelectList GetSelectList() 
        {
            return new SelectList(KeyValuePairs, "Key", "Value");
        }
    }
}

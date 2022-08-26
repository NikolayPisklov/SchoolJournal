namespace SchoolJournal.ViewModels
{
    public class SearchString
    {
        public string? SearchValue { get; set; } = "";

        public SearchString() { }
        public SearchString(string searchString)
        {
            SearchValue = searchString;
        }
    }
}

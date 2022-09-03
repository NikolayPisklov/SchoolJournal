namespace SchoolJournal.ViewModels
{
    public enum SubjectsRank 
    {
        None,
        Beginner,
        Middle,
        Senior
    }
    public class SubjectsList
    {
        public List<Subject> Subjects { get; set; }
        private SubjectsRank _rank { get; set; }

        public SubjectsList(List<Subject> subjects, SubjectsRank rank)
        {
            Subjects = subjects;
            _rank = rank;
        }
        public List<Subject> GetSubjectsByRank() 
        {
            if (_rank.Equals(SubjectsRank.Beginner))
            {
                return Subjects.Where(s => s.IsBeginner == true).ToList();
            }
            else if (_rank.Equals(SubjectsRank.Middle))
            {
                return Subjects.Where(s => s.IsMiddle == true).ToList();
            }
            else if (_rank.Equals(SubjectsRank.Senior))
            {
                return Subjects.Where(s => s.IsSenior == true).ToList();
            }
            else
            {
                return Subjects;
            }
        }
    }
}

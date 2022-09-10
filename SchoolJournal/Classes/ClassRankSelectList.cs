namespace SchoolJournal.Classes
{
    public class ClassRankSelectList : SchoolSelectList
    {
        public IQueryable<ClassRank> ClassRanks { get; set; }

        public ClassRankSelectList(IQueryable<ClassRank> classRanks)
        {
            ClassRanks = classRanks;
            foreach (ClassRank c in ClassRanks) 
            {
                KeyValuePairs.Add(c.Id, c.Title);
            }
        }
    }
}

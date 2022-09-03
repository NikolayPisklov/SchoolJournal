using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolJournal.ViewModels
{
    public class JournalsFilter
    {
        public List<Journal> Journals;
        public Dictionary<int, string> ClassRangs;
        public Dictionary<int, string> Subjects;

        public JournalsFilter(SchoolJournalContext db)
        {
            List<Journal> journals = db.Journals.Where(j => j.FkSchoolYearNavigation.Id ==
                SchoolDateTime.GetCurrentYearId(db))
                .OrderBy(j => j.FkClassNavigation.Title.Length)
                .ThenBy(j => j.FkClassNavigation.Title).ToList();
            List<Subject> subjects = db.Subjects.OrderBy(s => s.Title).ToList();
            List<ClassRank> ranks = db.ClassRanks.ToList();
            ClassRangs = new Dictionary<int, string>();
            foreach (ClassRank r in ranks)
            {
                ClassRangs.Add(r.Id, r.Title);
            }
            Subjects = new Dictionary<int, string>();
            foreach (Subject s in subjects)
            {
                Subjects.Add(s.Id, s.Title);
            }
            Journals = journals;
        }
        public JournalsFilter(List<Subject> subjects, List<ClassRank> ranks)
        {
            ClassRangs = new Dictionary<int, string>();
            foreach (ClassRank r in ranks)
            {
                ClassRangs.Add(r.Id, r.Title);
            }
            Subjects = new Dictionary<int, string>();
            foreach (Subject s in subjects)
            {
                Subjects.Add(s.Id, s.Title);
            }
        }

        public List<Journal> FilterJournals(int? subjectId, int? rankId) 
        {
            if (subjectId != null && rankId == null)
            {
                return Journals.Where(j => j.FkTeacherSubjectNavigation.FkSubject == subjectId).ToList();
            }
            else if (subjectId == null && rankId != null)
            {
                return Journals.Where(j => j.FkClassNavigation.FkClassRank == rankId).ToList();
            }
            else if (subjectId != null && rankId != null)
            {
                var journalsTmp = Journals.Where(j => j.FkTeacherSubjectNavigation.FkSubject == subjectId).ToList();
                return journalsTmp.Where(j => j.FkClassNavigation.FkClassRank == rankId).ToList();
            }
            else
            {
                return Journals;
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

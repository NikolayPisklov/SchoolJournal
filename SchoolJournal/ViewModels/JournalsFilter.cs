using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolJournal.ViewModels
{
    public class JournalsFilter
    {
        public List<Journal> Journals;

        public JournalsFilter(List<Journal> journals)
        {
            Journals = journals;
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
    }
}

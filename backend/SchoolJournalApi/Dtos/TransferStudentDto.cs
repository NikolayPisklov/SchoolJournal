namespace SchoolJournalApi.Dto_s
{
    public class TransferStudentDto
    {
        //int studentId, int newClassId, int? oldClassId
        public int StudentId { get; set; }
        public int NewClassId { get; set; }
        public int? OldClassId { get; set; }
    }
}

namespace SchoolJournalApi.Models
{
    public class StudentClass
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public bool IsActive { get; set; }

        public User Student {  get; set; } = null!;
        public Class Class {  get; set; } = null!;
    }
}

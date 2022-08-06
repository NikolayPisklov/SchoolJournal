namespace SchoolJournal.ViewModels
{
    public class User
    {
        public int Id { get; set; }
        public int? FkClass { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? ParrentEmail { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Middlename { get; set; } = null!;
        public DateTime? HireDate { get; set; }
        public DateTime? FireDate { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SchoolJournal.Models
{
    public partial class Administrator
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Middlename { get; set; } = null!;
        public DateTime HireDate { get; set; }
        public DateTime FireDate { get; set; }
    }
}

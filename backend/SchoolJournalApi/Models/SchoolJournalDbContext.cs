using Microsoft.EntityFrameworkCore;

namespace SchoolJournalApi.Models
{
    public class SchoolJournalDbContext : DbContext
    {
        public SchoolJournalDbContext(DbContextOptions<SchoolJournalDbContext> options) : base(options) 
        {
        } 
        public SchoolJournalDbContext() 
        {
        }

        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<EducationalLevel> EducationalLevels { get; set; }
        public DbSet<Journal> Journals { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Progress> Progresses { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }
        public DbSet<User> Users { get; set; }

        //Restrict on delete 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Attendance
            modelBuilder.Entity<Attendance>()
                .HasMany(a => a.Progresses)
                .WithOne(p => p.Attendance)
                .HasForeignKey(p => p.AttendanceId)
                .OnDelete(DeleteBehavior.Restrict);

            //Class
            modelBuilder.Entity<Class>()
                .HasOne(c => c.EducationalLevel)
                .WithMany(e => e.Classes)
                .HasForeignKey(c => c.EducationalLevelId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Class>()
                .HasMany(c => c.StudentClasses)
                .WithOne(s => s.Class)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Class>()
                .HasMany(c => c.Journals)
                .WithOne(j => j.Class)
                .HasForeignKey(j => j.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            //Level
            modelBuilder.Entity<EducationalLevel>()
                .HasMany(e => e.Subjects)
                .WithOne(s => s.EducationalLevel)
                .HasForeignKey(s => s.EducationalLevelId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<EducationalLevel>()
                .HasMany(e => e.Classes)
                .WithOne(c => c.EducationalLevel)
                .HasForeignKey(c => c.EducationalLevelId)
                .OnDelete(DeleteBehavior.Restrict);

            //Journal
            modelBuilder.Entity<Journal>()
                .HasMany(j => j.Lessons)
                .WithOne(l => l.Journal)
                .HasForeignKey(l => l.JournalId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Journal>()
                .HasOne(j => j.TeacherSubject)
                .WithMany(t => t.Journals)
                .HasForeignKey(j => j.TeacherSubjectId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Journal>()
                .HasOne(j => j.Class)
                .WithMany(c => c.Journals)
                .HasForeignKey(j => j.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            //Lesson
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Journal)
                .WithMany(j => j.Lessons)
                .HasForeignKey(l => l.JournalId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Lesson>()
                .HasMany(l => l.Progresses)
                .WithOne(p => p.Lesson)
                .HasForeignKey(p => p.LessonId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Lesson>().Property(l => l.Homework).HasDefaultValue("");
            modelBuilder.Entity<Lesson>().Property(l => l.Theme).HasDefaultValue("");
            modelBuilder.Entity<Lesson>().Property(l => l.IsDeleted).HasDefaultValue(false);

            //Mark
            modelBuilder.Entity<Mark>()
                .HasMany(m => m.Progresses)
                .WithOne(p => p.Mark)
                .HasForeignKey(p => p.MarkId)
                .OnDelete(DeleteBehavior.Restrict);

            //Progress
            modelBuilder.Entity<Progress>()
                .HasOne(p => p.Mark)
                .WithMany(m => m.Progresses)
                .HasForeignKey(p => p.MarkId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Progress>()
                .HasOne(p => p.Student)
                .WithMany(u => u.Progresses)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Progress>()
                .HasOne(p => p.Lesson)
                .WithMany(l => l.Progresses)
                .HasForeignKey(p => p.LessonId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Progress>()
                .HasOne(p => p.Attendance)
                .WithMany(a => a.Progresses)
                .HasForeignKey(p => p.AttendanceId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Progress>().Property(t => t.IsUpdated).HasDefaultValue(false);

            //Status
            modelBuilder.Entity<Status>()
                .HasMany(s => s.Users)
                .WithOne(u => u.Status)
                .HasForeignKey(u => u.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            //StudentClass
            modelBuilder.Entity<StudentClass>()
                .HasOne(s => s.Student)
                .WithMany(u => u.StudentClasses)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StudentClass>()
                .HasOne(s => s.Class)
                .WithMany(c => c.StudentClasses)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StudentClass>()
                .HasIndex(s => new { s.UserId, s.ClassId })
                .IsUnique();

            //Subject
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.EducationalLevel)
                .WithMany(e => e.Subjects)
                .HasForeignKey(s => s.EducationalLevelId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Subject>()
                .HasMany(s => s.TeacherSubjects)
                .WithOne(t => t.Subject)
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            //TeacherSubject
            modelBuilder.Entity<TeacherSubject>()
                .HasOne(t => t.Subject)
                .WithMany(s => s.TeacherSubjects)
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TeacherSubject>()
                .HasOne(t => t.Teacher)
                .WithMany(u => u.TeacherSubjects)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TeacherSubject>()
                .HasIndex(t => new { t.UserId, t.SubjectId })
                .IsUnique();

            //User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Status)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>()
                .HasMany(u => u.Progresses)
                .WithOne(p => p.Student)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>()
                .HasMany(u => u.TeacherSubjects)
                .WithOne(t => t.Teacher)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>()
                .HasMany(u => u.StudentClasses)
                .WithOne(s => s.Student)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().Property(x => x.LastName).HasMaxLength(100);
            modelBuilder.Entity<User>().HasIndex(x => x.LastName).HasDatabaseName("IX_Users_LastName");
            modelBuilder.Entity<User>().HasIndex(x => x.FirstName).HasDatabaseName("IX_Users_FirstName");
            modelBuilder.Entity<User>().HasIndex(x => x.MiddleName).HasDatabaseName("IX_Users_MiddleName");
        }
    }
}

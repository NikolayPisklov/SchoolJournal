using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SchoolJournal.Models;

public partial class SchoolJournalContext : DbContext
{
    public SchoolJournalContext()
    {
    }

    public SchoolJournalContext(DbContextOptions<SchoolJournalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Journal> Journals { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Mark> Marks { get; set; }

    public virtual DbSet<PersonClass> PersonClasses { get; set; }

    public virtual DbSet<Progress> Progresses { get; set; }

    public virtual DbSet<ProgressStatus> ProgressStatuses { get; set; }

    public virtual DbSet<Rank> Ranks { get; set; }

    public virtual DbSet<SchoolYear> SchoolYears { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<TeacherSubject> TeacherSubjects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=NIKOLAY;Database=SchoolJournal;Trusted_Connection=True; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_100_CI_AS_SC_UTF8");

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.ToTable("Attendance");

            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("Class");

            entity.Property(e => e.FkRank).HasColumnName("FK_Rank");
            entity.Property(e => e.Number).HasDefaultValueSql("((1))");
            entity.Property(e => e.Title)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.FkRankNavigation).WithMany(p => p.Classes)
                .HasForeignKey(d => d.FkRank)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rank_Class");
        });

        modelBuilder.Entity<Journal>(entity =>
        {
            entity.ToTable("Journal");

            entity.Property(e => e.FkClass).HasColumnName("FK_Class");
            entity.Property(e => e.FkSchoolYear).HasColumnName("FK_SchoolYear");
            entity.Property(e => e.FkSubject).HasColumnName("FK_Subject");
            entity.Property(e => e.FkTeacher).HasColumnName("FK_Teacher");

            entity.HasOne(d => d.FkClassNavigation).WithMany(p => p.Journals)
                .HasForeignKey(d => d.FkClass)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Class_Journal");

            entity.HasOne(d => d.FkSchoolYearNavigation).WithMany(p => p.Journals)
                .HasForeignKey(d => d.FkSchoolYear)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SchoolYear_Journal");

            entity.HasOne(d => d.Fk).WithMany(p => p.Journals)
                .HasForeignKey(d => new { d.FkTeacher, d.FkSubject })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TS_Journal");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.ToTable("Lesson");

            entity.Property(e => e.FkJournal).HasColumnName("FK_Journal");
            entity.Property(e => e.Homework)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");
            entity.Property(e => e.LessonDateTime).HasColumnType("datetime");
            entity.Property(e => e.Link)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");
            entity.Property(e => e.Theme)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            entity.HasOne(d => d.FkJournalNavigation).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.FkJournal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Journal_Lesson");
        });

        modelBuilder.Entity<Mark>(entity =>
        {
            entity.ToTable("Mark");
        });

        modelBuilder.Entity<PersonClass>(entity =>
        {
            entity.ToTable("PersonClass");

            entity.Property(e => e.FkClass).HasColumnName("FK_Class");
            entity.Property(e => e.FkPerson).HasColumnName("FK_Person");

            entity.HasOne(d => d.FkClassNavigation).WithMany(p => p.PersonClasses)
                .HasForeignKey(d => d.FkClass)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Class_PC");

            entity.HasOne(d => d.FkPersonNavigation).WithMany(p => p.PersonClasses)
                .HasForeignKey(d => d.FkPerson)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_PC");
        });

        modelBuilder.Entity<Progress>(entity =>
        {
            entity.ToTable("Progress");

            entity.Property(e => e.FkAttendance).HasColumnName("FK_Attendance");
            entity.Property(e => e.FkLesson).HasColumnName("FK_Lesson");
            entity.Property(e => e.FkMark).HasColumnName("FK_Mark");
            entity.Property(e => e.FkProgressStatus).HasColumnName("FK_ProgressStatus");
            entity.Property(e => e.FkStudent).HasColumnName("FK_Student");
            entity.Property(e => e.ProgressDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.FkAttendanceNavigation).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.FkAttendance)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_Progress");

            entity.HasOne(d => d.FkLessonNavigation).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.FkLesson)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lesson_Progress");

            entity.HasOne(d => d.FkMarkNavigation).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.FkMark)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mark_Progress");

            entity.HasOne(d => d.FkProgressStatusNavigation).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.FkProgressStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProgressStatus_Progress");

            entity.HasOne(d => d.FkStudentNavigation).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.FkStudent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_Progress");
        });

        modelBuilder.Entity<ProgressStatus>(entity =>
        {
            entity.ToTable("ProgressStatus");

            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Rank>(entity =>
        {
            entity.ToTable("Rank");

            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SchoolYear>(entity =>
        {
            entity.ToTable("SchoolYear");

            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.StartDate).HasColumnType("date");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.ToTable("Subject");

            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TeacherSubject>(entity =>
        {
            entity.HasKey(e => new { e.FkTeacher, e.FkSubject });

            entity.ToTable("TeacherSubject");

            entity.Property(e => e.FkTeacher).HasColumnName("FK_Teacher");
            entity.Property(e => e.FkSubject).HasColumnName("FK_Subject");

            entity.HasOne(d => d.FkSubjectNavigation).WithMany(p => p.TeacherSubjects)
                .HasForeignKey(d => d.FkSubject)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subject_TS");

            entity.HasOne(d => d.FkTeacherNavigation).WithMany(p => p.TeacherSubjects)
                .HasForeignKey(d => d.FkTeacher)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Teacher_TS");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Login, "UQ__User__5E55825BF1096E1A").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FkStatus).HasColumnName("FK_Status");
            entity.Property(e => e.Login)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Middlename)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Surname)
                .HasMaxLength(250)
                .IsUnicode(false);

            entity.HasOne(d => d.FkStatusNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.FkStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Status_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

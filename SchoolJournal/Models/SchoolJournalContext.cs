using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SchoolJournal.Models
{
    public partial class SchoolJournalContext : DbContext
    {
        public SchoolJournalContext()
        {
        }

        public SchoolJournalContext(DbContextOptions<SchoolJournalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrators { get; set; } = null!;
        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<ClassRank> ClassRanks { get; set; } = null!;
        public virtual DbSet<Journal> Journals { get; set; } = null!;
        public virtual DbSet<Lesson> Lessons { get; set; } = null!;
        public virtual DbSet<LessonTime> LessonTimes { get; set; } = null!;
        public virtual DbSet<Mark> Marks { get; set; } = null!;
        public virtual DbSet<Progress> Progresses { get; set; } = null!;
        public virtual DbSet<SchoolYear> SchoolYears { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<Teacher> Teachers { get; set; } = null!;
        public virtual DbSet<TeacherSubject> TeacherSubjects { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Latin1_General_100_CI_AS_SC_UTF8");

            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.ToTable("Administrator");

                entity.Property(e => e.FireDate).HasColumnType("date");

                entity.Property(e => e.HireDate).HasColumnType("date");

                entity.Property(e => e.Login)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Middlename)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Surname)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("Class");

                entity.Property(e => e.FkClassRank).HasColumnName("FK_ClassRank");

                entity.Property(e => e.RecruitmentDate).HasColumnType("date");

                entity.Property(e => e.ReleaseDate).HasColumnType("date");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.FkClassRankNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.FkClassRank)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rank_Class");
            });

            modelBuilder.Entity<ClassRank>(entity =>
            {
                entity.ToTable("ClassRank");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Journal>(entity =>
            {
                entity.ToTable("Journal");

                entity.Property(e => e.FkClass).HasColumnName("FK_Class");

                entity.Property(e => e.FkSchoolYear).HasColumnName("FK_SchoolYear");

                entity.Property(e => e.FkTeacherSubject).HasColumnName("FK_Teacher_Subject");

                entity.HasOne(d => d.FkClassNavigation)
                    .WithMany(p => p.Journals)
                    .HasForeignKey(d => d.FkClass)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Class_Journal");

                entity.HasOne(d => d.FkSchoolYearNavigation)
                    .WithMany(p => p.Journals)
                    .HasForeignKey(d => d.FkSchoolYear)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SchoolYear_Journal");

                entity.HasOne(d => d.FkTeacherSubjectNavigation)
                    .WithMany(p => p.Journals)
                    .HasForeignKey(d => d.FkTeacherSubject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TS_Journal");
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.ToTable("Lesson");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.FkJournal).HasColumnName("FK_Journal");

                entity.Property(e => e.FkLessonTime).HasColumnName("FK_LessonTime");

                entity.Property(e => e.Homework)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Theme)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.HasOne(d => d.FkJournalNavigation)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.FkJournal)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Journal_Lesson");

                entity.HasOne(d => d.FkLessonTimeNavigation)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.FkLessonTime)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LessonTime_Lesson");
            });

            modelBuilder.Entity<LessonTime>(entity =>
            {
                entity.ToTable("LessonTime");

                entity.Property(e => e.EndTime)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.StartTime)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Mark>(entity =>
            {
                entity.ToTable("Mark");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Progress>(entity =>
            {
                entity.ToTable("Progress");

                entity.Property(e => e.FkLesson).HasColumnName("FK_Lesson");

                entity.Property(e => e.FkMark).HasColumnName("FK_Mark");

                entity.Property(e => e.FkStudent).HasColumnName("FK_Student");

                entity.HasOne(d => d.FkLessonNavigation)
                    .WithMany(p => p.Progresses)
                    .HasForeignKey(d => d.FkLesson)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lesson_Progress");

                entity.HasOne(d => d.FkMarkNavigation)
                    .WithMany(p => p.Progresses)
                    .HasForeignKey(d => d.FkMark)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Mark_Progress");

                entity.HasOne(d => d.FkStudentNavigation)
                    .WithMany(p => p.Progresses)
                    .HasForeignKey(d => d.FkStudent)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Student_Progress");
            });

            modelBuilder.Entity<SchoolYear>(entity =>
            {
                entity.ToTable("SchoolYear");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.HasIndex(e => e.Login, "UQ__Student__5E55825B2FD56674")
                    .IsUnique();

                entity.Property(e => e.FkClass).HasColumnName("FK_Class");

                entity.Property(e => e.Login)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Middlename)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ParrentEmail)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Surname)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.FkClassNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.FkClass)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Class_Student");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Subject");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teacher");

                entity.HasIndex(e => e.Login, "UQ__Teacher__5E55825BFF248B83")
                    .IsUnique();

                entity.Property(e => e.FireDate).HasColumnType("date");

                entity.Property(e => e.HireDate).HasColumnType("date");

                entity.Property(e => e.Login)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Middlename)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Surname)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TeacherSubject>(entity =>
            {
                entity.ToTable("Teachre_Subject");

                entity.Property(e => e.FkSubject).HasColumnName("FK_Subject");

                entity.Property(e => e.FkTeacher).HasColumnName("FK_Teacher");

                entity.HasOne(d => d.FkSubjectNavigation)
                    .WithMany(p => p.TeacherSubjects)
                    .HasForeignKey(d => d.FkSubject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Subject_TS");

                entity.HasOne(d => d.FkTeacherNavigation)
                    .WithMany(p => p.TeacherSubjects)
                    .HasForeignKey(d => d.FkTeacher)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Teacher_TS");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

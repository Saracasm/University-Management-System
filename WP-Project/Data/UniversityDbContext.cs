// Data/UniversityDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WP_project.Models;

namespace WP_project.Data
{
    // CHANGE THIS LINE — this is the key!
    public class UniversityDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
            : base(options)
        {
        }

        // Your DbSets
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Faculty> Faculties => Set<Faculty>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Prerequisite> Prerequisites => Set<Prerequisite>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<CourseOffering> CourseOfferings => Set<CourseOffering>();
        public DbSet<Grade> Grades => Set<Grade>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<Semester> Semesters => Set<Semester>();
        public DbSet<Transcript> Transcripts => Set<Transcript>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // VERY IMPORTANT — this creates all Identity tables
            base.OnModelCreating(modelBuilder);

            // Enrollment composite primary key
            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.StudentId, e.CourseOfferingId });

            // Prerequisites — clean and proper
            modelBuilder.Entity<Prerequisite>()
                .HasOne(p => p.Course)
                .WithMany(c => c.Prerequisites)
                .HasForeignKey(p => p.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prerequisite>()
                .HasOne(p => p.RequiredCourse)
                .WithMany(c => c.RequiredFor)        // ← Now properly linked!
                .HasForeignKey(p => p.RequiredCourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent duplicate prerequisites
            modelBuilder.Entity<Prerequisite>()
                .HasIndex(p => new { p.CourseId, p.RequiredCourseId })
                .IsUnique();

            // One-to-one: ApplicationUser → Student
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Student)
                .WithOne(s => s.ApplicationUser!)
                .HasForeignKey<ApplicationUser>(u => u.StudentId);

            // One-to-one: ApplicationUser → Faculty
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Faculty)
                .WithOne(f => f.ApplicationUser!)
                .HasForeignKey<ApplicationUser>(u => u.FacultyId);
        }
    }
}
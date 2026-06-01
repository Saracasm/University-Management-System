// 7. CourseOffering.cs
namespace WP_project.Models
{
    public class CourseOffering
    {
        public int CourseOfferingId { get; set; }

        public int CourseId { get; set; }
        public int SemesterId { get; set; }
        public int InstructorId { get; set; }               // FacultyId

        public string? Section { get; set; }                // e.g., A, 001
        public int Capacity { get; set; } = 40;

        // Navigation
        public Course Course { get; set; } = null!;
        public Semester Semester { get; set; } = null!;
        public Faculty Instructor { get; set; } = null!;
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
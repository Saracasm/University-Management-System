// 5. Prerequisite.cs
namespace WP_project.Models
{
    public class Prerequisite
    {
        public int PrerequisiteId { get; set; }

        public int CourseId { get; set; }         // The course that has prerequisite
        public int RequiredCourseId { get; set; } // The course that must be taken first

        // Navigation
        public Course Course { get; set; } = null!;         // belongs to this course
        public Course RequiredCourse { get; set; } = null!; // the actual prerequisite
    }
}
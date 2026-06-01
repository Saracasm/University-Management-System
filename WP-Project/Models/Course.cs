// 4. Course.cs
namespace WP_project.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Code { get; set; } = null!;           // e.g., CSE101
        public string Title { get; set; } = null!;
        public int Credits { get; set; }
        public int DepartmentId { get; set; }

        // Navigation
        public Department Department { get; set; } = null!;
        public ICollection<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>();          // courses that require THIS
        public ICollection<Prerequisite> RequiredFor { get; set; } = new List<Prerequisite>();              // this course requires THESE
        public ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();
    }
}
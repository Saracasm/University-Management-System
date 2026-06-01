// 2. Department.cs
namespace WP_project.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }               // PK
        public string Name { get; set; } = null!;
        public string? Code { get; set; }                   // e.g., CSE, EE, MATH

        // Navigation
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Faculty> Faculties { get; set; } = new List<Faculty>();
    }
}
// 3. Faculty.cs
namespace WP_project.Models
{
    public class Faculty
    {
        public int FacultyId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int DepartmentId { get; set; }

        // Navigation
        public Department Department { get; set; } = null!;
        public ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();

        public ApplicationUser? ApplicationUser { get; set; }
    }
}
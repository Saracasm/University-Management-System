// 6. Semester.cs
namespace WP_project.Models
{
    public class Semester
    {
        public int SemesterId { get; set; }
        public string Name { get; set; } = null!;           // e.g., Fall 2025
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();
    }
}
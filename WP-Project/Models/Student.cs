// 1. Student.cs (you already have this – slightly improved)
namespace WP_project.Models
{
    public class Student
    {
        public int StudentId { get; set; }                  // PK
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;          // renamed from 'email' → 'Email' (C# convention)
        public string RollNumber { get; set; } = null!;
        public int DepartmentId { get; set; }

        // Navigation
        public Department Department { get; set; } = null!;
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public Transcript? Transcript { get; set; }         // optional one-to-one

        public ApplicationUser? ApplicationUser { get; set; }
    }
}
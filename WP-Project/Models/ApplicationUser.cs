using Microsoft.AspNetCore.Identity;

namespace WP_project.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;   // Admin / Student / Faculty

        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        public int? FacultyId { get; set; }
        public Faculty? Faculty { get; set; }
    }
}

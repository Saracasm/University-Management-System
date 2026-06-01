using System.ComponentModel.DataAnnotations;

namespace WP_project.ViewModels
{
    public class CreateStudentViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string RollNumber { get; set; } = null!;

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }

    public class CreateFacultyViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}


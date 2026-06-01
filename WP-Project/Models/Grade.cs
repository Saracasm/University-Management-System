// 9. Grade.cs (Optional – if you want grades in separate table)
namespace WP_project.Models
{
    public class Grade
    {
        public int GradeId { get; set; }
        public int EnrollmentId { get; set; }
        public string LetterGrade { get; set; } = null!;
        public decimal? GradePoint { get; set; }

        public Enrollment Enrollment { get; set; } = null!;
    }
}
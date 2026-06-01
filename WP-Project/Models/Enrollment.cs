// 8. Enrollment.cs (Junction table with payload)
namespace WP_project.Models
{
    public class Enrollment
    {
        public int StudentId { get; set; }
        public int CourseOfferingId { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        
        // Final Letter Grade (calculated)
        public string? Grade { get; set; }

        // Detailed Component Scores (0-100)
        public double? AssignmentScore { get; set; } // 10%
        public double? QuizScore { get; set; }       // 10%
        public double? ProjectScore { get; set; }    // 10%
        public double? MidScore { get; set; }        // 20%
        public double? FinalScore { get; set; }      // 50%
        
        public double? TotalScore { get; set; }      // 100%

        // Navigation
        public Student Student { get; set; } = null!;
        public CourseOffering CourseOffering { get; set; } = null!;
    }
}

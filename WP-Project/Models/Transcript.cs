// 10. Transcript.cs (Optional – one per student)
namespace WP_project.Models
{
    public class Transcript
    {
        public int TranscriptId { get; set; }
        public int StudentId { get; set; }
        public decimal CGPA { get; set; }
        public int TotalCredits { get; set; }

        public Student Student { get; set; } = null!;
    }
}
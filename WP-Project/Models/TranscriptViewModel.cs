namespace WP_project.Models
{
    public class TranscriptViewModel
    {
        public string StudentName { get; set; } = "";
        public string RollNumber { get; set; } = "";
        public double CGPA { get; set; }
        public List<SemesterTranscriptViewModel> Semesters { get; set; } = new();
    }

    public class SemesterTranscriptViewModel
    {
        public string SemesterName { get; set; } = "";
        public double SemesterGPA { get; set; }
        public List<TranscriptCourseViewModel> Courses { get; set; } = new();
    }

    public class TranscriptCourseViewModel
    {
        public string Code { get; set; } = "";
        public string Title { get; set; } = "";
        public int Credits { get; set; }
        public string? Grade { get; set; }
        public double Points { get; set; }
    }
}


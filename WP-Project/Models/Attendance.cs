namespace WP_project.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        
        public int CourseOfferingId { get; set; }
        public int StudentId { get; set; }
        
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }

        // Navigation
        public CourseOffering CourseOffering { get; set; } = null!;
        public Student Student { get; set; } = null!;
    }
}

